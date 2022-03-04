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
	[Register ("BidDataHomePageView")]
	partial class BidDataHomePageView
	{
		[Outlet]
		UIKit.UIButton btnDeleteBids { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint btnViewHeight { get; set; }

		[Outlet]
		UIKit.UICollectionView collectionBids { get; set; }

		[Outlet]
		UIKit.UIView containerView { get; set; }

		[Action ("btnDeleteBidsClicked:")]
		partial void btnDeleteBidsClicked (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDeleteBids != null) {
				btnDeleteBids.Dispose ();
				btnDeleteBids = null;
			}

			if (collectionBids != null) {
				collectionBids.Dispose ();
				collectionBids = null;
			}

			if (containerView != null) {
				containerView.Dispose ();
				containerView = null;
			}

			if (btnViewHeight != null) {
				btnViewHeight.Dispose ();
				btnViewHeight = null;
			}
		}
	}
}
