
using System;

using Foundation;
using UIKit;
using CoreGraphics;
using Bidvalet.Business.BusinessLogic;
using System.Collections.ObjectModel;
using Bidvalet.Model;
using System.Linq;


namespace Bidvalet.iOS
{
	public partial class LineDetailedCalenderView : UIViewController
	{
		public ObservableCollection<CalendarData> calData;
		public int SelectedLine;
		public LineDetailedCalenderView () : base ("LineDetailedCalenderView", null)
		{
		}
		public string selectedTrip;
		public bool isLastTrip;
		public int ViewSelectedLine;
		NSObject TripNavigationNotification;
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			var selectedAttributes = new UITextAttributes {
				Font = UIFont.FromName("HelveticaNeue", 12), // this does not work
				TextColor = UIColor.White // but this does work
			};

			SegweekView.SetTitleTextAttributes (selectedAttributes, UIControlState.Normal);
			SegweekView.SelectedSegment = -1;
			SegweekView.UserInteractionEnabled = false;
			this.NavigationController.SetNavigationBarHidden (false, false);
          
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

            var layout = new UICollectionViewFlowLayout();
            layout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);
            layout.MinimumInteritemSpacing = 0;
            layout.MinimumLineSpacing = 0;
            //layout.CollectionView.BackgroundColor = UIColor.White;
            nfloat Size = UIScreen.MainScreen.Bounds.Width;

            //TripNavigationNotification = NSNotificationCenter.DefaultCenter.AddObserver (new Foundation.NSString ("NavigateToTrip"), NavigateToTripView);

            layout.ItemSize = new CGSize((Size / 7), 65);
            //CalenderPopoverController calCollection = new CalenderPopoverController (layout);
    
            //			this.AddChildViewController (calCollection);
            //			calCollection.View.Frame = this.baseViewCollection.Frame;
            //			calCollection.View.BackgroundColor = UIColor.LightGray;
            //
            //			baseViewCollection.BackgroundColor = UIColor.LightGray;
            //
            //			this.baseViewCollection.AddSubview (calCollection.View);

            UIBarButtonItem BackButton = new UIBarButtonItem();
            UITextAttributes icoFontAttribute = new UITextAttributes();

            icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(18);
            //icoFontAttribute.TextColor = UIColor.Blue;
            BackButton.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
            BackButton.Title = "< Back";

            BackButton.Style = UIBarButtonItemStyle.Plain;
            BackButton.Clicked += (sender, args) =>
            {
                this.NavigationController.PopViewController(true);
            };
            this.NavigationItem.LeftBarButtonItem = BackButton;

            CalenderColletionView.SetCollectionViewLayout(layout, true);

            BindCalendar();
			// Perform any additional setup after loading the view, typically from a nib.
		}

        private void BindCalendar()
        {
            this.Title = "Lines " + GlobalSettings.Lines[ViewSelectedLine].LineNum;
          
            calData = CalendarViewBL.GenerateCalendarDetails(GlobalSettings.Lines[ViewSelectedLine]);
         
            CalenderColletionView.Source = new calenderPopListViewSource(calData, this);
            CalenderColletionView.ReloadData();
			btnArrowUp.Enabled = ViewSelectedLine > 0;
			btnArrowDown.Enabled =ViewSelectedLine < GlobalSettings.Lines.Count - 1;
        }
	

//
//
//		public override nint NumberOfSections (UICollectionView collectionView)
//		{
//			// TODO: return the actual number of sections
//			return 1;
//		}
//
//		public override nint GetItemsCount (UICollectionView collectionView, nint section)
//		{
//			// TODO: return the actual number of items in the section
//			return calData.Count;
//		}
//
//		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
//		{
//			collectionView.RegisterNibForCell (UINib.FromName ("CalenderPopCell", NSBundle.MainBundle),new NSString ("CalenderPopCell"));
//			var cell = collectionView.DequeueReusableCell (CalenderPopCell.Key, indexPath) as CalenderPopCell;
//
//			// TODO: populate the cell with the appropriate data based on the indexPath
//			cell.BackgroundColor = UIColor.White;
//			cell.bindData (calData [indexPath.Row]);
//			return cell;
//		}
//		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
//		{
//			string trip = calData [indexPath.Row].TripNumber;
//			//string ss = trip;
//			if (calData [indexPath.Row].TripNumber != null) {
//				TripDetailsView ObjTrip = new TripDetailsView ();
//
//				ObjTrip.selectedTrip = trip;
//				ObjTrip.SelectedLine = indexPath.Row;
//				ObjTrip.isLastTrip = calData [indexPath.Row].IsLastTrip;
//				this.NavigationController.PushViewController (ObjTrip, true);
//
//
//			} else {
//
//			}
//			collectionView.ReloadData ();
//		}

		partial void DownArrowClicked (NSObject sender)
        {
            int index = ViewSelectedLine;
               //GlobalSettings.Lines.ToList().FindIndex(a => a.LineNum == ViewSelectedLine);
           if (index >= 0 && index <= GlobalSettings.Lines.Count - 2)
           {
               ViewSelectedLine++;
               //ViewSelectedLine = GlobalSettings.Lines[index + 1].LineNum;
               BindCalendar();
           }

            
			
		}
		partial void UpArrowClicked (NSObject sender)
		{
            
            int index = ViewSelectedLine;
                //GlobalSettings.Lines.ToList().FindIndex(a => a.LineNum == ViewSelectedLine);
            if (index >= 1 && index <= GlobalSettings.Lines.Count - 1)
            {
                ViewSelectedLine--;
                //ViewSelectedLine = GlobalSettings.Lines[index - 1].LineNum;
                BindCalendar();
            }
		}


	
		public void NAvigateToTrip()
		{
			TripDetailsView ObjTrip = new TripDetailsView ();

			ObjTrip.selectedTrip = selectedTrip;
            ObjTrip.SelectedLine = GlobalSettings.Lines[ViewSelectedLine].LineNum;
                //SelectedLine;
			ObjTrip.isLastTrip = isLastTrip;
			this.NavigationController.PushViewController (ObjTrip, true);

		}
		public override void ViewWillDisappear (bool animated)
		{
		//	NSNotificationCenter.DefaultCenter.RemoveObserver(TripNavigationNotification);
		}
	}
}

