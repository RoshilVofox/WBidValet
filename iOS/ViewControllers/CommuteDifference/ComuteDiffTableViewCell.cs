using System;
using Bidvalet.Model;
using Foundation;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.CommuteDifference
{
    public partial class ComuteDiffTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ComuteDiffTableViewCell");
        public static readonly UINib Nib;

        static ComuteDiffTableViewCell()
        {
            Nib = UINib.FromName("ComuteDiffTableViewCell", NSBundle.MainBundle);
        }

        protected ComuteDiffTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        public void LabelValues(CommuteFltChangeValues flightDataChangeVacValues)
        {
            lblLine.Text= flightDataChangeVacValues.LineNum.ToString();

            lblOldCmtOv.Text = flightDataChangeVacValues.OldCmtOV.ToString();
            lblNewCmtOv.Text = flightDataChangeVacValues.NewCmtOV.ToString();

            lblOldCmtFr.Text = flightDataChangeVacValues.OldCmtFr.ToString();
            lblNewCmtFr.Text = flightDataChangeVacValues.NewCmtFr.ToString();

            lblOldCmtBa.Text = flightDataChangeVacValues.OldCmtBa.ToString();
            lblNewCmtBa.Text = flightDataChangeVacValues.NewCmtBa.ToString();





        }
    }
}
