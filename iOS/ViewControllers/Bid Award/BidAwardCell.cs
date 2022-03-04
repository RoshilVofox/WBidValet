using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
	public partial class BidAwardCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString("BidAwardCell");
		public static readonly UINib Nib;

		static BidAwardCell()
		{
			Nib = UINib.FromName("BidAwardCell", NSBundle.MainBundle);
		}
		public void bindData(BidAwardView.Data ObjValue)
		{
			lblAwardText.Text = ObjValue.Name;
		}
		protected BidAwardCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
	}
}
