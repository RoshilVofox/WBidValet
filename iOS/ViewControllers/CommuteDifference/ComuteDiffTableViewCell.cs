using System;

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
    }
}
