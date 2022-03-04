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
    [Register ("BidChoicesViewController")]
    partial class BidChoicesViewController
    {
        [Outlet]
        UIKit.UIButton btnCalculateBid { get; set; }


        [Outlet]
        UIKit.UITextField edSeniority { get; set; }


        [Outlet]
        UIKit.UILabel lbChooseSeniority { get; set; }


        [Outlet]
        UIKit.UILabel lblMaxChoice { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segSelectSeniority { get; set; }


        [Action ("OnCalculateBidClickEvent:")]
        partial void OnCalculateBidClickEvent (Foundation.NSObject sender);


        [Action ("OnEditTextSeniorityEditting:")]
        partial void OnEditTextSeniorityEditting (Foundation.NSObject sender);


        [Action ("OnEditTextSeniorityTouchInside:")]
        partial void OnEditTextSeniorityTouchInside (Foundation.NSObject sender);


        [Action ("OnInputSeniorityChange:")]
        partial void OnInputSeniorityChange (Foundation.NSObject sender);


        [Action ("OnSegmentSeniorityChangeValue:")]
        partial void OnSegmentSeniorityChangeValue (Foundation.NSObject sender);


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