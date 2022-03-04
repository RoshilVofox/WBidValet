using System;

using Foundation;
using UIKit;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
	public partial class LinesTypeCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("LinesTypeCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _viewController;
		LineTypeConstraint _cellObject;

		static LinesTypeCell ()
		{
			Nib = UINib.FromName ("LinesTypeCell", NSBundle.MainBundle);
		}

		public LinesTypeCell (IntPtr handle) : base (handle)
		{
		}
		public static LinesTypeCell Create()
		{
			return (LinesTypeCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, LineTypeConstraint cellData)
		{
            btnInt.SetTitle("Intl", UIControlState.Normal);
            btnRest.SetTitle("Reserve", UIControlState.Normal);
			_viewController = parentVC;
			_cellObject = cellData;

            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                btnBlank.SetTitle("Ready",UIControlState.Normal);
            }
            else
            {
                btnBlank.SetTitle("Blank", UIControlState.Normal);
              
            }

            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
            {
                btnRest.Enabled = btnBlank.Enabled = false;
                _cellObject.Res = _cellObject.Blank = false;

            }
            else
            {
                btnRest.Enabled = btnBlank.Enabled = true;

            }

            if (GlobalSettings.CurrentBidDetails.Postion != "FA" && GlobalSettings.CurrentBidDetails.Round != "M")
            {
                btnBlank.Enabled = false;
                _cellObject.Blank = false;

            }

			UpdateUI ();
		}
		private void UpdateUI (){
			//
			if (_cellObject.Hard) {
				btnHard.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnHard.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Res) {
				btnRest.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnRest.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Blank) {
				btnBlank.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnBlank.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Int) {
				btnInt.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnInt.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.NonCon) {
				btnNonCon.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnNonCon.BackgroundColor = Colors.BidOrange;
			}
       
		}
		partial void OnDeleteEvent (UIButton sender)
		{
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnNonConButtonEvent (UIButton sender)
		{
			if(_cellObject!=null){
				_cellObject.NonCon= !_cellObject.NonCon;
			}
			UpdateUI();
		}




		partial void OnIntButtonEvent (UIButton sender)
		{
			if(_cellObject!=null){
				_cellObject.Int= !_cellObject.Int;
			}
			UpdateUI();
		}

		partial void OnBlankButtonEvent (UIButton sender)
		{
			if(_cellObject!=null){
				_cellObject.Blank= !_cellObject.Blank;
			}
			UpdateUI();
		}

		partial void OnRestButtonEvent (UIButton sender)
		{
			if(_cellObject!=null){
				_cellObject.Res= !_cellObject.Res;
			}
			UpdateUI();
		}

		partial void OnHardButtonEvent (UIButton sender)
		{
			if(_cellObject!=null){
				_cellObject.Hard= !_cellObject.Hard;

			}
			UpdateUI();
		}
	}
}
