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
    [Register ("PositionsAvoidanceViewController")]
    partial class PositionsAvoidanceViewController
    {
        [Outlet]
        UIKit.UIButton btnCalculateBid { get; set; }


        [Outlet]
        UIKit.UITextField edBidAvoidOne { get; set; }


        [Outlet]
        UIKit.UITextField edBidAvoidThree { get; set; }


        [Outlet]
        UIKit.UITextField edBidAvoidTwo { get; set; }


        [Outlet]
        UIKit.UITextField edSeniority { get; set; }


        [Outlet]
        UIKit.UILabel lbSeniority { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segSelectSeniority { get; set; }


        [Action ("OnCalculateBidClickEvent:")]
        partial void OnCalculateBidClickEvent (Foundation.NSObject sender);


        [Action ("OnEDSeniorityChangedValue:")]
        partial void OnEDSeniorityChangedValue (Foundation.NSObject sender);


        [Action ("OnSegmentSeniorityChangeValue:")]
        partial void OnSegmentSeniorityChangeValue (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}