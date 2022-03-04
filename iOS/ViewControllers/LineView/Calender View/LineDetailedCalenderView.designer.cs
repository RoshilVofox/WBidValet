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
    [Register ("LineDetailedCalenderView")]
    partial class LineDetailedCalenderView
    {
        [Outlet]
        UIKit.UIView baseViewCollection { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnArrowDown { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnArrowUp { get; set; }


        [Outlet]
        UIKit.UICollectionView CalenderColletionView { get; set; }


        [Outlet]
        UIKit.UIView SegBaseView { get; set; }


        [Outlet]
        UIKit.UISegmentedControl SegweekView { get; set; }


        [Action ("DownArrowClicked:")]
        partial void DownArrowClicked (Foundation.NSObject sender);


        [Action ("UpArrowClicked:")]
        partial void UpArrowClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}