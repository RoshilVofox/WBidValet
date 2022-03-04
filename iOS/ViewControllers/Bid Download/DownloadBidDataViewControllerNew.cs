
#region NameSpace
using System;
using System.Globalization;
using Foundation;
using UIKit;
using iOSPasswordStorage;
using Bidvalet.Business;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bidvalet.Shared;
using System.Linq;
using System.Text;
using System.IO;
using Bidvalet.Model;
using System.Text.RegularExpressions;
using System.Net;
using System.IO.Compression;
using System.Collections.ObjectModel;
using Xamarin;
using Bidvalet.Business.BusinessLogic;
using CoreGraphics;
//using WBid.WBidClient.Models;



#endregion


namespace Bidvalet.iOS
{
    public partial class DownloadBidDataViewController : UIViewController
    {

        #region Public Variables

        public UIViewController ObjParentController;

        public bool IsMissingTripFailed { get; set; }
        public bool isVacationFileDownloaded { get; set; }
        private string _stateFileName = string.Empty;
        private bool _isCompanyServerData { get; set; }
        private List<Vacation> _vacation { get; set; }
        private SeniorityListItem _seniorityListItem = new SeniorityListItem();
        private List<Absense> _fVVacation { get; set; }
        private WBidState wBIdStateContent;
        public int _paperCount = 0;
        public bool _isInSeriority { get; set; }
        BidDataFileResponse biddataresponse;
        BidDataFiles vacationlinesObject;
        public Dictionary<string, TripMultiVacData> VacationData { get; set; }
        UIAlertController AlertController;
        public TripTtpParser ObjTripTtpParser
        {
            get
            {
                return _objTripTtpParser ?? (_objTripTtpParser = new TripTtpParser());
            }
        }
        public CalculateTripProperties ObjCalculateTripProperties
        {
            get
            {
                return _objCalculateTripProperties ?? (_objCalculateTripProperties = new CalculateTripProperties());
            }
        }
        public CalculateLineProperties ObjCalculateLineProperties
        {
            get
            {
                return _objCalculateLineProperties ?? (_objCalculateLineProperties = new CalculateLineProperties());
            }
        }

        #endregion

        #region Private Variables
        private DownloadBid _objDownloadBidObject = new DownloadBid();
        public DownloadInfo _objDownloadInfoDetails;
        private Dictionary<string, Trip> _objTrips = null;
        private Dictionary<string, Line> _objLines = null;
        private List<SeniorityListMember> _objSeniorityListMembers;
        private SeniorityListItem _objSeniorityListItem = new SeniorityListItem();
        List<NSObject> arrObserver = new List<NSObject>();
        /// <summary>
        /// create single instance of TripTtpParser class
        /// </summary>
        private TripTtpParser _objTripTtpParser;
        private CalculateTripProperties _objCalculateTripProperties;
        private CalculateLineProperties _objCalculateLineProperties;



        #endregion


        public DownloadBidDataViewController(IntPtr handle)
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
            vacCorrectionText.Text = "";
            if(!GlobalSettings.IsHistorical)
                vacCorrectionText.Text = "The Vacation Correction can take up to 60 seconds or more.It should not take more than 2 minutes.";
            
            Console.WriteLine("Download View Init");
            base.ViewDidLoad();
            this.prgrsVw.SetProgress(0.0f, false);
            this.observeNotifications();
            GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();
            GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
            GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
            GlobalSettings.MenuBarButtonStatus.IsMIL = false;

            NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);

            NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);

            NSNotificationCenter.DefaultCenter.PostNotificationName("authCheckSuccess", null);


            /////--------------------------------------------
            if (GlobalSettings.DownloadBidDetails != null && !GlobalSettings.IsHistorical)
            {
                GlobalSettings.DownloadBidDetails.Month = GlobalSettings.IsCurrentMonthOn ? DateTime.Now.Month : DateTime.Now.AddMonths(1).Month;


                GlobalSettings.DownloadBidDetails.Year = (DateTime.Now.Month == 12 && GlobalSettings.DownloadBidDetails.Month == 1) ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year;
            }
            /////--------------------------------------------

            //Checking data available for  Practise and Submit feature
            //bool isDataAvailable = CheckDataAvailable();
            //isDataAvailable = true;

            //GlobalSettings.IsHistorical = !isDataAvailable;


            if (GlobalSettings.IsHistorical)
            {

                //string message = "The Bid Period is not open for the ";
                //if (GlobalSettings.DownloadBidDetails != null && GlobalSettings.DownloadBidDetails.Round == "D")
                //    message += "1st ";
                //else
                //    message += "2nd ";
                //message += "Round. \nBut you can still practice.";

                //message += "\n\nWe will download the Historical Bid data from the previous month.";

                //InvokeOnMainThread(() =>
                //{

                //    var alertHistorical = new UIAlertView(GlobalSettings.ApplicationName, message, null, "NO", new string[] { "OK" });
                //    alertHistorical.Clicked += (object senderObject, UIButtonEventArgs ev) =>
                //    {
                        //int index = (int)ev.ButtonIndex;
                        //if (index == 1)
                        //{
                            InvokeInBackground(() =>
                            {
                                DownloadProcessSetup();
                            });
                        //}
                        //else
                        //{
                        //    DismissCurrentView();

                       // }


                //    };
                //    alertHistorical.Show();
                //});



            }
            else
            {


                //PerformSelector (new ObjCRuntime.Selector ("DownloadProcessSetup:"), null, 10);
                InvokeInBackground(() =>
                {
                    DownloadProcessSetup();
                });
            }


            // Perform any additional setup after loading the view, typically from a nib.
        }


        private bool CheckDataAvailable()
        {
            bool status = true;


            //GlobalSettings.IsCurrentMonthOn  --This feature is only for Admin
            if (!GlobalSettings.IsCurrentMonthOn)
            {
                //Getting the Current CST time
                //----------------------------------------------
                DateTime timeUtc = DateTime.UtcNow;
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
                DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                //-------------------------------------------------------------------------

                //Generating default bid available time of each position
                //-----------------------------------------------------------------------
                DateTime bidAvailableTime;

                if (GlobalSettings.DownloadBidDetails.Round == "D")//First Round
                {
                    if (GlobalSettings.DownloadBidDetails.Postion == "FA")
                    {   //Flt Att  round1 opens on 2nd at 12:00
                        bidAvailableTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2, 12, 0, 0);
                    }
                    else
                    {   //Pilot round 1 opens on 4th at 12:00
                        bidAvailableTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4, 12, 0, 0);
                    }
                }
                else//Second Round
                {
                    bidAvailableTime = GlobalSettings.DownloadBidDetails.Postion == "FA" ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 12, 12, 0, 0) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 17, 12, 0, 0);
                }

                //if current cst time  greater than bidAvailable time then Data availble  vale will be TRUE
                status = (bidAvailableTime <= cstTime);

            }
            //status = true;
            return status;

        }





        #endregion

        #region Methods

        /// <summary>
        /// Downloads  process .
        /// </summary>
        public void DownloadProcessSetup()
        {

            startProgress();
            try
            {
                
                _objDownloadInfoDetails = new DownloadInfo
                {
                    UserId = KeychainHelpers.GetPasswordForUsername("user", "WBid.BidValet.cwa", false),
                    Password = KeychainHelpers.GetPasswordForUsername("pass", "WBid.BidValet.cwa", false),
                    SessionCredentials = GlobalSettings.SessionCredentials,
                    DownloadList = WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails)
                };
                _isCompanyServerData = GlobalSettings.WBidINIContent.Data.IsCompanyData;

                if (!GlobalSettings.IsHistorical)
                {
                   
                    //Download and save Bid data//
                    bool isBidDataAvailable = DownloadAndSaveBidDataFromWBid(false, false);

                    if (isBidDataAvailable == true)
                    {



                        WBidHelper.SetCurrentBidInformationfromStateFileName(_stateFileName);


                        GenerateStateFile(_stateFileName);


                        SetValuesToStateFile();

                        ReadWbUpdateAndUpdateINIFile();


                        WBidHelper.GenerateDynamicOverNightCitiesList();
                        GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList(); var linePairing = GlobalSettings.Lines.SelectMany(y => y.Pairings);

                        SetDRPColoring();

                        WBidHelper.RecalculateAMPMAndWekProperties(false);

                        ChecktheUserIsInEBGGroup();


                        NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);

                        StateManagement statemanagement = new StateManagement();
                        statemanagement.ApplyCSW(wBIdStateContent);

                       

                        DownloadFlightData();
                        ShowSeniorityListInformation();
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                                   {
                                       DismissCurrentView();
                                       DisplayAlertView("Error!!!", "Data Transfer Failed");
                                   });

                        //showAlertAndSimissView("Data Transfer Failed", "Error");
                        //DismisscCurrentView();
                    }
                }
                else
                {
                    
                    //hisotrical data download process
                    bool isBidDataAvailable = DownloadAndSaveBidDataFromWBid(true, false);

                    if (isBidDataAvailable == true)
                    {

                        WBidHelper.SetCurrentBidInformationfromStateFileName(_stateFileName);
                        GenerateStateFile(_stateFileName);

                        SetValuesToStateFile();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);
                        //ShowSeniorityListInformation();
                        //NSNotificationCenter.DefaultCenter.PostNotificationName("getDataFilesSuccess", null);

                        //((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);
                        InvokeOnMainThread(() =>
                        {
                            if (ObjParentController != null)
                            {

                                ((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);

                            }
                            moveProgressToEnd();
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            DismissCurrentView();
                            DisplayAlertView("Error!!!", "Data Transfer Failed");
                        });

                        // showAlertAndSimissView("Data Transfer Failed", "Error");
                        //DismisscCurrentView();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //InvokeOnMainThread(() =>
            //{
            //    if (ObjParentController != null)
            //    {
                    
            //        ((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);

            //    }
            //});



        }



        /// <summary>
        /// Download and save WBL,WBP ,Vacation Files, Seniority and Cover letter from Wbid VPS, For historic bid data, it will give only WBL and WBP file
        /// </summary>
        public bool DownloadAndSaveBidDataFromWBid(bool isHistorical, bool isFromMultipleBidDownload)
        {
            try
            {
                vacationlinesObject = new BidDataFiles();
                //IMplement the service
                //Set vacation,senlist and FV vacation from the service
                biddataresponse = new BidDataFileResponse();
                BidDataRequestDTO bidDetails = new BidDataRequestDTO();
                bidDetails.EmpNum = GlobalSettings.IsDifferentUser ? Convert.ToInt32(GlobalSettings.ModifiedEmployeeNumber) : Convert.ToInt32(Regex.Match(_objDownloadInfoDetails.UserId, @"\d+").Value);
                bidDetails.Base = GlobalSettings.DownloadBidDetails.Domicile;
                bidDetails.Position = GlobalSettings.DownloadBidDetails.Postion;
                bidDetails.Month = GlobalSettings.DownloadBidDetails.Month;
                bidDetails.Year = GlobalSettings.DownloadBidDetails.Year;
                bidDetails.Round = GlobalSettings.DownloadBidDetails.Round == "D" ? "M" : "S";
                bidDetails.IsHistoricBid = isHistorical;
                bidDetails.IsQATest = GlobalSettings.QATest;
                try
                {
                    var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<BidDataRequestDTO>(bidDetails);

                    StreamReader dr = RestHelper.GetRestData("GetMonthlyBidFiles", jsonData);

                    biddataresponse = SerializeHelper.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());


                    if (GlobalSettings.UserInfo.AppDataBidFiles == null)
                        GlobalSettings.UserInfo.AppDataBidFiles = new List<AppDataBidFileNames>();
                    AppDataBidFileNames appDataBidFileNames = GlobalSettings.UserInfo.AppDataBidFiles.FirstOrDefault(x => x.Domicile == bidDetails.Base && x.Position == bidDetails.Position && x.Round == bidDetails.Round && x.Month == bidDetails.Month && x.Year == bidDetails.Year);
                    if (appDataBidFileNames == null)
                    {

                        //it means file is not already downlaoded. Then we need to add app bid data file names into the users file
                        GlobalSettings.UserInfo.AppDataBidFiles.Add(new AppDataBidFileNames
                        {
                            lstBidFileNames = biddataresponse.BidFileNames,
                            Domicile = bidDetails.Base,
                            Month = bidDetails.Month,
                            Position = bidDetails.Position,
                            Round = bidDetails.Round,
                            Year = bidDetails.Year
                        });
                    }
                    else
                    {
                        if (appDataBidFileNames.lstBidFileNames.Count() <= biddataresponse.BidFileNames.Count())
                        {
                            appDataBidFileNames.lstBidFileNames = biddataresponse.BidFileNames;
                        }
                    }

                    if (!isFromMultipleBidDownload)
                    {
                        UpdateUserDomicileAndPosition();
                        WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);
                    }




                }
                catch (Exception ex)
                {

                    return false;
                }
                if (biddataresponse.Status == true)
                {



                    if (!isFromMultipleBidDownload)
                    {
                        //Downlaod news.pdf, wbidupdate.dat etc
                        DownloadWBidFiles();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("getDataFilesSuccess", null);
                    }
                    //Show alert if the bid data is not available
                    if (biddataresponse.bidData.Count > 0)
                    {

                        vacationlinesObject = biddataresponse.bidData.FirstOrDefault((x => x.FileName.Contains(".WBV")));
                        if (vacationlinesObject != null && vacationlinesObject.IsError == false && !isFromMultipleBidDownload)
                        {
                            isVacationFileDownloaded = true;
                            //vacation exists.
                        }
                        //Ierate through all Bid data files and save the file
                        foreach (var item in biddataresponse.bidData)
                        {
                            if (!item.IsError)
                            {
                                var fileExtension = item.FileName.Split('.')[1].ToString().ToLower();

                                switch (fileExtension)
                                {

                                    case "wbl":

                                        _stateFileName = item.FileName.Substring(0, 10) + "737";
                                        //Decompress the string using LZ compress.
                                        string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                                        File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
                                        if (isVacationFileDownloaded == false && !isFromMultipleBidDownload)
                                        {
                                            //desrialise the Json
                                            LineInfo wblLine = SerializeHelper.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);

                                            GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);


                                        }


                                        break;
                                    case "wbp":

                                        //Decompress the string using LZ compress.
                                        string tripfileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                                        File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
                                        if (!isFromMultipleBidDownload)
                                        {
                                            //desrialise the Json
                                            Dictionary<string, Trip> wbpLine = SerializeHelper.ConvertJSonStringToObject<Dictionary<string, Trip>>(tripfileJsoncontent);

                                            GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);
                                        }

                                        break;
                                    case "json":

                                        break;
                                    case "wbv":
                                        //Decompress the string using LZ compress.
                                        string vacationlinefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                                        File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);

                                        if (isVacationFileDownloaded)
                                        {
                                            //desrialise the Json
                                            LineInfo wblLine = SerializeHelper.ConvertJSonStringToObject<LineInfo>(vacationlinefileJsoncontent);

                                            GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);

                                        }
                                        break;

                                    default:
                                        if (!isHistorical && !item.IsError)
                                        {
                                            File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
                                        }
                                        break;

                                }
                            }
                        }
                        //NSNotificationCenter.DefaultCenter.PostNotificationName("parseDataSuccess", null);

                    }

                    if (!isFromMultipleBidDownload)
                    {
                        ////SeniorityList
                        _objSeniorityListItem.SeniorityNumber = biddataresponse.DomcileSeniority;
                        _objSeniorityListItem.TotalCount = biddataresponse.TotalSenliorityMember;
                        if (biddataresponse.ISEBGUser)
                            _objSeniorityListItem.EBgType = "Y";
                        _paperCount = biddataresponse.paperCount;
                        _isInSeriority = biddataresponse.IsSeniorityExist;

                        ////Vacation
                        if (biddataresponse.Vacation.Count > 0)
                            _vacation = biddataresponse.Vacation;

                        ////FV Vacation
                        if (biddataresponse.FVVacation.Count > 0)
                            _fVVacation = biddataresponse.FVVacation;
                    }

                    return true;
                }
                else
                {
                    InvokeOnMainThread(() =>
                            {
                                DismissCurrentView();
                                DisplayAlertView("Error!!!", biddataresponse.Message);
                            });


                    //Show the server message from response.
                    return false;
                }

            }
            catch (Exception ex)
            {
                // WBidHelper.AddDetailsToMailCrashLogs(ex);
                return false;

            }
        }

        /// <summary>
		/// Alert to user when user is in seniority list or not. Also Display the paper bid information ///Differnt alert for Pilots and FA
		/// </summary>
		private void ShowSeniorityListInformation()
        {
            try
            {
                _seniorityListItem = _objSeniorityListItem;
                string message = string.Empty;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" || ((GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") && GlobalSettings.CurrentBidDetails.Round == "S"))
                {

                    if (_isInSeriority)
                    {
                        message = "WBidMax found you in the Seniority List !! ";
                    }
                    else
                    {
                        message = "WBidMax DID NOT find you in the Seniority List." +
                                                    "\nYou may want to check your assigned Domicile for next month." +
                                                    "\n\nDO NOT BID THESE LINES!!!!!";
                    }
                }
                else
                {

                    if (_isInSeriority)
                    {
                        int actualSeniority = _seniorityListItem.SeniorityNumber - _paperCount;

                        message = "WBidMax found you in the Seniority List !! . You are number " + _seniorityListItem.SeniorityNumber + " out of " + _seniorityListItem.TotalCount + "\n\n There are " + _paperCount + " paper bids and ExTO/ETO above you, making you " + actualSeniority + " on the bid list";
                    }
                    else
                    {

                        message = "WBidMax DID NOT find you in the Seniority List." +
                                                    "\nYou may want to check your assigned Domicile for next month." +
                                                    "\n\nDO NOT BID THESE LINES!!!!!";
                    }
                }
                string seniorityFileName = string.Empty;
                if (GlobalSettings.CurrentBidDetails.Round == "M")
                {
                    seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S";
                }
                else if (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                    seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R";
                }
                else if (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "SR";
                }
                var sencheckempnum = (GlobalSettings.IsDifferentUser) ? GlobalSettings.ModifiedEmployeeNumber : _objDownloadInfoDetails.UserId;
                int intEmpNum = Convert.ToInt32(Regex.Match(sencheckempnum, @"\d+").Value);
                Convert.ToInt32(Regex.Match(_objDownloadInfoDetails.UserId, @"\d+").Value);
                InvokeOnMainThread(() =>
                {

                    AlertController = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);

                    AlertController.AddAction(UIAlertAction.Create("View Seniority List", UIAlertActionStyle.Default, (actionViewSen) =>
                    {
                        //NSNotificationCenter.DefaultCenter.PostNotificationName("DismissDownloadAndRedirectoLineView", null);

                        //DismisscAndRedirectToLineView();
                        webPrint fileViewer = new webPrint();

                        fileViewer.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        this.PresentViewController(fileViewer, true, () =>
                        {
                            fileViewer.loadFileFromUrlFromDownload(seniorityFileName + ".TXT", intEmpNum.ToString());
                            //	DismisscAndRedirectToLineView();
                        });

                    }));
                    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionOK) =>
                    {

                        if ((biddataresponse.Vacation.Count > 0 || biddataresponse.FVVacation.Count > 0) && (vacationlinesObject != null && vacationlinesObject.IsError))
                        {


                            InvokeOnMainThread(() =>
                            {
                                AlertController = UIAlertController.Create("WbidMax", biddataresponse.Message, UIAlertControllerStyle.Alert);
                                AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        if (!GlobalSettings.IsHistorical)
                                        {
                                            if (ObjParentController != null)
                                            {

                                                ((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);

                                            }
                                            // GlobalSettings.ExtraErrorInfo += "Parse ParseSeniorityList Finished\n<br>";
                                            //GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Started\n<br>";
                                            //DismisscAndRedirectToLineView();
                                            //GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Finished\n<br>";
                                        }

                                    });
                                }));

                                this.PresentViewController(AlertController, true, null);

                            });
                            //showAlertView(biddataresponse.Message, "Error!!");

                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (!GlobalSettings.IsHistorical)
                                {
                                    if (ObjParentController != null)
                                    {

                                        ((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);

                                    }
                                    //GlobalSettings.ExtraErrorInfo += "Parse ParseSeniorityList Finished\n<br>";
                                    //GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Started\n<br>";
                                    //DismisscAndRedirectToLineView();
                                    //GlobalSettings.ExtraErrorInfo += "Parse CheckAndPerformVacationCorrection Finished\n<br>";
                                }

                            });
                        }
                    }));

                    this.PresentViewController(AlertController, true, null);

                });
            }
            catch (Exception ex)
            {
                //WBidHelper.AddDetailsToMailCrashLogs(ex);
            }
        }
        /// <summary>
		/// update the users domicile and position into the local user information
		/// </summary>
		private void UpdateUserDomicileAndPosition()
        {
            GlobalSettings.UserInfo.Domicile = GlobalSettings.DownloadBidDetails.Domicile;
            GlobalSettings.UserInfo.BidSeat = GlobalSettings.DownloadBidDetails.Postion;
            //WBidHelper.SaveUserFile(GlobalSettings.WbidUserContent, WBidHelper.WBidUserFilePath);
        }


        /// <summary>
		/// Generate state file if the file is not available
		/// </summary>
		private void GenerateStateFile(string StateFileName)
        {
            //Read the intial state file value from DWC file and create state file
            if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS"))
            {
                try
                {
                    WBidIntialState wbidintialState = null;
                    try
                    {
                        wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    }
                    catch (Exception)
                    {
                        //WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                        try
                        {
                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                        }
                        catch (Exception)
                        {

                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                            XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                            //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");

                        }
                        XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                        // obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");

                    }
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS", GlobalSettings.Lines.Count, GlobalSettings.Lines.First().LineNum, wbidintialState);
                    if (GlobalSettings.IsHistorical)
                    {
                        GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    }
                    else
                        GlobalSettings.WBidStateCollection.DataSource = (_isCompanyServerData) ? "Original" : "Mock";
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                try
                {
                    //Read the state file object and store it to global settings.
                    GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS");
                }
                catch (Exception)
                {

                    //Recreate state file
                    //--------------------------------------------------------------------------------
                    WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + StateFileName + ".WBS", GlobalSettings.Lines.Count, GlobalSettings.Lines.First().LineNum, wbidintialState);
                    //WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                    //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");
                    if (GlobalSettings.IsHistorical)
                    {
                        GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    }
                    else
                        GlobalSettings.WBidStateCollection.DataSource = (_isCompanyServerData) ? "Original" : "Mock";
                }

                if (GlobalSettings.IsHistorical)
                {
                    GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                }
                else if (GlobalSettings.WBidStateCollection.DataSource == "Original" && _isCompanyServerData == false)
                {
                    GlobalSettings.WBidStateCollection.DataSource = "Mock";

                }
                else if (GlobalSettings.WBidStateCollection.DataSource == "Mock" && _isCompanyServerData == true)
                {
                    GlobalSettings.WBidStateCollection.DataSource = "Original";
                }

            }

            //GlobalSettings.WBidStateCollection.DownlaodedbidFiles = downloadedBidList;
            wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            ManageStateFile();


        }
        private void ManageStateFile()
        {
            if (wBIdStateContent.CxWtState.MixedHardReserveTrip == null)
                wBIdStateContent.CxWtState.MixedHardReserveTrip = new StateStatus { Cx = false, Wt = false };
            if (wBIdStateContent.CxWtState.StartDay == null)
                wBIdStateContent.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
            if (wBIdStateContent.CxWtState.ReportRelease == null)
                wBIdStateContent.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };

        }

        /// <summary>
		/// Set VA,FC,CFV vacation details to the state file. We are getting this information from VPS.
		/// </summary>
		private void SetValuesToStateFile()
        {
            try
            {
                GlobalSettings.WBidStateCollection.Vacation = _vacation;
                GlobalSettings.WBidStateCollection.SeniorityListItem = _seniorityListItem;

                GlobalSettings.WBidStateCollection.FVVacation = _fVVacation;
                GlobalSettings.FVVacation = _fVVacation;
                //GlobalSettings.SeniorityListMember = _seniorityListItem;
                GlobalSettings.IsVacationCorrection = false;

                if (!GlobalSettings.IsHistorical)
                {
                    if (GlobalSettings.WBidStateCollection.Vacation != null && GlobalSettings.WBidStateCollection.Vacation.Count > 0)
                    {
                        GlobalSettings.IsVacationCorrection = (GlobalSettings.CurrentBidDetails.Round == "M" || (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA"));
                    }

                    GlobalSettings.IsFVVacation = (GlobalSettings.FVVacation != null && GlobalSettings.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

                    //In case if the vacation file is not downloaded , we dont need to ON VAC and DRP button. We need to show alert to the user in this case
                    if (GlobalSettings.IsVacationCorrection && isVacationFileDownloaded)
                    {
                        GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                        GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                        GlobalSettings.iSNeedToShowMonthtoMonthAlert = true;

                        wBIdStateContent.IsVacationOverlapOverlapCorrection = true;
                    }
                    if (GlobalSettings.IsFVVacation)
                    {
                        GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                    }
                    wBIdStateContent.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.IsFVVacation;
                    wBIdStateContent.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
                }
                WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
            }
            catch (Exception ex)
            {
                // WBidHelper.AddDetailsToMailCrashLogs(ex);
                throw ex;
            }
            //Set Menu bar button state
            //wBIdStateContent.MenuBarButtonState.is
        }


        private void SetDRPColoring()
        {
            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
            {
                RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
                objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
            }
        }

        private void ChecktheUserIsInEBGGroup()
        {
            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                //After the Seniority LIst is parsed, if the USER is NOT in the EBG, we should filter/ constrain for ETOPS.However, some users may want to look at the ETOPS bid data, so in the Alert we give them the chance to "Turn OFF"
                bool isEBG = (GlobalSettings.WBidStateCollection.SeniorityListItem != null && GlobalSettings.WBidStateCollection.SeniorityListItem.EBgType == "Y") ? true : false;
                if (isEBG == false)
                {
                    wBIdStateContent.Constraints.ETOP = true;
                    wBIdStateContent.Constraints.ReserveETOPS = true;
                }
            }

        }


        private void DismissCurrentView()
        {

            foreach (NSObject obj in arrObserver)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
            }

            //NSNotificationCenter.DefaultCenter.RemoveObserver(CommonClass.bidObserver);

            DismissViewController(true, null);

        }


        private void DownloadFlightData()
        {

            string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
            string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
            string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";

            try
            {
                var wcClient = new WebClient();
                //Downloading networkdat file
                wcClient.DownloadFile(serverPath, zipLocalFile);
            }
            catch (Exception ex)
            {
            }

            InvokeOnMainThread(() =>
            {
                if (File.Exists(networkDataPath))
                {
                    File.Delete(networkDataPath);
                }
                string target = WBidHelper.GetAppDataPath() + "/";
                //Path.Combine(WBidHelper.GetAppDataPath(), WBidHelper.GetAppDataPath() + "/");// + Path.GetFileNameWithoutExtension(zipLocalFile))+ "/";
                if (File.Exists(zipLocalFile))
                {
                    ZipFile.ExtractToDirectory(zipLocalFile, target);
                }

                if (File.Exists(zipLocalFile))
                {
                    File.Delete(zipLocalFile);
                }
            });



        }



        /// <summary>
        /// Read  WBidUpdate file and update the content to INI file
        /// </summary>
        private void ReadWbUpdateAndUpdateINIFile()
        {
            InvokeOnMainThread(() =>
            {
                if (!File.Exists(WBidHelper.WBidUpdateFilePath))
                    return;
            });

            WBidUpdate wBidUpdate = WBidHelper.ReadValuesfromWBUpdateFile(WBidHelper.WBidUpdateFilePath);

            if (wBidUpdate != null)
            {
                bool isUpdateFound = WBidCollection.UpdateINIFile(wBidUpdate);
                //Save the INI file
                if (isUpdateFound)
                {
                    WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                }
                //				if (GlobalSettings.WBidINIContent.Updates.News > previousnewsverion)
                //				{
                //					GlobalSettings.IsNewsShow = true;
                //				}
            }

        }



        /// <summary>
        /// Download Wbid Files
        /// </summary>
        /// <returns><c>true</c>, if W bid files was downloaded, <c>false</c> otherwise.</returns>
        private bool DownloadWBidFiles()
        {
            bool status = true;

            //we are not needed to download the "news.pdf" file, since we are keeping the bidvalet as simple.
            // Also we dont need to download the "falistwb4.dat" file  since we are not planning to implement  the buddy
            //bidding for bidvalet at this time.

            try
            {
                // 

                List<string> lstWBidFiles = new List<string>() { "WBUPDATE.DAT", "trips.ttp" };

                foreach (var bidFile in lstWBidFiles)
                {

                    var success = DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), bidFile);
                }


                //				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
                //				{
                //					DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), "falistwb4.dat");
                //				}

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }

        private void observeNotifications()
        {
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("reachabilityCheckSuccess"), reachabilityCheck));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("cwaCheckSuccess"), cwaCredentialsCheck));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("authCheckSuccess"), authCheckSuccess));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("getDataFilesSuccess"), getDataFilesSuccess));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("parseDataSuccess"), parseDataSuccess));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("saveDataSuccess"), saveDataSuccess));
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DismissDownloadAndRedirectoLineView"), DismissDownloadAndRedirectoLineView));
            //arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("calcVACCorrection"), calcVACCorrection));
        }

        private void DismissDownloadAndRedirectoLineView(NSNotification obj)
        {

            InvokeOnMainThread(() =>
            {
                if (ObjParentController != null)
                {
                    DismissCurrentView();
                    ((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);

                }
            });

        }

        public void setButtonState(UIButton theButton, bool status)
        {

            InvokeOnMainThread(() =>
            {
                theButton.Selected = status;
                    //                this.prgrsVw.Progress = 1.0f;
                    this.moveProgressToEnd();
            });

        }

        #region Progress Bar
        public void startProgress()
        {

            InvokeOnMainThread(() =>
        {
            this.prgrsVw.Progress = 0.0f;
            PerformSelector(new ObjCRuntime.Selector("incrementProgress"), null, 1);
        });
        }

        [Export("incrementProgress")]
        void incrementProgress()
        {
            if (this.prgrsVw.Progress >= 0.7f)
                return;
            InvokeOnMainThread(() =>
            {
                //this.prgrsVw.Progress += this.prgrsVw.Progress + 0.1f;
                prgrsVw.SetProgress(prgrsVw.Progress + 0.1f, true);
            });
            PerformSelector(new ObjCRuntime.Selector("incrementProgress"), null, 1);

        }
        [Export("moveProgressToEnd")]
        void moveProgressToEnd()
        {

            if (this.prgrsVw.Progress >= 1.0f)
                return;
            InvokeOnMainThread(() =>
            {
                    //this.prgrsVw.Progress += this.prgrsVw.Progress + 0.1f;
                    prgrsVw.SetProgress(prgrsVw.Progress + 0.1f, true);

            });
            PerformSelector(new ObjCRuntime.Selector("moveProgressToEnd"), null, 0.5);

        }
        #endregion


        /// <summary>
        /// Show alert view
        /// </summary>
        /// <param name="caption">Caption.</param>
        /// <param name="message">Message.</param>
        private void DisplayAlertView(string caption, String message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        }

        #endregion

        #region Observe Notifications



        public void reachabilityCheck(NSNotification n)
        {

            this.setButtonState(this.btnCheckInternetConnection, true);
            Console.WriteLine("reachabilityCheck");

        }
        public void cwaCredentialsCheck(NSNotification n)
        {

            this.setButtonState(this.btnCheckCWACredentials, true);
            Console.WriteLine("cwaCredentialsCheck");

        }
        public void authCheckSuccess(NSNotification n)
        {

            this.setButtonState(this.btnCheckAuthorization, true);
            Console.WriteLine("authCheckSuccess");
            this.startProgress();

        }
        public void getDataFilesSuccess(NSNotification n)
        {
            this.setButtonState(this.btnGetDataFiles, true);
            Console.WriteLine("getDataFilesSuccess");

            this.startProgress();

        }
        public void parseDataSuccess(NSNotification n)
        {

            this.setButtonState(this.btnParseData, true);
            Console.WriteLine("parseDataSuccess");

        }

        public void saveDataSuccess(NSNotification n)
        {

            //this.setButtonState(this.btnParseData, true);
            Console.WriteLine("saveDataSuccess");
            this.startProgress();
            
        }


        #endregion
    }
}

