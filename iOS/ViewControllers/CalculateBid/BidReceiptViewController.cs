#region NameSpace
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using UIKit;
using System.Linq;
using Bidvalet.Model;
using Bidvalet.Business;

#endregion

namespace Bidvalet.iOS
{
    public partial class BidReceiptViewController : BaseViewController
    {


        #region Variables
        private string _bidFileName;
        LoadingOverlay _loadingOverlay;

        public bool IsneedToShowSync; 
        #endregion

        public BidReceiptViewController(IntPtr handle)
            : base(handle)
        {
        }

        #region Events
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Bid Receipt";
            var btnSort = new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, null);
            btnSort.Clicked += onDoneClickedEvent;
            NavigationItem.RightBarButtonItem = btnSort;
            UIHelpers.StyleForButtons(new UIButton[] { btnDone });
            UIHelpers.StyleForButtonsRadiusBorderBlack(new UIButton[] { btnEmail, btnPrint });


            List<string> fileNames = Directory.EnumerateFiles(WBidHelper.GetAppDataPath(), "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
                 .Where(s => s.ToLower().Contains("rct.pdf")).ToList();

            if (fileNames.Count > 0)
            {
                _bidFileName = fileNames[0];
                string fileName = WBidHelper.GetAppDataPath() + "/" + _bidFileName; // remember case-sensitive

                if (File.Exists(fileName))
                {
                    // string localDocUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);
                    webviewBidReceipt.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
                    webviewBidReceipt.ScrollView.SetZoomScale(20, true);
                    webviewBidReceipt.UserInteractionEnabled = true;
                    webviewBidReceipt.ScalesPageToFit = true;

                    //  this.View.AddSubview(webviewBidReceipt);
                }

                if (IsneedToShowSync)
                {


                    string message = "Would you like to Sync your bid?  By doing so, you can view your bid in WBidMax on the PC, Mac or iPad.";
                    var alertVW = new UIAlertView(GlobalSettings.ApplicationName, message, null, "No", new string[] { "Yes" });
                    alertVW.Clicked += (object senderObj, UIButtonEventArgs ee) =>
                    {

                        int index = (int)ee.ButtonIndex;
                        //Yes
                        if (index == 1)
                        {
                            _loadingOverlay = new LoadingOverlay(this.View.Bounds, "Syncing the state");
                            this.View.AddSubview(_loadingOverlay);

                            InvokeInBackground(() =>
                            {
                                UploadLocalVersionToServer();
                            });


                        }
                    };
                    alertVW.Show();




                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBarHidden = false;
            ShowNavigationBar();
        }

        void onDoneClickedEvent(object sender, EventArgs e)
        {
            SubScriptionViewController viewController = Storyboard.InstantiateViewController("SubScriptionViewController") as SubScriptionViewController;
            PushViewController(viewController, true);

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void OnDoneClickEvent(Foundation.NSObject sender)
        {
            onDoneClickedEvent(sender, null);
        }

        partial void OnEmailClickEvent(Foundation.NSObject sender)
        {


            _loadingOverlay = new LoadingOverlay(this.View.Bounds, "Checking Internet Connection..");
            this.View.AddSubview(_loadingOverlay);
            int typeOfInternetConnection = InternetHelper.CheckInterNetConnection();

            // typeOfInternetConnection = (int)InternetType.Air;
            //No internet connection
            if (typeOfInternetConnection == (int)InternetType.NoInternet)
            {
                InvokeOnMainThread(() =>
                {
                    _loadingOverlay.Hide();
                    RedirectToMessageView((int)AuthStaus.VPSDownAlert);
                });

            }
            else if (typeOfInternetConnection == (int)InternetType.Air)
            {
                InvokeOnMainThread(() =>
                {
                    _loadingOverlay.Hide();
                    RedirectToMessageView((int)AuthStaus.SouthWestConnectionAlert);
                });
            }
            //if (typeOfInternetConnection == (int)InternetType.NoInternet)
            //{
            //    InvokeOnMainThread(() =>
            //    {
            //        _loadingOverlay.Hide();
            //        RedirectToMessageView((int)AuthStaus.No_internet);
            //    });
            //}
            //Grount type internet
            else if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
            {

                InvokeOnMainThread(() =>
                {
                    _loadingOverlay.updateLoadingText("Sending Email. Please Wait...");

                });


                SendEmailBidReceipt();

                InvokeOnMainThread(() =>
                {
                    _loadingOverlay.Hide();
                    EmailSentViewController viewController = Storyboard.InstantiateViewController("EmailSentViewController") as EmailSentViewController;
                    viewController.isEmailSent = true;
                    PushViewController(viewController, true);
                });

            }
            //Airtype internet
            else if (typeOfInternetConnection == (int)InternetType.Air)
            {
                InvokeOnMainThread(() =>
                {
                    _loadingOverlay.Hide();

                    DisplayAlertView(GlobalSettings.ApplicationName, GlobalMessages.LimittedInternet);

                });

            }



        }

        partial void OnPrintClickEvent(Foundation.NSObject sender)
        {
            if (File.Exists(WBidHelper.GetAppDataPath() + "/" + _bidFileName))
            {

                var printInfo = UIPrintInfo.PrintInfo;

                printInfo.Duplex = UIPrintInfoDuplex.LongEdge;

                printInfo.OutputType = UIPrintInfoOutputType.General;

                printInfo.JobName = "Bid Receipt";

                var printer = UIPrintInteractionController.SharedPrintController;

                printer.PrintInfo = printInfo;

                printer.PrintingItem = NSData.FromFile(WBidHelper.GetAppDataPath() + "/" + _bidFileName);

                printer.ShowsPageRange = true;

                printer.Present(true, (handler, completed, err) =>
                {
                    if (!completed && err != null)
                    {
                        Console.WriteLine("Printer Error");
                        DisplayAlertView(GlobalSettings.ApplicationName, "Error. Please try again.");
                    }

                    else if (completed)
                    {
                        EmailSentViewController viewController = Storyboard.InstantiateViewController("EmailSentViewController") as EmailSentViewController;
                        viewController.isEmailSent = false;
                        PushViewController(viewController, true);


                    }
                });


            }
        }
        
        #endregion

        #region Private Methods

        private void RedirectToMessageView(int index)
        {
            AuthorizationTestCaseViewController testCaseViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
            testCaseViewController.messageError = Constants.ErrorMessages.ElementAt(index - 1);
            testCaseViewController.topBarTitle = Constants.listTitleTopBar.ElementAt(index - 1);

            PushViewController(testCaseViewController, true);
        }

        private void SendEmailBidReceipt()
        {







            WBidMail objMailAgent = new WBidMail();

            // GlobalSettings.UserInfo.Email = "ranish@vofoxsolutions.com";
            if (GlobalSettings.UserInfo != null && !string.IsNullOrEmpty(GlobalSettings.UserInfo.Email))
            {
                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + _bidFileName))
                {
                    byte[] attachment = System.IO.File.ReadAllBytes(WBidHelper.GetAppDataPath() + "/" + _bidFileName);
                    objMailAgent.SendMailtoUser("Hi <Br/><Br/> Please find the attached Bid Receipt. <Br/><Br/> WBidValet", GlobalSettings.UserInfo.Email, "Bid Receipt", attachment, _bidFileName);
                }
            }

        }

        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        }
        
        private void UploadLocalVersionToServer()
        {
            string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

            string response = SaveStateToServer(fileToSave);
            int version = int.Parse(response);
            if (version != -1)
            {

                GlobalSettings.WBidStateCollection.SyncVersion = version.ToString();
                GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime(); //GetServerVersion(stateFileName).LastUpdated;
                GlobalSettings.WBidStateCollection.IsModified = false;

                InvokeOnMainThread(() =>
                {
                    WBidHelper.SaveStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                   
                    DateTime timeUtc = DateTime.UtcNow;
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
                    DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);

                    string syncMessage = "Actual Bid" + Environment.NewLine;
                    syncMessage += GlobalSettings.CurrentBidDetails.Domicile + " " + GlobalSettings.CurrentBidDetails.Postion + " " + new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM") + " Rnd " + (GlobalSettings.CurrentBidDetails.Round == "M" ? 1 : 2) + ":" + Environment.NewLine;
                    syncMessage += String.Format("{0:MM/dd/yyyy @ H:mm CST}", cstTime);
                    GlobalSettings.UserInfo.LastSyncInfo = syncMessage;

                    WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);


                   
                    _loadingOverlay.Hide();
                    DisplayAlertView(GlobalSettings.ApplicationName, "Successfully Synchronized  your bid with the server.");
                });


            }
        }
        
        private string SaveStateToServer(string stateFileName)
        {
            string response = "-1";
            try
            {

                //VersionInfo versionInfo = null;
                //  string url = GlobalSettings.synchServiceUrl + "SaveWBidStateToServer/";

                WBidStateCollection wBidStateCollection = GlobalSettings.WBidStateCollection;

                foreach (var item in wBidStateCollection.StateList)
                {
                    if (item.FAEOMStartDate == DateTime.MinValue)
                    {
                        item.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
                    }

                }

                string data = string.Empty;
                StateSync stateSync = new StateSync();
                stateSync.EmployeeNumber = GlobalSettings.UserInfo.EmpNo;
                stateSync.StateFileName = stateFileName;
                stateSync.VersionNumber = 0;
                stateSync.Year = GlobalSettings.CurrentBidDetails.Year;
                stateSync.StateContent = SerializeHelper.JsonObjectToStringSerializer<WBidStateCollection>(wBidStateCollection);
                stateSync.LastUpdatedTime = DateTime.MinValue.ToUniversalTime();

                response = RestHelper.SyncState(stateSync);
                if (response == string.Empty)
                {
                    InvokeOnMainThread(() =>
                    {
                        _loadingOverlay.Hide();
                        DisplayAlertView(GlobalSettings.ApplicationName, "An error occured while synchronizing your state to the server.");
                    });
                }
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {
                    _loadingOverlay.Hide();
                    DisplayAlertView(GlobalSettings.ApplicationName, "An error occured while synchronizing your state.");
                });

            }

            return response;
        }

        #endregion

    }
}


