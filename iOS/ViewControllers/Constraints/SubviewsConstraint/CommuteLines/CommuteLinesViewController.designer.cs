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
	[Register ("CommuteLinesViewController")]
	partial class CommuteLinesViewController
	{
		[Outlet]
		UIKit.UIButton btnBackToBase { get; set; }

		[Outlet]
		UIKit.UIButton btnCityName { get; set; }

		[Outlet]
		UIKit.UIButton btnDoneSetting { get; set; }

		[Outlet]
		UIKit.UIButton btnInfoBackToBase { get; set; }

		[Outlet]
		UIKit.UIButton btnInfoCmtCity { get; set; }

		[Outlet]
		UIKit.UIButton btnInfoConnectTime { get; set; }

		[Outlet]
		UIKit.UIButton btnInfoPadCheckIn { get; set; }

		[Outlet]
		UIKit.UIButton btnInfoViewCommuteTime { get; set; }

		[Outlet]
		UIKit.UIButton btnNonStop { get; set; }

		[Outlet]
		UIKit.UIButton btnTimeCheckIn { get; set; }

		[Outlet]
		UIKit.UIButton btnTimeConnect { get; set; }

		[Outlet]
		UIKit.UIButton btnViewCommuteTime { get; set; }

		[Action ("btnNonStopClick:")]
		partial void btnNonStopClick (UIKit.UIButton sender);

		[Action ("btnNonStopClicked:")]
		partial void btnNonStopClicked (Foundation.NSObject sender);

		[Action ("OnBackToBaseEvent:")]
		partial void OnBackToBaseEvent (Foundation.NSObject sender);

		[Action ("OnDoneSettingEvent:")]
		partial void OnDoneSettingEvent (Foundation.NSObject sender);

		[Action ("OnInfoBackToBaseEvent:")]
		partial void OnInfoBackToBaseEvent (Foundation.NSObject sender);

		[Action ("OnInfoCommuteCityEvent:")]
		partial void OnInfoCommuteCityEvent (Foundation.NSObject sender);

		[Action ("OnInfoConnectTimeEvent:")]
		partial void OnInfoConnectTimeEvent (Foundation.NSObject sender);

		[Action ("OnInfoPadCheckInEvent:")]
		partial void OnInfoPadCheckInEvent (Foundation.NSObject sender);

		[Action ("OnInfoViewCommuteTime:")]
		partial void OnInfoViewCommuteTime (Foundation.NSObject sender);

		[Action ("OnInfoViewCommuteTimeEvent:")]
		partial void OnInfoViewCommuteTimeEvent (Foundation.NSObject sender);

		[Action ("OnPadCheckInEvent:")]
		partial void OnPadCheckInEvent (Foundation.NSObject sender);

		[Action ("OnSetCityNameEvent:")]
		partial void OnSetCityNameEvent (Foundation.NSObject sender);

		[Action ("OnSetConnectTimeEvent:")]
		partial void OnSetConnectTimeEvent (Foundation.NSObject sender);

		[Action ("OnViewCommuteTime:")]
		partial void OnViewCommuteTime (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBackToBase != null) {
				btnBackToBase.Dispose ();
				btnBackToBase = null;
			}

			if (btnCityName != null) {
				btnCityName.Dispose ();
				btnCityName = null;
			}

			if (btnDoneSetting != null) {
				btnDoneSetting.Dispose ();
				btnDoneSetting = null;
			}

			if (btnInfoBackToBase != null) {
				btnInfoBackToBase.Dispose ();
				btnInfoBackToBase = null;
			}

			if (btnInfoCmtCity != null) {
				btnInfoCmtCity.Dispose ();
				btnInfoCmtCity = null;
			}

			if (btnInfoConnectTime != null) {
				btnInfoConnectTime.Dispose ();
				btnInfoConnectTime = null;
			}

			if (btnInfoPadCheckIn != null) {
				btnInfoPadCheckIn.Dispose ();
				btnInfoPadCheckIn = null;
			}

			if (btnInfoViewCommuteTime != null) {
				btnInfoViewCommuteTime.Dispose ();
				btnInfoViewCommuteTime = null;
			}

			if (btnNonStop != null) {
				btnNonStop.Dispose ();
				btnNonStop = null;
			}

			if (btnTimeCheckIn != null) {
				btnTimeCheckIn.Dispose ();
				btnTimeCheckIn = null;
			}

			if (btnTimeConnect != null) {
				btnTimeConnect.Dispose ();
				btnTimeConnect = null;
			}

			if (btnViewCommuteTime != null) {
				btnViewCommuteTime.Dispose ();
				btnViewCommuteTime = null;
			}
		}
	}
}
