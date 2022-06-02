#region NameSpace
using iOSPasswordStorage;
using Security;
using System;
using System.Text.RegularExpressions;
using UIKit;
using System.Linq;
using System.Collections.Generic;
using Bidvalet.Shared;
using Bidvalet.Model;
using System.Collections.ObjectModel;
using Bidvalet.Shared.SWA;
using Bidvalet.iOS.Utility;
using Foundation;
using System.IO;
using Xamarin;
using Bidvalet.Business;
using static Bidvalet.iOS.Utility.CommonClass;
using System.Collections.Specialized;
#endregion

namespace Bidvalet.iOS
{
    public partial class SubmitBidViewController : BaseViewController
    {
        #region Variables

        LoadingOverlay _loadingOverlay;
        private string _empNumber = string.Empty;
        private string _password = string.Empty;
        private DateTime _expirationDate;
        int _typeOfInternetConnection;
        #endregion

        public SubmitBidViewController(IntPtr handle)
            : base(handle)
        {
        }

        #region Events
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            ShowNavigationBar();
            Title = "Submit";
            UIHelpers.StyleForButtons(new UIButton[] { btnLoginSubmitBid });
            lbCountReserve.Text = "You are about to submit " + GlobalSettings.SubmitBidDetails.SeniorityNumber + " choices";
            if (GlobalSettings.SubmitBidDetails.IsAddReserveToEnd)
            {
                lbCountReserve.Text += " plus reserve at end.";
            }
            setLoginCredentialsFromKeychaninToTextField();

            edtEmployeeNumber.KeyboardType = UIKeyboardType.NumbersAndPunctuation;

            this.edtEmployeeNumber.ShouldReturn += (textField) =>
            {
                edtCWAPassword.BecomeFirstResponder();
                return true;
            };

            this.edtCWAPassword.ShouldReturn += (textField) =>
            {
                textField.ResignFirstResponder();
                return true;
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void OnLoginSubmitBidClickEvent(Foundation.NSObject sender)
        {

            //@"Since you are not in the bid period			
            //You can practice with WBidValet	and sync with WBidMax. \n\nNo bid will be submitted since you are not in the bid period.Additionally, there will not be any bid receipt displayed."	

            if (GlobalSettings.IsHistorical)
            {
                HistoricalBidSubmission();

            }
            else
            {
                bool isinBidPeriod = CheckIsInSubmissionPeriod();


                if (!isinBidPeriod)
                {
                    PracticeBidSubmission();
                }
                else
                {


                    _loadingOverlay = new LoadingOverlay(this.View.Bounds, "Validating \n Please wait..");
                    this.View.AddSubview(_loadingOverlay);
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(true, true);
                    InvokeInBackground(() =>
                    {

                        if (CheckValidCredentials())
                        {


                            GlobalSettings.SubmitBidDetails.Base = GlobalSettings.CurrentBidDetails.Domicile;
                            GlobalSettings.SubmitBidDetails.BidRound = (GlobalSettings.CurrentBidDetails.Round == "S") ? "Round 2" : "Round 1";
                            GlobalSettings.SubmitBidDetails.PacketId = WBidHelper.GenaratePacketId(GlobalSettings.CurrentBidDetails);
                            GlobalSettings.SubmitBidDetails.Seat = GlobalSettings.CurrentBidDetails.Postion;
                            GlobalSettings.SubmitBidDetails.Bidder = _empNumber.ToLower().Replace("x", "").Replace("e", "");

                            GlobalSettings.SubmitBidDetails.TotalBidCount = GlobalSettings.SubmitBidDetails.SeniorityNumber;

                            if (GlobalSettings.CurrentBidDetails.Postion == "CP")
                            {
                                GlobalSettings.SubmitBidDetails.Bid = WBidHelper.GenarateBidLineString(GlobalSettings.SubmitBidDetails.IsSubmitAllChoices, GlobalSettings.SubmitBidDetails.SeniorityNumber);
                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
                            {
                                GlobalSettings.SubmitBidDetails.Bid = WBidHelper.GenarateBidLineString(GlobalSettings.SubmitBidDetails.IsSubmitAllChoices, GlobalSettings.SubmitBidDetails.SeniorityNumber);
                                AvoidanceBids avoidanceBids = GlobalSettings.WBidINIContent.AvoidanceBids;
                                GlobalSettings.SubmitBidDetails.Pilot1 = (avoidanceBids.Avoidance1 == "0") ? null : avoidanceBids.Avoidance1;
                                GlobalSettings.SubmitBidDetails.Pilot2 = (avoidanceBids.Avoidance2 == "0") ? null : avoidanceBids.Avoidance2;
                                GlobalSettings.SubmitBidDetails.Pilot3 = (avoidanceBids.Avoidance3 == "0") ? null : avoidanceBids.Avoidance3;
                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
                            {
                                GlobalSettings.SubmitBidDetails.Bid = GenarateBidLineString();
                                GlobalSettings.SubmitBidDetails.Buddy1 = GlobalSettings.SubmitBidDetails.Buddy2 = null;
                                if (GlobalSettings.SubmitBidDetails.IsAddReserveToEnd)
                                {

                                    GlobalSettings.SubmitBidDetails.Bid += string.IsNullOrEmpty(GlobalSettings.SubmitBidDetails.Bid) ? string.Empty : ",";
                                    GlobalSettings.SubmitBidDetails.Bid += "R";
                                    GlobalSettings.SubmitBidDetails.TotalBidCount++;

                                }
                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round != "M")
                            {
                                GlobalSettings.SubmitBidDetails.Bid = WBidHelper.GenarateBidLineString(GlobalSettings.SubmitBidDetails.IsSubmitAllChoices, GlobalSettings.SubmitBidDetails.SeniorityNumber);
                                GlobalSettings.SubmitBidDetails.Buddy1 = GlobalSettings.SubmitBidDetails.Buddy2 = null;
                                if (GlobalSettings.SubmitBidDetails.IsAddReserveToEnd)
                                {

                                    GlobalSettings.SubmitBidDetails.Bid += string.IsNullOrEmpty(GlobalSettings.SubmitBidDetails.Bid) ? string.Empty : ",";
                                    GlobalSettings.SubmitBidDetails.Bid += "R";
                                    GlobalSettings.SubmitBidDetails.TotalBidCount++;

                                }
                            }



                            if (GlobalSettings.SubmitBidDetails.Bid == string.Empty)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                    _loadingOverlay.Hide();
                                    DisplayAlertView(GlobalSettings.ApplicationName, "We cannot submit  empty bid order");
                                    return;
                                });
                            }


                            else
                            {


                                //0--No internet , 1-- on ground  ,2--on AIr 

                                _typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
                                // _typeOfInternetConnection = (int)InternetType.Air;
                                GlobalSettings.InternetType = _typeOfInternetConnection;
                                //typeOfInternetConnection = (int)InternetType.Air;
                                //No internet connection
                                if (_typeOfInternetConnection == (int)InternetType.NoInternet)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                        _loadingOverlay.Hide();
                                        RedirectToMessageView((int)AuthStaus.VPSDownAlert);
                                    });
                                }
                                //Grount type internet
                                else if (_typeOfInternetConnection == (int)InternetType.Ground || _typeOfInternetConnection == (int)InternetType.AirPaid)
                                {

                                    InvokeOnMainThread(() =>
                                    {
                                        _loadingOverlay.updateLoadingText("Checking credentials");
                                    });
                                    bool isAuthSuccess = CheckCWAAuthentication();
                                    if (isAuthSuccess)
                                    {
                                        //Removed on March 5 based on Frank's request
                                        //InvokeOnMainThread(() =>
                                        //{
                                        //    loadingOverlay.updateLoadingText("Checking Authorization");
                                        //});
                                        //isAuthSuccess = CheckWBidAuthentication();

                                        if (isAuthSuccess)
                                        {
                                            SubmitBid();
                                        }

                                    }


                                }
                                //Airtype internet
                                else if (_typeOfInternetConnection == (int)InternetType.Air)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                        _loadingOverlay.Hide();
                                        RedirectToMessageView((int)AuthStaus.SouthWestConnectionAlert);
                                    });
                                    //InvokeOnMainThread(() =>
                                    //{
                                    //    _loadingOverlay.updateLoadingText("Checking credentials");
                                    //});
                                    //bool isAuthSuccess = CheckCWAAuthentication();
                                    //// In the case of free wifi we dont need to  check the autorization process
                                    //if (isAuthSuccess)
                                    //{
                                    //    SubmitBid();
                                    //}
                                }
                            }


                        }
                        else
                        {

                            InvokeOnMainThread(() =>
                            {
                                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                                _loadingOverlay.Hide();
                                //DisplayAlertView(GlobalSettings.ApplicationName, "Invalid Credentials");
                            });
                        }


                    });

                }
            }

        }

        private bool CheckIsInSubmissionPeriod()
        {
            bool status = true;

            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
            DateTime submitStartTime = DateTime.MinValue;
            DateTime submitEndTime = DateTime.MinValue;
            //First Round
            if (GlobalSettings.CurrentBidDetails.Round == "M")
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    //3rd to 5th 12 pm  -- FA first round time
                    submitStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2, 12, 0, 0);
                    submitEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 5, 12, 0, 0);

                }
                else
                {
                    //5th to 9th 12 pm  -- Pilot first round time

                    submitStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4, 12, 0, 0);
                    submitEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 9, 12, 0, 0);


                }

            }
            else
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    //13th 15th 12 pm  -- FA first round time
                    submitStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 12, 12, 0, 0);
                    submitEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 12, 0, 0);


                }
                else
                {
                    //18th 19th 12 pm  -- Pilot first round time
                    submitStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 17, 12, 0, 0);
                    submitEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 19, 12, 0, 0);

                }

            }


            status = (cstTime >= submitStartTime) && (cstTime <= submitEndTime);



            return status;
        }

        private void HistoricalBidSubmission()
        {
            var alertHistorical = new UIAlertView(GlobalSettings.ApplicationName, "You are attempting to submit \"Practice\" data.  This app will not allow you to do so.  However, you can sync this practice bid with WBidMax" + Environment.NewLine + Environment.NewLine + "You previously downloaded this \"practice\" data when the bid period was NOT open.  If you want to submit a bid, touch \"Get Actual Bid Data\" button below and download the actual bid data for the bid period", null, "Sync Practice Bid ", new string[] { "Get Actual Bid Data" });
            alertHistorical.Clicked += (object senderObject, UIButtonEventArgs ev) =>
            {
                int index = (int)ev.ButtonIndex;
                if (index == 1)
                {

                    GlobalSettings.IsNeedToRedirectToBase = true;
                    NavigationController.PopToRootViewController(false);
                }
                else
                {
                    Practice();
                }


            };
            alertHistorical.Show();
        }

        private void PracticeBidSubmission()
        {
            var alertPractice = new UIAlertView(GlobalSettings.ApplicationName, @"Since you are not in the bid period You can practice with WBidValet and sync with WBidMax." + Environment.NewLine + Environment.NewLine + "No bid will be submitted since you are not in the bid period. Additionally, there will not be any bid receipt displayed.", null, "OK", null);
            alertPractice.Clicked += (object senderObject, UIButtonEventArgs ev) =>
            {

                Practice();
            };
            alertPractice.Show();
        }

        private void Practice()
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
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(true, true);
                    InvokeInBackground(() =>
                    {
                        UploadLocalVersionToServer();
                    });


                }
            };
            alertVW.Show();
        }
        #endregion
	

        #region Methods


        private void SubmitBid()
        {
            InvokeOnMainThread(() =>
            {
                _loadingOverlay.updateLoadingText("Submitting the Bid...");
            });


            SWASubmitBid objSWASubmitBid = new SWASubmitBid();
            string submitResult = objSWASubmitBid.SubmitBid(GlobalSettings.SubmitBidDetails, _empNumber, _password, GlobalSettings.SessionCredentials);

            // submitResult = "server failure";
            if (submitResult.Contains("SUBMITTED BY:"))
            {

                string fileName = submitResult.Substring(0, submitResult.IndexOf("\n")) + "Rct.pdf";
                if (fileName != null)
                {
                    string path = WBidHelper.GetAppDataPath();
                    List<string> fileNames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
                        .Where(s => s.ToLower().Contains("rct.pdf")).ToList();

                    foreach (var file in fileNames)
                    {

                        File.Delete(path + "/" + file);
                    }
                    if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                    {
                        CommonClass.SaveFormatBidReceipt(submitResult);
                    }
                    else
                    {
                        CommonClass.SaveFormattedBidReceiptForPilot(submitResult);
                    }

                    if (GlobalSettings.InternetType == (int)InternetType.Air)
                    {
                        WBidHelper.SaveInternalLog("submitBid", int.Parse(GlobalSettings.SubmitBidDetails.Bidder), WBidHelper.GetWBidInternalLogFilePath());
                    }
                    else if ((GlobalSettings.InternetType == (int)InternetType.AirPaid) || (GlobalSettings.InternetType == (int)InternetType.Ground))
                    {
                        try
                        {


                            RestHelper.LogOperation(SetSubmitLogDetails("submitBid"));


                        }
                        catch (Exception ex)
                        {

                        }
                        try
                        {


                            string submittedby = string.Empty;
                            string submittedfor = string.Empty;
                            string submitteddtg = string.Empty;
                            string submittedbid = string.Empty;
                            string[] stringSeparators = new string[] { "SUBMITTED BY:" };
                            var splittedstring = submitResult.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                            string[] lastlineseparator = new string[] { "  " };
                            var lastsplittedString = splittedstring[1].Split(lastlineseparator, StringSplitOptions.RemoveEmptyEntries);
                            if (lastsplittedString.Count() > 2)
                            {
                                submittedby = Regex.Replace(lastsplittedString[0], @"[^\d]", "");
                                submittedfor = lastsplittedString[1];
                                submitteddtg = lastsplittedString[2].Replace("\r\n", "");
                                submittedbid = splittedstring[0].Substring(splittedstring[0].IndexOf('\n'), splittedstring[0].Length - splittedstring[0].IndexOf('\n')).Replace('\n', ',').Replace("*E,", ",").Trim().TrimEnd(',').TrimStart(',');

                            }
                            else
                            {
                                submittedbid = GlobalSettings.SubmitBidDetails.Bid;
                            }
                            int submitbidder = 0;
                            if ((GlobalSettings.SubmitBidDetails.Bidder.Contains('x')) || (GlobalSettings.SubmitBidDetails.Bidder.Contains('X')))
                                submitbidder = int.Parse(GlobalSettings.SubmitBidDetails.Bidder.Replace("x", "").Replace("X", ""));
                            else
                                submitbidder = int.Parse(GlobalSettings.SubmitBidDetails.Bidder.Replace("e", "").Replace("E", ""));
                           
                            if (submittedby == string.Empty || submittedby == null)
                            {

                                InvokeOnMainThread(() =>
                                {

                                    DisplayAlertView(GlobalSettings.ApplicationName, "Your bid receipt has been returned with NO employee number.  This can occur when you are on a leave of absence.  Please contact us if you are not on a leave of absence ");

                                });
                                                              
                                RestHelper.LogOperation(SetSubmitLogDetails("MissingEmpNumReceipt"));

                               
                            }

                            SaveSubmittedDataToDB(submitbidder, submittedbid, submittedby, submittedfor, submitteddtg, GlobalSettings.SubmitBidDetails, GlobalSettings.SessionCredentials);
                            //GlobalSettings.WBidStateCollection.SubmittedResult = submbittingbid;
                            //int submitbidder = 0;
                            //if ((GlobalSettings.SubmitBid.Bidder.Contains('x')) || (GlobalSettings.SubmitBid.Bidder.Contains('X')))
                            //    submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("x", "").Replace("X", ""));
                            //else
                            //    submitbidder = int.Parse(GlobalSettings.SubmitBid.Bidder.Replace("e", "").Replace("E", ""));

                            //SaveSubmittedDataToDB(submitbidder, GlobalSettings.SubmitBid.Bid);
                            //GlobalSettings.WBidStateCollection.SubmittedResult = submbittingbid;
                        }
                        catch (Exception ex)
                        {}
                    
                    }


                    InvokeOnMainThread(() =>
                    {

                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        _loadingOverlay.Hide();

                        //try
                        //{


                        //    string dataDesc = GlobalSettings.SubmitBidDetails.Bidder + " " + GlobalSettings.SubmitBidDetails.Base + " " + GlobalSettings.SubmitBidDetails.BidRound + " " + GlobalSettings.SubmitBidDetails.Seat;
                        //    Insights.Identify(GlobalSettings.UserInfo.EmpNo, new Dictionary<string, string> { 
                        //        {Insights.Traits.Email, GlobalSettings.UserInfo.Email},
                        //        {Insights.Traits.Name, GlobalSettings.UserInfo.FirstName+" "+GlobalSettings.UserInfo.LastName},
                        //        {Insights.Traits.Description, "Submit Bid "+dataDesc}
               
                        //             });
                        //}
                        //catch (Exception)
                        //{


                        //}


                        BidReceiptViewController viewController = Storyboard.InstantiateViewController("BidReceiptViewController") as BidReceiptViewController;
                        if ((GlobalSettings.InternetType == (int)InternetType.AirPaid) || (GlobalSettings.InternetType == (int)InternetType.Ground))
                        {
                            viewController.IsneedToShowSync = true;
                        }
                        //viewController.BidReceipt = result;
                        PushViewController(viewController, true);
                    });
                }
            }
            else if (submitResult == "server failure")
            {

                if (GlobalSettings.InternetType == (int)InternetType.Air)
                {
                    WBidHelper.SaveInternalLog("bidSubmitTimeOut", int.Parse(GlobalSettings.SubmitBidDetails.Bidder), WBidHelper.GetWBidInternalLogFilePath());
                }
                else
                {

                    RestHelper.LogOperation(SetSubmitLogDetails("bidSubmitTimeOut"));
                }
                SaveSubmittedRawDataToDB(GlobalSettings.SubmitBidDetails,  GlobalSettings.SessionCredentials);
                InvokeOnMainThread(() =>
                {
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                             _loadingOverlay.Hide();
                             DisplayAlertView(GlobalSettings.ApplicationName, "Your attempt to submit a bid or download bid data has failed. Specifically, the Southwest Airlines server did not respond with a certain time, and as a result, you received a \"Server Timeout\".\n\n\nThis can happen for many reasons.  Our suggestion is to keep trying over the next 10 minutes or so, and if the app still fails to submit a bid or download bid data, we suggest the following:\n\n\nChange your internet connection from wifi to cellular.  \n\n\nYou should also try the following additional steps:  \n1.  Save your bid\n2.  Sync your work\n3.  Quit WBidMax\n4.  Power down the iPhone\n5.  Restart the iPhone\n6.  Restart WBidMax and attempt to submit your bid or download bid data\n\n\nFinally, send us an email if you are continuously having trouble.");
                         });

            }
            else
            {
                SaveSubmittedRawDataToDB(GlobalSettings.SubmitBidDetails, GlobalSettings.SessionCredentials);
                string msgalert = submitResult;
                if (submitResult.Contains("STRINGINDEXOUTOFBOUNDSEXCEPTION"))
                {
                    msgalert = "Your bid receipt has been returned with NO employee number.  This can occur when you are on a leave of absence.  Please contact us if you are not on a leave of absence ";
                    RestHelper.LogOperation(SetSubmitLogDetails("MissingEmpNumReceipt"));
                }
                InvokeOnMainThread(() =>
                {
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    _loadingOverlay.Hide();
                    
                    
                    DisplayAlertView(GlobalSettings.ApplicationName, msgalert);
                });
            }


            //}
        }
        private void SaveSubmittedRawDataToDB(SubmitBid submitBid, string sessioncredential)
        {
            try
            {
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                if (isConnectionAvailable)
                {
                    int employeenumber = 0;
                    if ((GlobalSettings.SubmitBidDetails.Bidder.Contains('x')) || (GlobalSettings.SubmitBidDetails.Bidder.Contains('X')))
                        employeenumber = int.Parse(GlobalSettings.SubmitBidDetails.Bidder.Replace("x", "").Replace("X", ""));
                    else
                        employeenumber = int.Parse(GlobalSettings.SubmitBidDetails.Bidder.Replace("e", "").Replace("E", ""));
                    
                    SubmittedDataRaw biddetails = new SubmittedDataRaw();
                    biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    biddetails.EmployeeNumber = employeenumber.ToString();
                    biddetails.RawData = GenerateSubmittedRawData(submitBid, sessioncredential);
                    biddetails.FromApp = 3;

                    var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<SubmittedDataRaw>(biddetails);

                    StreamReader dr = RestHelper.GetRestData("AddSubmittedRawDataToServer", jsonData);


                }
            }
            catch (Exception ex)
            {
            }
        }
        private string GenerateSubmittedRawData(SubmitBid submitBid, string sessioncredential)
        {
            //set the formdata values
            NameValueCollection formData = new NameValueCollection();
            formData["REQUEST"] = "UPLOAD_BID";
            formData["BASE"] = submitBid.Base;
            formData["BID"] = submitBid.Bid;
            formData["BIDDER"] = submitBid.Bidder;
            formData["BIDROUND"] = submitBid.BidRound;
            //formData["CLOSEDBIDSIM"] = "N";
            formData["CREDENTIALS"] = sessioncredential;
            formData["PACKETID"] = submitBid.PacketId;
            formData["SEAT"] = submitBid.Seat;
            formData["VENDOR"] = "WBidValet";
            // should always be null for CP and FA
            if (submitBid.Pilot1 != null) formData["PILOT1"] = submitBid.Pilot1;
            if (submitBid.Pilot2 != null) formData["PILOT2"] = submitBid.Pilot2;
            if (submitBid.Pilot3 != null) formData["PILOT3"] = submitBid.Pilot3;
            // should always be null for CP and FO
            if (submitBid.Buddy1 != null) formData["BUDDY1"] = submitBid.Buddy1;
            if (submitBid.Buddy2 != null) formData["BUDDY2"] = submitBid.Buddy2;
            string submittedraw = string.Empty;
            foreach (var item in formData.Keys)
            {
                submittedraw += item.ToString() + ":" + formData.GetValues(item.ToString())[0] + ",";
            }
            return submittedraw;
        }
        private void SaveSubmittedDataToDB(int empnum, string bid, string submittedby, string submittedfor, string submitteddtg, SubmitBid submitBid, string sessioncredential)
        {
            try
            {
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                if (isConnectionAvailable)
                {

                    BidSubmittedData biddetails = new BidSubmittedData();
                    biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    biddetails.EmpNum = empnum;
                    biddetails.SubmittedResult = bid;
                    biddetails.SubmitBy = submittedby;
                    biddetails.SubmitFor = submittedfor;
                    biddetails.SubmitDTG = submitteddtg;
                    biddetails.FromApp = 3;
                    biddetails.RawData = GenerateSubmittedRawData(submitBid, sessioncredential);
                    var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<BidSubmittedData>(biddetails);
                   
                    StreamReader dr = RestHelper.GetRestData("SaveBidSubmittedData", jsonData);
                    //GlobalSettings.WBidStateCollection.SubmittedResult = (WBidCollection.ConvertJSonStringToObject<BidSubmittedData>(dr.ReadToEnd())).SubmittedResult;
                }
            }
            catch (Exception ex)
            {
            }
        }


        private LogInformation SetSubmitLogDetails(string eventName)
        {

            //AppNum=(int)AppNum.BidValet,
            //        Base= GlobalSettings.CurrentBidDetails.Domicile,
            //        Date=DateTime.Now,
            //        EmployeeNumber=empNum,
            //        Event=eventName,
            //       // IpAddress="192.168.10.19",
            //        Message="Download From WBidValet",
            //        Month=GlobalSettings.CurrentBidDetails.Month.ToString(),
            //        OperatingSystemNum=GlobalSettings.OperatingSystem,
            //        PlatformNumber=GlobalSettings.Platform,
            //        Position=GlobalSettings.CurrentBidDetails.Postion,
            //        Round=(GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2,
            //        VersionNumber= System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString (),

            LogInformation logInfo = new LogInformation()
            {
                AppNum = (int)AppNum.BidValet,
                Base = GlobalSettings.CurrentBidDetails.Domicile,
                BidForEmpNum = int.Parse(GlobalSettings.SubmitBidDetails.Bidder),
                Date = DateTime.Now,
                EmployeeNumber = int.Parse(_empNumber.Replace("e", "").Replace("E", "")),
                Event = eventName,
                Message = eventName + " from WBidValet",
                Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper(),
                OperatingSystemNum = GlobalSettings.OperatingSystem,
                PlatformNumber = GlobalSettings.Platform,
                Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2,
                VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Position = GlobalSettings.CurrentBidDetails.Postion,


            };
            return logInfo;
        }

		private string GenarateBidLineString()
        {
            string bidlines = string.Empty;
            int seniorityNum;
            if (GlobalSettings.SubmitBidDetails.IsSubmitAllChoices)
                seniorityNum = 2000;
            else
                seniorityNum = GlobalSettings.SubmitBidDetails.SeniorityNumber;


            ObservableCollection<Line> lines = null;

            if (!GlobalSettings.SubmitBidDetails.IsReserveOnly)
            {
                lines = GlobalSettings.Lines;
            }
            else
            {
                lines = new ObservableCollection<Line>(GlobalSettings.Lines.Where(x => x.ReserveLine).ToList());
            }

            ObservableCollection<string> bidLineList = new ObservableCollection<string>();
            int count = 0;
            if (GlobalSettings.SubmitBidDetails.IsRepeatline)
            {

                foreach (Line line in lines)
                {

                    foreach (string position in GlobalSettings.SubmitBidDetails.PositionChoices)
                    {
                        if (line.FAPositions.Contains(position))
                        {
                            bidLineList.Add(line.LineNum + position);
                            count++;
                            if (count == seniorityNum)
                            {
                                break;
                            }
                        }
                    }

                    if (count == seniorityNum)
                        break;


                }

            }
            else
            {

                foreach (string position in GlobalSettings.SubmitBidDetails.PositionChoices)
                {
                    foreach (Line line in lines)
                    {
                        if (line.FAPositions.Contains(position))
                        {
                            bidLineList.Add(line.LineNum + position);
                            count++;
                            if (count == seniorityNum)
                            {
                                break;
                            }
                        }

                    }
                    if (count == seniorityNum)
                        break;

                }

            }

            bidlines = string.Join(",", bidLineList.Select(x => x.ToString()));
            return bidlines;
        }

        private bool CheckValidCredentials()
        {

            bool status = false;
            InvokeOnMainThread(() =>
            {
                try
                {
                    if (Regex.Match(edtEmployeeNumber.Text.Trim(), "^[e,E,x,X,0-9][0-9]*$").Success)
                    {
                        if (edtCWAPassword.Text.Length > 0)
                        {
                            _empNumber = edtEmployeeNumber.Text.Trim().ToLower();
                            if (_empNumber[0] != 'e' && _empNumber[0] != 'x')
                                _empNumber = "e" + _empNumber;

                            _password = edtCWAPassword.Text;
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

        private bool CheckCWAAuthentication()
        {

            bool status = false;
            try
            {


                SWAAuthentication authentication = new SWAAuthentication();
                string authResult = authentication.CheckCredential(_empNumber, _password);
                if (authResult.Contains("ERROR: "))
                {
					RestHelper.LogOperation(WBidHelper.LogBadPasswordUsage(_empNumber,false));
                    InvokeOnMainThread(() =>
                    {
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        _loadingOverlay.Hide();
                        RedirectToMessageView((int)AuthStaus.Invalid_Login);
                    });
                }
                else if (authResult.Contains("Exception"))
                {
                    InvokeOnMainThread(() =>
                    {
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        _loadingOverlay.Hide();
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
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    _loadingOverlay.Hide();
                    RedirectToMessageView((int)AuthStaus.TO_3rd_pty_ground);
                });
            }
            return status;
        }

        private bool CheckWBidAuthentication()
        {
            bool status = false;
            try
            {

                ClientRequestModel requestModel = SetClientRequestDetails();

                AuthServiceResponseModel responseModel = RestHelper.CheckWBidAuthentication(requestModel);
                if (responseModel != null)
                {

                    if (responseModel.Type == "TimeOut")
                    {

                        InvokeOnMainThread(() =>
                        {
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            _loadingOverlay.Hide();
                            RedirectToMessageView((int)AuthStaus.TO_BVDB_ground);
                        });
                    }
                    else if (responseModel.Type == "Success" || responseModel.Type == "TemporaryAuthenticate")
                    {
                        InvokeOnMainThread(() =>
                        {
                            status = true;
                            //loadingOverlay.Hide();
                            //RedirectToMessageView((int)AuthStaus.Inavalid_Account);
                        });

                    }
                    else if (responseModel.Type == "Invalid Account")
                    {
                        InvokeOnMainThread(() =>
                        {
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            _loadingOverlay.Hide();
                            RedirectToDynamicMessageView((int)AuthStaus.Inavalid_Account,responseModel.Message);
                        });
                        //DisplayMessage("Auth","Invalid Account");
                    }
                    else if (responseModel.Type == "Invalid Version")
                    {
                        InvokeOnMainThread(() =>
                        {
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            _loadingOverlay.Hide();
                            RedirectToDynamicMessageView((int)AuthStaus.Version_not_supported,responseModel.Message);
                        });

                        //DisplayMessage("Auth","Invalid Version");

                    }
                    else if (responseModel.Type == "Invalid Subscription")
                    {
                        _expirationDate = responseModel.ExpirationDate;
                        InvokeOnMainThread(() =>
                        {
                            this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                            _loadingOverlay.Hide();
                              RedirectToDynamicMessageView((int)AuthStaus.Expired_subscription,responseModel.Message);
                        });
                        //DisplayMessage("Auth","Invalid Subscription");

                    }
                }

                //}
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {



                    status = true;

                    //loadingOverlay.Hide();
                    //RedirectToMessageView((int)AuthStaus.TO_BVDB_ground);
                });
            }
            return status;
        }


        #region SetLoginDetails
        private void setLoginCredentialsFromKeychaninToTextField()
        {
            try
            {
                this.edtEmployeeNumber.Text = KeychainHelpers.GetPasswordForUsername("user", "WBid.BidValet.cwa", false);
                this.edtCWAPassword.Text = KeychainHelpers.GetPasswordForUsername("pass", "WBid.BidValet.cwa", false);
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


        private ClientRequestModel SetClientRequestDetails()
        {
            ClientRequestModel obj = new ClientRequestModel();
            obj.Base = GlobalSettings.CurrentBidDetails.Domicile;
            int round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
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
            obj.RequestType = (int)RequestTypes.SubmitBid;
            //new DateTime(2016, 2, 1).ToString("MMM").ToUpper();
            obj.OperatingSystem = GlobalSettings.OperatingSystem;
            obj.Platform = GlobalSettings.Platform;
            obj.Postion = GlobalSettings.CurrentBidDetails.Postion;
            obj.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")].ToString()+".0";
            // obj.app   

            return obj;
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

        private void RedirectToDynamicMessageView(int index,string message)
        {
            AuthorizationTestCaseViewController testCaseViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
            testCaseViewController.messageError = message.Replace("\\n", "\n");;
            testCaseViewController.topBarTitle = Constants.listTitleTopBar.ElementAt(index - 1);

            testCaseViewController.numberRow = (int)(index);
            if (index == Constants.EXPIRED_SUBSCRIPTION)
            {
                testCaseViewController.isShowPurchaseButton = true;
            }
            
            PushViewController(testCaseViewController, true);
        }


        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        }

        public ObservableCollection<string> bidLineList { get; set; }


        private void UploadLocalVersionToServer()
        {
            _typeOfInternetConnection = InternetHelper.CheckInterNetConnection();

            if (_typeOfInternetConnection == (int)InternetType.NoInternet)
            {
                InvokeOnMainThread(() =>
                {
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    _loadingOverlay.Hide();
                    RedirectToMessageView((int)AuthStaus.VPSDownAlert);
                });
            }
            //Grount type internet
            else if (_typeOfInternetConnection == (int)InternetType.Ground || _typeOfInternetConnection == (int)InternetType.AirPaid)
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
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        _loadingOverlay.Hide();

                        DateTime timeUtc = DateTime.UtcNow;
                        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
                        DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);

                        string syncMessage= "Practice"+Environment.NewLine ;
                        syncMessage += GlobalSettings.CurrentBidDetails.Domicile + " " + GlobalSettings.CurrentBidDetails.Postion + " " + new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM") + " Rnd " + (GlobalSettings.CurrentBidDetails.Round == "M" ? 1 : 2) + ":" + Environment.NewLine;
                        syncMessage += String.Format("{0:MM/dd/yyyy @ H:mm CST}", cstTime);
                        GlobalSettings.UserInfo.LastSyncInfo = syncMessage;

                        WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);

                        ///DisplayAlertView(GlobalSettings.ApplicationName, "Successfully Synchronized  your bid with the server.");
                        ///
                        var alertSync = new UIAlertView(GlobalSettings.ApplicationName, "Successfully Synchronized  your bid with the server.", null, "OK", null);
                        alertSync.Clicked += (object senderObject, UIButtonEventArgs ev) =>
                        {

                            NavigationController.PopToRootViewController(false);
                        };
                        alertSync.Show();
                      
                    });


                }


            }
            else
            {
                InvokeOnMainThread(() =>
                {
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    _loadingOverlay.Hide();
                    //DisplayAlertView(GlobalSettings.ApplicationName, "You are using the Southwest Wifi on the airplane. This internet connection is limited.  ");
                    DisplayAlertView(GlobalSettings.ApplicationName,Constants.SouthWestConnectionAlert );
                });

            }



        }




        private string SaveStateToServer(string stateFileName)
        {
            string response = "-1";
            try
            {



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
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        _loadingOverlay.Hide();
                        DisplayAlertView(GlobalSettings.ApplicationName, "An error occured while synchronizing your state to the server.");
                    });
                }
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {
                    this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    _loadingOverlay.Hide();
                    DisplayAlertView(GlobalSettings.ApplicationName, "An error occured while synchronizing your state.");
                });

            }

            return response;
        }
        #endregion
    }
}


