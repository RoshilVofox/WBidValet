#region NameSpace

using System;

using Foundation;
using UIKit;
using System.Collections.Generic; 
#endregion

namespace Bidvalet.iOS
{
	public partial class UserAccountDifferenceScreen : UIViewController
	{

        #region Public Variables
        public List<KeyValuePair<string, string>> DifferenceList = new List<KeyValuePair<string, string>>();
        public bool IsFromMainView { get; set; }
        public UIViewController ParentController { get; set; }
        #endregion


        #region Private Varibales
        private UserDifferenceTableDataSource ObjDataSource;
        private LoadingOverlay loadingOverlay;

        
        #endregion

		public UserAccountDifferenceScreen (IntPtr handle) : base (handle)
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
            //DifferenceList.Add ("aa", "ss,ll");
            //DifferenceList.Add ("aa1", "ss1,ll1");
            ObjDataSource = new UserDifferenceTableDataSource(DifferenceList);
            tableView.Source = ObjDataSource;
            tableView.BackgroundColor = UIColor.Clear;

            tableView.ReloadData();

            // Perform any additional setup after loading the view, typically from a nib.
        }

        partial void UpdateButtonClicked(NSObject sender)
        {
            try
            {

                loadingOverlay = new LoadingOverlay(this.View.Bounds, "Updating \n Please wait..");
                this.View.AddSubview(loadingOverlay);
                InvokeInBackground(() =>
                {

                    ServerUserInformation sUserInformation = new ServerUserInformation();
                    sUserInformation.EmpNum = int.Parse(ObjDataSource.LocalSelectedUser.EmpNo.ToLower().Replace("x", "").Replace("e", ""));
                    sUserInformation.FirstName = ObjDataSource.LocalSelectedUser.FirstName;
                    sUserInformation.LastName = ObjDataSource.LocalSelectedUser.LastName;
                    sUserInformation.Position = (ObjDataSource.LocalSelectedUser.Position == "FA") ? 3 : 4;
                    sUserInformation.Email = ObjDataSource.LocalSelectedUser.Email;
                    sUserInformation.CellPhone = ObjDataSource.LocalSelectedUser.CellNumber;
                    sUserInformation.CarrierNum = ObjDataSource.LocalSelectedUser.CellCarrier;
                    sUserInformation.AcceptEmail = ObjDataSource.LocalSelectedUser.isAcceptMail;
                    sUserInformation.AppNum = (int)AppNum.BidValet;
                    CustomServiceResponse customServiceResponse = RestHelper.UpdateAllUserDetails(sUserInformation);
                    if (customServiceResponse != null)
                    {
                        if (customServiceResponse.status)
                        {
                            GlobalSettings.UserInfo.CellCarrier = sUserInformation.CarrierNum;
                            GlobalSettings.UserInfo.FirstName = ObjDataSource.LocalSelectedUser.FirstName;
                            GlobalSettings.UserInfo.LastName = ObjDataSource.LocalSelectedUser.LastName;
                            GlobalSettings.UserInfo.Position = ObjDataSource.LocalSelectedUser.Position;
                            GlobalSettings.UserInfo.Email = ObjDataSource.LocalSelectedUser.Email;
                            GlobalSettings.UserInfo.CellNumber = ObjDataSource.LocalSelectedUser.CellNumber;
                            WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);
                            GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation>(WBidHelper.WBidUserFilePath);

                            if (IsFromMainView)
                             {
                                InvokeOnMainThread(() =>
                                {
                                    loadingOverlay.Hide();
                                    UIAlertView alert = new UIAlertView(GlobalSettings.ApplicationName, "Successfully Updated", null, "OK", null);
                                    alert.Clicked += (senderobj, e) =>
                                    {
                                        this.DismissViewController(true, null);
                                    };
                                    alert.Show();
                                });

                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    loadingOverlay.Hide();

                                    if (ParentController != null && ParentController.GetType().Name == "CreateAccountTableViewController")
                                    {
                                        ((CreateAccountTableViewController)ParentController).DismissEditAndNavigateToAccount(this);
                                    }
                                    else if (ParentController != null && ParentController.GetType().Name == "LoginViewController")
                                    {
                                        ((LoginViewController)ParentController).DismissEditAndNavigateToAccount(this);
                                    }
                                   
                                    
                                 //   this.ParentController.DismissViewController()
                                    //RedirectTodownloadView();
                                });
                            }



                        }
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            DisplayAlertView(GlobalSettings.ApplicationName, "Please try again.");
                        });

                    }
                });
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {
                    loadingOverlay.Hide();
                    UIAlertView alert = new UIAlertView(GlobalSettings.ApplicationName, "Please try again.", null, "OK", null);
                    alert.Clicked += (senderobj, e) =>
                    {
                        this.DismissViewController(true, null);
                    };
                });

            }

        }

        partial void CancelbuttonClicked(NSObject sender)
        {
            this.DismissViewController(true, null);
        } 
        #endregion

        #region Private Methods

        #region Navigation Method
        private void RedirectTodownloadView()
        {
            DownloadBidDataViewController objDownloadData = Storyboard.InstantiateViewController("DownloadBidDataViewController") as DownloadBidDataViewController;
            if (this.NavigationController != null && this.NavigationController.NavigationItem != null)
            {
                this.NavigationController.NavigationItem.HidesBackButton = false;
            }

            this.PresentViewController(objDownloadData, true, null);

        }
        #endregion

        #region DisplayAlertView
        private void DisplayAlertView(string caption, String message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        }
        #endregion 
        #endregion
	}
}

