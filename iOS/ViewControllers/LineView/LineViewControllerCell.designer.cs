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
    [Register ("LineViewControllerCell")]
    partial class LineViewControllerCell
    {
        [Outlet]
        UIKit.UIButton btnSelection { get; set; }


        [Outlet]
        UIKit.UILabel lblCellNumber { get; set; }


        [Outlet]
        UIKit.UILabel lblLineNumber { get; set; }


        [Outlet]
        UIKit.UICollectionView LinePropertyCollection { get; set; }


        [Outlet]
        UIKit.UICollectionView MiniCalenderCollectionView { get; set; }


        [Action ("btnSelectionClicked:")]
        partial void btnSelectionClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}