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
    [Register ("TripDetailsView")]
    partial class TripDetailsView
    {
        [Outlet]
        UIKit.UIBarButtonItem btnArrowDown { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnArrowUp { get; set; }


        [Outlet]
        UIKit.UITableView TripTableView { get; set; }


        [Action ("DownArrowClicked:")]
        partial void DownArrowClicked (Foundation.NSObject sender);


        [Action ("UpArrowclicked:")]
        partial void UpArrowclicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}