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
	[Register ("AdminArea")]
	partial class AdminArea
	{
		[Outlet]
		UIKit.UIButton btnDone { get; set; }

		[Outlet]
		UIKit.UIButton btnDownloadBid { get; set; }

		[Outlet]
		UIKit.UISegmentedControl SegServer { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchCurrentMonth { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchOtherUser { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchQATest { get; set; }

		[Outlet]
		UIKit.UISwitch SwitchSeniorityList { get; set; }

		[Outlet]
		UIKit.UISwitch SwithcSouthWestWifi { get; set; }

		[Outlet]
		UIKit.UITextField txtEmployeeNumber { get; set; }

		[Outlet]
		UIKit.UITextField txtPassword { get; set; }

		[Action ("BtnDoneClicked:")]
		partial void BtnDoneClicked (Foundation.NSObject sender);

		[Action ("BtnDownloadBidClicked:")]
		partial void BtnDownloadBidClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDone != null) {
				btnDone.Dispose ();
				btnDone = null;
			}

			if (btnDownloadBid != null) {
				btnDownloadBid.Dispose ();
				btnDownloadBid = null;
			}

			if (SegServer != null) {
				SegServer.Dispose ();
				SegServer = null;
			}

			if (SwitchCurrentMonth != null) {
				SwitchCurrentMonth.Dispose ();
				SwitchCurrentMonth = null;
			}

			if (SwitchOtherUser != null) {
				SwitchOtherUser.Dispose ();
				SwitchOtherUser = null;
			}

			if (SwitchQATest != null) {
				SwitchQATest.Dispose ();
				SwitchQATest = null;
			}

			if (SwitchSeniorityList != null) {
				SwitchSeniorityList.Dispose ();
				SwitchSeniorityList = null;
			}

			if (SwithcSouthWestWifi != null) {
				SwithcSouthWestWifi.Dispose ();
				SwithcSouthWestWifi = null;
			}

			if (txtEmployeeNumber != null) {
				txtEmployeeNumber.Dispose ();
				txtEmployeeNumber = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}
		}
	}
}
