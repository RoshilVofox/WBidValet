
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
//using WBid.WBidClient.Models;



#endregion


namespace Bidvalet.iOS
{
    public partial class DownloadBidDataViewController1 : UIViewController
    {

        #region Public Variables
		private DownloadInfo _downloadFileDetails;
        public UIViewController ObjParentController;
        public bool IsMissingTripFailed { get; set; }
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
        private DownloadInfo _objDownloadInfoDetails;
        private Dictionary<string, Trip> _objTrips = null;
        private Dictionary<string, Line> _objLines = null;
        private List<SeniorityListMember> _objSeniorityListMembers;
        private SeniorityListItem _objSeniorityListItem;
        List<NSObject> arrObserver = new List<NSObject>();
        /// <summary>
        /// create single instance of TripTtpParser class
        /// </summary>
        private TripTtpParser _objTripTtpParser;
        private CalculateTripProperties _objCalculateTripProperties;
        private CalculateLineProperties _objCalculateLineProperties;



        #endregion


        public DownloadBidDataViewController1(IntPtr handle)
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
			
				Console.WriteLine("Download View Init");
				base.ViewDidLoad();
				//this.prgrsVw.SetProgress(0.0f, false);
				this.observeNotifications();

				NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);

				NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);

				NSNotificationCenter.DefaultCenter.PostNotificationName("authCheckSuccess", null);


				/////--------------------------------------------
				if (GlobalSettings.DownloadBidDetails != null)
				{
					GlobalSettings.DownloadBidDetails.Month = GlobalSettings.IsCurrentMonthOn ? DateTime.Now.Month : DateTime.Now.AddMonths(1).Month;


					GlobalSettings.DownloadBidDetails.Year = (DateTime.Now.Month == 12 && GlobalSettings.DownloadBidDetails.Month == 1) ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year;
				}
				/////--------------------------------------------

				//Checking data available for  Practise and Submit feature
				bool isDataAvailable = CheckDataAvailable();
             isDataAvailable = true;

                GlobalSettings.IsHistorical = !isDataAvailable;


				if (GlobalSettings.IsHistorical)
				{

					string message = "The Bid Period is not open for the ";
					if (GlobalSettings.DownloadBidDetails != null && GlobalSettings.DownloadBidDetails.Round == "D")
						message += "1st ";
					else
						message += "2nd ";
					message += "Round. \nBut you can still practice.";

					message += "\n\nWe will download the Historical Bid data from the previous month.";

					InvokeOnMainThread(() =>
					{

						var alertHistorical = new UIAlertView(GlobalSettings.ApplicationName, message, null, "NO", new string[] { "OK" });
						alertHistorical.Clicked += (object senderObject, UIButtonEventArgs ev) =>
						{
							int index = (int)ev.ButtonIndex;
							if (index == 1)
							{
								InvokeInBackground(() =>
								{
									DownloadProcessSetup();
								});
							}
							else
							{
								DismissCurrentView();

							}


						};
						alertHistorical.Show();
					});



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

            return status;

        }



        //private bool CheckDataAvailable()
        //{
        //    bool status = true;
        //    //Getting the Current CST time
        //    //----------------------------------------------
        //    DateTime timeUtc = DateTime.UtcNow;
        //    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
        //    DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
        //    //-------------------------------------------------------------------------

        //    //Generating default bid available time of each position
        //    //-----------------------------------------------------------------------
        //    DateTime bidAvailableTime = DateTime.MinValue;

        //    if (GlobalSettings.DownloadBidDetails.Round == "D")//First Round
        //    {
        //        if (GlobalSettings.DownloadBidDetails.Postion == "FA")
        //        {   //Flt Att  round1 opens on 2nd at 12:00
        //            bidAvailableTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2, 12, 0, 0);
        //        }
        //        else
        //        {   //Pilot round 1 opens on 4th at 12:00
        //            bidAvailableTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4, 12, 0, 0);
        //        }
        //    }
        //    else//Second Round
        //    {
        //        if (GlobalSettings.DownloadBidDetails.Postion == "FA")
        //        {   //Flt Att round 2 opens on the 12th at 12:00
        //            bidAvailableTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 12, 12, 0, 0);
        //        }
        //        else
        //        {   //Pilot round 2 opens on the 17th at 12:00
        //            bidAvailableTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 17, 12, 0, 0);
        //        }
        //    }

        //    //if current cst time  greater than bidAvailable time then Data availble  vale will be TRUE
        //    status = (bidAvailableTime <= cstTime);
        //    return status;
        //}


        #endregion

        #region Methods

        /// <summary>
        /// Downloads  process .
        /// </summary>
        private void DownloadProcessSetup()
        {

            try
            {

                startProgress();
                GlobalSettings.ExtraErrorInfo = new StringBuilder();
                _objDownloadInfoDetails = new DownloadInfo
                {
                    UserId = KeychainHelpers.GetPasswordForUsername("user", "WBid.BidValet.cwa", false),
                    Password = KeychainHelpers.GetPasswordForUsername("pass", "WBid.BidValet.cwa", false),
                    SessionCredentials = GlobalSettings.SessionCredentials,
                    DownloadList = WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails)
                };

                GlobalSettings.ExtraErrorInfo.Append("Generated Download Files list");
                List<DownloadedFileInfo> lstDownloadedFiles;


                if (GlobalSettings.IsHistorical)
                {
                    GlobalSettings.DownloadBidDetails.Month = DateTime.Now.Month;
                    GlobalSettings.DownloadBidDetails.Year = (DateTime.Now.Month == 12 && GlobalSettings.DownloadBidDetails.Month == 1) ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year;
                    _objDownloadInfoDetails.DownloadList = WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails);
                    if (GlobalSettings.InternetType == (int)InternetType.Ground || GlobalSettings.InternetType == (int)InternetType.AirPaid)
                    {
                        lstDownloadedFiles = HistoricalDataDownload();
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            DismissCurrentView();
                            DisplayAlertView(GlobalSettings.ApplicationName, GlobalMessages.LimittedInternetInfoHistorical);
                        });
                        return;
                    }

                    if (lstDownloadedFiles.Count == 0)
                    {

                        InvokeOnMainThread(() =>
                        {
                            DismissCurrentView();
                            DisplayAlertView(GlobalSettings.ApplicationName, "Practice Data is Not Available");
                        });

                        return;
                    }

                }
                else
                {
                    lstDownloadedFiles = DownloadFiles(_objDownloadInfoDetails);
                }

                if (lstDownloadedFiles == null)
                {

                    InvokeOnMainThread(() =>
                    {
                        DismissCurrentView();
                        DisplayAlertView(GlobalSettings.ApplicationName, "Data Transfer Failed");
                    });

                    return;
                }
                if (lstDownloadedFiles.Any(x => x.IsException))
                {
                    InvokeOnMainThread(() =>
                    {
                        DismissCurrentView();
                        DisplayAlertView(GlobalSettings.ApplicationName, "The internet connection has been lost. Try again later. ");
                    });

                    return;
                }


                DeleteOldBidFiles();


                //Save the downloaded files
              bool isError=  _objDownloadBidObject.SaveDownloadedBidFiles(lstDownloadedFiles, WBidHelper.GetAppDataPath());
                if (isError)
                {
                    InvokeOnMainThread(() =>
                    {
                        DismissCurrentView();
                        DisplayAlertView(GlobalSettings.ApplicationName, "Data Tranfer Failed. Please try again.");
                    });
                    return;
                }

                GlobalSettings.ExtraErrorInfo.Append("Saved downloaded file completed");


                string zipFileName = _objDownloadInfoDetails.DownloadList.FirstOrDefault(x => x.Contains(".737"));
                DownloadedFileInfo zipFile = lstDownloadedFiles.FirstOrDefault(x => x.FileName == zipFileName);

                if (zipFile != null && zipFile.IsError)
                {
                    if (zipFile.Message.Contains("BIDINFO DATA NOT AVAILABLE"))
                    {
                    }

                    InvokeOnMainThread(() =>
                    {
                        DismissCurrentView();
                        DisplayAlertView(GlobalSettings.ApplicationName, "Data Transfer Failed");

                    });
                }
                else
                {

                    string dataDesc = GlobalSettings.DownloadBidDetails.Domicile + " " + GlobalSettings.DownloadBidDetails.Postion + " " + GlobalSettings.DownloadBidDetails.Month + " " + ((GlobalSettings.DownloadBidDetails.Round == "D") ? "Monthly" : "2nd Round");

                    if (GlobalSettings.IsHistorical)
                        dataDesc += "(Historical)";
                    //try
                    //{
                    //    Insights.Identify(GlobalSettings.UserInfo.EmpNo, new Dictionary<string, string> { 
                    //            {Insights.Traits.Email, GlobalSettings.UserInfo.Email},
                    //            {Insights.Traits.Name, GlobalSettings.UserInfo.FirstName+" "+GlobalSettings.UserInfo.LastName},
                    //            {Insights.Traits.Description, "Download "+dataDesc}
               
                    //                 });
                    //}
                    //catch (Exception)
                    //{


                    //}

                    string path = WBidHelper.GetAppDataPath() + "/" + Path.GetFileNameWithoutExtension(zipFileName);

                    if (!(File.Exists(path + "/" + "TRIPS") && File.Exists(path + "/" + "PS")))
                    {

                        InvokeOnMainThread(() =>
                            {
                                DismissCurrentView();
                                DisplayAlertView(GlobalSettings.ApplicationName, "There is an error while downloading the data. Please check your internet connection and try again.");

                            });
                        return;
                    }
                    WBidHelper.SetCurrentBidInformationfromZipFileName(zipFileName, false);
                    GlobalSettings.ExtraErrorInfo.Append("Set current information after saved data\n<br>");

                    //Write to currentBidDetailsfile for Error log
                    if (GlobalSettings.CurrentBidDetails != null)
                    {
                        FileOperations.WriteCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData());
                    }

                    //

                    //  GlobalSettings.InternetType = (int)InternetType.Air; 
                    if (GlobalSettings.InternetType == (int)InternetType.Ground || GlobalSettings.InternetType == (int)InternetType.AirPaid)
                    {
                        DownloadWBidFiles();



                        GlobalSettings.ExtraErrorInfo.Append("ReadWbUpdateAndUpdateINIFile Started\n<br>");
                        ReadWbUpdateAndUpdateINIFile();
                        GlobalSettings.ExtraErrorInfo.Append("ReadWbUpdateAndUpdateINIFile Finished\n<br>");
                        GlobalSettings.ExtraErrorInfo.Append("Download Flight data Started\n<br>");
                        if (!GlobalSettings.IsHistorical)
                        {
                            DownloadFlightData();
                            GlobalSettings.ExtraErrorInfo.Append("Download Flight data finished\n<br>");
                        }

                        NSNotificationCenter.DefaultCenter.PostNotificationName("getDataFilesSuccess", null);
                        GlobalSettings.ExtraErrorInfo.Append("Parse File Started\n<br>");
                        bool isSuccess = ParseData(zipFileName);

                        if (!isSuccess)
                            return;
                      
                        GlobalSettings.ExtraErrorInfo.Append("Parse File Finished\n<br>");


                        InvokeOnMainThread(() =>
                        {
                            if (ObjParentController != null)
                            {

                                    ////Icloud prevention
                                    //var exURL = NSUrl.FromFilename(WBidHelper.GetAppDataPath());
                                    //bool flag=  exURL.SetResource(new NSString("NSURLIsExcludedFromBackupKey"), NSNumber.FromBoolean(true));

                                    //if(flag) Console.WriteLine("success");
                                    //else	Console.WriteLine("failer");

                               // ((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);
                            }
                        });

                    }
                    //Limitted internet
                    else
                    {
                        InvokeOnMainThread(() =>
                       {
                           var alertLimittedVW = new UIAlertView(GlobalSettings.ApplicationName, GlobalMessages.LimittedInternetInfo, null, "OK", null);

                           alertLimittedVW.Clicked += alertLimittedVW_Clicked;
                           alertLimittedVW.Show();

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
        }

        void alertLimittedVW_Clicked(object sender, UIButtonEventArgs e)
        {
            InvokeInBackground(() =>
            {


                string zipFileName = _objDownloadInfoDetails.DownloadList.FirstOrDefault(x => x.Contains(".737"));
                NSNotificationCenter.DefaultCenter.PostNotificationName("getDataFilesSuccess", null);
                GlobalSettings.ExtraErrorInfo.Append("Parse File Started\n<br>");
               bool isSuccess= ParseData(zipFileName);

               if (!isSuccess)
                   return;
                GlobalSettings.ExtraErrorInfo.Append("Parse File Finished\n<br>");
                int empNum = int.Parse(_objDownloadInfoDetails.UserId.ToLower().Replace("x", "").Replace("e", ""));

                WBidHelper.SaveInternalLog("getData", empNum, WBidHelper.GetWBidInternalLogFilePath());


                InvokeOnMainThread(() =>
                {
                    if (ObjParentController != null)
                    {

                        //((LoginViewController)ObjParentController).DismissDownloadAndNavigateToConstraint(this);

                    }
                });
            });

        }

		private void DeleteOldBidFiles()
		{


			InvokeOnMainThread(() =>
				{
					try
					{
						string path = WBidHelper.GetAppDataPath();
						List<string> fileNames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
							.Where(s => s.ToLower() != "currentdetails.txt" && s.ToLower() != "wbiddwc.xml" && s.ToLower() != "wbidini.xml" && s.ToLower() != "user.xml" && s.ToLower() != "bvquickfilter.qf" && s.ToLower() != "event.log" && s.ToLower() != "payment.log").ToList();


						foreach (var file in fileNames)
						{

							File.Delete(path + "/" + file);
						}


						List<string> directories = Directory.EnumerateDirectories(path).ToList();

						foreach (var directory in directories)
						{

							Directory.Delete(directory, true);
						}

						//delete bid award file
						DirectoryInfo bidawrddirectory = new DirectoryInfo(WBidHelper.GetBidAwardPath());
						var monthfolder = bidawrddirectory.GetDirectories().Select(x => x.Name);
						foreach (var item in monthfolder)
						{
							var bidmonth = WBidCollection.GetMonthName(GlobalSettings.DownloadBidDetails.Month);
							if (bidmonth != item)
							{
							Directory.Delete(WBidHelper.GetBidAwardPath() + "/" + bidmonth,true);
							}
						}


						//FileInfo[] files = directory.GetFiles("*.TXT", SearchOption.AllDirectories);

						//foreach (FileInfo file in files)
						//{

					}
					catch (Exception ex)
					{



					}
					//DisplayAlertView("Bid",ss);
				});


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

        private bool ParseData(string zipFilename)
        {

            GlobalSettings.ExtraErrorInfo.Append("Inside Parse data\n<br>");

            try
            {
                //Prase Trip files
                _objTrips = ParseTripFile(zipFilename);
                GlobalSettings.ExtraErrorInfo.Append("Parse  trip file completed\n<br>");
                if (_objTrips != null)
                {

                    if (zipFilename.Substring(0, 1) == "A" && zipFilename.Substring(1, 1) == "B")
                    {
                        var fASecondRound = new FASecondRoundParser();
                        _objLines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + "/" + zipFilename.Substring(0, 6) + "/PS", ref _objTrips, GlobalSettings.FAReserveDayPay, zipFilename.Substring(2, 3));
                    }
                    else
                    {
                        _objLines = ParseLineFiles(zipFilename);
                    }
                    if (!GlobalSettings.IsHistorical)
                    {
                        try
                        {


                            if (GlobalSettings.CurrentBidDetails.Round == "M")
                            {

                                var coverletterparser = new CoverLetterParser();
                                string coverlettername = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "C" + ".TXT";
                                var coverletterdata = new CoverLetterData();
                                if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                {
                                    coverletterdata = coverletterparser.ParseCoverLetteForPilots(WBidHelper.GetAppDataPath() + "/" + coverlettername, GlobalSettings.CurrentBidDetails.Domicile, GlobalSettings.CurrentBidDetails.Postion);
                                }
                                else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                                {
                                    coverletterdata = coverletterparser.ParseCoverLetteForFlightAttendants(WBidHelper.GetAppDataPath() + "/" + coverlettername, GlobalSettings.CurrentBidDetails.Domicile);
                                }
                                if (GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Year == coverletterdata.Year && GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Month == coverletterdata.Month)
                                {
                                    if (coverletterdata.TotalLine != _objLines.Count)
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            var message = "The cover letter says there should be " + coverletterdata.TotalLine + "  lines, but the bid data contains " + _objLines.Count + "  lines.  Please send us an email to support@wbidmax.com";
                                            //var message ="It appears the bid data is incorrect.  The cover letter says there will be " + coverletterdata.TotalLine + " lines, but the bid data only contains " + lines.Count + " lines.";
                                            //var alert = new UIAlertView ("WBidMax", message, null, "OK", null);
                                            //alert.Show ();

                                            DisplayAlertView(GlobalSettings.ApplicationName, message);
                                            //										UIAlertView alertVW1 = new UIAlertView (message, "Error", null, "OK", null);
                                            //										alertVW1.Clicked += (object obj, UIButtonEventArgs f) => {
                                            //											//DismissCurrentView();
                                            //										};
                                            //										alertVW1.Show ();
                                        });


                                        //Xceed.Wpf.Toolkit.MessageBox.Show(downloadWindow, "It appears the bid data is incorrect.  The cover letter says there will be " + coverletterdata.TotalLine + " lines, but the bid data only contains " + lines.Count + " lines.", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                                    }
                                }


                            }
                            else
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                                {
                                    var parser = new SeniorityListParser();
                                    string senioriyListname = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "SR" + ".TXT";
                                    int totalFACount = parser.ParseSeniorityListForSecondRoundFAForTotalFACount(WBidHelper.GetAppDataPath() + "/" + senioriyListname);
                                    if (totalFACount != _objLines.Count)
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            var message = "The seniority list has " + totalFACount + " Flight Attendants, but the bid data contains " + _objLines.Count + " lines.  Please send up an email to support@wbidmax.com.";

                                            UIAlertView alertVW1 = new UIAlertView(message, "Error", null, "OK", null);
                                            alertVW1.Clicked += (object obj, UIButtonEventArgs f) =>
                                            {
                                                //DismissCurrentView();
                                            };
                                            alertVW1.Show();
                                        });

                                        //Xceed.Wpf.Toolkit.MessageBox.Show(downloadWindow, "The seniority list has " + totalFACount + " Flight Attendants, but the bid data contains " + lines.Count + " lines.  Please send up an email to support@wbidmax.com.", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw;

                        }
                    }
                    GlobalSettings.ExtraErrorInfo.Append("Parse  Scrap missing trip started\n<br>");
                    _objTrips = ScrapMissingTrip(_objLines, _objTrips);
                    GlobalSettings.ExtraErrorInfo.Append("Parse  Scrap missing trip completed\n<br>");

                    // Parse trip.ttp file.
                    GlobalSettings.ExtraErrorInfo.Append("Parse  City file started\n<br>");
                    List<CityPair> listCityPair = ObjTripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
                    GlobalSettings.TtpCityPairs = listCityPair;
                    GlobalSettings.ExtraErrorInfo.Append("Parse  City file completed\n<br>");

                    // Additional processing needs to be done to FA trips before CalculateTripPropertyValues
                    if (zipFilename.Substring(0, 1) == "A")
                        ObjCalculateTripProperties.PreProcessFaTrips(_objTrips, listCityPair);

                    GlobalSettings.ExtraErrorInfo.Append("Parse-CalculateTripPropertyValues started\n<br>");
                        ObjCalculateTripProperties.CalculateTripPropertyValues(_objTrips, listCityPair);
                    GlobalSettings.ExtraErrorInfo.Append("Parse CalculateTripPropertyValues finished\n<br>");
                    //WBidHelper.SetCurrentBidInformationfromZipFileName(zipFilename);
                    GlobalSettings.ExtraErrorInfo.Append("Parse  CalculateLinePropertyValues started\n<br>");
                    try
                    {
                        ObjCalculateLineProperties.CalculateLinePropertyValues(_objTrips, _objLines, GlobalSettings.CurrentBidDetails);
                    }
                    catch (Exception)
                    {
                        InvokeOnMainThread(() =>
                        {
                            DismissCurrentView();
                            DisplayAlertView(GlobalSettings.ApplicationName, "The Trip file is missing information.  Admin will contact the company.  You will have to re-download the bid data later");
                        });
                   
                        return false;
                    }
                  
                    GlobalSettings.ExtraErrorInfo.Append("Parse  City CalculateLinePropertyValues Finished\n<br>");


                    //Send Notif
                    NSNotificationCenter.DefaultCenter.PostNotificationName("parseDataSuccess", null);

                    if (!GlobalSettings.IsHistorical)
                    {
                        GlobalSettings.ExtraErrorInfo.Append("Parse ParseSeniorityList started\n<br>");
                        // GlobalSettings.IsNeedToDownloadSeniority = true;
                        ParseSeniorityList();
                        GlobalSettings.ExtraErrorInfo.Append("Parse ParseSeniorityList Finished\n<br>");
                        GlobalSettings.ExtraErrorInfo.Append("Parse CheckAndPerformVacationCorrection Started\n<br>");
                        CheckAndPerformVacationCorrection();
                        GlobalSettings.ExtraErrorInfo.Append("Parse CheckAndPerformVacationCorrection Finished\n<br>");
                    }

                    GlobalSettings.ExtraErrorInfo.Append("Parse SaveParsedFiles Started\n<br>");
                    SaveParsedFiles(_objTrips, _objLines);
                    GlobalSettings.ExtraErrorInfo.Append("Parse SaveParsedFiles Finished\n<br>");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private Dictionary<string, Trip> ParseTripFile(string fileName)
        {
            GlobalSettings.ExtraErrorInfo.Append("Inside parse trip file\n");
            Dictionary<string, Trip> trips;
            try
            {
                var tripParser = new TripParser();
                string filePath = WBidHelper.GetAppDataPath() + "/" + fileName.Substring(0, 6) + "/TRIPS";
                byte[] byteArray = File.ReadAllBytes(filePath);

                DateTime[] dSTProperties = DSTProperties.SetDSTProperties();
                if (dSTProperties[0] != null && dSTProperties[0] != DateTime.MinValue)
                {
                    GlobalSettings.FirstDayOfDST = dSTProperties[0];
                }
                if (dSTProperties[1] != null && dSTProperties[1] != DateTime.MinValue)
                {
                    GlobalSettings.LastDayOfDST = dSTProperties[1];
                }
                //WBidHelper.SetDSTProperties();
                trips = tripParser.ParseTrips(fileName, byteArray, GlobalSettings.FirstDayOfDST, GlobalSettings.LastDayOfDST);
            }
            catch (Exception ex)
            {

                throw;
            }
            return trips;
        }

        private Dictionary<string, Line> ParseLineFiles(string fileName)
        {
            var lineParser = new LineParser();
            string filePath = WBidHelper.GetAppDataPath() + "/" + fileName.Substring(0, 6) + "/PS";
            byte[] byteArray = File.ReadAllBytes(filePath);
            Dictionary<string, Line> lines = lineParser.ParseLines(fileName, byteArray);
            return lines;
        }

        private Dictionary<string, Trip> ScrapMissingTrip(Dictionary<string, Line> lines, Dictionary<string, Trip> trips)
        {
            //bid round is second round
           // if (GlobalSettings.CurrentBidDetails.Round == "S")
           // {
                //Finding if any missed trip exists
                List<string> allPair = lines.SelectMany(x => x.Value.Pairings).Distinct().ToList();
                var pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x && x.Substring(1, 1) == "P"))).ToList();

                //Checking any missed trip  exist
                if (pairingwHasNoDetails.Count > 0)
                {


                    try
                    {
                        List<string> temppairingwHasNoDetails = new List<string>();
                        bool isscrapRequired = true;
                        MonthlyBidDetails biddetails = new MonthlyBidDetails();
                        biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                        biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                        biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                        biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                        biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                        var missedtrips = WBidHelper.GetMissingtripFromVPS(biddetails);
                        if (missedtrips.Count >= pairingwHasNoDetails.Count)
                        {
                            //trips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                            var temptrips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                            temppairingwHasNoDetails = allPair.Where(x => !temptrips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x))).ToList();
                            if (temppairingwHasNoDetails.Count == 0)
                            {
                                trips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                                isscrapRequired = false;
                            }
                        }
                        if (isscrapRequired)
                        {

                            if ((GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(-1).Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(1).Month))
                            {

                                GlobalSettings.parsedDict = new Dictionary<string, Trip>();

                                string empNumber = _objDownloadInfoDetails.UserId;
                                if (!empNumber.Contains("e") && !empNumber.Contains("x"))
                                {
                                    empNumber = "e" + empNumber;
                                }
                                string password = (GlobalSettings.QATest) ? GlobalSettings.QAScrapPassword : _objDownloadInfoDetails.Password;
                                scrap(empNumber, password, pairingwHasNoDetails, GlobalSettings.DownloadBidDetails.Month, GlobalSettings.DownloadBidDetails.Year, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay);

                                if (GlobalSettings.parsedDict == null || GlobalSettings.parsedDict.Count == 0)
                                {
                                    InvokeOnMainThread(() =>
                                    {

                                        DisplayAlertView(GlobalSettings.ApplicationName, "Missing Data\", \"Unable to get missing data.  Only partial data will be displayed for split pairings.");


                                        IsMissingTripFailed = true;
                                        string bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                        var bidLineParser = new BidLineParser();
                                        var firstOrDefault = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile);
                                        if (firstOrDefault != null)
                                        {
                                            var domcilecode = firstOrDefault.Code;

                                            trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                                        }
                                    });

                                }
                                else
                                {
                                    //IsMissingTripFailed = true;
                                    trips = trips.Concat(GlobalSettings.parsedDict).ToDictionary(pair => pair.Key, pair => pair.Value);
                                }

                            }
                            else
                            {
                                IsMissingTripFailed = true;
                                string bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                var bidLineParser = new BidLineParser();
                                var firstOrDefault = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile);
                                if (firstOrDefault != null)
                                {
                                    var domcilecode = firstOrDefault.Code;

                                    trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        InvokeOnMainThread(() =>
                        {
                            // Parse Missing trip details from  Bidline File
                            GlobalSettings.IsScrapStart = false;
                            DisplayAlertView(GlobalSettings.ApplicationName, "Unable to get missing data.  Only partial data will be displayed for split pairings.");

                            IsMissingTripFailed = true;
                            string bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                            if (GlobalSettings.IsHistorical)
                            {
                                HistoricalBidLineDataDownload();
                            }
                            var bidLineParser = new BidLineParser();
                            var firstOrDefault = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile);
                            if (firstOrDefault != null)
                            {
                                var domcilecode = firstOrDefault.Code;

                                trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                            }
                        });

                    }


                    //}
                    //// Parse Missing trip details from  Bidline File
                    //else
                    //{

                    //    string bidFileName = string.Empty;
                    //    bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                    //    BidLineParser bidLineParser = new BidLineParser();
                    //    var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;
                    //    trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "\\" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);

                    //}
                }
            //}
            return trips;


        }

        private void SaveParsedFiles(Dictionary<string, Trip> trips, Dictionary<string, Line> lines)
        {
            string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


            var tripInfo = new TripInfo
            {
                TripVersion = GlobalSettings.TripVersion,
                Trips = trips

            };

            var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP");
            ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP", tripInfo, stream);
            stream.Dispose();
            stream.Close();

            GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));



            if (GlobalSettings.IsVacationCorrection && GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0)
            {//set  vacation details  to line object. 

                var calVacationdetails = new CaculateVacationDetails();
                calVacationdetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);

                //set the Vacpay,Vdrop,Vofont and VoBack columns in the line summary view 
                //ManageVacationColumns managevacationcolumns = new ManageVacationColumns();
                //managevacationcolumns.SetVacationColumns();
            }

            var lineInfo = new LineInfo
            {
                LineVersion = GlobalSettings.LineVersion,
                Lines = lines

            };




            GlobalSettings.Lines = new ObservableCollection<Line>(lines.Select(x => x.Value));


            //  int[] arr = GlobalSettings.Lines[0].DaysOfWeekWork;
            //save the line file to app data folder
            // Task linetask = Task.Run(() =>
            // {

            try
            {
                var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
                ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
                linestream.Dispose();
                linestream.Close();
            }
            catch (Exception ex)
            {

                throw;
            }


            foreach (Line line in GlobalSettings.Lines)
            {
                line.ConstraintPoints = new ConstraintPoints();
                line.WeightPoints = new WeightPoints();
            }

            //			if (GlobalSettings.IsOverlapCorrection && GlobalSettings.CurrentBidDetails.Round == "M")
            //			{
            //				OverlapData overlapData = new OverlapData();
            //				overlapData.LastLegArrivalTime = GlobalSettings.LastLegArrivalTime.ToString();
            //				//List<OverlapDay> overlapdays = new List<OverlapDay>();
            //				//foreach (Day day in GlobalSettings.LeadOutDays)
            //				//{
            //				//    overlapdays.Add(new OverlapDay
            //				//    {
            //				//        ArrivalCity = day.ArrivalCity,
            //				//        ArrivalTime = day.ArrivalTime,
            //				//        Date = day.Date,
            //				//        DepartutreCity = day.DepartutreCity,
            //				//        DepartutreTime = day.DepartutreTime,
            //				//        FlightTime = day.FlightTime,
            //				//        FlightTimeHour = day.FlightTimeHour,
            //				//        OffDuty = day.OffDuty
            //				//    });
            //
            //				//}
            //				overlapData.LeadOutDays = GlobalSettings.LeadOutDays;
            //				var overlapfile = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL");
            //				ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL", overlapData, overlapfile);
            //				overlapfile.Dispose();
            //				overlapfile.Close();
            //				//WBidHelper.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL", overlapData);
            //			}
            FileOperations.WriteCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData());
            //Read the intial state file value from DWC file and create state file
            if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS"))
            {
                try
                {

                    WBidIntialState wbidintialState;
                    try
                    {
                        wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                        }
                        catch (Exception)
                        {

                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                            XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                            WBidHelper.LogDetails(GlobalSettings.UserInfo.EmpNo, "dwcRecreate", "0", "0");

                        }
                        XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                        WBidHelper.LogDetails(GlobalSettings.UserInfo.EmpNo, "dwcRecreate", "0", "0");

                    }
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);



                    //if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
                    //{
                    //    if (GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0)
                    //    {
                    //        GlobalSettings.UserInfo.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount;
                    //    }
                    //    else
                    //    {
                    //        GlobalSettings.UserInfo.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber;
                    //    }
                    //}

                    //					if (GlobalSettings.isHistorical)
                    //					{
                    //						GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    //					}
                    //					else
                    //GlobalSettings.WBidStateCollection.DataSource = (_isCompanyServerData) ? "Original" : "Mock";
                    //WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            else
            {
                try
                {
                    //Read the state file object and store it to global settings.
                    GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                }
                catch (Exception)
                {

                    //Recreate state file
                    //--------------------------------------------------------------------------------
                    var wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
                    WBidHelper.LogDetails(GlobalSettings.UserInfo.EmpNo, "wbsRecreate", "0", "0");
                    //					if (GlobalSettings.isHistorical)
                    //					{
                    //						GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    //					}
                    //					else
                    //						GlobalSettings.WBidStateCollection.DataSource = (_isCompanyServerData) ? "Original" : "Mock";
                }

                //XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                //				if (GlobalSettings.isHistorical)
                //				{
                //					GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                //				}
                //				else if (GlobalSettings.WBidStateCollection.DataSource == "Original" && _isCompanyServerData == false)
                //				{
                //					GlobalSettings.WBidStateCollection.DataSource = "Mock";
                //
                //					//WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                //				}
                //				else if (GlobalSettings.WBidStateCollection.DataSource == "Mock" && _isCompanyServerData == true)
                //				{
                //					GlobalSettings.WBidStateCollection.DataSource = "Original";
                //					// WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                //				}



            }
            GlobalSettings.WBidStateCollection.SeniorityListItem = _objSeniorityListItem;



            if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
            {
                GlobalSettings.UserInfo.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0 ? 0 : GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber;
            }

            //save the vacation to state file
            GlobalSettings.WBidStateCollection.Vacation = new List<Vacation>();
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBIdStateContent != null)
            {
                wBIdStateContent.MenuBarButtonState.IsMIL = false;

                if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null && GlobalSettings.IsVacationCorrection)
                {
                    var vacation = GlobalSettings.SeniorityListMember.Absences.Where(x => x.AbsenceType == "VA").Select(y => new Vacation { StartDate = y.StartAbsenceDate.ToShortDateString(), EndDate = y.EndAbsenceDate.ToShortDateString() });

                    GlobalSettings.WBidStateCollection.Vacation.AddRange(vacation.ToList());

                    wBIdStateContent.IsVacationOverlapOverlapCorrection = true;
                }
                else
                    wBIdStateContent.IsVacationOverlapOverlapCorrection = false;
                wBIdStateContent.IsOverlapCorrection = GlobalSettings.IsOverlapCorrection;

                GlobalSettings.WBidStateCollection.FVVacation = new List<Absense>();
                GlobalSettings.WBidStateCollection.FVVacation = GlobalSettings.FVVacation;

                if (GlobalSettings.MenuBarButtonStatus == null)
                {
                    GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();
                }

                wBIdStateContent.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.IsFVVacation;;
                wBIdStateContent.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;

                GlobalSettings.WBidStateCollection.CompanyVA = GlobalSettings.CompanyVA;

                GlobalSettings.WBidStateCollection.DataSource = GlobalSettings.IsHistorical ? "HistoricalData" : "Original";
                //GlobalSettings.WBidStateCollection.IsModified = true;
                WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

                //if (GlobalSettings.CurrentBidDetails.Round == "M" || (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA"))
                //{
                //    string sFileName = string.Empty;
                //    if (GlobalSettings.CurrentBidDetails.Round == "M")
                //    {
                //        //First round Pilot and FA  SeniorityList
                //        sFileName = WBidHelper.GetAppDataPath() + "\\" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S" + ".SL";
                //    }
                //    else
                //    {   //Second round Pilot SeniorityList
                //        sFileName = WBidHelper.GetAppDataPath() + "\\" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R" + ".SL";
                //    }

                //    //List<SeniorityListMember> seniorityListMembers = null;
                //    //seniorityListMembers = (List<SeniorityListMember>)WBidHelper.DeSerializeObject(sFileName);
                //    //if (seniorityListMembers != null)
                //    //{

                //    //    if (GlobalSettings.IsDifferentUser)
                //    //    {
                //    //        GlobalSettings.SeniorityListMember = seniorityListMembers.FirstOrDefault(x => x.EmpNum == (GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0')));
                //    //    }
                //    //    else
                //    //    {
                //    //        //GlobalSettings.SeniorityListMember = seniorityListMembers.FirstOrDefault(x => x.EmpNum == downloadInfo.UserId.Substring(1, downloadInfo.UserId.Length - 1).PadLeft(6, '0'));
                //    //    }
                //    //}
                //}


                WBidHelper.GenerateDynamicOverNightCitiesList();
                //  GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
                GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList(); var linePairing = GlobalSettings.Lines.SelectMany(y => y.Pairings);

                // var sabu = GlobalSettings.Trip.Where(x => linePairing.Contains(x.TripNum)).SelectMany(z => z.DutyPeriods.SelectMany(r => r.Flights.Select(t => new { arrival = t.ArrSta, depart = t.DepSta })));
                // int a = 0;
                //GlobalSettings.Trip.Where(x => linePairing.Contains(x.TripNum)).SelectMany(a => a.DutyPeriods).SelectMany(b => b.Flights).SelectMany(c => c.ArrSta && c.DepSta);

                //temporary code..
                // var WBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                if (wBIdStateContent.CxWtState.AMPMMIX == null)
                    wBIdStateContent.CxWtState.AMPMMIX = new AMPMConstriants();
                if (wBIdStateContent.CxWtState.FaPosition == null)
                    wBIdStateContent.CxWtState.FaPosition = new PostionConstraint();
                if (wBIdStateContent.CxWtState.TripLength == null)
                    wBIdStateContent.CxWtState.TripLength = new TripLengthConstraints();
                if (wBIdStateContent.CxWtState.DaysOfWeek == null)
                    wBIdStateContent.CxWtState.DaysOfWeek = new DaysOfWeekConstraints();
                if (wBIdStateContent.Constraints.DaysOfMonth == null)
                    wBIdStateContent.Constraints.DaysOfMonth = new DaysOfMonthCx();


                if (wBIdStateContent.Weights.NormalizeDaysOff == null)
                {
                    wBIdStateContent.Weights.NormalizeDaysOff = new Wt2Parameter { Type = 1, Weight = 0 };

                }
                if (wBIdStateContent.CxWtState.NormalizeDays == null)
                {
                    wBIdStateContent.CxWtState.NormalizeDays = new StateStatus() { Cx = false, Wt = false };

                }
            }

            var statemanagement = new StateManagement();
            //statemanagement.ReloadDataFromStateFile();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
            //Setting  status to Global variables
            statemanagement.SetVacationOrOverlapExists(wBidStateContent);
            //St the line order based on the state file.
            statemanagement.ReloadStateContent(wBidStateContent);

            //SET FV Vacation 
            PerformFVVacation();

            statemanagement.RecalculateLineProperties(wBidStateContent);

            var sort = new SortCalculation();


            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }

            if (IsMissingTripFailed)
            {
                IsMissingTripFailed = false;
                InvokeOnMainThread(() =>
                    {


                        var alertVW = new UIAlertView(GlobalSettings.ApplicationName, "Vacation Corrections are not available at this time – contact admin@wbidmax.com", null, "OK", null);
                        alertVW.Show();
                        alertVW.Clicked += alertVW_Clicked;
                    });


            }
            else
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);

            }


        }

        void alertVW_Clicked(object sender, UIButtonEventArgs e)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);
        }

        private void scrap(string userName, string password, List<string> pairingwHasNoDetails, int month, int year, int show1stDay, int showAfter1stDay)
        {

            GlobalSettings.IsScrapStart = true;

            //Task aa = new Task(() =>
            //{
            InvokeOnMainThread(() =>
                {

					if(userName.ToLower()=="e21221")
					{
					
						userName="x21221";
					ContractorEmpScrap scrap = new ContractorEmpScrap(userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay,GlobalSettings.CurrentBidDetails.Postion);


						this.AddChildViewController(scrap);
						scrap.View.Hidden = true;

						this.View.AddSubview(scrap.View);
					}
					else
					{
					webView scrapper = new webView(userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay,GlobalSettings.CurrentBidDetails.Postion);


                    this.AddChildViewController(scrapper);
                    scrapper.View.Hidden = true;

                    this.View.AddSubview(scrapper.View);
					}
                });



            while (GlobalSettings.IsScrapStart)
            {
            };

            //});

            //aa.Start();
            //aa.Wait();

        }

        /// <summary>
        /// Parse Seniority List
        /// </summary>
        private void ParseSeniorityList()
        {
            var senParser = new SeniorityListParser();
            GlobalSettings.IsFVVacation = false;
            string seniorityFileName = string.Empty;
            // List<SeniorityListMember> seniorityListMembers = new List<SeniorityListMember>();
            _objSeniorityListMembers = new List<SeniorityListMember>();
            var modifiedSeniorityListParser = new ModifiedSeniorityListParser();

            //Download wbid seniority file
            if ((GlobalSettings.IsNeedToDownloadSeniority || GlobalSettings.IsNeedToDownloadSeniorityUser) && GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                string sFile;
                if (GlobalSettings.CurrentBidDetails.Round == "M")
                    sFile = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S";
                else
                    sFile = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R";
                DownloadBid.DownloadWBidSeniorityFile(WBidHelper.GetAppDataPath(), (sFile + ".TXT"));

            }

            try
            {
                SenListFormat senlistformat = null;
                int round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                try
                {
                    int typeOfInternetConnection = InternetHelper.CheckInterNetConnection();

                    // typeOfInternetConnection = (int)InternetType.Air;
                    //No internet connection
                    if (typeOfInternetConnection == (int)InternetType.NoInternet)
                    {
                        senlistformat = GlobalSettings.WBidINIContent.SenioritylistFormat.FirstOrDefault(x => x.Round == round && x.Position == GlobalSettings.CurrentBidDetails.Postion);
                        //GlobalSettings.IsVacationCorrection = false;
                        //InvokeOnMainThread(() => DisplayAlertView(GlobalSettings.ApplicationName, "No Internet connection and We are not able to read the seniority list."));

                    }
                    //Grount type internet
                    else if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
                    {
                        //string serviceurl = (GlobalSettings.CurrentBidDetails.Round == "M") ? "GetSeniorityListFormat/1" : "GetSeniorityListFormat/2";
                       // string serviceurl = (GlobalSettings.CurrentBidDetails.Round == "M") ? "GetSeniorityListFormatFromDB/1/" + GlobalSettings.CurrentBidDetails.Postion : "GetSeniorityListFormatFromDB/2" + GlobalSettings.CurrentBidDetails.Postion;
                        string serviceurl = "GetAllSeniorityListFormatFromDB";
                        StreamReader dr = RestHelper.GetRestData(serviceurl);
                        List<SenListFormat> allsenlistformat = SerializeHelper.ConvertJSonStringToObject<List<SenListFormat>>(dr.ReadToEnd());
                        senlistformat = allsenlistformat.FirstOrDefault(x => x.Round == round && x.Position == GlobalSettings.CurrentBidDetails.Postion);
                        GlobalSettings.WBidINIContent.SenioritylistFormat = allsenlistformat;
                        WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                    }
                    //Airtype internet
                    else if (typeOfInternetConnection == (int)InternetType.Air)
                    {
                        senlistformat = GlobalSettings.WBidINIContent.SenioritylistFormat.FirstOrDefault(x => x.Round == round && x.Position == GlobalSettings.CurrentBidDetails.Postion);
                       // GlobalSettings.IsVacationCorrection = false;
                        //InvokeOnMainThread(() => DisplayAlertView(GlobalSettings.ApplicationName, "We are not able to read the seniority list when you use the Southwest Wifi."));

                    }

                }
                catch (Exception ex)
                {
                    senlistformat = GlobalSettings.WBidINIContent.SenioritylistFormat.FirstOrDefault(x => x.Round == round && x.Position == GlobalSettings.CurrentBidDetails.Postion);
                   // GlobalSettings.IsVacationCorrection = false;
                   // InvokeOnMainThread(() => DisplayAlertView(GlobalSettings.ApplicationName, "We are not able to read the seniority list, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view."));

                   
                }
                if (GlobalSettings.CurrentBidDetails.Round == "M")
                {
                    seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S";
                    if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                    {

                        _objSeniorityListMembers = senParser.ParseSeniorityListForFirstRoundFA(seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);

                    }
                    else
                    {
                        try
                        {
                            _objSeniorityListMembers = modifiedSeniorityListParser.ParseSeniorityListForPilot(seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month,GlobalSettings.CurrentBidDetails.Round,senlistformat);


                        }
                        catch
                        {
                            _objSeniorityListMembers = senParser.ParseSeniorityListForFirstRoundPilot(seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);


                        }


                    }

                }
                else if (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                    seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R";


                    try
                    {
                        _objSeniorityListMembers = modifiedSeniorityListParser.ParseSeniorityListForPilot(seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month,GlobalSettings.CurrentBidDetails.Round,senlistformat);


                    }
                    catch
                    {
                        _objSeniorityListMembers = senParser.ParseSeniorityListForSecondRoundPilot(seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);

                    }

                }
                //SerializeObject(sFileName + ".SL", seniorityListMembers);


                if (seniorityFileName == string.Empty)
                    return;

                //Saving seniority content
                //---
                try
                {
                    var stream = File.Create(seniorityFileName + ".SL");
                    ProtoSerailizer.SerializeObject(seniorityFileName + ".SL", _objSeniorityListMembers, stream);
                    stream.Dispose();
                    stream.Close();
                }
                catch (Exception ex)
                {

                    //throw ex;
                }
                //--------------

                bool iSinSeniorityList = CheckEmpNumExistInSeniorityList(seniorityFileName + ".TXT");

                string message = string.Empty;

                if (iSinSeniorityList)
                {
                    // _seniorityListItem = new SeniorityListItem() { SeniorityNumber = seniorityListMembers.FirstOrDefault(x => x.EmpNum == _downloadInfoDetails.UserId.Substring(1, _downloadInfoDetails.UserId.Length - 1).PadLeft(6, '0')).DomicileSeniority, TotalCount = seniorityListMembers[seniorityListMembers.Count - 1].DomicileSeniority };


                    string empNumber = string.Empty;

                    if (GlobalSettings.IsDifferentUser)
                    {
                        empNumber = GlobalSettings.ModifiedEmployeeNumber.PadLeft(6, '0');

                       // _objSeniorityListItem = new SeniorityListItem() { SeniorityNumber = _objSeniorityListMembers.FirstOrDefault(x => x.EmpNum ==empNumber).DomicileSeniority, TotalCount = _objSeniorityListMembers[_objSeniorityListMembers.Count - 1].DomicileSeniority };

                    }
                    else
                    {
                        empNumber = _objDownloadInfoDetails.UserId.PadLeft(6, '0');
                       // _objSeniorityListItem = new SeniorityListItem() { SeniorityNumber = _objSeniorityListMembers.FirstOrDefault(x => x.EmpNum == empNumber).DomicileSeniority, TotalCount = _objSeniorityListMembers[_objSeniorityListMembers.Count - 1].DomicileSeniority };
                    }



                    if (_objSeniorityListMembers.FirstOrDefault(x => x.EmpNum == empNumber) != null)
                    {
                        var seniorityListMember = _objSeniorityListMembers.FirstOrDefault(x => x.EmpNum == empNumber);
                        if (seniorityListMember != null)
                            _objSeniorityListItem = new SeniorityListItem() { SeniorityNumber = seniorityListMember.DomicileSeniority, TotalCount = _objSeniorityListMembers[_objSeniorityListMembers.Count - 1].DomicileSeniority };
                    }
                    else 
                    {
                        _objSeniorityListItem = new SeniorityListItem { SeniorityNumber = 0, TotalCount = _objSeniorityListMembers[_objSeniorityListMembers.Count - 1].DomicileSeniority };
                    }

                    //message = "WBidMax found you in the Seniority List";
                    //InvokeOnMainThread(() =>
                    //{
                    //    UIAlertView alert = new UIAlertView("WBidMax", "WBidMax found you in the Seniority List", null, "OK", null);
                    //    alert.Show();
                    //});



                }
                else
                {
                    //zero indicates the seniority number is not in domcile
                    _objSeniorityListItem = new SeniorityListItem { SeniorityNumber = 0, TotalCount = _objSeniorityListMembers[_objSeniorityListMembers.Count - 1].DomicileSeniority };
                    //message = "WBidMax DID NOT find you in the Seniority List." +
                    //    "You may want to check your assigned Domicile for next month." +
                    //    "DO NOT BID THESE LINES!!!!!";
                    //InvokeOnMainThread(() =>
                    //{
                    //    UIAlertView alert = new UIAlertView("WBidMax", "WBidMax DID NOT find you in the Seniority List." +
                    //                                  "You may want to check your assigned Domicile for next month." +
                    //                                  "DO NOT BID THESE LINES!!!!!", null, "OK", null);
                    //    alert.Show();
                    //});

                }


                //InvokeOnMainThread(() =>
                //    {
                //        DisplayAlertView(GlobalSettings.ApplicationName, message);
                //        //alertVW = new UIAlertView("WBidMax", message, null, "OK", null);
                //        //alertVW.Show();
                //    });



                //  seniorityListMembers=

            }
            catch (Exception)
            {


                GlobalSettings.IsVacationCorrection = false;
                InvokeOnMainThread(() => DisplayAlertView(GlobalSettings.ApplicationName, "The Seniority List is improperly formatted, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view."));



            }
            //  return seniorityListMembers;

        }
        private void PerformFVVacation()
        {
            //SET FV Vacation 
            if (GlobalSettings.IsFVVacation)
            {
                //GlobalSettings.WBidStateCollection.FVVacation = GlobalSettings.FVVacation;
                FVVacation objFvVacation = new FVVacation();
                GlobalSettings.Lines = new ObservableCollection<Line>(objFvVacation.SetFVVacationValuesForAllLines(GlobalSettings.Lines.ToList()));
            }
        }
        private void CheckAndPerformVacationCorrection()
        {

            GlobalSettings.IsVacationCorrection = false;



            if (_objSeniorityListMembers.Count > 0)
            {

                GlobalSettings.SeniorityListMember = GlobalSettings.IsDifferentUser ? _objSeniorityListMembers.FirstOrDefault(x => x.EmpNum == (GlobalSettings.ModifiedEmployeeNumber.ToString(CultureInfo.InvariantCulture).PadLeft(6, '0'))) : _objSeniorityListMembers.FirstOrDefault(x => x.EmpNum == _objDownloadInfoDetails.UserId.PadLeft(6, '0'));


                GlobalSettings.OrderedVacationDays = null;

                if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null)
                {
                    var FAvacationabsence = GlobalSettings.SeniorityListMember.Absences.Where(x => x.AbsenceType == "FV" ||  x.AbsenceType == "CFV");
                    GlobalSettings.FVVacation = new List<Absense>();
                    FAvacationabsence.ToList().ForEach(x => GlobalSettings.FVVacation.Add(new Absense { StartAbsenceDate = x.StartAbsenceDate, EndAbsenceDate = x.EndAbsenceDate,AbsenceType=x.AbsenceType }));

                    if (GlobalSettings.SeniorityListMember.Absences.Any(x => x.StartAbsenceDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && x.EndAbsenceDate >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate && x.AbsenceType == "VA"))
                    {
                        GlobalSettings.IsVacationCorrection = (GlobalSettings.CurrentBidDetails.Round == "M" || (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA"));
                        GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();

                        GlobalSettings.TempOrderedVacationDays = GlobalSettings.OrderedVacationDays;
                    }
                    GlobalSettings.IsFVVacation = (GlobalSettings.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

                }



            }


            if (GlobalSettings.IsVacationCorrection)
            {
                // networkClient.GetFlightsAsync(GlobalSettings.CurrentBidDetails.BidPeriodStartDate, GlobalSettings.CurrentBidDetails.BidPeriodStartDate.AddDays(10));

                // networkClient.GetFlightsAsync(new DateTime(2014, 06, 05), new DateTime(2014, 06, 17));
                InvokeOnMainThread(() =>
                    {
                        this.startProgress();
                    });


                if (IsMissingTripFailed)
                {
                    GlobalSettings.IsVacationCorrection = false;

                }
                else
                {
                    if (GlobalSettings.SeniorityListMember != null && (GlobalSettings.CurrentBidDetails.Postion != "FA" &&
                                                                       GlobalSettings.SeniorityListMember.Absences.Any(x => x.AbsenceType == "VA" && x.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)))
                    {

                        string dynamicdate = string.Empty;
                        var leadinvacation = GlobalSettings.SeniorityListMember.Absences.FirstOrDefault(x => x.AbsenceType == "VA" && x.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate);
                        if (leadinvacation != null)
                        {
                            GlobalSettings.CompanyVA = (leadinvacation.EndAbsenceDate.Date.Day * GlobalSettings.DailyVacPay).ToString(CultureInfo.InvariantCulture);

                        }
                        //removed the company VA dialogue as per Frank's suggestion
                        //                        _waitCompanyVADialog = true;
                        //
                        //                        ShowVacationOverlapView();
                        //
                        //                        while (_waitCompanyVADialog)
                        //                        {
                        //                            
                        //                        }

                        if (GlobalSettings.IsVacationCorrection)
                        {

                            PerformVacation();
                        }

                    }
                    else
                    {
                        PerformVacation();

                    }

                }

                InvokeOnMainThread(() =>
                    {
                        this.moveProgressToEnd();
                    });



            }
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

        private void PerformVacation()
        {
            try
            {
                var vacationParams = new VacationCorrectionParams();

                if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                    //
                    //					string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
                    //					string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
                    string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";
                    //
                    //
                    //					WebClient wcClient = new WebClient();
                    //					//Downloading networkdat file
                    //					wcClient.DownloadFile(serverPath, zipLocalFile);

                    if (GlobalSettings.InternetType == (int)InternetType.Ground || GlobalSettings.InternetType == (int)InternetType.AirPaid)
                    {

                       // setButtonState(btnVacationData, true);
                    }
                    // Console.WriteLine("VacationDataDownloadSuccess");
                    //Extracting the zip file
                    //                    var zip = new ZipArchive();
                    //                    zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");
                    //					string target = Path.Combine(WBidHelper.GetAppDataPath(), WBidHelper.GetAppDataPath() + "/");// + Path.GetFileNameWithoutExtension(zipLocalFile))+ "/";

                    //					if (File.Exists(networkDataPath))
                    //					{
                    //						File.Delete(networkDataPath);
                    //					}
                    //
                    //					if (File.Exists(zipLocalFile))
                    //					{
                    //						ZipFile.ExtractToDirectory(zipLocalFile,target);
                    //					}
                    //ZipStorer.

                    // Open an existing zip file for reading
                    //					ZipStorer zip = ZipStorer.Open(zipLocalFile, FileAccess.Read);
                    //
                    //					// Read the central directory collection
                    //					List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
                    //
                    //					// Look for the desired file
                    //					foreach (ZipStorer.ZipFileEntry entry in dir)
                    //					{
                    //						zip.ExtractFile(entry, target+entry);
                    //					}
                    //					zip.Close();

                    //Deserializing data to FlightPlan object

                    GlobalSettings.VacationData = null;

                    var fp = new FlightPlan();

                    if (File.Exists(networkDataPath))
                    {
                        FlightPlan flightPlan;
                        using (FileStream networkDatatream = File.OpenRead(networkDataPath))
                        {
                            //  new FlightPlan();
                            flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);
                        }


                        vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
                            (fr, f) =>
                            new FlightRouteDetails
                            {
                                Flight = f.FlightId,
                                FlightDate = fr.FlightDate,
                                Orig = f.Orig,
                                Dest = f.Dest,
                                Cdep = f.Cdep,
                                Carr = f.Carr,
                                Ldep = f.Ldep,
                                Larr = f.Larr,
                                RouteNum = fr.RouteNum,

                            }).ToList();
                    }

                }


                vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
                vacationParams.Trips = _objTrips;
                vacationParams.Lines = _objLines;
                //  VacationData = new Dictionary<string, TripMultiVacData>();


                //Performing vacation correction algoritham
                var vacationBL = new VacationCorrectionBL();


                GlobalSettings.VacationData = GlobalSettings.CurrentBidDetails.Postion != "FA" ? vacationBL.PerformVacationCorrection(vacationParams) : vacationBL.PerformFAVacationCorrection(vacationParams);

                // GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);

                if (GlobalSettings.VacationData != null)
                {
                    string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


                    // save the VAC file to app data folder

                    var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
                    ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
                    stream.Dispose();
                    stream.Close();

                    if (GlobalSettings.MenuBarButtonStatus == null)
                    {
                        GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();
                    }
                    GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;

                }
                else
                {
                    GlobalSettings.IsVacationCorrection = false;
                }



            }
            catch (Exception)
            {
                GlobalSettings.IsVacationCorrection = false;
                throw;
            }
        }

        private bool CheckEmpNumExistInSeniorityList(string seniorityFileName)
        {
            var reader = new StreamReader(seniorityFileName);
            //string employeeNumber = _objDownloadInfoDetails.UserId;

            string employeeNumber = GlobalSettings.IsDifferentUser ? GlobalSettings.ModifiedEmployeeNumber : _objDownloadInfoDetails.UserId;
            employeeNumber = employeeNumber.ToLower().Replace("e", "").Replace("x", "");
            string allRead = reader.ReadToEnd();                                       //Reads the whole text file to the end
            reader.Close();                                                            //Closes the text file after it is fully read.
            //string regMatch = employeeNumber.Substring(1, employeeNumber.Length - 1);
            string regMatch = employeeNumber;
            //string to search for inside of text file. It is case sensitive.
            return Regex.IsMatch(allRead, regMatch);
            //if (Regex.IsMatch(allRead, regMatch))                                        //If the match is found in allRead
            //{
            //    MessageBox.Show(System.Windows.Application.Current.MainWindow, "WBidMax found you in the Seniority List", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            //}
            //else
            //{
            //    SoundPlayer player = new SoundPlayer();
            //    string fileName = "SIREN.WAV";
            //    string path = Path.Combine(WBidHelper.GetExecutablePath(), fileName);
            //    player.SoundLocation = path;
            //    player.Play();
            //    MessageBox.Show(System.Windows.Application.Current.MainWindow, "WBidMax DID NOT find you in the Seniority List." +
            //                                    "You may want to check your assigned Domicile for next month." +
            //                                    "DO NOT BID THESE LINES!!!!!", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            //}

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
        /// Downloads the files.
        /// </summary>
        /// <returns>The files.</returns>
        /// <param name="downloadInfo">Download info.</param>
        private List<DownloadedFileInfo> DownloadFiles(DownloadInfo downloadInfo)
        {
            try
            {
                // _serverUrl = SWAConstants.SWAUrl;
                var downloadedFileDetails = new List<DownloadedFileInfo>();

                foreach (string filename in downloadInfo.DownloadList)
                {
                    DownloadedFileInfo downloadedFileInfo = ChekFileLengthAndDownload(downloadInfo, filename);

                    downloadedFileDetails.Add(downloadedFileInfo);
                }


                return downloadedFileDetails;

            }
            catch (Exception ex)
            {
                throw ex;
                return null;

            }
        }

        private List<DownloadedFileInfo> HistoricalDataDownload()
        {
            try
            {

                var historical = new HistoricalBidDetails()
                {
                    Domicile = GlobalSettings.DownloadBidDetails.Domicile,
                    Month = GlobalSettings.DownloadBidDetails.Month,
                    Position = GlobalSettings.DownloadBidDetails.Postion,
                    Round = GlobalSettings.DownloadBidDetails.Round == "D" ? 1 : 2,
                    FileName = _objDownloadInfoDetails.DownloadList.FirstOrDefault(x => x.Length == 10 && x.Substring(7, 3) == "737"),
                    Year = GlobalSettings.DownloadBidDetails.Year
                };

                HistoricalFileInfo historicalFileInfo = RestHelper.DownloadHistoricalData(historical, true);
                List<DownloadedFileInfo> lstDownloadedFiles = new List<DownloadedFileInfo>();
                DownloadedFileInfo sWAFileInfo = new DownloadedFileInfo();

                if (historicalFileInfo != null)
                {
                    if (!string.IsNullOrEmpty(historicalFileInfo.DataString))
                    {
                        sWAFileInfo.byteArray = Convert.FromBase64String(historicalFileInfo.DataString);
                        sWAFileInfo.FileName = historicalFileInfo.Title;

                        sWAFileInfo.IsError = (sWAFileInfo.byteArray == null);

                        lstDownloadedFiles.Add(sWAFileInfo);
                    }

                }


                return lstDownloadedFiles;

            }
            catch (Exception ex)
            {
                throw ex;
                return null;

            }
        }


        private void HistoricalBidLineDataDownload()
        {
            try
            {

                var historical = new HistoricalBidDetails()
                {
                    Domicile = GlobalSettings.DownloadBidDetails.Domicile,
                    Month = GlobalSettings.DownloadBidDetails.Month,
                    Position = GlobalSettings.DownloadBidDetails.Postion,
                    Round = GlobalSettings.DownloadBidDetails.Round == "D" ? 1 : 2,
                    FileName = GlobalSettings.DownloadBidDetails.Domicile + GlobalSettings.DownloadBidDetails.Postion + "N.TXT",
                    Year = GlobalSettings.DownloadBidDetails.Year
                };

                HistoricalFileInfo historicalFileInfo = RestHelper.DownloadHistoricalData(historical, true);
                // List<DownloadedFileInfo> lstDownloadedFiles = new List<DownloadedFileInfo>();
                var sWAFileInfo = new DownloadedFileInfo();

                if (historicalFileInfo != null)
                {
                    if (!string.IsNullOrEmpty(historicalFileInfo.DataString))
                    {
                        sWAFileInfo.byteArray = Convert.FromBase64String(historicalFileInfo.DataString);
                        sWAFileInfo.FileName = historicalFileInfo.Title;

                        sWAFileInfo.IsError = (sWAFileInfo.byteArray == null) ? true : false;



                        string path = Path.Combine(WBidHelper.GetAppDataPath(), sWAFileInfo.FileName);
                        var fStream = new FileStream(path, FileMode.Create);
                        fStream.Write(sWAFileInfo.byteArray, 0, sWAFileInfo.byteArray.Length);
                        fStream.Dispose();

                        //lstDownloadedFiles.Add(sWAFileInfo);
                    }

                }




            }
            catch (Exception ex)
            {


            }
        }

        /// <summary>
        /// Cheks the file length and download.
        /// </summary>
        /// <returns>The file length and download.</returns>
        /// <param name="downloadInfo">Download info.</param>
        /// <param name="filename">Filename.</param>
        private DownloadedFileInfo ChekFileLengthAndDownload(DownloadInfo downloadInfo, string filename)
        {
            try
            {

                var downloadedFileDetails = new DownloadedFileInfo();
                string packetType = string.Empty;
                //if length <10 or > 11 will add an error message  and continue to download next file. 
                if (filename.Length < 10 || filename.Length > 11)
                {
                    downloadedFileDetails = new DownloadedFileInfo { IsError = true, Message = "", FileName = filename };

                }
                else
                {
                    //finding packet type
                    packetType = (filename.Substring(7, 3) == "737") ? "ZIPPACKET" : "TXTPACKET";
                    //Download the selected file and adding  downloaded information to downloadedFileDetails list.
                    downloadedFileDetails = _objDownloadBidObject.DownloadBidFile(downloadInfo, filename.ToUpper(), packetType);

                }
                return downloadedFileDetails;
            }
            catch (Exception ex)
            {
                throw ex;
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

            //arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("calcVACCorrection"), calcVACCorrection));
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
                    //this.prgrsVw.Progress = 0.0f;
                    PerformSelector(new ObjCRuntime.Selector("incrementProgress"), null, 1);
                });
        }
        [Export("incrementProgress")]
        void incrementProgress()
        {
            //if (this.prgrsVw.Progress >= 0.7f)
            //    return;
            InvokeOnMainThread(() =>
                {
                    //this.prgrsVw.Progress += this.prgrsVw.Progress + 0.1f;
                    //prgrsVw.SetProgress(prgrsVw.Progress + 0.1f, true);
                });
            PerformSelector(new ObjCRuntime.Selector("incrementProgress"), null, 1);

        }
        [Export("moveProgressToEnd")]
        void moveProgressToEnd()
        {

            //if (this.prgrsVw.Progress >= 1.0f)
            //    return;
            InvokeOnMainThread(() =>
                {
                    //this.prgrsVw.Progress += this.prgrsVw.Progress + 0.1f;
                   // prgrsVw.SetProgress(prgrsVw.Progress + 0.1f, true);

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
            //this.setButtonState(this.btnCheckInternetConnection, true);
            Console.WriteLine("reachabilityCheck");

        }
        public void cwaCredentialsCheck(NSNotification n)
        {
           // this.setButtonState(this.btnCheckCWACredentials, true);
            Console.WriteLine("cwaCredentialsCheck");

        }
        public void authCheckSuccess(NSNotification n)
        {
           // this.setButtonState(this.btnCheckAuthorization, true);
            Console.WriteLine("authCheckSuccess");

        }
        public void getDataFilesSuccess(NSNotification n)
        {
            //this.setButtonState(this.btnGetDataFiles, true);
            Console.WriteLine("getDataFilesSuccess");

            this.startProgress();

        }
        public void parseDataSuccess(NSNotification n)
        {
            //this.setButtonState(this.btnParseData, true);
            Console.WriteLine("parseDataSuccess");

        }

        public void saveDataSuccess(NSNotification n)
        {
            //this.setButtonState(this.btnParseData, true);
            Console.WriteLine("saveDataSuccess");
            //			if (GlobalSettings.IsVacationCorrection)
            //				applyVacation ();
            //			else if (GlobalSettings.IsOverlapCorrection)
            //				applyOverlapCorrection ();
            //
            //			InvokeOnMainThread(() =>
            //				{
            //					foreach (NSObject obj in arrObserver)
            //					{
            //						NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
            //					}
            //					//NSNotificationCenter.DefaultCenter.RemoveObserver(CommonClass.bidObserver);
            //
            //					if (alertVW != null)
            //					{
            //						alertVW.DismissWithClickedButtonIndex(0, false);
            //						alertVW = null;
            //					}
            //					this.DismissViewController(false, ()=>{
            //						lineViewController lineController = new lineViewController();
            //						CommonClass.lineVC = lineController;
            //						UINavigationController navController = new UINavigationController(lineController);
            //						navController.NavigationBar.BarStyle = UIBarStyle.Black;
            //						navController.NavigationBar.Hidden = true;
            //						UIApplication.SharedApplication.KeyWindow.RootViewController = navController;
            //					});
            //
            //				});
        }


        #endregion
    }
}

