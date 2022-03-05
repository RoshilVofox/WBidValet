// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Bidvalet.iOS.ViewControllers.HistoryBidData
{
	[Register ("HistoryBidDataRetrieve")]
	partial class HistoryBidDataRetrieve
	{
		[Outlet]
		UIKit.UIButton[] btnMonthCollection { get; set; }

		[Outlet]
		UIKit.UIButton[] btnTest1collection { get; set; }

		[Outlet]
		UIKit.UIButton[] btnTestCollection { get; set; }

		[Outlet]
		UIKit.UIButton[] btnYearCollection { get; set; }

		[Outlet]
		UIKit.UICollectionView collectionView { get; set; }

		[Outlet]
		UIKit.UIButton cpButton { get; set; }

		[Outlet]
		UIKit.UIButton downloadButton { get; set; }

		[Outlet]
		UIKit.UIButton faButton { get; set; }

		[Outlet]
		UIKit.UIButton foButton { get; set; }

		[Outlet]
		UIKit.UICollectionView monthCollectionView { get; set; }

		[Outlet]
		UIKit.UIButton round1Button { get; set; }

		[Outlet]
		UIKit.UIButton round2Button { get; set; }

		[Outlet]
		UIKit.UIButton year1Button { get; set; }

		[Outlet]
		UIKit.UIButton year2Button { get; set; }

		[Outlet]
		UIKit.UIButton year3Button { get; set; }

		[Action ("btnMonthTapped:")]
		partial void btnMonthTapped (Foundation.NSObject sender);

		[Action ("btnYearTap:")]
		partial void btnYearTap (UIKit.UIButton sender);

		[Action ("btnYearTapped:")]
		partial void btnYearTapped (Foundation.NSObject sender);

		[Action ("downloadButtonAction:")]
		partial void downloadButtonAction (Foundation.NSObject sender);

		[Action ("positionButtonAction:")]
		partial void positionButtonAction (Foundation.NSObject sender);

		[Action ("RoundButtonAction:")]
		partial void RoundButtonAction (Foundation.NSObject sender);

		[Action ("yearSelectionAction:")]
		partial void yearSelectionAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (collectionView != null) {
				collectionView.Dispose ();
				collectionView = null;
			}

			if (cpButton != null) {
				cpButton.Dispose ();
				cpButton = null;
			}

			if (downloadButton != null) {
				downloadButton.Dispose ();
				downloadButton = null;
			}

			if (faButton != null) {
				faButton.Dispose ();
				faButton = null;
			}

			if (foButton != null) {
				foButton.Dispose ();
				foButton = null;
			}

			if (monthCollectionView != null) {
				monthCollectionView.Dispose ();
				monthCollectionView = null;
			}

			if (round1Button != null) {
				round1Button.Dispose ();
				round1Button = null;
			}

			if (round2Button != null) {
				round2Button.Dispose ();
				round2Button = null;
			}

			if (year1Button != null) {
				year1Button.Dispose ();
				year1Button = null;
			}

			if (year2Button != null) {
				year2Button.Dispose ();
				year2Button = null;
			}

			if (year3Button != null) {
				year3Button.Dispose ();
				year3Button = null;
			}
		}
	}
}
