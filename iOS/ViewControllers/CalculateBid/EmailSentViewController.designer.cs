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
    [Register ("EmailSentViewController")]
    partial class EmailSentViewController
    {
        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UILabel lbDepricationSent { get; set; }


        [Outlet]
        UIKit.UILabel lbSentTo { get; set; }


        [Action ("OnDoneClickEvent:")]
        partial void OnDoneClickEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}