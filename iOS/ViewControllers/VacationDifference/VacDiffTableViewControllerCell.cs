using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.VacationDifference
{
    public partial class VacDiffTableViewControllerCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("VacDiffTableViewControllerCell");
        public static readonly UINib Nib;

        static VacDiffTableViewControllerCell()
        {
            Nib = UINib.FromName("VacDiffTableViewControllerCell", NSBundle.MainBundle);
        }

        protected VacDiffTableViewControllerCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
