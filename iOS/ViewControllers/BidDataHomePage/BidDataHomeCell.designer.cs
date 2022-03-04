// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Bidvalet.iOS
{
	[Register ("BidDataHomeCell")]
	partial class BidDataHomeCell
	{
		[Outlet]
		UIKit.UIButton btnDelete { get; set; }

		[Outlet]
		UIKit.UILabel lblSubTitle { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Action ("btnDeleteTapped:")]
		partial void btnDeleteTapped (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDelete != null) {
				btnDelete.Dispose ();
				btnDelete = null;
			}

			if (lblSubTitle != null) {
				lblSubTitle.Dispose ();
				lblSubTitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}
		}
	}
}
