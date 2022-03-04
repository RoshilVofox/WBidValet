﻿using System;

using Foundation;
using UIKit;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
	public partial class StartDayOfWeekCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("StartDayOfWeekCell");
		public static readonly UINib Nib;

		ConstraintsChangeViewController _viewController;
		StartDayOfWeek _cellObject;

		static StartDayOfWeekCell ()
		{
			Nib = UINib.FromName ("StartDayOfWeekCell", NSBundle.MainBundle);
		}

		public StartDayOfWeekCell (IntPtr handle) : base (handle)
		{
		}
		public static StartDayOfWeekCell Create()
		{
			return (StartDayOfWeekCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, StartDayOfWeek cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
			lbStartDayOfWeek.Text = "SDOW"; 
			UpdateUI ();
		}

		partial void OnDeleteEvent (UIButton sender)
		{
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}

		partial void OnSunButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Su = !_cellObject.Su;
                if (_cellObject.Su)
                {
                    _cellObject.Mo = _cellObject.Tu = _cellObject.We = _cellObject.Th = _cellObject.Fr = _cellObject.Sa = false;
                }
				UpdateUI();
			}
		}

		partial void OnMonButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Mo = !_cellObject.Mo;
                if (_cellObject.Mo)
                {
                    _cellObject.Su = _cellObject.Tu = _cellObject.We = _cellObject.Th = _cellObject.Fr = _cellObject.Sa = false;
                }
				UpdateUI();
			}
		}

		partial void OnTueButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Tu = !_cellObject.Tu;
                if (_cellObject.Tu)
                {
                    _cellObject.Su = _cellObject.Mo = _cellObject.We = _cellObject.Th = _cellObject.Fr = _cellObject.Sa = false;
                }
				UpdateUI();
			}
		}

		partial void OnWedButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.We = !_cellObject.We;
                if (_cellObject.We)
                {
                    _cellObject.Su = _cellObject.Mo = _cellObject.Tu = _cellObject.Th = _cellObject.Fr = _cellObject.Sa = false;
                }
				UpdateUI();
			}
		}

		partial void OnThuButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Th = !_cellObject.Th;
                if (_cellObject.Th)
                {
                    _cellObject.Su = _cellObject.Mo = _cellObject.Tu = _cellObject.We = _cellObject.Fr = _cellObject.Sa = false;
                }
				UpdateUI();
			}
		}

		partial void OnFriButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Fr = !_cellObject.Fr;
                if (_cellObject.Fr)
                {
                    _cellObject.Su = _cellObject.Mo = _cellObject.Tu = _cellObject.We = _cellObject.Th = _cellObject.Sa = false;
                }
				UpdateUI();
			}
		}

		partial void OnSatButtonEvent (UIButton sender)
		{
			if (_cellObject != null) {
				_cellObject.Sa = !_cellObject.Sa;
                if (_cellObject.Sa)
                {
                    _cellObject.Su = _cellObject.Mo = _cellObject.Tu = _cellObject.We = _cellObject.Th = _cellObject.Fr = false;
                }
				UpdateUI();
			}
		}

		private void UpdateUI (){
			if (_cellObject.Su) {
				btnSun.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnSun.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Mo) {
				btnMon.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnMon.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Tu) {
				btnTue.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnTue.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.We) {
				btnWed.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnWed.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Th) {
				btnThu.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnThu.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Fr) {
				btnFri.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnFri.BackgroundColor = Colors.BidOrange;
			}
			if (_cellObject.Sa) {
				btnSat.BackgroundColor = Colors.BidRowGreen;
			} else {
				btnSat.BackgroundColor = Colors.BidOrange;
			}
		}
	}
}
