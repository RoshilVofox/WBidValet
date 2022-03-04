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
    [Register ("PostionsPrioritiesViewController")]
    partial class PostionsPrioritiesViewController
    {
        [Outlet]
        UIKit.UIButton btCalculateBid { get; set; }


        [Outlet]
        UIKit.UIButton btNonePriority { get; set; }


        [Outlet]
        UIKit.UIButton btPriorityPositionFirst { get; set; }


        [Outlet]
        UIKit.UIButton btPriorityPositionSecond { get; set; }


        [Outlet]
        UIKit.UIButton btPriorityPositionThird { get; set; }


        [Outlet]
        UIKit.UITextField edChoosePriority { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segBidChoices { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segPriorities { get; set; }


        [Outlet]
        UIKit.UISwitch swAddReserve { get; set; }


        [Outlet]
        UIKit.UISwitch swReserveOnly { get; set; }


        [Action ("OnAddReserveChangeValue:")]
        partial void OnAddReserveChangeValue (Foundation.NSObject sender);


        [Action ("OnCalculateBidClickEvent:")]
        partial void OnCalculateBidClickEvent (Foundation.NSObject sender);


        [Action ("OnNonePriorityClickEvent:")]
        partial void OnNonePriorityClickEvent (Foundation.NSObject sender);


        [Action ("OnPriorityFirstClickEvent:")]
        partial void OnPriorityFirstClickEvent (Foundation.NSObject sender);


        [Action ("OnPrioritySecondClickEvent:")]
        partial void OnPrioritySecondClickEvent (Foundation.NSObject sender);


        [Action ("OnPriorityThirdClickEvent:")]
        partial void OnPriorityThirdClickEvent (Foundation.NSObject sender);


        [Action ("OnReserveOnlyChangeValue:")]
        partial void OnReserveOnlyChangeValue (Foundation.NSObject sender);


        [Action ("OnSegBidChoicesChangeValue:")]
        partial void OnSegBidChoicesChangeValue (Foundation.NSObject sender);


        [Action ("OnSegPrioritiesChangeValue:")]
        partial void OnSegPrioritiesChangeValue (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}