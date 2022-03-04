// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Bidvalet.iOS
{
    [Register ("BidAwardView")]
    partial class BidAwardView
    {
        [Outlet]
        UIKit.UICollectionView collectionviewAwards { get; set; }


        [Outlet]
        UIKit.UILabel lblBidMonthInfo { get; set; }


        [Action ("btnCancelClicked:")]
        partial void btnCancelClicked (Foundation.NSObject sender);


        [Action ("btnRetriveclicked:")]
        partial void btnRetriveclicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}