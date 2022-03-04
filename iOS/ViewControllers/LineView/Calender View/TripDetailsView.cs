
using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using Bidvalet.Business.BusinessLogic;
using System.Linq;

namespace Bidvalet.iOS
{
    public partial class TripDetailsView : UIViewController
    {
        public TripDetailsView()
            : base("TripDetailsView", null)
        {
        }
        public string selectedTrip;
        public bool isLastTrip;
        public int SelectedLine;
        TripPopListViewController tripList;
        UILabel TitleLabel;
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
             tripList = new TripPopListViewController();
             TripTableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
             TripTableView.SeparatorColor = ColorClass.ListSeparatorColor;
             TripTableView.SeparatorInset = new UIEdgeInsets(0, 5, 0, 5);
             TripTableView.ContentInset = new UIEdgeInsets(0, 0, 5, 0);
             TripTableView.ScrollIndicatorInsets = new UIEdgeInsets(0, 0, 5, 0);
             TripTableView.AllowsSelection = false;
             TripTableView.Bounces = false;

            //UIView titleView=new UIView();
            //titleView.BackgroundColor = UIColor.Red;
            // TitleLabel=new UILabel();
            // TitleLabel.Font.WithSize(16);
            //TitleLabel.Text="ggg jjjjjj jjjjj jjjjjjjjjjjjjjjjjjjjj";
            //TitleLabel.Frame = new CoreGraphics.CGRect(0f, 0f, 40f, 40f);
            //titleView.Frame = new CoreGraphics.CGRect(10f, 10f,this.View.Bounds.Width-100, 40f);
            //titleView.AddSubview(TitleLabel);
            //this.NavigationController.View.AddSubview(titleView);
            //this.NavigationController.Add(titleView)

             //Submit Button 
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
            BindTrip();

            // Perform any additional setup after loading the view, typically from a nib.
        }

        private void BindTrip()
        {
            string day = string.Empty;

            string titleStr=string.Empty;
            if (selectedTrip.Length >= 6)
            {
                day = selectedTrip.Substring(4, 2) + " ";
            }

            titleStr = selectedTrip.Substring(0, 4) + " on " + day + new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM");

           
           
            CorrectionParams correctionParams = new Model.CorrectionParams();
            correctionParams.selectedLineNum = SelectedLine;
            tripList.tripData = TripViewBL.GenerateTripDetails(selectedTrip, correctionParams, isLastTrip);
            TripTableView.Source = new TripPopListViewSource(TripViewBL.GenerateTripDetails(selectedTrip, correctionParams, isLastTrip));
            TripTableView.ReloadData();
			Line line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == SelectedLine);


            string positionStr = string.Empty;

            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                if (line != null)
                {
                    if (line.FAPositions != null && line.FAPositions.Count > 0)
                    {
                        positionStr = " - ";
                        positionStr += string.Join("", line.FAPositions) ;
                    }
                }

            }

            if (positionStr != string.Empty)
            {
                titleStr += positionStr;
            }

            //var attributedString = new NSMutableAttributedString(titleStr);
            //attributedString.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Helvetica-Bold", 16f), new NSRange(0, titleStr.Length));
            //attributedString.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Red, new NSRange(titleStr.Length - positionStr.Length, positionStr.Length));
            //this.Title = attributedString;

            this.Title = titleStr;

			int index = line.Pairings.FindIndex(x => x == selectedTrip);
			btnArrowUp.Enabled = index > 0;
			btnArrowDown.Enabled = index < line.Pairings.Count - 1;
           
        }
        partial void DownArrowClicked(NSObject sender)
        {
            Line line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == SelectedLine);
            int index= line.Pairings.FindIndex(x => x == selectedTrip);
            if (index < line.Pairings.Count - 1)
            {
                index++;
                selectedTrip = line.Pairings[index];

                isLastTrip= (index==line.Pairings.Count-1);
            }
            BindTrip();
            //GlobalSettings.Lines.ToList().FindIndex(a => a.LineNum == SelectedLine);

        }
        partial void UpArrowclicked(NSObject sender)
        {
            Line line = GlobalSettings.Lines.FirstOrDefault(x=>x.LineNum==SelectedLine);
            int index = line.Pairings.FindIndex(x => x == selectedTrip);
            if (index > 0)
            {
                index--;
                selectedTrip = line.Pairings[index];
                isLastTrip = (index == line.Pairings.Count - 1);
            }
            BindTrip();
        }
    }
}

