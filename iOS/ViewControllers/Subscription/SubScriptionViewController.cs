
#region NameSpace
using System;
using Foundation;
using UIKit;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Bidvalet.Model;
using Bidvalet.Business;
using System.Collections.ObjectModel;
using CoreGraphics;
using MessageUI;
using System.Text;

#endregion

namespace Bidvalet.iOS
{
    public partial class SubScriptionViewController : BaseViewController
    {
        #region Variables
        public DateTime expiredTime;
        public bool isFirstTimeUsing;
        private bool _isNeedToCheckInternetCOnnection;
        LoadingOverlay loadingOverlay;
        bool isNeedtoAllowDataDownload;
        // NSObject notificationReload;
        bool isLoaded;

        NSObject nsObserver;
        NSObject nsHelpDocumentObserver;
        NSObject nsHelpVideoObjserver;
        private UIAlertView alertVWCommon;
        #endregion
        //UILabel lblSynchStatus;

        public SubScriptionViewController(IntPtr handle)
            : base(handle)
        {
        }

        #region Events
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }



        partial void btnBidAwardClicked(NSObject sender)
        {
            Console.WriteLine("Bid award clicked");
            BidAwardView objBidaward = new BidAwardView();
            objBidaward.objSubscriptionView = this;
            NavigationController.PushViewController(objBidaward, true);

            //this.PresentViewController(objBidaward, true, null);

        }

        public override void ViewDidLoad()
        {

            base.ViewDidLoad();
            isNeedtoAllowDataDownload = true;
            lblVersionNumber.Text = "V " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //loading column definition
            GlobalSettings.ColumnDefinition = (List<ColumnDefinition>)XmlHelper.DeserializeFromXml<ColumnDefinitions>(WBidHelper.GetWBidColumnDefinitionFilePath());




            //			SidebarController  ObjAide= new SidebarController(this, this, new SideMenuController());
            //			ObjAide.MenuLocation = MenuLocations.Left;

            //var frame = new CGRect(100, 430, 300, 50);
            //            var frame = new CGRect(40, 410 , 300, 50);
            //            lblSynchStatus = new UILabel(frame);
            //            lblSynchStatus.Lines = 3;
            //            lblSynchStatus.Font = UIFont.FromName(lblSynchStatus.Font.Name, 12f);
            //            lblSynchStatus.TextAlignment = UITextAlignment.Center;
            //            View.Add(lblSynchStatus);





            //GlobalSettings.IsCurrentMonthOn = true;
            // GlobalSettings.IsVPSServiceOn = true;




            //AuthorizationServiceViewController AuthorizationServiceViewController = Storyboard.InstantiateViewController ("AuthorizationServiceViewController") as AuthorizationServiceViewController;
            //NavigationController.PushViewController(AuthorizationServiceViewController, true);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBarHidden = true;

            //if (isLoaded)
            //{
            //    var value = NSUserDefaults.StandardUserDefaults["PrivacyAccepted"];
            //    bool isPrivacyAccepted = false;
            //    if (value != null)
            //    {
            //        isPrivacyAccepted = true;
            //    }


            //    if (!isPrivacyAccepted)
            //    {
            //        HandlePrivacyPolicy();

            //    }
            //}

            nsObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("UpdateUI"), UpdateView);
            nsHelpDocumentObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("CallHelpDocument"), CallDocumentHelpView);
            nsHelpVideoObjserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("CallHelpVideo"), CallVideoHelpView);

        }



        public void CallVideoHelpView(NSNotification n)
        {
            this.PerformSelector(new ObjCRuntime.Selector("callVideoView:"), (NSString)n.Object.ToString(), 1);
        }
        public void CallDocumentHelpView(NSNotification n)
        {



            this.PerformSelector(new ObjCRuntime.Selector("callDocView:"), (NSString)n.Object.ToString(), 1);
        }

        [Export("callDocView:")]
        void callDocView(NSString data)
        {
            WebViewForLicence webViewController = new WebViewForLicence();

            webViewController.ViewType = WebViewForLicence.WebType.Document;
            //webViewController.ViewType = WebViewForLicence.WebType.Video;
            webViewController.DocumentType = (string)data;
            //webViewController.ModalPresentationStyle= UIModalPresentationStyle.FormSheet;
            this.View.BringSubviewToFront(webViewController.View);
            this.PresentViewController(webViewController, true, null);
        }

        [Export("callVideoView:")]
        void callVideoView(NSString data)
        {
            WebViewForLicence webViewController = new WebViewForLicence();


            webViewController.ViewType = WebViewForLicence.WebType.Video;
            webViewController.DocumentType = (string)data;
            //webViewController.ModalPresentationStyle= UIModalPresentationStyle.FormSheet;
            this.View.BringSubviewToFront(webViewController.View);
            this.PresentViewController(webViewController, true, null);
        }
        public override void ViewWillDisappear(bool animated)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(nsObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(nsHelpVideoObjserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(nsHelpDocumentObserver);
            ;
        }

        partial void ContactUsButtonClicked(NSObject sender)
        {

            int typeOfInternetConnection = InternetHelper.CheckInterNetConnection();

            if (typeOfInternetConnection == (int)InternetType.NoInternet)
            {

                RedirectToMessageView((int)AuthStaus.VPSDownAlert);

            }
            else if (typeOfInternetConnection == (int)InternetType.Air)
            {

                RedirectToMessageView((int)AuthStaus.SouthWestConnectionAlert);

            }
            //Grount type internet
            else if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
            {

                LoadContactUs();
            }
            else if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
            {
                DisplayAlertView(GlobalSettings.ApplicationName, GlobalMessages.LimittedInternet);
            }

        }

        private void LoadContactUs()
        {
           
            MFMailComposeViewController mailController;
            if (MFMailComposeViewController.CanSendMail)
            {
                mailController = new MFMailComposeViewController();

                mailController.SetToRecipients(new string[] { GlobalSettings.SupportEmail });
                mailController.SetSubject("WBidValet Help");

                string empNum = string.Empty;
                string baseName = string.Empty;
                string position = string.Empty;
                string phone = string.Empty;


                if (GlobalSettings.UserInfo!= null)
                {
                    empNum = string.IsNullOrEmpty(GlobalSettings.UserInfo.EmpNo) ? string.Empty : GlobalSettings.UserInfo.EmpNo;
                    baseName = string.IsNullOrEmpty(GlobalSettings.UserInfo.Domicile) ? string.Empty : GlobalSettings.UserInfo.Domicile;
                    position = string.IsNullOrEmpty(GlobalSettings.UserInfo.Position) ? string.Empty : GlobalSettings.UserInfo.Position;
                    phone = string.IsNullOrEmpty(GlobalSettings.UserInfo.CellNumber) ? string.Empty : GlobalSettings.UserInfo.CellNumber;


                }
                StringBuilder content = new StringBuilder();
                content.Append("\n\n");
                content.Append(string.Format("\nEmpnum :{0}", empNum));
                content.Append(string.Format("\nBase :{0}", baseName));
                content.Append(string.Format("\nPosition :{0}", position));
                content.Append(string.Format("\nCell Phone :{0}", phone));
                content.Append(string.Format("\nios Version :{0}", UIDevice.CurrentDevice.SystemVersion));
                mailController.SetMessageBody(content.ToString(), false);
               
                //mailController.BecomeFirstResponder();
                mailController.Finished += (object s, MFComposeResultEventArgs args) =>
                {
                    Console.WriteLine(args.Result.ToString());
                    args.Controller.DismissViewController(true, null);
                };
                this.PresentViewController(mailController, true, null);
            }
        }
        partial void HelpButtonClicked(NSObject sender)
        {
            SideMenuViewController OBjSide = new SideMenuViewController();
            OBjSide.ProvidesPresentationContextTransitionStyle = true;
            OBjSide.DefinesPresentationContext = true;
            OBjSide.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            this.PresentViewController(OBjSide, false, null);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            //COde for  last sync





            if (GlobalSettings.IsVPSServiceOn)
            {
                GlobalSettings.WBidAuthenticationServiceUrl = GlobalSettings.VPSAuthenticationServiceUrl;
            }
            else
            {
                GlobalSettings.WBidAuthenticationServiceUrl = GlobalSettings.VofoxAuthenticationServiceUrl;

            }



            btnEditUserAccount.Alpha = 0;
            btnNewBidReceipt.Alpha = 0;
            lbSmallTitle.Alpha = 0;
            lbMoreDetails.Alpha = 0;
            lbExpiredDate.Alpha = 0;
            //expiredTime = DateTime.Now;
            btnNewBidReceipt.Enabled = false;


            var value = NSUserDefaults.StandardUserDefaults["PrivacyAccepted"];

            bool isPrivacyAccepted = false;
            if (value != null)
            {
                isPrivacyAccepted = true;
            }


            if (!isPrivacyAccepted)
            {
                HandlePrivacyPolicy();

            }
            else
            {
                HomeViewLoading();

            }

            //if (GlobalSettings.IsNeedToReload)
            //{
            //    GlobalSettings.IsNeedToReload = false;
            //    if (GlobalSettings.UserInfo != null)
            //    {
            //        ReloadHomeView();
            //    }
            //}


            if (GlobalSettings.UserInfo != null)
            {
                if (!string.IsNullOrEmpty(GlobalSettings.UserInfo.LastSyncInfo))
                {
                    lblSyncStatus.Text = "Your last sync was: " + GlobalSettings.UserInfo.LastSyncInfo;
                }
                else
                {
                    lblSyncStatus.Text = string.Empty;
                }
                ReloadHomeView();
            }

            if (GlobalSettings.IsNeedToRedirectToBase)
            {
                GlobalSettings.isAwardDownload = false;
                GlobalSettings.IsNeedToRedirectToBase = false;
                GlobalSettings.IsDownloadProcess = true;
                // GlobalSettings.UserInfo.EmpNo="2222222";
                SelectBaseViewController selectBaseViewController = Storyboard.InstantiateViewController("SelectBaseViewController") as SelectBaseViewController;
                NavigationController.PushViewController(selectBaseViewController, false);

            }


        }
        partial void OnEditUserEvent(Foundation.NSObject sender)
        {

            CreateAccountTableViewController createAccountView = Storyboard.InstantiateViewController("CreateAccountTableViewController") as CreateAccountTableViewController;
            createAccountView.IsFoundAccount = true;//true;
            createAccountView.IsFromMainView = true;
            createAccountView.EmpNumber = int.Parse(GlobalSettings.UserInfo.EmpNo);
            createAccountView.ParentController = this;

            this.PresentViewController(createAccountView, true, null);
        }

        partial void OnNewBidEvent(Foundation.NSObject sender)
        {
            BidReceiptViewController viewController = Storyboard.InstantiateViewController("BidReceiptViewController") as BidReceiptViewController;
            this.NavigationController.PushViewController(viewController, true);
        }

        public void showBaseSelectionView()
        {
            GlobalSettings.isAwardDownload = true;
            //Checking internet connection Type
            var typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
            // typeOfInternetConnection = (int)InternetType.Air;
            //No internet connection
            if (typeOfInternetConnection == (int)InternetType.NoInternet)
            {
                //DisplayAlertView(GlobalSettings.ApplicationName, "Sorry!\n\nYou do not have an Internet connection.\n\nYou will need to establish an internet connection to continue.");
                DisplayAlertView(GlobalSettings.ApplicationName, Constants.VPSDownAlert);

            }
            else if (typeOfInternetConnection == (int)InternetType.Air)
            {
                DisplayAlertView(GlobalSettings.ApplicationName, Constants.SouthWestConnectionAlert);

            }
            else
            {
                GlobalSettings.isAwardDownload = false;
                GlobalSettings.IsDownloadProcess = true;
                SelectBaseViewController selectBaseViewController = Storyboard.InstantiateViewController("SelectBaseViewController") as SelectBaseViewController;
                NavigationController.PushViewController(selectBaseViewController, true);
            }

        }

        partial void OnStartEvent(Foundation.NSObject sender)
        {

            //LoadingOverlay _loadingOverlay;
            /*ConstraintsChangeViewController viewController = Storyboard.InstantiateViewController("ConstraintsChangeViewController") as ConstraintsChangeViewController;
            this.NavigationController.PushViewController(viewController,true);
            */
            InvokeInBackground(() =>
           {
               try
               {


                   if (_isNeedToCheckInternetCOnnection)
                   {
                       LoadHomeView();

                       if (_isNeedToCheckInternetCOnnection)
                           return;
                   }

                   if (!isFirstTimeUsing && DateTime.Compare(expiredTime, DateTime.Now) < 0 && ((GlobalSettings.UserInfo.IsMonthlySubscribed || GlobalSettings.UserInfo.IsMonthlySubscribed ||
                       GlobalSettings.UserInfo.IsCBYearlySubscribed || GlobalSettings.UserInfo.IsCBMonthlySubscribed) && expiredTime.AddDays(10) < DateTime.Now))

                   {

                       InvokeOnMainThread(() =>
                       {
                           ExpiredViewController expiredViewController = Storyboard.InstantiateViewController("ExpiredViewController") as ExpiredViewController;
                           expiredViewController.expiredTime = expiredTime;
                           // notificationReload = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ReloadHomeView"), ReloadHomeView);
                           NavigationController.PushViewController(expiredViewController, true);
                       });
                   }
                   else
                   {

                       //string fileName = CheckBidFileExist();
                       List<string> linefilenames = Directory.EnumerateFiles(WBidHelper.GetAppDataPath(), "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
                        .Where(s => s.ToLower().EndsWith(".wbl")).ToList();
                       if (linefilenames.Count > 0)
                       {

                           InvokeOnMainThread(() =>
                           {
                               //BidDataHomePageView objBidaHomePage = Storyboard.InstantiateViewController("BidDataHomePageView") as BidDataHomePageView;

                               //PushViewController(objBidaHomePage, true);


                               BidDataHomePageView objBidaHomePage = new BidDataHomePageView();
                               NavigationController.PushViewController(objBidaHomePage, true);



                               //var alertVW = new UIAlertView(GlobalSettings.ApplicationName, "The Bid Data you have requested already exists", null, "Download Again", new string[] { "Manual Sort Current Bid List", "Auto Sort with Current Data" });
                               //alertVW.Clicked += (object senderObj, UIButtonEventArgs e) =>
                               //{
                               //    int index = (int)e.ButtonIndex;
                               //    //Manual Sort current bid list. this will redirect to Bid list directly
                               //    if (index == 1)
                               //    {
                               //        loadingOverlay = new LoadingOverlay(View.Bounds, "Loading...");
                               //        AuthorizationTestCaseViewController ConstraintsViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
                               //        ConstraintsViewController.FileName = fileName;
                               //        if (GlobalSettings.WBidINIContent == null || GlobalSettings.WBidINIContent.Cities == null)
                               //        {
                               //            Constants.listCities = new List<string>();
                               //        }
                               //        else
                               //        {
                               //            Constants.listCities = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                               //        }
                               //        View.Add(loadingOverlay);
                               //        InvokeInBackground(() =>
                               //        {
                               //            ConstraintsViewController.LoadExistingBidDetails(fileName);
                               //            WBidHelper.CalculateBidlist();
                               //            InvokeOnMainThread(() =>
                               //                 {




                               //                     //this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                               //                     loadingOverlay.Hide();
                               //                     AuthorizationTestCaseViewController ConstraintsViewController1 = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
                               //                     ConstraintsViewController1.needTobyPassingScreen = true;
                               //                     ConstraintsViewController1.buttonTitle = Constants.GO_TO_CONSTRAINTS;
                               //                     ConstraintsViewController1.messageError = Constants.ErrorMessages.ElementAt((int)AuthStaus.Filters - 1);
                               //                     ConstraintsViewController1.topBarTitle = Enumerable.ElementAt(Constants.listTitleTopBar, (int)AuthStaus.Filters - 1);
                               //                     ConstraintsViewController1.numberRow = (int)AuthStaus.Filters;
                               //                     ConstraintsViewController1.FileName = fileName;
                               //                     PushViewController(ConstraintsViewController1, false);

                               //                     // LineViewController ObjLine = new LineViewController();
                               //                     //PushViewController(ObjLine, true);

                               //                 });
                               //        });


                               //    }
                               //    //Auto sort with Current bid data, This will redirect to the sort and filter selction page
                               //    else if (index == 2)
                               //    {

                               //        // LoadExistingBidDetails(fileName);

                               //        GlobalSettings.IsDownloadProcess = false;
                               //        AuthorizationTestCaseViewController ConstraintsViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
                               //        ConstraintsViewController.buttonTitle = Constants.GO_TO_CONSTRAINTS;
                               //        ConstraintsViewController.messageError = Constants.ErrorMessages.ElementAt((int)AuthStaus.Filters - 1);
                               //        ConstraintsViewController.topBarTitle = Enumerable.ElementAt(Constants.listTitleTopBar, (int)AuthStaus.Filters - 1);
                               //        ConstraintsViewController.numberRow = (int)AuthStaus.Filters;
                               //        ConstraintsViewController.FileName = fileName;
                               //        PushViewController(ConstraintsViewController, true);


                               //    }
                               //    //New download
                               //    else
                               //    {
                               //        GlobalSettings.isAwardDownload = false;                                  //Checking internet connection Type
                               //        var typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
                               //        // typeOfInternetConnection = (int)InternetType.Air;
                               //        //No internet connection
                               //        if (typeOfInternetConnection == (int)InternetType.NoInternet)
                               //        {
                               //            //DisplayAlertView(GlobalSettings.ApplicationName, "Sorry!\n\nYou do not have an Internet connection.\n\nYou will need to establish an internet connection to continue.");
                               //            DisplayAlertView(GlobalSettings.ApplicationName, Constants.VPSDownAlert);

                               //        }
                               //        else if (typeOfInternetConnection == (int)InternetType.Air)
                               //        {
                               //            DisplayAlertView(GlobalSettings.ApplicationName, Constants.SouthWestConnectionAlert);

                               //        }
                               //        else
                               //        {
                               //            GlobalSettings.IsDownloadProcess = true;
                               //            // GlobalSettings.UserInfo.EmpNo="2222222";
                               //            SelectBaseViewController selectBaseViewController = Storyboard.InstantiateViewController("SelectBaseViewController") as SelectBaseViewController;
                               //            NavigationController.PushViewController(selectBaseViewController, true);
                               //        }


                               //    }
                               //};
                               //alertVW.Show();
                           });
                       }
                       else
                       {

                           InvokeOnMainThread(() =>
                           {
                               GlobalSettings.isAwardDownload = false;
                               GlobalSettings.IsDownloadProcess = true;

                               SelectBaseViewController selectBaseViewController = Storyboard.InstantiateViewController("SelectBaseViewController") as SelectBaseViewController;
                               NavigationController.PushViewController(selectBaseViewController, true);
                           });
                       }
                   }
               }
               catch (Exception ex)
               {
                   InvokeOnMainThread(() =>
                   {
                       throw ex;
                   });
               }
           });

        }



        #endregion

        #region Methods

        /// <summary>
        /// Handle the privacy policy alert view
        /// </summary>
        private void HandlePrivacyPolicy()
        {
            alertVWCommon = new UIAlertView(GlobalSettings.ApplicationName, "WBidValet collects personal information and information regarding the problem with the app.  The specific details are in our privacy policy.", null, "NO", new string[] { "OK" });

            UIButton OBjbutton = new UIButton(new CoreGraphics.CGRect(0, 40, 100, 60));
            OBjbutton.SetTitle("Privacy Policy", UIControlState.Normal);
            OBjbutton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            OBjbutton.TouchUpInside += OBjbutton_TouchUpInside;

            alertVWCommon.SetValueForKey(OBjbutton, (NSString)"accessoryView");
            //UIView aa = new UIView();
            //aa.Add(OBjbutton);
            //alertVW.Add(aa);

            alertVWCommon.Clicked += (object senderObj, UIButtonEventArgs e) =>
            {
                int index = (int)e.ButtonIndex;
                if (index == 1)
                {
                    //Createfile/SetFalag
                    SetPrivacySettings();
                    HomeViewLoading();

                }
                else if (index == 0)
                {
                    var alertPrivacyVW = new UIAlertView(GlobalSettings.ApplicationName, "In order to use WBidValet, you will have to agree to our privacy policy, which includes collecting your personal information and information regarding problems with the app.", null, "NO", new string[] { "OK" });
                    alertPrivacyVW.Clicked += (object senderObject, UIButtonEventArgs ev) =>
                    {
                        index = (int)ev.ButtonIndex;
                        if (index == 1)
                        {
                            //Createfile/SetFalag
                            SetPrivacySettings();
                            HomeViewLoading();
                        }
                        else
                        {
                            //Again alling the same methid
                            HandlePrivacyPolicy();
                        }


                    };
                    alertPrivacyVW.Show();
                }
            };
            alertVWCommon.Show();
        }

        void OBjbutton_TouchUpInside(object sender, EventArgs e)
        {

            alertVWCommon.DismissWithClickedButtonIndex(((nint)(-1)), true);
            isLoaded = true;


            this.PerformSelector(new ObjCRuntime.Selector("NavigateToPrivacy"), null, 0.7);
        }

        [Export("NavigateToPrivacy")]
        void NavigateToPrivacy()
        {
            WebViewForLicence webViewController = new WebViewForLicence();
            // this.View.BringSubviewToFront(webViewController.View);

            webViewController.ViewType = WebViewForLicence.WebType.Policy;
            //webViewController.ModalPresentationStyle= UIModalPresentationStyle.FormSheet;
            this.PresentViewController(webViewController, true, null);
        }




        /// <summary>
        /// Set up home view based on net connection and local account and subscription date
        /// </summary>
        private void HomeViewLoading()
        {
            //Add loading panel
            loadingOverlay = new LoadingOverlay(View.Bounds, "Loading...");
            View.Add(loadingOverlay);
            InvokeInBackground(() =>
            {
                LoadHomeView();
            });
            //lbMoreDetails
            //  lbSmallTitle

        }

        void SetPrivacySettings()
        {

            NSUserDefaults.StandardUserDefaults.SetValueForKey(new NSNumber(true), (NSString)"PrivacyAccepted");


        }

        void SetupView()
        {
            if (isFirstTimeUsing)
            {
                // hide buttons and lable
                btnEditUserAccount.Alpha = 0;
                btnNewBidReceipt.Alpha = 0;
                lbSmallTitle.Alpha = 0;
                lbMoreDetails.Alpha = 0;
                lbExpiredDate.Alpha = 0;
            }
            else
            {


                if (DateTime.Compare(expiredTime, DateTime.Now) < 0)
                {

                    if ((GlobalSettings.UserInfo.IsMonthlySubscribed || GlobalSettings.UserInfo.IsMonthlySubscribed ||
                        GlobalSettings.UserInfo.IsCBYearlySubscribed || GlobalSettings.UserInfo.IsCBMonthlySubscribed) && expiredTime.AddDays(10d) >= DateTime.Now)
                    {
                        lbSmallTitle.Text = GlobalSettings.UserInfo.TopSubscriptionLine;
                        lbExpiredDate.TextColor = Colors.BidGreen;
                        lbExpiredDate.Text += " (-" + (DateTime.Now - expiredTime).Days + "Days)";
                    }
                    else
                    {
                        // expired

                        lbSmallTitle.Text = Constants.SubScriptionExpired;
                        lbExpiredDate.TextColor = Colors.BidRed;
                    }
                }
                else
                {
                    //change color
                    lbExpiredDate.TextColor = Colors.BidGreen;
                    lbSmallTitle.Text = Constants.SubScriptionExpires;
                    // hide text
                    lbMoreDetails.Alpha = 0;
                }
                btnEditUserAccount.Alpha = 1;
                btnNewBidReceipt.Alpha = 1;
                HandleBidReceiptButton();
                lbSmallTitle.Alpha = 1;
                lbMoreDetails.Alpha = 1;
                lbExpiredDate.Alpha = 1;
            }
            // style for buttons
            UIHelpers.StyleForButtons(new UIButton[] { btnEditUserAccount, btnStart, btnNewBidReceipt, btnhelp, btnContactUs, btnBidAward });
        }




        void UpdateView(NSNotification obj)
        {

            if (GlobalSettings.UserInfo != null)
            {
                expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;

                lbExpiredDate.Text = expiredTime.ToString("MM/dd/yyyy");
                if (DateTime.Compare(expiredTime, DateTime.Now) < 0)
                {
                    // expired
                    if ((GlobalSettings.UserInfo.IsMonthlySubscribed || GlobalSettings.UserInfo.IsMonthlySubscribed ||
                        GlobalSettings.UserInfo.IsCBYearlySubscribed || GlobalSettings.UserInfo.IsCBMonthlySubscribed) && expiredTime.AddDays(10) >= DateTime.Now)
                    {
                        lbSmallTitle.Text = GlobalSettings.UserInfo.TopSubscriptionLine;
                        lbExpiredDate.TextColor = Colors.BidRed;
                    }
                    else
                    {
                        lbSmallTitle.Text = Constants.SubScriptionExpired;

                        lbExpiredDate.TextColor = Colors.BidRed;
                    }
                }
                else
                {
                    //change color
                    lbExpiredDate.TextColor = Colors.BidGreen;
                    lbSmallTitle.Text = Constants.SubScriptionExpires;
                    // hide text
                    lbMoreDetails.Alpha = 0;
                }
                btnEditUserAccount.Alpha = 1;
                btnNewBidReceipt.Alpha = 1;
                HandleBidReceiptButton();
                lbSmallTitle.Alpha = 1;
                lbMoreDetails.Alpha = 1;
                lbExpiredDate.Alpha = 1;

                // style for buttons
                UIHelpers.StyleForButtons(new UIButton[] { btnEditUserAccount, btnStart, btnNewBidReceipt });
            }
        }

        private ServerUserInformation GetUserDetails()
        {
            ServerUserInformation objUserInfo = null;
            try
            {

                var url = GlobalSettings.WBidAuthenticationServiceUrl + "/GetAllUserAccountDetails/";
                url = url + GlobalSettings.UserInfo.EmpNo.ToString() + "/" + (int)AppNum.BidValet;
                objUserInfo = RestHelper.GetResponse<ServerUserInformation>(url);

            }
            catch (Exception ex)
            {
            }
            return objUserInfo;
        }



        private void ReloadHomeView()
        {
            expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;
            lbExpiredDate.Text = expiredTime.ToString("MM/dd/yyyy");
            SetupView();
            //NSNotificationCenter.DefaultCenter.RemoveObserver(notificationReload);

        }
        /// <summary>
        /// Set Up the home view
        /// </summary>
        private void LoadHomeView()
        {

            //File.Delete (WBidHelper.WBidUserFilePath);
            //GlobalSettings.UserInfo = null;

            try
            {


                int typeOfInternetConnection;
                _isNeedToCheckInternetCOnnection = false;
                isFirstTimeUsing = true;

                //Checking internet connection Type
                typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
                // typeOfInternetConnection = (int)InternetType.Air;
                //No internet connection
                if (typeOfInternetConnection == (int)InternetType.NoInternet)
                {
                    if (CheckLocalAccountExist() && CheckBidFileExist() != string.Empty)
                    {
                        _isNeedToCheckInternetCOnnection = false;
                        isNeedtoAllowDataDownload = false;
                    }
                    else
                    {
                        _isNeedToCheckInternetCOnnection = true;
                        isNeedtoAllowDataDownload = true;
                    }

                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                        if (_isNeedToCheckInternetCOnnection)
                        {
                            DisplayAlertView(GlobalSettings.ApplicationName, Constants.VPSDownAlert);
                            //DisplayAlertView(GlobalSettings.ApplicationName ,"Sorry!\n\nYou do not have an Internet connection.\n\nYou will need to establish an internet connection to continue." );	
                        }

                        //RedirectToMessageView((int)AuthStaus.No_internet);
                    });
                }

                //Grount type internet
                else if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
                {


                    if (CheckLocalAccountExist())
                    {
                        //ReadUserDetails ();
                        isFirstTimeUsing = false;

                        if (GlobalSettings.UserInfo != null)
                        {
                            ServerUserInformation objUserInfo = GetUserDetails();
                            if (objUserInfo != null && objUserInfo.EmpNum != 0)
                            {
                                //if (objUserInfo.WBExpirationDate != GlobalSettings.UserInfo.PaidUntilDate)
                                GlobalSettings.UserInfo.PaidUntilDate = objUserInfo.WBExpirationDate;
                                GlobalSettings.UserInfo.IsFree = objUserInfo.IsFree;
                                GlobalSettings.UserInfo.IsMonthlySubscribed = objUserInfo.IsMonthlySubscribed;
                                GlobalSettings.UserInfo.IsYearlySubscribed = objUserInfo.IsYearlySubscribed;
                                GlobalSettings.UserInfo.IsCBMonthlySubscribed = objUserInfo.IsCBMonthlySubscribed;
                                GlobalSettings.UserInfo.IsCBYearlySubscribed = objUserInfo.IsCBYearlySubscribed;
                                GlobalSettings.UserInfo.TopSubscriptionLine = objUserInfo.TopSubscriptionLine;
                                GlobalSettings.UserInfo.SecondSubscriptionLine = objUserInfo.SecondSubscriptionLine;
                                GlobalSettings.UserInfo.ThirdSubscriptionLine = objUserInfo.ThirdSubscriptionLine;

                                WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);
                                //}
                                expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;
                                // expiredTime = new DateTime(2016, 01, 01);
                                InvokeOnMainThread(() =>
                                {
                                    lbExpiredDate.Text = expiredTime.ToString("MM/dd/yyyy");
                                });
                            }
                            else if (objUserInfo == null)
                            {
                                expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;
                                InvokeOnMainThread(() =>
                                {
                                    lbExpiredDate.Text = expiredTime.ToString("MM/dd/yyyy");
                                });
                            }
                            else
                            {
                                GlobalSettings.UserInfo = null;
                                if (File.Exists(WBidHelper.WBidUserFilePath))
                                    File.Delete(WBidHelper.WBidUserFilePath);
                                isFirstTimeUsing = true;
                            }


                        }


                    }
                    else
                    {
                        isFirstTimeUsing = true;
                    }
                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                    });
                }
                //Air free
                else if (typeOfInternetConnection == (int)InternetType.Air)
                {

                    if (CheckLocalAccountExist())
                    {

                        //ReadUserDetails ();
                        isFirstTimeUsing = false;
                        expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;
                        InvokeOnMainThread(() =>
                        {
                            lbExpiredDate.Text = expiredTime.ToString("MM/dd/yyyy");
                        });

                    }
                    else
                    {
                        _isNeedToCheckInternetCOnnection = true;
                        InvokeOnMainThread(() =>
                        {
                            DisplayAlertView(GlobalSettings.ApplicationName, Constants.SouthWestConnectionAlert);
                        });
                    }
                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                    });
                }

                InvokeOnMainThread(() =>
                {
                    SetupView();
                });
                //});
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {
                    if (loadingOverlay != null)
                    {
                        loadingOverlay.Hide();
                    }
                    throw ex;
                });
            }
        }

        private void HandleBidReceiptButton()
        {


            List<string> fileNames = Directory.EnumerateFiles(WBidHelper.GetAppDataPath(), "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
                  .Where(s => s.ToLower().Contains("rct.pdf")).ToList();

            btnNewBidReceipt.Enabled = fileNames.Count > 0;
        }

        private bool CheckLocalAccountExist()
        {
            return File.Exists(WBidHelper.WBidUserFilePath);
        }

        private void ReadUserDetails()
        {
            GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation>(WBidHelper.WBidUserFilePath);
        }

        private void RedirectToMessageView(int index)
        {
            AuthorizationTestCaseViewController testCaseViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
            testCaseViewController.messageError = Constants.ErrorMessages.ElementAt(index - 1);
            testCaseViewController.topBarTitle = Constants.listTitleTopBar.ElementAt(index - 1);
            PushViewController(testCaseViewController, true);
        }

        private string CheckBidFileExist()
        {
            string fileName = string.Empty;
            //string currentbidmonth = DateTime.Now.AddMonths(1).Month.ToString("d2");
            //  string currentbidmonth = DateTime.Now.Month.ToString("d2");

            if (GlobalSettings.UserInfo == null)
                return fileName;

            string currentBidmonth = string.Empty;
            string nextBidMonth = string.Empty;
            currentBidmonth = DateTime.Now.Month.ToString("d2");
            nextBidMonth = DateTime.Now.AddMonths(1).Month.ToString("d2");
            string nextBidYear = DateTime.Now.AddMonths(1).ToString("yy");
            string currentBidYear = DateTime.Now.ToString("yy");

            //if (GlobalSettings.IsCurrentMonthOn)
            //{
            //    currentbidmonth = DateTime.Now.Month.ToString("d2");
            //}
            //else
            //{
            //    currentbidmonth = DateTime.Now.AddMonths(1).Month.ToString("d2");
            //}

            //string year = (DateTime.Now.Month == 12) ? DateTime.Now.AddYears(1).ToString("yy") : DateTime.Now.ToString("yy");

            //store the current bid month
            //string currrentmonth = DateTime.Now.Month.ToString("d2");

            string path = WBidHelper.GetAppDataPath();

            //InvokeOnMainThread(() =>
            //{
            //    List<string> fileNamess = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName).ToList();

            //    string ss = "";
            //    foreach (var file in fileNamess)
            //    {
            //        ss += file + "\r\n";

            //    }
            //    DisplayAlertView(GlobalSettings.ApplicationName, ss);
            //});



            InvokeOnMainThread(() =>
            {
                List<string> fileNames = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly).Select(Path.GetFileName)
                    .Where(s => (s.ToLower().EndsWith(".wbl") || s.ToLower().EndsWith(".wbp") || s.ToLower().EndsWith(".wbs")) && ((s.Substring(5, 2) == nextBidMonth
                        && s.Substring(7, 2) == nextBidYear) || (s.Substring(5, 2) == currentBidmonth
                        && s.Substring(7, 2) == currentBidYear))).ToList();


                if (fileNames.Count() == 3)
                {
                    fileName = fileNames[0].Split('.')[0];
                }

            });

            return fileName;

        }
        partial void BtnSecretSwitchClicked(NSObject sender)
        {

            if (btnSecret1.TouchInside && btnSecret2.TouchInside)
            {
                UIAlertView Alert = new UIAlertView("Admin area", "Enter Password", null, "Cancel", new string[] { "Ok" });
                Alert.AlertViewStyle = UIAlertViewStyle.SecureTextInput;
                Alert.Show();
                Alert.Clicked += Alert_Clicked;
            }
        }
        
        void Alert_Clicked(object sender, UIButtonEventArgs e)
        {
            UIAlertView ObjAlert = (UIAlertView)sender;

            if (e.ButtonIndex == 1)
            {

                string Passowrd = ObjAlert.GetTextField(0).Text;
                if (Passowrd == "Vofox2013-1")
                {
                    AdminArea ObjAdminArea = new AdminArea();
                    ObjAdminArea._parentVC = this;
                    this.PresentViewController(ObjAdminArea, true, null);
                }
            }
        }
        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();



        }
        partial void btnSecretSwitchForUI(NSObject sender)
        {
            UIAlertView Alert = new UIAlertView("Admin area", "Enter Password", null, "Cancel", new string[] { "Ok" });
            Alert.AlertViewStyle = UIAlertViewStyle.SecureTextInput;
            Alert.Show();
            Alert.Clicked += Alert_Clicked;
        }
        
        
        #endregion


    }
}

