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
	[Register ("BidListCustomHeader")]
	partial class BidListCustomHeader
	{
		[Outlet]
		UIKit.UIButton btnDownScroll { get; set; }

		[Outlet]
		UIKit.UIButton btnEOM { get; set; }

		[Outlet]
		UIKit.UIButton btnUpScroll { get; set; }

		[Outlet]
		UIKit.UIButton btnVAC { get; set; }

		[Outlet]
		UIKit.UITextField SearchBar { get; set; }

		[Action ("btnEOMTapped:")]
		partial void btnEOMTapped (UIKit.UIButton sender);

		[Action ("btnVACTapped:")]
		partial void btnVACTapped (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDownScroll != null) {
				btnDownScroll.Dispose ();
				btnDownScroll = null;
			}

			if (btnUpScroll != null) {
				btnUpScroll.Dispose ();
				btnUpScroll = null;
			}

			if (SearchBar != null) {
				SearchBar.Dispose ();
				SearchBar = null;
			}

			if (btnVAC != null) {
				btnVAC.Dispose ();
				btnVAC = null;
			}

			if (btnEOM != null) {
				btnEOM.Dispose ();
				btnEOM = null;
			}
		}
	}
}
