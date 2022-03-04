using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
namespace Bidvalet.iOS
{
	public partial class CommutableLinesCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("CommutableLinesCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _parentVC;
		FtCommutableLine _cellData;
		static CommutableLinesCell ()
		{
			Nib = UINib.FromName ("CommutableLinesCell", NSBundle.MainBundle);
		}

		public CommutableLinesCell (IntPtr handle) : base (handle)
		{
		}

		public static CommutableLinesCell Create()
		{
			return (CommutableLinesCell)Nib.Instantiate(null, null)[0];
		}

		public void Filldata (ConstraintsChangeViewController constraintsChangeViewController, FtCommutableLine cxCommutableLine)
		{
			_parentVC = constraintsChangeViewController;
			_cellData = cxCommutableLine;

			UpdateUI ();
		}

		void UpdateUI()
		{
			//UIHelpers.StyleForButtonsBorderBlackRectangeThin (new UIButton[]{btnAny, btnRonBoth});
			lbCommuteName.Text = string.Format("Cmut Lines ({0})", _cellData.City);
			if (_cellData.NoNights) {
				btnNightInMiddle.BackgroundColor = Colors.BidRowGreen;
				//btnNightInMiddle.SetTitleColor (UIColor.White, UIControlState.Normal);
			
			} else {
				btnNightInMiddle.BackgroundColor = Colors.BidOrange;
				//btnNightInMiddle.SetTitleColor (UIColor.White, UIControlState.Normal);
			
			}
			if (_cellData.ToWork) {
				btnWork.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnWork.BackgroundColor = Colors.BidOrange;
			}
			if (_cellData.ToHome) {
				btnHome.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnHome.BackgroundColor = Colors.BidOrange;
			}
		}

		partial void NoNightInMiddieClicked (Foundation.NSObject sender)
		{
			_cellData.NoNights = !_cellData.NoNights;
			UpdateUI();
		}

		partial void OnDeleteEvent (Foundation.NSObject sender){
			if (_parentVC != null) {
				_parentVC.DeleteObject(_cellData);
			}
		}

		partial void OnHomeEvent (Foundation.NSObject sender){

			if(_cellData.ToWork == true)
			{
			_cellData.ToHome = !_cellData.ToHome;
			UpdateUI();
			}
		}



		partial void OnWorkEvent (Foundation.NSObject sender)
		{
			if(_cellData.ToHome == true)
			{
			_cellData.ToWork = !_cellData.ToWork;
			UpdateUI();
			}
		}

		partial void OnCommuteLineEvent (Foundation.NSObject sender){
			if(_parentVC!=null){
				_parentVC.PushViewControllView(_cellData);
			}
		}

	}
}
