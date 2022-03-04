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
	[Register ("DownloadBidDataViewController")]
	partial class DownloadBidDataViewController
	{
		[Outlet]
		UIKit.UIButton btnCalculateVACCorrection { get; set; }

		[Outlet]
		UIKit.UIButton btnCheckAuthorization { get; set; }

		[Outlet]
		UIKit.UIButton btnCheckCWACredentials { get; set; }

		[Outlet]
		UIKit.UIButton btnCheckInternetConnection { get; set; }

		[Outlet]
		UIKit.UIButton btnGetDataFiles { get; set; }

		[Outlet]
		UIKit.UIButton btnParseData { get; set; }

		[Outlet]
		UIKit.UIButton btnVacationData { get; set; }

		[Outlet]
		UIKit.UIButton btnVacDone { get; set; }

		[Outlet]
		UIKit.UIButton btnVacLater { get; set; }

		[Outlet]
		UIKit.UILabel lblMessage1 { get; set; }

		[Outlet]
		UIKit.UILabel lblMessage2 { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UIProgressView prgrsVw { get; set; }

		[Outlet]
		UIKit.UITextField txtVANumber { get; set; }

		[Outlet]
		UIKit.UILabel vacCorrectionText { get; set; }

		[Outlet]
		UIKit.UIView vwVacOverLap { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCalculateVACCorrection != null) {
				btnCalculateVACCorrection.Dispose ();
				btnCalculateVACCorrection = null;
			}

			if (btnCheckAuthorization != null) {
				btnCheckAuthorization.Dispose ();
				btnCheckAuthorization = null;
			}

			if (btnCheckCWACredentials != null) {
				btnCheckCWACredentials.Dispose ();
				btnCheckCWACredentials = null;
			}

			if (btnCheckInternetConnection != null) {
				btnCheckInternetConnection.Dispose ();
				btnCheckInternetConnection = null;
			}

			if (btnGetDataFiles != null) {
				btnGetDataFiles.Dispose ();
				btnGetDataFiles = null;
			}

			if (btnParseData != null) {
				btnParseData.Dispose ();
				btnParseData = null;
			}

			if (btnVacationData != null) {
				btnVacationData.Dispose ();
				btnVacationData = null;
			}

			if (btnVacDone != null) {
				btnVacDone.Dispose ();
				btnVacDone = null;
			}

			if (btnVacLater != null) {
				btnVacLater.Dispose ();
				btnVacLater = null;
			}

			if (lblMessage1 != null) {
				lblMessage1.Dispose ();
				lblMessage1 = null;
			}

			if (lblMessage2 != null) {
				lblMessage2.Dispose ();
				lblMessage2 = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (prgrsVw != null) {
				prgrsVw.Dispose ();
				prgrsVw = null;
			}

			if (txtVANumber != null) {
				txtVANumber.Dispose ();
				txtVANumber = null;
			}

			if (vwVacOverLap != null) {
				vwVacOverLap.Dispose ();
				vwVacOverLap = null;
			}

			if (vacCorrectionText != null) {
				vacCorrectionText.Dispose ();
				vacCorrectionText = null;
			}
		}
	}
}
