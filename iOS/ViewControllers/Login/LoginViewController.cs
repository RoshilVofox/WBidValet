#region NameSpace
using System;
using Foundation;
using UIKit;
using System.Linq;
using Bidvalet.Shared;
using System.Text.RegularExpressions;
using System.Net;
using Bidvalet.Business;
using System.Text;
using System.IO;
using iOSPasswordStorage;
using Security;
using Bidvalet.Model;
using System.Collections.Generic;
using Xamarin;
using static Bidvalet.iOS.Utility.CommonClass;
using Bidvalet.iOS.ViewControllers.HistoryBidData;
#endregion

namespace Bidvalet.iOS
{
    public partial class LoginViewController : BaseViewController
    {


        #region Variables
        private DownloadInfo _downloadFileDetails;
        LoadingOverlay loadingOverlay;
        public string loginTitle;
        //public bool isRecentBidDownload;
        private string _empNumber = string.Empty;
        private string _password = string.Empty;
        private DateTime _expirationDate;
        public bool IsFromMainView { get; set; }
        #endregion


        public LoginViewController(IntPtr handle)
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Login";

            //			loadingOverlay = new LoadingOverlay (this.View.Bounds, "Checking \n Please wait..");
            //			this.View.AddSubview(loadingOverlay);

            lbLoginTitle.Text = loginTitle;
            UIHelpers.StyleForButtons(new UIButton[] { btLogin });
            setLoginCredentialsFromKeychaninToTextField();

            edUsername.KeyboardType = UIKeyboardType.NumbersAndPunctuation;

            this.edUsername.ShouldReturn += (textField) =>
            {
                edPassword.BecomeFirstResponder();
                return true;
            };

            this.edPassword.ShouldReturn += (textField) =>
            {
                textField.ResignFirstResponder();
                return true;
            };

        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ShowNavigationBar();
        }

        partial void OnLoginClicked(Foundation.NSObject sender)
        {
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
            //			AuthorizationServiceViewController testInternet = Storyboard.InstantiateViewController("AuthorizationServiceViewController") as AuthorizationServiceViewController;
            //			PushViewController(testInternet,true);
            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(true, true);
            loadingOverlay = new LoadingOverlay(this.View.Bounds, "Authenticating \n Please wait..");
            this.View.AddSubview(loadingOverlay);
            InvokeInBackground(() =>
            {

                if (CheckValidCredentials())
                {

                    //0--No internet , 1-- on ground  ,2--on AIr 
                    int typeOfInternetConnection;
                    typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
                    //typeOfInternetConnection = (int)InternetType.Air;
                    GlobalSettings.InternetType = typeOfInternetConnection;
                    //typeOfInternetConnection = (int)InternetType.Air;
                    //No internet connection
                    if (typeOfInternetConnection == (int)InternetType.NoInternet)
                    {
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            RedirectToMessageView((int)AuthStaus.VPSDownAlert);
                        });
                    }
                    //Grount type internet
                    else if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
                    {

                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.updateLoadingText("Checking credentials");
                        });
                        bool isAuthSuccess = CheckCWAAuthentication();
                        if (isAuthSuccess)
                        {
                            InvokeOnMainThread(() =>
                            {
                                loadingOverlay.updateLoadingText("Checking Authorization");
                            });
                            CheckWBidAuthentication();

                        }


                    }
                    //Airtype internet
                    else if (typeOfInternetConnection == (int)InternetType.Air)
                    {
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            RedirectToMessageView((int)AuthStaus.SouthWestConnectionAlert);
                        });
                        //InvokeOnMainThread(() =>
                        //{
                        //    loadingOverlay.updateLoadingText("Checking credentials");
                        //});
                        //bool isAuthSuccess = CheckCWAAuthentication();

                        //if (isAuthSuccess)
                        //{

                        //    InvokeOnMainThread(() =>
                        //    {

                        //        loadingOverlay.Hide();
                        //        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        //        if (CheckValidLocalAccountExist())
                        //        {

                        //            RedirectTodownloadView();
                        //        }
                        //        else
                        //        {
                        //            DisplayAlertView(GlobalSettings.ApplicationName, GlobalMessages.LimittedInternet);
                        //        }
                        //    });
                        //}
                        //if(isAuthSuccess)
                        //{
                        //	InvokeOnMainThread(()=>{
                        //		loadingOverlay.Hide();
                        //		RedirectTodownloadView();
                        //	});

                        //}
                    }


                }
                else
                {

                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    });
                }

            });


        }
        #endregion

        #region Private Methods
        private bool CheckValidLocalAccountExist()
        {
            bool status = File.Exists(WBidHelper.WBidUserFilePath);

            if (status)
            {
                if (GlobalSettings.UserInfo != null && (GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue) >= DateTime.Now)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            return status;
        }

        #region SetLoginDetails
        private void setLoginCredentialsFromKeychaninToTextField()
        {
            try
            {
                this.edUsername.Text = KeychainHelpers.GetPasswordForUsername("user", "WBid.BidValet.cwa", false);
                this.edPassword.Text = KeychainHelpers.GetPasswordForUsername("pass", "WBid.BidValet.cwa", false);
            }
            catch
            {
                Console.WriteLine("Setting credentials execprion");
            }
        }
        #endregion

        #region KeyChain Access

        public void saveToKeyChain(string uName, string pass, string service)
        {
            var userResult = KeychainHelpers.SetPasswordForUsername("user", uName.ToLower().Replace("x", "").Replace("e", ""), service, SecAccessible.Always, false);
            var passResult = KeychainHelpers.SetPasswordForUsername("pass", pass, service, SecAccessible.Always, false);
            if (!((userResult == Security.SecStatusCode.Success) && (passResult == Security.SecStatusCode.Success)))
            {
                DisplayAlertView("Oops", "Couldn't save information sucurely, please try again.");

                return;
            }
        }
        #endregion

        private bool CheckValidCredentials()
        {

            bool status = false;
            InvokeOnMainThread(() =>
            {
                try
                {
                    if (Regex.Match(edUsername.Text.Trim(), "^[e,E,x,X,0-9][0-9]*$").Success)
                    {
                        if (edPassword.Text.Length > 0)
                        {
                            _empNumber = edUsername.Text.Trim().ToLower();
                            if (_empNumber[0] != 'e' && _empNumber[0] != 'x')
                                _empNumber = "e" + _empNumber;

                            _password = edPassword.Text;
                            this.saveToKeyChain(_empNumber, _password, "WBid.BidValet.cwa");
                            status = true;
                        }
                        else
                        {


                            DisplayAlertView("Login", "Password is required.");

                        }
                    }
                    else
                    {

                        DisplayAlertView("Login", "Invalid Employee Number");


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Checking Valid Credentials");
                }
            });
            return status;
        }

        private ServerUserInformation GetUserDetails()
        {
            ServerUserInformation userInfo = null;
            try
            {

                var url = GlobalSettings.WBidAuthenticationServiceUrl + "/GetAllUserAccountDetails/";
                url = url + int.Parse(_empNumber.ToLower().Replace("x", "").Replace("e", "")) + "/" + (int)AppNum.BidValet + "/";
                userInfo = RestHelper.GetResponse<ServerUserInformation>(url);

            }
            catch (Exception ex)
            {
            }
            return userInfo;
        }

        private ClientRequestModel SetClientRequestDetails()
        {
            ClientRequestModel obj = new ClientRequestModel();
            obj.Base = GlobalSettings.DownloadBidDetails.Domicile;
            int round = (GlobalSettings.DownloadBidDetails.Round == "D") ? 1 : 2;
            obj.BidRound = round;
            obj.EmployeeNumber = int.Parse(_empNumber.ToLower().Replace("x", "").Replace("e", ""));
            if (GlobalSettings.IsCurrentMonthOn)
            {
                obj.Month = DateTime.Now.ToString("MMM").ToUpper();
            }
            else
            {
                obj.Month = DateTime.Now.AddMonths(1).ToString("MMM").ToUpper();
            }
            if (GlobalSettings.isAwardDownload)
            {
                obj.RequestType = (int)RequestTypes.DownloadAward;
            }
            else
            {
                obj.RequestType = (int)RequestTypes.DownnloadBid;
            }
            //new DateTime(2016, 2, 1).ToString("MMM").ToUpper();
            obj.OperatingSystem = GlobalSettings.OperatingSystem;
            obj.Platform = GlobalSettings.Platform;
            obj.Postion = GlobalSettings.DownloadBidDetails.Postion;
            obj.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //"8.0.0.0";


            return obj;
        }

        private bool CheckWBidAuthentication()
        {
            bool status = false;
            try
            {

                //var url = GlobalSettings.WBidAuthenticationServiceUrl + "/GetAllAuthentication";
                //var data = string.Empty;
                ////var
                //var objClient = SetClientRequestDetails();
                //var request = (HttpWebRequest)WebRequest.Create(url);
                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //data = SerializeHelper.JsonObjectToStringSerializer<ClientRequestModel>(objClient);
                //var bytes = Encoding.UTF8.GetBytes(data);
                //request.ContentLength = bytes.Length;
                //request.GetRequestStream().Write(bytes, 0, bytes.Length);
                //request.Timeout = 30000;
                ////Response
                //var response = (HttpWebResponse)request.GetResponse();

                //if (response.StatusCode == HttpStatusCode.OK)
                //{
                //    var stream = response.GetResponseStream();

                //    var reader = new StreamReader(stream);

                //    AuthServiceResponseModel responseModel = SerializeHelper.ConvertJSonStringToObject<AuthServiceResponseModel>(reader.ReadToEnd());



                ClientRequestModel requestModel = SetClientRequestDetails();

                AuthServiceResponseModel responseModel = RestHelper.CheckWBidAuthentication(requestModel);
                if (responseModel != null)
                {

                    GlobalSettings.IsNeedToDownloadSeniorityUser = responseModel.IsNeedToDownloadSeniorityFromServer;
                    if (responseModel.Type == "TimeOut")
                    {

                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            RedirectToMessageView((int)AuthStaus.TO_BVDB_ground);
                        });
                    }
                    // responseModel.Type = "Invalid Account";
                    else if (responseModel.Type == "Success" || responseModel.Type == "TemporaryAuthenticate")
                    {
                        //if it is different user then we need to move to download process
                        if (GlobalSettings.UserInfo != null && GlobalSettings.UserInfo.EmpNo != _empNumber.ToLower().Replace("x", "").Replace("e", ""))
                        {
                            InvokeOnMainThread(() =>
                            {

                                loadingOverlay.Hide();
                                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                //if (isRecentBidDownload)
                                //    RedirectToConstraintsView();
                                //else
                                    RedirectTodownloadView();
                            });
                        }
                        else
                        {

                            ServerUserInformation objServerUserInfo = GetUserDetails();
                            if (objServerUserInfo == null)
                            {
                                InvokeOnMainThread(() =>
                                {

                                    DisplayAlertView(GlobalSettings.ApplicationName, "Error. Please try again");
                                });
                                return false;

                            }
                            UserInformation objServerUser = MapServerUserToLocal(objServerUserInfo);

                            objServerUser.Domicile = GlobalSettings.Domicile;

                            //Already  account exist
                            if (GlobalSettings.UserInfo != null)
                            {
                                if (objServerUser != null)
                                {
                                    bool isContentModified = CheckProfileModified(GlobalSettings.UserInfo, objServerUser);
                                    //Updating the paid until to local
                                    if (GlobalSettings.UserInfo.EmpNo == objServerUser.EmpNo)
                                    {
                                        GlobalSettings.UserInfo.PaidUntilDate = objServerUser.PaidUntilDate;
                                        GlobalSettings.UserInfo.IsFree = objServerUser.IsFree;
                                        GlobalSettings.UserInfo.IsMonthlySubscribed = objServerUser.IsMonthlySubscribed;
                                        GlobalSettings.UserInfo.IsYearlySubscribed = objServerUser.IsYearlySubscribed;
                                        GlobalSettings.UserInfo.IsCBMonthlySubscribed = objServerUser.IsCBMonthlySubscribed;
                                        GlobalSettings.UserInfo.IsCBYearlySubscribed = objServerUser.IsCBYearlySubscribed;
                                        GlobalSettings.UserInfo.TopSubscriptionLine = objServerUser.TopSubscriptionLine;
                                        GlobalSettings.UserInfo.SecondSubscriptionLine = objServerUser.SecondSubscriptionLine;
                                        GlobalSettings.UserInfo.ThirdSubscriptionLine = objServerUser.ThirdSubscriptionLine;
                                        GlobalSettings.UserInfo.Domicile = GlobalSettings.Domicile; ;


                                        WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);


                                        GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation>(WBidHelper.WBidUserFilePath);



                                        if (isContentModified)
                                        {
                                            InvokeOnMainThread(() =>
                                            {
                                                loadingOverlay.Hide();
                                                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                                List<KeyValuePair<string, string>> differenceList = GenerateDifferentList(GlobalSettings.UserInfo, objServerUser);
                                                RedirectUserDifferenceScreen(differenceList);
                                            });

                                            //this.NavigationController.PopToRootViewController
                                            //RedirectTo compare view
                                            //Need to update the data to server

                                        }
                                        else
                                        {

                                            InvokeOnMainThread(() =>
                                            {

                                                loadingOverlay.Hide();
                                                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                                //if (isRecentBidDownload)
                                                //    RedirectToConstraintsView();
                                                //else
                                                    RedirectTodownloadView();
                                            });
                                        }
                                    }
                                }

                                //									GlobalSettings.UserInfo.FirstName=objServerUser.FirstName;
                                //									GlobalSettings.UserInfo.LastName=objServerUser.LastName;
                                //									GlobalSettings.UserInfo.EmpNo=objServerUser.EmpNo;
                                //									GlobalSettings.UserInfo.Position=objServerUser.Position;
                                //									GlobalSettings.UserInfo.Email=objServerUser.Email;
                                //									GlobalSettings.UserInfo.CellNumber=objServerUser.CellNumber;
                                //									GlobalSettings.UserInfo.CellCarrier=objServerUser.CellCarrier;
                                //									GlobalSettings.UserInfo.isAcceptMail=objServerUser.isAcceptMail;
                                //
                                //
                                //
                                //									if(GlobalSettings.UserInfo.EmpNo==objServerUser.EmpNo && isChange)
                                //									{
                                //										InvokeOnMainThread(()=>{
                                //											loadingOverlay.Hide(); 
                                //										RedirectToAccountView ((int)AuthStaus.Found_account_edit,int.Parse(GlobalSettings.UserInfo.EmpNo));
                                //										});
                                //									}
                                //									else
                                //									{
                                //										InvokeOnMainThread(()=>{
                                //											loadingOverlay.Hide();
                                //										RedirectTodownloadView();
                                //									});
                                //										//Download view
                                //									}
                                //								}

                            }
                            else
                            {


                                if (objServerUser != null)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        var alertVW = new UIAlertView(GlobalSettings.ApplicationName, "Great! We found a  valid WBid account!\n\n Next we are going to import your account details locally. If you need, you can change your account details. ", null, "OK", null);

                                        alertVW.Clicked += (object sender, UIButtonEventArgs e) =>
                                        {
                                            WBidHelper.SaveUserFile(objServerUser, WBidHelper.WBidUserFilePath);
                                            GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation>(WBidHelper.WBidUserFilePath);



                                            loadingOverlay.Hide();
                                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);

                                            //       try
                                            //       {
                                            //           Insights.Identify(GlobalSettings.UserInfo.EmpNo, new Dictionary<string, string> { 
                                            //           {Insights.Traits.Email, GlobalSettings.UserInfo.Email},
                                            //           {Insights.Traits.Name, GlobalSettings.UserInfo.FirstName+" "+GlobalSettings.UserInfo.LastName},
                                            //           {Insights.Traits.Description, "Account Imported"}

                                            //});
                                            //}
                                            //catch (Exception)
                                            //{


                                            //}
                                            RedirectToAccountView((int)AuthStaus.Found_account_edit, int.Parse(GlobalSettings.UserInfo.EmpNo));

                                        };
                                        alertVW.Show();
                                    });

                                    //									WBidHelper.SaveUserFile(objUser,WBidHelper.WBidUserFilePath);
                                    //									GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation> (WBidHelper.WBidUserFilePath);
                                    //									InvokeOnMainThread(()=>{
                                    //										loadingOverlay.Hide();
                                    //									RedirectToAccountView ((int)AuthStaus.Found_account_edit,int.Parse(GlobalSettings.UserInfo.EmpNo));
                                    //									});
                                }
                                //								else
                                //								{
                                //									InvokeOnMainThread(()=>{
                                //										var alertVW = new UIAlertView(GlobalSettings.ApplicationName, "You need to create an wbid account ", null, "OK", null);
                                //										alertVW.Clicked += (object sender, UIButtonEventArgs e) =>
                                //										{
                                //											WBidHelper.SaveUserFile(objUser,WBidHelper.WBidUserFilePath);
                                //											GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation> (WBidHelper.WBidUserFilePath);
                                //											loadingOverlay.Hide();
                                //											RedirectToAccountView ((int)AuthStaus.Found_account_edit,int.Parse(GlobalSettings.UserInfo.EmpNo));
                                //
                                //										};
                                //										alertVW.Show();
                                //									});
                                //									
                                //}

                            }
                        }
                        //status=true;
                        //DisplayMessage("Auth","Success");
                    }
                    else if (responseModel.Type == "Invalid Account")
                    {
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            RedirectToAccountView((int)AuthStaus.New_CB_WB_user, requestModel.EmployeeNumber);
                        });
                        //DisplayMessage("Auth","Invalid Account");
                    }

                    else if (responseModel.Type == "Invalid Version")
                    {
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            RedirectToDynamicMessageView((int)AuthStaus.Version_not_supported, responseModel.Message);
                        });

                        //DisplayMessage("Auth","Invalid Version");

                    }
                    else if (responseModel.Type == "BidDownloadBlocked")
                    {
                        InvokeOnMainThread(() =>
                        {
                            loadingOverlay.Hide();
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            RedirectToDynamicMessageView((int)AuthStaus.BidDownloadBlocked, responseModel.Message);
                        });
                        //DisplayMessage("Auth","Invalid Account");
                    }
                    else if (responseModel.Type == "Invalid Subscription")
                    {
                        _expirationDate = responseModel.WBExpirationDate;
                        InvokeOnMainThread(() =>
                        {
                            string message = string.Empty;

                            if (GlobalSettings.UserInfo == null)
                            {
                                message = "Great! We found a  valid WBid account!\n\n Next we are going to import your account details locally.";
                            };
                            //message += "\n\nYour Subscription expired on " + _expirationDate.ToString("dd MMM yy") + ". You can subscribe here for $5.99 a one month usage.Or you can go to www.wbidmax.com for additional account options.";
                            message += "\n\n" + responseModel.Message.Replace("\\n", "\n");
                            message += "\n\nDo you want to continue?";
                            var alertPrivacyVW = new UIAlertView(GlobalSettings.ApplicationName, message, null, "NO", new string[] { "YES" });
                            alertPrivacyVW.Clicked += (object senderObject, UIButtonEventArgs e) =>
                            {
                                int index = (int)e.ButtonIndex;
                                //OK
                                if (index == 1)
                                {

                                    InvokeInBackground(() =>
                                    {
                                        if (GlobalSettings.UserInfo == null)
                                        {

                                            ServerUserInformation objServerUserInfo = GetUserDetails();
                                            if (objServerUserInfo == null)
                                            {
                                                InvokeOnMainThread(() =>
                                                {
                                                    loadingOverlay.Hide();
                                                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                                    DisplayAlertView(GlobalSettings.ApplicationName, "Error. Please try again");
                                                });

                                            }
                                            else
                                            {
                                                UserInformation objServerUser = MapServerUserToLocal(objServerUserInfo);

                                                objServerUser.Domicile = GlobalSettings.Domicile;
                                                GlobalSettings.UserInfo = new UserInformation();
                                                GlobalSettings.UserInfo.EmpNo = objServerUser.EmpNo;
                                                GlobalSettings.UserInfo.FirstName = objServerUser.FirstName;
                                                GlobalSettings.UserInfo.LastName = objServerUser.LastName;
                                                GlobalSettings.UserInfo.Position = objServerUser.Position;
                                                GlobalSettings.UserInfo.Email = objServerUser.Email;
                                                GlobalSettings.UserInfo.CellNumber = objServerUser.CellNumber;
                                                GlobalSettings.UserInfo.PaidUntilDate = objServerUser.PaidUntilDate;
                                                GlobalSettings.UserInfo.IsFree = objServerUser.IsFree;
                                                GlobalSettings.UserInfo.IsMonthlySubscribed = objServerUser.IsMonthlySubscribed;
                                                GlobalSettings.UserInfo.IsYearlySubscribed = objServerUser.IsYearlySubscribed;
                                                GlobalSettings.UserInfo.IsCBMonthlySubscribed = objServerUser.IsCBMonthlySubscribed;
                                                GlobalSettings.UserInfo.IsCBYearlySubscribed = objServerUser.IsCBYearlySubscribed;
                                                GlobalSettings.UserInfo.TopSubscriptionLine = objServerUser.TopSubscriptionLine;
                                                GlobalSettings.UserInfo.SecondSubscriptionLine = objServerUser.SecondSubscriptionLine;
                                                GlobalSettings.UserInfo.ThirdSubscriptionLine = objServerUser.ThirdSubscriptionLine;
                                                GlobalSettings.UserInfo.Domicile = GlobalSettings.Domicile; ;
                                                WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);
                                                GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation>(WBidHelper.WBidUserFilePath);

                                                InvokeOnMainThread(() =>
                                                {
                                                    loadingOverlay.Hide();
                                                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);

                                                    //try
                                                    //{
                                                    //    Insights.Identify(GlobalSettings.UserInfo.EmpNo, new Dictionary<string, string> { 
                                                    //    {Insights.Traits.Email, GlobalSettings.UserInfo.Email},
                                                    //    {Insights.Traits.Name, GlobalSettings.UserInfo.FirstName+" "+GlobalSettings.UserInfo.LastName},
                                                    //    {Insights.Traits.Description, "Account Imported"}

                                                    // });
                                                    //}
                                                    //catch (Exception)
                                                    //{


                                                    //}
                                                    ExpiredViewController expiredViewController = Storyboard.InstantiateViewController("ExpiredViewController") as ExpiredViewController;
                                                    expiredViewController.expiredTime = _expirationDate;
                                                    NavigationController.PushViewController(expiredViewController, true);
                                                });
                                            }
                                        }
                                        else
                                        {
                                            InvokeOnMainThread(() =>
                                            {
                                                loadingOverlay.Hide();
                                                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                                ExpiredViewController expiredViewController = Storyboard.InstantiateViewController("ExpiredViewController") as ExpiredViewController;
                                                expiredViewController.expiredTime = _expirationDate;
                                                NavigationController.PushViewController(expiredViewController, true);
                                            });
                                        }

                                    });
                                    //RedirectToMessageView((int)AuthStaus.Expired_subscription);
                                }
                                else
                                {
                                    InvokeOnMainThread(() =>
                                          {
                                              loadingOverlay.Hide();
                                              this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                          });


                                }


                            };

                            alertPrivacyVW.Show();



                        });
                        //DisplayMessage("Auth","Invalid Subscription");

                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        DisplayAlertView(GlobalSettings.ApplicationName, "Error. Please try again.");
                    });

                }

                //}
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {
                    loadingOverlay.Hide();
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    RedirectToMessageView((int)AuthStaus.TO_BVDB_ground);
                });
            }
            return status;
        }



        private List<KeyValuePair<string, string>> GenerateDifferentList(UserInformation local, UserInformation server)
        {
            List<KeyValuePair<string, string>> tempList = new List<KeyValuePair<string, string>>();

            if (local.FirstName != server.FirstName)
                tempList.Add(new KeyValuePair<string, string>("FirstName", local.FirstName + "," + server.FirstName));

            if (local.LastName != server.LastName)
                tempList.Add(new KeyValuePair<string, string>("LastName", local.LastName + "," + server.LastName));


            if (local.EmpNo != server.EmpNo)
                tempList.Add(new KeyValuePair<string, string>("EmployeeNumber", local.EmpNo + "," + server.EmpNo));

            if (local.Position != server.Position)
                tempList.Add(new KeyValuePair<string, string>("Position", local.Position + "," + server.Position));

            if (local.Email != server.Email)
                tempList.Add(new KeyValuePair<string, string>("Email", local.Email + "," + server.Email));


            if (local.CellNumber != server.CellNumber)
                tempList.Add(new KeyValuePair<string, string>("CellNumber", local.CellNumber + "," + server.CellNumber));

            if (local.CellCarrier != server.CellCarrier)
            {
                int index = server.CellCarrier;
                index = index - 1;
                if (index < 0)
                    index = 0;

                int locindex = local.CellCarrier;
                locindex = locindex - 1;
                if (locindex < 0)
                    locindex = 0;

                tempList.Add(new KeyValuePair<string, string>("CellCarrier", Constants.ListCarrier[locindex] + "," + Constants.ListCarrier[index]));

            }


            return tempList;
        }

        private UserInformation MapServerUserToLocal(ServerUserInformation objServerUserInfo)
        {
            UserInformation objUser = new UserInformation
            {
                //BidBase=objServerUserInfo.BidBase,
                //BidSeat=objServerUserInfo.BidSeat,
                CellCarrier = objServerUserInfo.CarrierNum,
                CellNumber = objServerUserInfo.CellPhone,
                //Domicile= objServerUserInfo.BidBase,
                Email = objServerUserInfo.Email,
                EmpNo = objServerUserInfo.EmpNum.ToString(),
                FirstName = objServerUserInfo.FirstName,
                LastName = objServerUserInfo.LastName,
                isAcceptMail = objServerUserInfo.AcceptEmail,
                PaidUntilDate = objServerUserInfo.WBExpirationDate,
                Position = (objServerUserInfo.Position == 3) ? "Flt Att" : "Pilot",
                UserAccountDateTime = objServerUserInfo.UserAccountDateTime ?? DateTime.MinValue,
                IsFree = objServerUserInfo.IsFree,
                IsMonthlySubscribed = objServerUserInfo.IsMonthlySubscribed,
                IsYearlySubscribed = objServerUserInfo.IsYearlySubscribed,
                IsCBMonthlySubscribed = objServerUserInfo.IsCBMonthlySubscribed,
                IsCBYearlySubscribed = objServerUserInfo.IsCBYearlySubscribed,
                TopSubscriptionLine = objServerUserInfo.TopSubscriptionLine,
                SecondSubscriptionLine = objServerUserInfo.SecondSubscriptionLine,
                ThirdSubscriptionLine = objServerUserInfo.ThirdSubscriptionLine


                //IsFemale=
                //isAcceptUserTermsAndCondition


            };
            return objUser;
        }

        private bool ComapreDetails(UserInformation localUser, UserInformation serverUser)
        {
            bool status = false;

            status = (localUser.FirstName != serverUser.FirstName) || (localUser.LastName != serverUser.LastName)
            || (localUser.EmpNo != serverUser.EmpNo) || (localUser.Position != serverUser.Position)
            || (localUser.Email != serverUser.Email) || (localUser.CellNumber != serverUser.CellNumber)
            || (localUser.CellCarrier != serverUser.CellCarrier)
            || (localUser.isAcceptMail != serverUser.isAcceptMail);
            return status;
        }

        private bool CheckProfileModified(UserInformation localInfo, UserInformation serverInfo)
        {
            bool status = false;
            status = (localInfo.FirstName != serverInfo.FirstName) || (localInfo.LastName != serverInfo.LastName)
                || (localInfo.CellCarrier != serverInfo.CellCarrier) || (localInfo.CellNumber != serverInfo.CellNumber)
                || (localInfo.Email != serverInfo.Email) || (localInfo.Position != serverInfo.Position);
            return status;

        }

        private bool CheckCWAAuthentication()
        {

            bool status = false;
            try
            {


                SWAAuthentication authentication = new SWAAuthentication();
                string authResult = authentication.CheckCredential(_empNumber, _password);
                if (authResult.Contains("ERROR: "))
                {
                    RestHelper.LogOperation(WBidHelper.LogBadPasswordUsage(_empNumber, true));
                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        RedirectToMessageView((int)AuthStaus.Invalid_Login);
                    });
                }
                else if (authResult.Contains("Exception"))
                {
                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        RedirectToMessageView((int)AuthStaus.TO_3rd_pty_ground);
                    });

                }
                else
                {
                    GlobalSettings.SessionCredentials = authResult;
                    status = true;
                    //DisplayMessage("Login","Success");

                }
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {
                    loadingOverlay.Hide();

                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    RedirectToMessageView((int)AuthStaus.TO_3rd_pty_ground);
                });
            }
            return status;
        }

        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();
            //UIAlertView alert = new UIAlertView("Instruction", "Password is required.", null, "OK", null);
            //alert.Show();
        }

        private void RedirectToMessageView(int index)
        {
            AuthorizationTestCaseViewController testCaseViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
            testCaseViewController.InvalidCredentialError = WBidHelper.SetInvalidCredentialAlertMessage();
            //Constants.ErrorMessages.ElementAt(index - 1);
            testCaseViewController.topBarTitle = Constants.listTitleTopBar.ElementAt(index - 1);

            if (index == (int)AuthStaus.Expired_subscription && _expirationDate != DateTime.MinValue)
            {
                testCaseViewController.messageError = Constants.ErrorMessages.ElementAt(index - 1).Replace("17 Sep 2015", _expirationDate.ToString("dd MMM yy"));
            }
            testCaseViewController.numberRow = (int)(index);
            if (index == Constants.EXPIRED_SUBSCRIPTION)
            {
                testCaseViewController.isShowPurchaseButton = true;
            }
            if (index == Constants.NEW_CB_WB_USER || index == Constants.VALID_SUBSCRIPTION)
            {
                testCaseViewController.buttonTitle = Constants.GO_TO_CONSTRAINTS;
            }

            PushViewController(testCaseViewController, true);
        }


        private void RedirectToDynamicMessageView(int index, string message)
        {
            AuthorizationTestCaseViewController testCaseViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
            testCaseViewController.messageError = message.Replace("\\n", "\n");
            testCaseViewController.topBarTitle = Constants.listTitleTopBar.ElementAt(index - 1);

            testCaseViewController.numberRow = (int)(index);
            if (index == Constants.EXPIRED_SUBSCRIPTION)
            {
                testCaseViewController.isShowPurchaseButton = true;
            }

            PushViewController(testCaseViewController, true);
        }


        private void RedirectUserDifferenceScreen(List<KeyValuePair<string, string>> differenceList)
        {
            UserAccountDifferenceScreen ObjUserDifference = Storyboard.InstantiateViewController("UserAccountDifferenceScreen") as UserAccountDifferenceScreen;
            ObjUserDifference.DifferenceList = differenceList;
            ObjUserDifference.IsFromMainView = false;
            this.NavigationController.NavigationItem.HidesBackButton = false;
            ObjUserDifference.ParentController = this;
            this.PresentViewController(ObjUserDifference, true, null);
            //PushViewController (DownloadData, true);

        }

        private void RedirectToAccountView(int index, int empNum)
        {
            CreateAccountTableViewController createAccountView = Storyboard.InstantiateViewController("CreateAccountTableViewController") as CreateAccountTableViewController;
            if (index == Constants.FOUND_ACCOUNT)
            {
                createAccountView.IsFoundAccount = true;
            }
            createAccountView.EmpNumber = empNum;
            createAccountView.IsFromMainView = false;
            createAccountView.ParentController = this;
            //this.NavigationController.NavigationItem.HidesBackButton = true;
            //this.NavigationController.PresentViewController (createAccountView, true, null);

            this.PresentViewController(createAccountView, true, null);

        }

        private void RedirectTodownloadView()
        {

            if (GlobalSettings.isAwardDownload)
            {
                InvokeOnMainThread(() =>
                {

                    GenarateAwardFileName();
                    AwardDownlaod();
                    //DismissCurrentView();
                });
            }
            else
            {


                DownloadBidDataViewController DownloadData = Storyboard.InstantiateViewController("DownloadBidDataViewController") as DownloadBidDataViewController;

                this.NavigationController.NavigationItem.HidesBackButton = false;
                DownloadData.ObjParentController = this;
                this.PresentViewController(DownloadData, true, null);


            }
            //PushViewController (DownloadData, true);

        }
        /// <summary>
        /// Create the Filename for the Award file based on teh UI selection
        /// </summary>
        private void GenarateAwardFileName()
        {
            List<string> downLoadList = new List<string>();
            try
            {
                int round = GlobalSettings.DownloadBidDetails.Round == "D" ? 1 : 2;
                if (GlobalSettings.DownloadBidDetails.Postion == "CP")
                {
                    string fileName = GlobalSettings.DownloadBidDetails.Domicile + "CP" + ((round == 1) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                    //fileName = GlobalSettings.DownloadBidDetails.Domicile + "FO" + ((round == 1) ? "M" : "W") + ".TXT";
                    //downLoadList.Add(fileName);
                }
                else if (GlobalSettings.DownloadBidDetails.Postion == "FO")
                {
                    string fileName = GlobalSettings.DownloadBidDetails.Domicile + "FO" + ((round == 1) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                    //fileName = GlobalSettings.DownloadBidDetails.Domicile + "CP" + ((round == 1) ? "M" : "W") + ".TXT";
                    //downLoadList.Add(fileName);
                }
                else if (GlobalSettings.DownloadBidDetails.Postion == "FA")
                {
                    string fileName = GlobalSettings.DownloadBidDetails.Domicile + "FA" + ((round == 1) ? "M" : "W") + ".TXT";
                    downLoadList.Add(fileName);
                }
                else
                {
                    return;
                }
                _downloadFileDetails = new DownloadInfo();
                _downloadFileDetails.SessionCredentials = GlobalSettings.SessionCredentials;
                _downloadFileDetails.DownloadList = new List<string>();
                _downloadFileDetails.DownloadList = downLoadList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Donwload the awards and show it ot the file viewer.
        /// </summary>
        private void AwardDownlaod()
        {
            try
            {
                DownloadAward downloadAward = new DownloadAward();
                List<DownloadedFileInfo> lstDownloadedFiles = downloadAward.DownloadAwardDetails(_downloadFileDetails);
                if (lstDownloadedFiles != null)
                {
                    if (lstDownloadedFiles[0].IsError)
                    {
                        List<string> lstMessages = new List<string>();

                        InvokeOnMainThread(() =>
                        {
                            UIAlertView alert = new UIAlertView("WBidMax", "The request data does not exist on the SWA  Servers. Make sure the proper month is  selected and  you are within the  normal timeframe for the request.", null, "OK", null);
                            alert.Show();
                            //overlay.RemoveFromSuperview();

                        });
                    }
                    else
                    {
                        bool isNeedtoShowFileViewer = true;

                        string filepath = Path.Combine(WBidHelper.GetBidAwardPath() + "/" + WBidCollection.GetMonthName(DateTime.Now.AddMonths(1).Month));
                        if (!Directory.Exists(filepath))
                        {
                            Directory.CreateDirectory(filepath);
                        }
                        foreach (DownloadedFileInfo fileinfo in lstDownloadedFiles)
                        {
                            FileStream fStream = new FileStream(Path.Combine(filepath, fileinfo.FileName), FileMode.Create);
                            fStream.Write(fileinfo.byteArray, 0, fileinfo.byteArray.Length);
                            fStream.Dispose();
                        }

                        var filename = lstDownloadedFiles[0].FileName;

                        UserBidDetails biddetails = new UserBidDetails();
                        biddetails.Domicile = filename.Substring(0, 3);
                        biddetails.Position = filename.Substring(3, 2);
                        biddetails.Round = filename.Substring(5, 1) == "M" ? 1 : 2;
                        biddetails.Year = DateTime.Now.AddMonths(1).Year;
                        biddetails.Month = DateTime.Now.AddMonths(1).Month;
                        if (biddetails.Round == 1)
                        {

                            if (GlobalSettings.IsDifferentUser)
                            {
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0'), @"\d+").Value);
                            }
                            else
                            {
                                //biddetails.EmployeeNumber= int.Parse(_empNumber.ToLower().Replace("x", "").Replace("e", ""));
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(_empNumber, @"\d+").Value);
                            }
                            string alertmessage = WBidHelper.GetAwardAlert(biddetails);
                            if (alertmessage != string.Empty)
                            {
                                isNeedtoShowFileViewer = false;
                                alertmessage = alertmessage.Insert(0, "\n\n");
                                alertmessage += "\n\n";
                                InvokeOnMainThread(() =>
                                {
                                    UIAlertController AlertController = UIAlertController.Create("WBidMax", alertmessage, UIAlertControllerStyle.Alert);
                                    //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
                                {

                                    webPrint fileViewer = new webPrint();
                                    fileViewer.strUrl = lstDownloadedFiles[0].FileName;
                                    fileViewer.loadFileFromUrl(lstDownloadedFiles[0].FileName);
                                    this.NavigationController.PushViewController(fileViewer, true);

                                    //webPrint fileViewer = new webPrint();

                                    //fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                    //this.PresentViewController(fileViewer, true, () =>
                                    //{
                                    //    fileViewer.strUrl = lstDownloadedFiles[0].FileName;
                                    //    //fileViewer.loadFileFromUrl(lstDownloadedFiles[0].FileName);
                                    //    //fileViewer.loadFileFromUrlFromDownload(seniorityFileName + ".TXT", intEmpNum.ToString());
                                    //    ////	DismisscAndRedirectToLineView();
                                    //});
                                }));
                                    this.PresentViewController(AlertController, true, null);

                                });

                            }
                        }
                        if (isNeedtoShowFileViewer)
                        {
                            InvokeOnMainThread(() =>
                    {
                        webPrint fileViewer = new webPrint();
                       // fileViewer.strUrl = lstDownloadedFiles[0].FileName;
                        fileViewer.loadFileFromUrl(lstDownloadedFiles[0].FileName);
                        this.NavigationController.PushViewController(fileViewer, true);

                    });
                        }

                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                        {
                            UIAlertView alert = new UIAlertView("WBidMax", "Please try again after some time", null, "OK", null);
                            alert.Show();
                            //overlay.RemoveFromSuperview();

                        });
                }
                InvokeOnMainThread(() =>
                {
                    //overlay.RemoveFromSuperview();

                });
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void DismissEditAndNavigateToDownload(CreateAccountTableViewController ObjCreate)
        {
            ObjCreate.DismissViewController(true, null);

            if (GlobalSettings.UserInfo != null && (DateTime)(GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue) < DateTime.Now)
            {

                ExpiredViewController expiredViewController = Storyboard.InstantiateViewController("ExpiredViewController") as ExpiredViewController;
                expiredViewController.expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;
                NavigationController.PushViewController(expiredViewController, true);

            }
            else
            {
                RedirectTodownloadView();
            }
        }


        public void DismissEditAndNavigateToAccount(UserAccountDifferenceScreen ObjUserDifference)
        {
            ObjUserDifference.DismissViewController(true, null);
            if (GlobalSettings.UserInfo != null && (DateTime)(GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue) < DateTime.Now)
            {

                ExpiredViewController expiredViewController = Storyboard.InstantiateViewController("ExpiredViewController") as ExpiredViewController;
                expiredViewController.expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;
                NavigationController.PushViewController(expiredViewController, true);

            }
            else
            {
                RedirectTodownloadView();
            }

        }

        public void DismissAllAndNavigateToDownload()
        {


            if (GlobalSettings.UserInfo != null && (DateTime)(GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue) < DateTime.Now)
            {

                ExpiredViewController expiredViewController = Storyboard.InstantiateViewController("ExpiredViewController") as ExpiredViewController;
                expiredViewController.expiredTime = GlobalSettings.UserInfo.PaidUntilDate ?? DateTime.MinValue;
                NavigationController.PushViewController(expiredViewController, true);

            }
            else
            {
                RedirectTodownloadView();
            }
        }

        public void DismissDownloadAndNavigateToConstraint(DownloadBidDataViewController ObjDownload)
        {
            if (ObjDownload != null)
            {
                ObjDownload.DismissViewController(true, null);
            }
            RedirectToConstraintsView();
        }
       

        public void RedirectToConstraintsView()
        {
            AuthorizationTestCaseViewController ConstraintsViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
            ConstraintsViewController.buttonTitle = Constants.GO_TO_CONSTRAINTS;
            ConstraintsViewController.messageError = Constants.ErrorMessages.ElementAt((int)AuthStaus.Filters - 1);
            ConstraintsViewController.topBarTitle = Enumerable.ElementAt(Constants.listTitleTopBar, (int)AuthStaus.Filters - 1);
            ConstraintsViewController.numberRow = (int)AuthStaus.Filters;
            PushViewController(ConstraintsViewController, true);
            //this.PresentViewController (DownloadData, true, null);
            //PushViewController (DownloadData, true);


        }
        #endregion
    }
}

