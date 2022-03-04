// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Bidvalet.iOS
{
	[Register ("ConstraintItemsView")]
	partial class ConstraintItemsView
	{
		UIButton ButtonSelected{ set; get;}
		UILabel MyLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ButtonSelected != null) {
				ButtonSelected.Dispose ();
				ButtonSelected = null;
			}
			if (MyLabel != null) {
				MyLabel.Dispose ();
				MyLabel = null;
			}
		}
	}
}
