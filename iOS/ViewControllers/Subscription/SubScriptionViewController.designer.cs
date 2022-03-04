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
	[Register ("SubScriptionViewController")]
	partial class SubScriptionViewController
	{
		[Outlet]
		UIKit.UIButton btnBidAward { get; set; }

		[Outlet]
		UIKit.UIButton btnContactUs { get; set; }

		[Outlet]
		UIKit.UIButton btnEditUserAccount { get; set; }

		[Outlet]
		UIKit.UIButton btnhelp { get; set; }

		[Outlet]
		UIKit.UIButton btnNewBidReceipt { get; set; }

		[Outlet]
		UIKit.UIButton btnSecret1 { get; set; }

		[Outlet]
		UIKit.UIButton btnSecret2 { get; set; }

		[Outlet]
		UIKit.UIButton btnSecretSwitch { get; set; }

		[Outlet]
		UIKit.UIButton btnStart { get; set; }

		[Outlet]
		UIKit.UILabel lbExpiredDate { get; set; }

		[Outlet]
		UIKit.UILabel lblSyncStatus { get; set; }

		[Outlet]
		UIKit.UILabel lblVersionNumber { get; set; }

		[Outlet]
		UIKit.UILabel lbMoreDetails { get; set; }

		[Outlet]
		UIKit.UILabel lbSmallTitle { get; set; }

		[Action ("btnBidAwardClicked:")]
		partial void btnBidAwardClicked (Foundation.NSObject sender);

		[Action ("BtnSecretSwitchClicked:")]
		partial void BtnSecretSwitchClicked (Foundation.NSObject sender);

		[Action ("btnSecretSwitchForUI:")]
		partial void btnSecretSwitchForUI (Foundation.NSObject sender);

		[Action ("ContactUsButtonClicked:")]
		partial void ContactUsButtonClicked (Foundation.NSObject sender);

		[Action ("HelpButtonClicked:")]
		partial void HelpButtonClicked (Foundation.NSObject sender);

		[Action ("OnEditUserEvent:")]
		partial void OnEditUserEvent (Foundation.NSObject sender);

		[Action ("OnNewBidEvent:")]
		partial void OnNewBidEvent (Foundation.NSObject sender);

		[Action ("OnStartEvent:")]
		partial void OnStartEvent (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBidAward != null) {
				btnBidAward.Dispose ();
				btnBidAward = null;
			}

			if (btnContactUs != null) {
				btnContactUs.Dispose ();
				btnContactUs = null;
			}

			if (btnEditUserAccount != null) {
				btnEditUserAccount.Dispose ();
				btnEditUserAccount = null;
			}

			if (btnhelp != null) {
				btnhelp.Dispose ();
				btnhelp = null;
			}

			if (btnNewBidReceipt != null) {
				btnNewBidReceipt.Dispose ();
				btnNewBidReceipt = null;
			}

			if (btnSecret1 != null) {
				btnSecret1.Dispose ();
				btnSecret1 = null;
			}

			if (btnSecret2 != null) {
				btnSecret2.Dispose ();
				btnSecret2 = null;
			}

			if (btnStart != null) {
				btnStart.Dispose ();
				btnStart = null;
			}

			if (lbExpiredDate != null) {
				lbExpiredDate.Dispose ();
				lbExpiredDate = null;
			}

			if (lblSyncStatus != null) {
				lblSyncStatus.Dispose ();
				lblSyncStatus = null;
			}

			if (lblVersionNumber != null) {
				lblVersionNumber.Dispose ();
				lblVersionNumber = null;
			}

			if (lbMoreDetails != null) {
				lbMoreDetails.Dispose ();
				lbMoreDetails = null;
			}

			if (lbSmallTitle != null) {
				lbSmallTitle.Dispose ();
				lbSmallTitle = null;
			}

			if (btnSecretSwitch != null) {
				btnSecretSwitch.Dispose ();
				btnSecretSwitch = null;
			}
		}
	}
}
