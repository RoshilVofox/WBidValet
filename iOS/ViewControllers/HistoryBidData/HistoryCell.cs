using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.HistoryBidData
{
    public partial class HistoryCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("HistoryCell");
        public static readonly UINib Nib;

        static HistoryCell()
        {
            Nib = UINib.FromName("HistoryCell", NSBundle.MainBundle);
        }

        protected HistoryCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        public void FillData(string title)
        {
            cellTitle.Text = title;
        }

    }
}
