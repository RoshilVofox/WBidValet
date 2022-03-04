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
    [Register ("CalculateBidViewController")]
    partial class CalculateBidViewController
    {
        [Outlet]
        UIKit.UIButton btnSubmitBid { get; set; }


        [Outlet]
        UIKit.UILabel lbCalculate { get; set; }


        [Outlet]
        UIKit.UILabel lbCalculatingCombo { get; set; }


        [Outlet]
        UIKit.UIProgressView progressCombo { get; set; }


        [Action ("OnSubmitBidClickEvent:")]
        partial void OnSubmitBidClickEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}