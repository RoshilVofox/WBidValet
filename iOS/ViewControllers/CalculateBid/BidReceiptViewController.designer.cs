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
    [Register ("BidReceiptViewController")]
    partial class BidReceiptViewController
    {
        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UIButton btnEmail { get; set; }


        [Outlet]
        UIKit.UIButton btnPrint { get; set; }


        [Outlet]
        UIKit.UIWebView webviewBidReceipt { get; set; }


        [Action ("OnDoneClickEvent:")]
        partial void OnDoneClickEvent (Foundation.NSObject sender);


        [Action ("OnEmailClickEvent:")]
        partial void OnEmailClickEvent (Foundation.NSObject sender);


        [Action ("OnPrintClickEvent:")]
        partial void OnPrintClickEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}