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
    [Register ("ExpiredViewController")]
    partial class ExpiredViewController
    {
        [Outlet]
        UIKit.UIButton btnPricePerMonth { get; set; }


        [Outlet]
        UIKit.UILabel lblFirstLine { get; set; }


        [Outlet]
        UIKit.UILabel lblSecondLine { get; set; }


        [Outlet]
        UIKit.UILabel lblSubscriptionMessage { get; set; }


        [Outlet]
        UIKit.UILabel lblThirdLine { get; set; }


        [Action ("OnSubscriptionEvent:")]
        partial void OnSubscriptionEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}