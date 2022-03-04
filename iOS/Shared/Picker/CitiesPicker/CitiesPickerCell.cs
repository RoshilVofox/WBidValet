using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
	public partial class CitiesPickerCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("CitiesPickerCell");
		public static readonly UINib Nib;

		static CitiesPickerCell ()
		{
			Nib = UINib.FromName ("CitiesPickerCell", NSBundle.MainBundle);
		}

		public CitiesPickerCell (IntPtr handle) : base (handle)
		{
		}
		public void FillData(string city)
		{
			lbTitle.Text = city;
		}

	}
}
