using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
namespace Bidvalet.iOS
{
	public partial class DaysOfWeekSomeCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("DaysOfWeekSomeCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _viewController;
		DaysOfWeekSome _cellObject;

		static DaysOfWeekSomeCell ()
		{
			Nib = UINib.FromName ("DaysOfWeekSomeCell", NSBundle.MainBundle);
		}

		public DaysOfWeekSomeCell (IntPtr handle) : base (handle)
		{
		}
		public static DaysOfWeekSomeCell Create()
		{
			return (DaysOfWeekSomeCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, DaysOfWeekSome cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
			//_cellObject.Date = "Mon";
			//_cellObject.LessOrMore = "Less than";
			//_cellObject.Value = 1;
			UpdateUI ();
		}
		private void UpdateUI (){
			btnDaySomeValue.SetTitle(String.Format("{0}",_cellObject.Value), UIControlState.Normal);
			btnDaySomeConstraint.SetTitle(_cellObject.Date, UIControlState.Normal);
			btnLessthan.SetTitle(_cellObject.LessOrMore, UIControlState.Normal);
		}

		partial void OnDaySomeConstraintEvent (Foundation.NSObject sender){
			UIActionSheet actionSheet = new UIActionSheet ("");
			actionSheet.AddButton ("Sun");
			actionSheet.AddButton ("Mon");
			actionSheet.AddButton ("Tue");
			actionSheet.AddButton ("Wed");
			actionSheet.AddButton ("Thu");
			actionSheet.AddButton ("Fri");
			actionSheet.AddButton ("Sat");
			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
				switch (b.ButtonIndex){
				case 0:
					_cellObject.Date = "Sun";
					break;
				case 1:
					_cellObject.Date = "Mon";
					break;
				case 2:
					_cellObject.Date = "Tue";
					break;
				case 3:
					_cellObject.Date = "Wed";
					break;
				case 4:
					_cellObject.Date = "Thu";
					break;
				case 5:
					_cellObject.Date = "Fri";
					break;
				case 6:
					_cellObject.Date = "Sat";
					break;
				}
				UpdateUI();
			};
			actionSheet.ShowInView (_viewController.View);
		}

		partial void OnDaySomeValueEvent (Foundation.NSObject sender){
			UIActionSheet actionSheet = new UIActionSheet ("");
			for (int i =0; i<=6;i++){
				actionSheet.AddButton (String.Format("{0}",i));	
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					_cellObject.Value =(int) b.ButtonIndex;
					UpdateUI();
				};
			}
			actionSheet.ShowInView (_viewController.View);
		}


		partial void OnLessthanEvent (Foundation.NSObject sender){
			UIActionSheet actionSheet = new UIActionSheet ("");
            actionSheet.AddButton("Less than");
            actionSheet.AddButton("Equal");
            actionSheet.AddButton("More than");
		
			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
				switch (b.ButtonIndex){
				case 0:
                        _cellObject.LessOrMore = "Less than";
					break;
                case 1:
                    _cellObject.LessOrMore = "Equal";
                    break;
				case 2:
                    _cellObject.LessOrMore = "More than";
					break;
				}
				UpdateUI();
			};
			actionSheet.ShowInView (_viewController.View);
		}

		partial void OnDeleteEvent (Foundation.NSObject sender){
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}
	}
}
