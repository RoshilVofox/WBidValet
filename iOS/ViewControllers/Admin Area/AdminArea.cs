
#region NameSpace
using System;
using Foundation;
using UIKit;
using System.Text.RegularExpressions;

#endregion
namespace Bidvalet.iOS
{
    public partial class AdminArea : UIViewController
    {
        public SubScriptionViewController _parentVC;
        public AdminArea()
            : base("AdminArea", null)
        {
        }

        #region Events
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SwitchCurrentMonth.On = GlobalSettings.IsCurrentMonthOn;
            SwitchQATest.On = GlobalSettings.QATest;
            SwithcSouthWestWifi.On = GlobalSettings.IsTestSouthWifiOn;
            SwitchOtherUser.On = GlobalSettings.IsDifferentUser;

            SwitchOtherUser.ValueChanged += SwitchOtherUser_ValueChanged;

            SwitchSeniorityList.On = GlobalSettings.IsNeedToDownloadSeniority;

            SegServer.SelectedSegment = GlobalSettings.IsVPSServiceOn ? 1 : 0;


            if (GlobalSettings.IsDifferentUser)
            {

                txtEmployeeNumber.Text = GlobalSettings.ModifiedEmployeeNumber;
            }


            txtEmployeeNumber.Enabled = GlobalSettings.IsDifferentUser;

            txtEmployeeNumber.ShouldReturn += (textField) =>
            {
                textField.ResignFirstResponder();
                return true;
            };


            // Perform any additional setup after loading the view, typically from a nib.
        }

        void SwitchOtherUser_ValueChanged(object sender, EventArgs e)
        {
            txtEmployeeNumber.Enabled = SwitchOtherUser.On;
        }

        partial void BtnDoneClicked(NSObject sender)
        {


            bool status = ValidateControls();
            if (status)
            {
				
				GlobalSettings.QAScrapPassword=txtPassword.Text.ToString();

                GlobalSettings.IsCurrentMonthOn = SwitchCurrentMonth.On;

                GlobalSettings.QATest = SwitchQATest.On;

                GlobalSettings.IsTestSouthWifiOn = SwithcSouthWestWifi.On;

                GlobalSettings.IsNeedToDownloadSeniority = SwitchSeniorityList.On ;

                GlobalSettings.IsVPSServiceOn = (SegServer.SelectedSegment == 1);
				NSUserDefaults.StandardUserDefaults.SetBool (GlobalSettings.IsVPSServiceOn, "IsVPSServiceOn");
                GlobalSettings.WBidAuthenticationServiceUrl = GlobalSettings.IsVPSServiceOn ? GlobalSettings.VPSAuthenticationServiceUrl : GlobalSettings.VofoxAuthenticationServiceUrl;

                GlobalSettings.IsDifferentUser = SwitchOtherUser.On;
                if (GlobalSettings.IsDifferentUser)
                {
                    GlobalSettings.ModifiedEmployeeNumber = txtEmployeeNumber.Text;
                }

                DismissViewController(true, null);
            }
        }

        partial void BtnDownloadBidClicked(NSObject sender)
        {
            this.DismissViewController(true, () => {
                MultipleBidsViewController objMultipleDownloadData = new MultipleBidsViewController();
                _parentVC.PresentViewController(objMultipleDownloadData, true, null);
            });

            


        }
        #endregion

        #region Methods

        private bool ValidateControls()
        {
            bool status = true;


            if (SwitchOtherUser.On)
            {

                if (txtEmployeeNumber.Text == string.Empty)
                {

                    DisplayAlertView(GlobalSettings.ApplicationName, "Please Enter EmployeeNumber");
                    status = false;

                }
                else
                {

                    if (!Regex.Match(txtEmployeeNumber.Text.Trim(), "^[e,E,x,X,0-9][0-9]*$").Success)
                    {
                        DisplayAlertView(GlobalSettings.ApplicationName, "Invalid Employee Number");
                        status = false;
                    }

                }
            }
            return status;
        }

        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        } 
        #endregion

    }
}

