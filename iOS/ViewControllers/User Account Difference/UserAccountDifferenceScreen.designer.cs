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
    [Register ("UserAccountDifferenceScreen")]
    partial class UserAccountDifferenceScreen
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnUpdate { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }


        [Action ("CancelbuttonClicked:")]
        partial void CancelbuttonClicked (Foundation.NSObject sender);


        [Action ("UpdateButtonClicked:")]
        partial void UpdateButtonClicked (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}