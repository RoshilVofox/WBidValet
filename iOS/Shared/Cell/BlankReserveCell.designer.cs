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
    [Register ("BlankReserveCell")]
    partial class BlankReserveCell
    {
        [Outlet]
        UIKit.UIButton btnBlankBeforeReserve { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnReserveBeforeBlank { get; set; }


        [Action ("OnBlankBeforeReserveEvent:")]
        partial void OnBlankBeforeReserveEvent (Foundation.NSObject sender);


        [Action ("OnDeleteEvent:")]
        partial void OnDeleteEvent (Foundation.NSObject sender);


        [Action ("OnReserveBeforeBlankEvent:")]
        partial void OnReserveBeforeBlankEvent (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}