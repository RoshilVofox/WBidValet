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
    [Register ("CreateAccountTableViewController")]
    partial class CreateAccountTableViewController
    {
        [Outlet]
        UIKit.UIButton btnCancel { get; set; }


        [Outlet]
        UIKit.UIButton btnCreate { get; set; }


        [Outlet]
        UIKit.UIButton btnPrivacyPolicy { get; set; }


        [Outlet]
        UIKit.UIButton btnSetCarrier { get; set; }


        [Outlet]
        UIKit.UILabel lbCheckAccountExits { get; set; }


        [Outlet]
        UIKit.UILabel lblPasswordHeader { get; set; }


        [Outlet]
        UIKit.UILabel lblRePasswordHeader { get; set; }


        [Outlet]
        UIKit.UISegmentedControl segUserPosition { get; set; }


        [Outlet]
        UIKit.UISwitch swAcceptEmail { get; set; }


        [Outlet]
        UIKit.UISwitch swAcceptTerm { get; set; }


        [Outlet]
        UIKit.UITextField tfConfirmPass { get; set; }


        [Outlet]
        UIKit.UITextField tfEmail { get; set; }


        [Outlet]
        UIKit.UITextField tfEmailConfirm { get; set; }


        [Outlet]
        UIKit.UITextField tfFirstName { get; set; }


        [Outlet]
        UIKit.UITextField tfLastName { get; set; }


        [Outlet]
        UIKit.UITextField tfNumber { get; set; }


        [Outlet]
        UIKit.UITextField tfPassword { get; set; }


        [Outlet]
        UIKit.UITextField tfPhone { get; set; }


        [Outlet]
        UIKit.UIView ViewPassword { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint ViewPasswordConHeight { get; set; }


        [Outlet]
        UIKit.UIView ViewReenterPassword { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint ViewrePasswordConHeight { get; set; }


        [Outlet]
        UIKit.UIView ViewTermsAndCondition { get; set; }


        [Action ("LicenseAndPrivacyPolicyClicked:")]
        partial void LicenseAndPrivacyPolicyClicked (Foundation.NSObject sender);


        [Action ("OnCancelClickEvent:")]
        partial void OnCancelClickEvent (Foundation.NSObject sender);


        [Action ("OnCreateButtonClickEvent:")]
        partial void OnCreateButtonClickEvent (Foundation.NSObject sender);


        [Action ("OnSegmentUserPositionChangeValue:")]
        partial void OnSegmentUserPositionChangeValue (Foundation.NSObject sender);


        [Action ("OnSetCarrierButtonClickEvent:")]
        partial void OnSetCarrierButtonClickEvent (Foundation.NSObject sender);


        [Action ("OnSwitchAcceptEmailChangeValue:")]
        partial void OnSwitchAcceptEmailChangeValue (Foundation.NSObject sender);


        [Action ("OnSwitchAcceptTermChangeValue:")]
        partial void OnSwitchAcceptTermChangeValue (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}