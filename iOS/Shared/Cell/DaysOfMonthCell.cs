using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using Bidvalet.Business;
using System.Collections.Generic;
using System.Linq;
namespace Bidvalet.iOS
{
	public partial class DaysOfMonthCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("DaysOfMonthCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _parentVC;
		DaysOfMonthCx _cellData;

		static DaysOfMonthCell ()
		{
			Nib = UINib.FromName ("DaysOfMonthCell", NSBundle.MainBundle);
		}

		public DaysOfMonthCell (IntPtr handle) : base (handle)
		{
		}

		public static DaysOfMonthCell Create()
		{
			return (DaysOfMonthCell)Nib.Instantiate(null, null)[0];
		}

		void UpdateUI()
		{

            List<DaysOfMonth> dayOfMonthList = WBidCollection.GetDaysOfMonthList();

            string offDays = string.Empty;
            if (_cellData.OFFDays != null)
            {
                foreach (int offDayId in _cellData.OFFDays)
                {
                    if (offDays != string.Empty)
                        offDays += ",";

                    offDays += dayOfMonthList.FirstOrDefault(x => x.Id == offDayId).Day;
 
                }
 
            }


            if (offDays != string.Empty)
            {
                offDays = "off[" + offDays + "]";
            }

            string workDays = string.Empty;
            if (_cellData.WorkDays != null)
            {
                foreach (int workDayId in _cellData.WorkDays)
                {
                    if (workDays != string.Empty)
                        workDays += ",";

                    workDays += dayOfMonthList.FirstOrDefault(x => x.Id == workDayId).Day;

                }

            }

            if (workDays != string.Empty)
            {
                workDays = "fly[" + workDays + "]";
            }






            //string offDays = "";
            //if (_cellData.OFFDays == null) {
            //    offDays = "off[]";
            //} else {
            //    offDays = "off[";
            //    for (int i = 0; i < _cellData.OFFDays.Count; i++) {
            //        if (i == _cellData.OFFDays.Count - 1) {
            //            offDays = offDays + _cellData.OFFDays[i]; // last element
            //        } else {
            //            offDays = offDays + _cellData.OFFDays[i] + ",";
            //        }
            //    }
            //    offDays = offDays + "]";
            //}


            //string workDays = "work[";
            //if (_cellData.WorkDays == null) {
            //    workDays = "work[]";
            //} else {
            //    for (int i = 0; i < _cellData.WorkDays.Count; i++) {
            //        if (i == _cellData.WorkDays.Count - 1) {
            //            workDays = workDays + _cellData.WorkDays[i]; // last element
            //        } else {
            //            workDays = workDays + _cellData.WorkDays[i] + ",";
            //        }
            //    }	
            //    workDays = workDays + "]";
            //}
			lbDayOfMonth.Text = string.Format ("{0} {1}",offDays, workDays);
		}

		public void Filldata (ConstraintsChangeViewController constraintsChangeViewController, DaysOfMonthCx daysOfMonthCx)
		{
			_parentVC = constraintsChangeViewController;
			_cellData = daysOfMonthCx;
			UpdateUI ();
		}

		partial void OnDeleteEvent (Foundation.NSObject sender)
		{
			if (_parentVC != null) {
				_parentVC.DeleteObject(_cellData);
			}
		}

		partial void OnOpenPickerEvent (Foundation.NSObject sender){
			ConstraintDaysMonthViewController viewController = _parentVC.Storyboard.InstantiateViewController("ConstraintDaysMonthViewController") as ConstraintDaysMonthViewController;
			viewController.data = _cellData;
			_parentVC.NavigationController.PushViewController(viewController, true);
		}
	}
}
