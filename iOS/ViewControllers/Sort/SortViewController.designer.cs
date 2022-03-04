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
    [Register ("SortViewController")]
    partial class SortViewController
    {
        [Outlet]
        UIKit.UIButton btnAdvancePay { get; set; }

        [Outlet]
        UIKit.UIButton btnAdvancePayBlock { get; set; }

        [Outlet]
        UIKit.UIButton btnAdvancePayDutty { get; set; }

        [Outlet]
        UIKit.UIButton btnAdvancePayPerDay { get; set; }

        [Outlet]
        UIKit.UIButton btnAdvancePayTafb { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockAmPm { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockDaysOff { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockPay { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockPayDutty { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockPerDiem { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockPmAm { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockVacPay { get; set; }

        [Outlet]
        UIKit.UIButton btnBlockWeekday { get; set; }

        [Outlet]
        UIKit.UIButton btnDoneBlockSort { get; set; }

        [Outlet]
        UIKit.UIButton btnEtops { get; set; }

        [Outlet]
        UIKit.UIButton btnLineNumber { get; set; }

        [Outlet]
        UIKit.UIButton btnPaySingle { get; set; }

        [Outlet]
        UIKit.UIScrollView scrollAdvance { get; set; }

        [Outlet]
        UIKit.UISegmentedControl sgSelectSort { get; set; }

        [Outlet]
        UIKit.UISwitch swAdvance { get; set; }

        [Outlet]
        UIKit.UISwitch swSingleAdvance { get; set; }

        [Outlet]
        UIKit.UITableView tvBlockSort { get; set; }

        [Outlet]
        UIKit.UIView viewBlockSort { get; set; }

        [Outlet]
        UIKit.UIView viewSinglePay { get; set; }

        [Action ("OnAdvancePayBlockClickEvent:")]
        partial void OnAdvancePayBlockClickEvent (Foundation.NSObject sender);

        [Action ("OnAdvancePayClickEvent:")]
        partial void OnAdvancePayClickEvent (Foundation.NSObject sender);

        [Action ("OnAdvancePayDuttyClickEvent:")]
        partial void OnAdvancePayDuttyClickEvent (Foundation.NSObject sender);

        [Action ("OnAdvancePayPerDayClickEvent:")]
        partial void OnAdvancePayPerDayClickEvent (Foundation.NSObject sender);

        [Action ("OnAdvancePayTafbClickEvent:")]
        partial void OnAdvancePayTafbClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockAmPmClickEvent:")]
        partial void OnBlockAmPmClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockAmPmClickEvetn:")]
        partial void OnBlockAmPmClickEvetn (Foundation.NSObject sender);

        [Action ("OnBlockDaysOffClickEvent:")]
        partial void OnBlockDaysOffClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockPayClickEvent:")]
        partial void OnBlockPayClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockPayDuttyClickEvent:")]
        partial void OnBlockPayDuttyClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockPerDiemClickEvent:")]
        partial void OnBlockPerDiemClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockPmAmClickEvent:")]
        partial void OnBlockPmAmClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockVacPayClickEvent:")]
        partial void OnBlockVacPayClickEvent (Foundation.NSObject sender);

        [Action ("OnBlockWeekdayClickEvent:")]
        partial void OnBlockWeekdayClickEvent (Foundation.NSObject sender);

        [Action ("OnDoneBlockSortClickEvent:")]
        partial void OnDoneBlockSortClickEvent (Foundation.NSObject sender);

        [Action ("OnEtopsClickEvent:")]
        partial void OnEtopsClickEvent (Foundation.NSObject sender);

        [Action ("OnLineNumberClickEvent:")]
        partial void OnLineNumberClickEvent (Foundation.NSObject sender);

        [Action ("OnPaySingleClickEvent:")]
        partial void OnPaySingleClickEvent (Foundation.NSObject sender);

        [Action ("OnSegSelectSortValueChange:")]
        partial void OnSegSelectSortValueChange (Foundation.NSObject sender);

        [Action ("OnSingleAdvanceChangeValue:")]
        partial void OnSingleAdvanceChangeValue (Foundation.NSObject sender);

        [Action ("OnSwitchAdvanceChangeValue:")]
        partial void OnSwitchAdvanceChangeValue (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}