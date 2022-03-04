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
    [Register ("CommutableLinesCell")]
    partial class CommutableLinesCell
    {
        [Outlet]
        UIKit.UIButton btnCommuteLine { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnHome { get; set; }


        [Outlet]
        UIKit.UIButton btnNightInMiddle { get; set; }


        [Outlet]
        UIKit.UIButton btnWork { get; set; }


        [Outlet]
        UIKit.UILabel lbCommuteName { get; set; }


        [Action ("NoNightInMiddieClicked:")]
        partial void NoNightInMiddieClicked (Foundation.NSObject sender);


        [Action ("OnAnyEvent:")]
        partial void OnAnyEvent (Foundation.NSObject sender);


        [Action ("OnCommuteLineEvent:")]
        partial void OnCommuteLineEvent (Foundation.NSObject sender);


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnHomeEvent:")]
        partial void OnHomeEvent (Foundation.NSObject sender);


        [Action ("OnRonBothEvent:")]
        partial void OnRonBothEvent (Foundation.NSObject sender);


        [Action ("OnWorkEvent:")]
        partial void OnWorkEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}