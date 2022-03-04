// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Bidvalet.iOS
{
    [Register ("SubmitBidViewController")]
    partial class SubmitBidViewController
    {
        [Outlet]
        UIKit.UIButton btnLoginSubmitBid { get; set; }


        [Outlet]
        UIKit.UITextField edtCWAPassword { get; set; }


        [Outlet]
        UIKit.UITextField edtEmployeeNumber { get; set; }


        [Outlet]
        UIKit.UILabel lbCountReserve { get; set; }


        [Action ("OnLoginSubmitBidClickEvent:")]
        partial void OnLoginSubmitBidClickEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}