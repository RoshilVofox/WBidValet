// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Bidvalet.iOS.ViewControllers
{
	[Register ("MonthToMonthAlertView")]
	partial class MonthToMonthAlertView
	{
		[Outlet]
		UIKit.UIButton btnLink1 { get; set; }

		[Outlet]
		UIKit.UIButton btnLink2 { get; set; }

		[Outlet]
		UIKit.UILabel lblAlert { get; set; }

		[Action ("btnLink1Tap:")]
		partial void btnLink1Tap (Foundation.NSObject sender);

		[Action ("btnLink2Tap:")]
		partial void btnLink2Tap (Foundation.NSObject sender);

		[Action ("btnOkTap:")]
		partial void btnOkTap (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnLink1 != null) {
				btnLink1.Dispose ();
				btnLink1 = null;
			}

			if (btnLink2 != null) {
				btnLink2.Dispose ();
				btnLink2 = null;
			}

			if (lblAlert != null) {
				lblAlert.Dispose ();
				lblAlert = null;
			}
		}
	}
}
