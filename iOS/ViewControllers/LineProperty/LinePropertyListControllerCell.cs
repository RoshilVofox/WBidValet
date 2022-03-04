
using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
	public class LinePropertyListControllerCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("LinePropertyListControllerCell");

		public LinePropertyListControllerCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

