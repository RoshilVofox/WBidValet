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
    [Register ("AuthorizationTestCaseViewController")]
    partial class AuthorizationTestCaseViewController
    {
        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UIButton btnPurchaseInApp { get; set; }


        [Outlet]
        UIKit.UILabel lbMessageError { get; set; }


        [Outlet]
        UIKit.UITextView tvMessageError { get; set; }


        [Action ("OnDoneButtonClickedEvent:")]
        partial void OnDoneButtonClickedEvent (Foundation.NSObject sender);


        [Action ("OnPurchaseInAppEvent:")]
        partial void OnPurchaseInAppEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}