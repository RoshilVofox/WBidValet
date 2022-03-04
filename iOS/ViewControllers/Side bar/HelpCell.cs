
using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
	public partial class HelpCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("HelpCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("HelpCell");

		public HelpCell (IntPtr handle) : base (handle)
		{
		}
		public void bindData(string value)
		{
			this.TextLabel.Text = "  " + value;
			this.TextLabel.TextColor = UIColor.White;
		}
		public static HelpCell Create ()
		{
			return (HelpCell)Nib.Instantiate (null, null) [0];
		}
	}
}

