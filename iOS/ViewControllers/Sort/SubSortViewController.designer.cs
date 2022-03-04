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
    [Register ("SubSortViewController")]
    partial class SubSortViewController
    {
        [Outlet]
        UIKit.UIButton btnContinue { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UILabel lbBlockSortFirst { get; set; }


        [Outlet]
        UIKit.UILabel lbBlockSortFouth { get; set; }


        [Outlet]
        UIKit.UILabel lbBlockSortSecond { get; set; }


        [Outlet]
        UIKit.UILabel lbBlockSortThird { get; set; }


        [Outlet]
        UIKit.UILabel lbCorrectBasic { get; set; }


        [Outlet]
        UIKit.UILabel lbCorrectBlock { get; set; }


        [Outlet]
        UIKit.UILabel lblBlankBottom { get; set; }


        [Outlet]
        UIKit.UILabel lblreserveBottom { get; set; }


        [Outlet]
        UIKit.UILabel lbSelectedSort { get; set; }


        [Outlet]
        UIKit.UISegmentedControl SegBlankReservePriority { get; set; }


        [Outlet]
        UIKit.UISwitch SwitchBlankAtBottom { get; set; }


        [Outlet]
        UIKit.UISwitch SwitchReserveAtBottom { get; set; }


        [Outlet]
        UIKit.UIView ViewBlankAndReserve { get; set; }


        [Outlet]
        UIKit.UIView viewBlockSort { get; set; }


        [Action ("OnContinueClickEvent:")]
        partial void OnContinueClickEvent (Foundation.NSObject sender);


        [Action ("OnDeleteClickEvent:")]
        partial void OnDeleteClickEvent (Foundation.NSObject sender);


        [Action ("SegPriorityChanged:")]
        partial void SegPriorityChanged (Foundation.NSObject sender);


        [Action ("SwitchBlankAtBottomClicked:")]
        partial void SwitchBlankAtBottomClicked (Foundation.NSObject sender);


        [Action ("SwitchReserveAtBottomClicked:")]
        partial void SwitchReserveAtBottomClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}