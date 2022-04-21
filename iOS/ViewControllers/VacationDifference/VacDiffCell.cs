using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.VacationDifference
{
    public partial class VacDiffCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("VacDiffCell");
        public static readonly UINib Nib;

        static VacDiffCell()
        {
            Nib = UINib.FromName("VacDiffCell", NSBundle.MainBundle);
        }

        protected VacDiffCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
