using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;


using System.Runtime.Serialization.Formatters.Binary;
using Bidvalet.Model;
using Bidvalet.Shared;
using System.IO;
using System.Text.RegularExpressions;
using Bidvalet.iOS.Utility;
using Bidvalet.Business;
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Bidvalet.iOS
{
    public class WBidHelper
    {

        /// <summary>
        /// PURPOSE : Get App Data path
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/WBidValet";
        }

        /// <summary>
        /// PURPOSE : Get App Data path
        /// </summary>
        /// <returns></returns>
        public static string GetBidAwardPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/BidAward";
        }

        /// <summary>
        /// PURPOSE : Get the path of the INI file
        /// </summary>
        /// <returns></returns>
        public static string GetWBidINIFilePath()
        {
            return GetAppDataPath() + "/WBidINI.XML";
        }

        public static string GetWBidDWCFilePath()
        {
            return GetAppDataPath() + "/WBidDWC.XML";
        }

        public static string WBidUserFilePath
        {
            get
            {
                return GetAppDataPath() + "/User.xml";
            }
        }

        public static string WBidQuickFilter
        {
            get
            {
                return GetAppDataPath() + "/BVQuickFilter.qf";
            }
        }

        public static string GetWBidInternalLogFilePath()
        {
            return GetAppDataPath() + "/Event.log";
        }

        public static string GetWBidOfflinePaymentFilePath()
        {
            return GetAppDataPath() + "/Payment.log";
        }

        public static string WBidStateFilePath
        {
            get
            {
                return WBidHelper.GetAppDataPath() + "/" + GenerateFileNameUsingCurrentBidDetails() + ".WBS";
            }
        }
        public static string WBidUpdateFilePath
        {
            get
            {
                return WBidHelper.GetAppDataPath() + "/WBUPDATE.DAT";
            }
        }
        public static string HistoricalFilesInfoPath
        {
            get
            {
                return GetAppDataPath() + "/History" + DateTime.Now.Month.ToString().PadLeft(2, '0') + ".HST";
            }
        }
        public static string GetQuickSetFilePath()
        {
            return GetAppDataPath() + "/WBidQuicksets.qs";
        }
        public static string MILFilePath
        {
            get
            {
                return GetAppDataPath() + "/" + GenerateFileNameUsingCurrentBidDetails() + ".MIL";
            }
        }

        /// <summary>
        /// Get the "Column defenition" path 
        /// </summary>
        /// <returns></returns>
        public static string GetWBidColumnDefinitionFilePath()
        {
            return NSBundle.MainBundle.ResourcePath + "/ColumnDefinitions.xml";
        }


        /// <summary>
        /// create the directory to store the data
        /// </summary>
        public static void CreateAppDataDirectory()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var directoryname = Path.Combine(documents, "WBidMax");
            Directory.CreateDirectory(directoryname);
        }

        /// <summary>
        /// Serialize the parsed trip file and saved it into the root folder.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="objectToSerialize"></param>
        public static void SerializeObject(string filename, Object objectToSerialize)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memStream, objectToSerialize);
            byte[] byteArray = memStream.ToArray();
            FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray.ToArray(), 0, byteArray.Length);
            fileStream.Close();
            memStream.Close();
            memStream.Dispose();
        }
        /// <summary>
        /// Derialize the file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Object DeSerializeObject(string filename)
        {
            object obj = new object();
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            obj = (object)bFormatter.Deserialize(stream);
            stream.Close();
            return obj;
        }




        /// <summary>
        /// PURPOSE : Set current Bid details
        /// </summary>
        /// <param name="fileName"></param>
        public static void SetCurrentBidInformationfromZipFileName(string fileName, bool isHistoricalData)
        {
            try
            {

                string domicile = fileName.Substring(2, 3);
                string position = fileName.Substring(0, 1);
                position = (position == "C" ? "CP" : (position == "F" ? "FO" : "FA"));
                int month = int.Parse(fileName.Substring(5, 1), System.Globalization.NumberStyles.HexNumber);
                string round = fileName.Substring(1, 1) == "D" ? "M" : "S";
                int equipment = Convert.ToInt32(fileName.Substring(7, 3));
                int year = 0;
                if (isHistoricalData)
                    year = GlobalSettings.DownloadBidDetails.Year;
                else
                    year = GetZipFolderCreationTime(fileName.Replace(".737", ""), month);
                DateTime bpStartDay = CalculateBpStartDayWithYear(position, month, year);
                GlobalSettings.CurrentBidDetails = new BidDetails
                {
                    Domicile = domicile,
                    Postion = position,
                    Round = round,
                    Month = month,
                    Equipment = equipment,
                    BidPeriodStartDate = bpStartDay,
                    BidPeriodEndDate = CalculateBPEndDateWithYear(position, month, year),
                    Year = bpStartDay.Year,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetApplicationBidData()
        {
            try

            {
                string currentbidDetails = string.Empty;
                string domicile = GlobalSettings.CurrentBidDetails.Domicile ?? string.Empty;
                string position = GlobalSettings.CurrentBidDetails.Postion ?? string.Empty;
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
                string round = GlobalSettings.CurrentBidDetails.Round == "M" ? "Monthly" : "2nd Round";
                currentbidDetails = domicile + "/" + position + "/" + " " + round + "  Line for " + strMonthName + " " + GlobalSettings.CurrentBidDetails.Year;

                var sb = new StringBuilder();
                if (GlobalSettings.UserInfo != null)
                {
                    sb.Append("<br/>" + "Base            :" + GlobalSettings.UserInfo.Domicile);
                    sb.Append("<br/>" + "Seat            :" + GlobalSettings.UserInfo.Position);
                    sb.Append("<br/>" + "Employee Number :" + GlobalSettings.UserInfo.EmpNo);
                    sb.Append("<br/>" + "App Email  :" + GlobalSettings.UserInfo.Email);
                }
                currentbidDetails = currentbidDetails + sb.ToString();
                return currentbidDetails;


            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// PURPOSE : Set current Bid details from State fileName,   
        /// </summary>
        /// <param name="fileName"></param>
        public static void SetCurrentBidInformationfromStateFileName(string fileName)
        {

            string domicile = fileName.Substring(0, 3);
            string position = fileName.Substring(3, 2);
            int month = Convert.ToInt32(fileName.Substring(5, 2));
            //string round = fileName.Substring(7, 1);
            //modified the file structure
            string round = fileName.Substring(9, 1);
            string linefilename = domicile + position + month.ToString("d2") + round + "737";
            int year = Convert.ToInt16(fileName.Substring(7, 2)) + 2000;
            DateTime bpStartDay = CalculateBpStartDayWithYear(position, month, year);

            GlobalSettings.CurrentBidDetails = new BidDetails
            {
                Domicile = domicile,
                Postion = position,
                Round = round,
                Month = month,
                BidPeriodStartDate = bpStartDay,
                BidPeriodEndDate = CalculateBPEndDateWithYear(position, month, year),
                Year = bpStartDay.Year,
            };
        }
        public static DateTime CalculateBpStartDayWithYear(string position, int month, int year)
        {
            DateTime startDay;
            if (position == "FA")
            {
                if (month == 2) startDay = new DateTime(year, month - 1, 31);                  // Jan 31
                else if (month == 3) startDay = new DateTime(year, month, 2);                  // Mar 2
                else startDay = new DateTime(year, month, 1);                                  // all other months, start day is the 1st
            }
            else
            {
                startDay = new DateTime(year, month, 1);
            }
            return startDay;
        }

        public static DateTime CalculateBPEndDateWithYear(string position, int month, int year)
        {
            DateTime endDay;
            int numberOfDays = DateTime.DaysInMonth(year, month);

            if (position == "FA")
            {
                if (month == 1) endDay = new DateTime(year, month, 30);
                else if (month == 2) endDay = new DateTime(year, month + 1, 1);                  // Mar 1
                else endDay = new DateTime(year, month, numberOfDays);
            }
            else
            {
                if (month == 1) endDay = new DateTime(year, month, 31);
                else endDay = new DateTime(year, month, numberOfDays);
            }
            return endDay;

        }

        private static int GetlineFileCreationTime(string linefilename, int month)
        {

            string lineFilename = Path.Combine(WBidHelper.GetAppDataPath(), linefilename + ".WBL");
            //get the file created time for the line file 
            DateTime filecreationTime = File.GetCreationTime(lineFilename);

            int year = 2013;
            //if the user donwloads the decemeber month data from the decemeber month ,we have to use the  file created year for the bid year.
            if (filecreationTime.Month == 12 && filecreationTime.Month == month)
                year = filecreationTime.Year;
            else
                //(Since the January month data is downloaded from the December month we need to add one month to the createddatetime of the line file to get the exact bid period year.)
                year = filecreationTime.AddMonths(1).Year;
            return year;
        }
        private static int GetZipFolderCreationTime(string zipfoldername, int month)
        {

            string zipFilename = Path.Combine(WBidHelper.GetAppDataPath(), zipfoldername);
            //get the folder created time for the zip file 
            DateTime foldercreationTime = Directory.GetCreationTime(zipFilename);

            int year = 2013;
            //if the user donwloads the decemeber month data from the decemeber month ,we have to use the  file created year for the bid year.
            if (foldercreationTime.Month == 12 && foldercreationTime.Month == month)
                year = foldercreationTime.Year;
            else
                //(Since the January month data is downloaded from the December month we need to add one month to the createddatetime of the line file to get the exact bid period year.)
                year = foldercreationTime.AddMonths(1).Year;
            return year;
        }

        /// <summary>
        /// PURPOSE : Generate Filename using current bid details
        /// </summary>
        /// <returns></returns>
        public static string GenerateFileNameUsingCurrentBidDetails()
        {
            return (GlobalSettings.CurrentBidDetails == null) ? string.Empty : GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + GlobalSettings.CurrentBidDetails.Month.ToString("d2") + (GlobalSettings.CurrentBidDetails.Year - 2000).ToString() + GlobalSettings.CurrentBidDetails.Round + "737";
        }

        /// <summary>
        /// save the state file to app data folder.
        /// </summary>
        /// <param name="stateFileName"></param>
        public static void SaveStateFile(string stateFileName)
        {
            GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime();
            XmlHelper.SerializeToXml(GlobalSettings.WBidStateCollection, stateFileName);
        }
        /// <summary>
        /// PURPOSE : Save INI File
        /// </summary>
        /// <param name="wBidINI"></param>
        /// <param name="fileName"></param>
        public static bool SaveINIFile(WBidINI wBidINI, string fileName)
        {
            try
            {
                return XmlHelper.SerializeToXml<WBidINI>(wBidINI, fileName);
            }
            catch (Exception)
            {

                throw;
            }

        }

        ///// <summary>
        ///// Save state file
        ///// </summary>
        ///// <param name="stateFileName"></param>
        //public static void SaveStateFile(string stateFileName)
        //{
        //    GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime();
        //    XmlHelper.SerializeToXml(GlobalSettings.WBidStateCollection, stateFileName);
        //}


        public static bool SaveQuickFilterFile(QuickFilterInfo qfInfo, string fileName)
        {
            try
            {
                return XmlHelper.SerializeToXml<QuickFilterInfo>(qfInfo, fileName);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        ///  Save the user information(recent file) to the user.xml file
        /// </summary>
        /// <param name="wbidUser"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool SaveUserFile(UserInformation wbidUser, string fileName)
        {
            try
            {
                return XmlHelper.SerializeToXmlForUserFile<UserInformation>(wbidUser, fileName);
                //return XmlHelper.SerializeToXml<UserInformation>(wbidUser, fileName);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static bool SaveInternalLog(string eventName, int empNum, string fileName)
        {
            try
            {

                List<LogInformation> logInformation = new List<LogInformation>();

                LogInformation log = new LogInformation()
                {
                    AppNum = (int)AppNum.BidValet,
                    Base = GlobalSettings.CurrentBidDetails.Domicile,
                    Date = DateTime.Now,
                    EmployeeNumber = empNum,
                    Event = eventName,
                    // IpAddress="192.168.10.19",
                    Message = "Download From WBidValet",
                    Month = GlobalSettings.CurrentBidDetails.Month.ToString(),
                    OperatingSystemNum = GlobalSettings.OperatingSystem,
                    PlatformNumber = GlobalSettings.Platform,
                    Position = GlobalSettings.CurrentBidDetails.Postion,
                    Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2,
                    VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),




                };

                if (eventName == "submitBid")
                {
                    log.BuddyBid1 = 0;
                    log.BuddyBid2 = 0;
                    log.Message = "Submit From WBidValet";

                }

                if (File.Exists(fileName))
                {
                    logInformation = XmlHelper.DeserializeFromXml<List<LogInformation>>(fileName);
                }

                logInformation.Add(log);

                XmlHelper.SerializeToXml<List<LogInformation>>(logInformation, fileName);


                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// PURPOSE : Read  file content ferom WBid Updated (WBUPDATE.DAT ) file
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public static WBidUpdate ReadValuesfromWBUpdateFile(string fileName)
        {

            WBidUpdateParser parser = new WBidUpdateParser();
            return parser.ParseWBidUpdateFile(fileName);


        }
        /// <summary>
        /// Read Coulmn Defenition Data from XML file
        /// </summary>
        /// <param name="Filename"></param>
        //		public static ColumnDefinitions ReadCoulumnDefenitionData(string Filename)
        //		{
        //			//Read Coulmn Defenition Data from XML file
        //			return XmlHelper.DeserializeFromXml<ColumnDefinitions>(Filename);
        //		}

        public static string GenarateZipFileName()
        {
            string filename = (GlobalSettings.CurrentBidDetails.Postion == "CP") ? "C" : (GlobalSettings.CurrentBidDetails.Postion == "FO") ? "F" : "A";
            filename += (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B";
            filename += GlobalSettings.CurrentBidDetails.Domicile;
            filename += GlobalSettings.CurrentBidDetails.Month.ToString("X");
            return filename;
        }


        public static void GenerateDynamicOverNightCitiesList()
        {
            GlobalSettings.OverNightCitiesInBid = new List<City>();
            foreach (Line line in GlobalSettings.Lines)
            {
                // bool isLastTrip = false; int paringCount = 0;
                Trip trip = null;
                DateTime tripDate = DateTime.MinValue;
                foreach (var pairing in line.Pairings)
                {                 //Get trip
                    trip = GetTrip(pairing);

                    // isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    // tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                    List<string> overNightCities = trip.DutyPeriods.Select(x => x.ArrStaLastLeg).Where(y => y.ToString() != GlobalSettings.CurrentBidDetails.Domicile).ToList();

                    foreach (string city in overNightCities)
                    {
                        if (!GlobalSettings.OverNightCitiesInBid.Any(x => x.Name == city))
                        {
                            var inicity = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city);
                            if (inicity == null)
                            {
                                var cityid = GlobalSettings.WBidINIContent.Cities.Max(x => x.Id) + 1;
                                var mail = new WBidMail();
                                mail.SendMailtoAdmin("Below city is added into the INI file for this user .Check the Wbupdate file.+\nc City= " + city + "Id =" + cityid, GlobalSettings.UserInfo.Email, "New City Has been added to the INI File");

                                GlobalSettings.WBidINIContent.Cities.Add(new City { Id = cityid, Name = city, Code = 6 });
                                inicity = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city);
                            }



                            GlobalSettings.OverNightCitiesInBid.Add(new City()
                            {
                                Name = city,
                                Id = inicity.Id
                            });
                        }

                    }

                }
            }

            GlobalSettings.OverNightCitiesInBid = GlobalSettings.OverNightCitiesInBid.OrderBy(x => x.Name).ToList();
        }

        private static Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
            if (trip == null)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
            }

            return trip;

        }
        /// <summary>
        /// push the Current state file to the Undo  stack and clear the Redo statck
        /// </summary>
        public static void PushToUndoStack()
        {
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //			if (GlobalSettings.UndoStack.Count == 99)
            //			{
            //				GlobalSettings.UndoStack.RemoveAt(98);
            //			}
            //			GlobalSettings.UndoStack.Insert(0,new WBidState(wBIdStateContent));
            //			GlobalSettings.RedoStack.Clear();
        }

        public static void LogDetails(string employeeNumber, string eventName, string buddy1, string buddy2)
        {
            try
            {

                //				WBidDataDwonloadAuthServiceClient client;
                //				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
                //				client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
                //				client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
                //				client.LogOperationCompleted += Client_LogOperationCompleted;
                //
                //
                //				string baseStr = GlobalSettings.WbidUserContent.UserInformation.Domicile;
                //				string roundStr = "M";
                //				string monthStr = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MMM").ToUpper();
                //				string positionStr = GlobalSettings.WbidUserContent.UserInformation.Position;
                //
                //				if (GlobalSettings.CurrentBidDetails != null)
                //				{
                //					baseStr = GlobalSettings.CurrentBidDetails.Domicile;
                //					roundStr = GlobalSettings.CurrentBidDetails.Round;
                //					monthStr = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
                //					positionStr = GlobalSettings.CurrentBidDetails.Postion;
                //
                //				}
                //
                //
                //				//DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
                //				WBidDataDownloadAuthorizationService.Model.LogDetails logDetails=new WBidDataDownloadAuthorizationService.Model.LogDetails();
                //				buddy1 = buddy1 ?? "0";
                //				buddy2 = buddy2 ?? "0";
                //
                //				logDetails.Base = baseStr;
                //				logDetails.Round = (roundStr == "M") ? 1 : 2;
                //				logDetails.Month = monthStr;
                //				logDetails.Position = positionStr;
                //				logDetails.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;;
                //				logDetails.PlatformNumber = "iPad";
                //				logDetails.EmployeeNumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", "").Replace("E", ""));
                //				logDetails.VersionNumber =  System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                //				logDetails.Event = eventName;
                //				logDetails.Message = eventName;
                //				logDetails.BidForEmpNum = int.Parse(employeeNumber.Replace("e", "").Replace("E", ""));
                //				logDetails.BuddyBid1 = int.Parse(buddy1.Replace("e", "").Replace("E", ""));
                //				logDetails.BuddyBid2 = int.Parse(buddy2.Replace("e", "").Replace("E", ""));
                //				client.LogOperationAsync(logDetails);
                //
                //
                //
                //

                //client.LogOperationAsync(
            }
            catch (Exception ex)
            {


            }
        }

        //		static void Client_LogOperationCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //		{
        //
        //		}


        public static string GenaratePacketId(BidDetails bidDetails)
        {
            string packetid = string.Empty;
            packetid = bidDetails.Domicile + bidDetails.Year + bidDetails.Month.ToString("d2");

            //Set-round-numbers:
            //1 - F/A monthly bids
            //2 - F/A supplemental bids
            //3 - reserved
            //4 - Pilot monthly bids
            //5 - Pilot supplemental bids

            if (bidDetails.Round == "M" && bidDetails.Postion == "FA")
            {
                packetid += "1";
            }
            else if (bidDetails.Round == "S" && bidDetails.Postion == "FA")
            {
                packetid += "2";
            }
            else if (bidDetails.Round == "M" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
            {
                packetid += "4";
            }
            else if (bidDetails.Round == "S" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
            {
                packetid += "5";
            }
            return packetid;
        }

        public static string GenarateBidLineString(bool IsAll, int seniorityNumber)
        {
            string bidLines = string.Empty;
            List<Line> lines = GlobalSettings.Lines.ToList();
            if (IsAll)
            {
                bidLines = string.Join(",", lines.Select(x => x.LineNum));
            }
            else
            {
                bidLines = string.Join(",", lines.Take(seniorityNumber).Select(x => x.LineNum));
            }
            return bidLines;
        }
        public static LogInformation LogBadPasswordUsage(string _empNumber, bool isDownload)
        {
            LogInformation logInfo = new LogInformation();

            logInfo.AppNum = (int)AppNum.BidValet;
            if (isDownload)
            {
                if (GlobalSettings.DownloadBidDetails != null)
                {
                    logInfo.Base = GlobalSettings.DownloadBidDetails.Domicile;
                    logInfo.Month = (GlobalSettings.IsCurrentMonthOn ? DateTime.Now : DateTime.Now.AddMonths(1)).ToString("MMM").ToUpper();
                    logInfo.Round = (GlobalSettings.DownloadBidDetails.Round == "B") ? 1 : 2;
                    logInfo.Position = GlobalSettings.DownloadBidDetails.Postion;
                }
            }
            else
            {
                if (GlobalSettings.CurrentBidDetails != null)
                {
                    logInfo.Base = GlobalSettings.CurrentBidDetails.Domicile;
                    logInfo.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
                    logInfo.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    logInfo.Position = GlobalSettings.CurrentBidDetails.Postion;
                }
            }
            logInfo.Date = DateTime.Now;
            logInfo.EmployeeNumber = Convert.ToInt32(Regex.Match(_empNumber, @"\d+").Value);
            logInfo.Event = "bad password";
            logInfo.Message = "bad password" + " from WBidValet";

            logInfo.OperatingSystemNum = GlobalSettings.OperatingSystem;
            logInfo.PlatformNumber = GlobalSettings.Platform;

            logInfo.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();




            return logInfo;
        }
        public static void CalculateBidlist()
        {
            GlobalSettings.Lines.ToList().ForEach(x =>
            {
                x.TopLock = false;
                x.BotLock = false;
                //x.WeightPoints.Reset();
                if (x.BAFilters != null)
                    x.BAFilters.Clear();
                x.BAGroup = string.Empty;
                x.IsGrpColorOn = 0;
            });


            BidAutomatorCalculations bidAutomatorCalculations = new BidAutomatorCalculations();
            bidAutomatorCalculations.CalculateLinePropertiesForBAFilters();
            //Apply COnstrint And Sort
            bidAutomatorCalculations.ApplyBAFilterAndSort();
            SetCurrentBADetailsToCalculateBAState();

            UpdateWBidStateContent();
            string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
            WBidHelper.SaveStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
            BidListViewBL.GenerateBidListIconCollection();
        }
        private static void SetCurrentBADetailsToCalculateBAState()
        {
            try
            {
                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                if (wBidStateContent.BidAuto != null)
                {
                    wBidStateContent.BidAutoOn = true;

                    wBidStateContent.CalculatedBA = new BidAutomator
                    {
                        IsBlankBottom = wBidStateContent.BidAuto.IsBlankBottom,
                        IsReserveBottom = wBidStateContent.BidAuto.IsReserveBottom,
                        IsReserveFirst = wBidStateContent.BidAuto.IsReserveFirst
                    };

                    //---------------------------------------------------------------------------
                    if (wBidStateContent.BidAuto.BAFilter != null)
                    {
                        wBidStateContent.CalculatedBA.BAFilter = new List<BidAutoItem>();
                        SetCurrentBAFilterDetailsToCalculateBAFilterState(wBidStateContent);

                    }
                    //---------------------------------------------------------------------------
                    //Ba Group object
                    //---------------------------------------------------------------------------
                    if (wBidStateContent.BidAuto.BAGroup != null)
                    {
                        wBidStateContent.CalculatedBA.BAGroup = new List<BidAutoGroup>();
                        SetCurrentBAGroupDetailsToCalculateBAGroupState(wBidStateContent);

                        // GlobalSettings.WBidStateContent.BidAuto.BAGroup = null;
                    }
                    //---------------------------------------------------------------------------

                    //Sort object
                    //---------------------------------------------------------------------------
                    if (wBidStateContent.BidAuto.BASort != null)
                    {
                        wBidStateContent.CalculatedBA.BASort = new SortDetails
                        {
                            SortColumn = wBidStateContent.BidAuto.BASort.SortColumn,
                            SortColumnName = wBidStateContent.BidAuto.BASort.SortColumnName,
                            SortDirection = wBidStateContent.BidAuto.BASort.SortDirection
                        };
                        //Block sort list
                        // if (GlobalSettings.WBidStateContent.CalculatedBA.BASort.BlokSort != null)
                        if (wBidStateContent.BidAuto.BASort.BlokSort != null)
                        {
                            wBidStateContent.CalculatedBA.BASort.BlokSort = new List<string>();
                            foreach (var item in wBidStateContent.BidAuto.BASort.BlokSort)
                            {
                                wBidStateContent.CalculatedBA.BASort.BlokSort.Add(item);
                            }
                        }

                    }
                    //---------------------------------------------------------------------------

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void SetCurrentBAGroupDetailsToCalculateBAGroupState(WBidState wBidStateContent)
        {

            foreach (var item in wBidStateContent.BidAuto.BAGroup)
            {
                wBidStateContent.CalculatedBA.BAGroup.Add(new BidAutoGroup { GroupName = item.GroupName, Lines = item.Lines });
            }

        }
        private static void SetCurrentBADetailsToCalculateBAState(WBidState wBidStateContent)
        {
            try
            {
                if (wBidStateContent.BidAuto != null)
                {
                    wBidStateContent.CalculatedBA = new BidAutomator
                    {
                        IsBlankBottom = wBidStateContent.BidAuto.IsBlankBottom,
                        IsReserveBottom = wBidStateContent.BidAuto.IsReserveBottom,
                        IsReserveFirst = wBidStateContent.BidAuto.IsReserveFirst
                    };


                    //Ba filter
                    //---------------------------------------------------------------------------
                    if (wBidStateContent.BidAuto.BAFilter != null)
                    {
                        wBidStateContent.CalculatedBA.BAFilter = new List<BidAutoItem>();
                        SetCurrentBAFilterDetailsToCalculateBAFilterState(wBidStateContent);
                    }
                    //---------------------------------------------------------------------------
                    //Ba Group object
                    //---------------------------------------------------------------------------
                    if (wBidStateContent.BidAuto.BAGroup != null)
                    {
                        wBidStateContent.CalculatedBA.BAGroup = new List<BidAutoGroup>();
                        SetCurrentBAGroupDetailsToCalculateBAGroupState(wBidStateContent);

                        // GlobalSettings.WBidStateContent.BidAuto.BAGroup = null;
                    }
                    //---------------------------------------------------------------------------

                    //Sort object
                    //---------------------------------------------------------------------------
                    if (wBidStateContent.BidAuto.BASort != null)
                    {
                        wBidStateContent.CalculatedBA.BASort = new SortDetails
                        {
                            SortColumn = wBidStateContent.BidAuto.BASort.SortColumn,
                            SortColumnName = wBidStateContent.BidAuto.BASort.SortColumnName,
                            SortDirection = wBidStateContent.BidAuto.BASort.SortDirection
                        };
                        //Block sort list
                        // if (GlobalSettings.WBidStateContent.CalculatedBA.BASort.BlokSort != null)
                        if (wBidStateContent.BidAuto.BASort.BlokSort != null)
                        {
                            wBidStateContent.CalculatedBA.BASort.BlokSort = new List<string>();
                            foreach (var item in wBidStateContent.BidAuto.BASort.BlokSort)
                            {
                                wBidStateContent.CalculatedBA.BASort.BlokSort.Add(item);
                            }
                        }

                    }
                    //---------------------------------------------------------------------------




                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void SetCurrentBAFilterDetailsToCalculateBAFilterState(WBidState wBidStateContent)
        {
            try
            {
                foreach (var item in wBidStateContent.BidAuto.BAFilter)
                {
                    var calculatedItem = new BidAutoItem
                    {
                        Name = item.Name,
                        Priority = item.Priority,
                        IsApplied = item.IsApplied
                    };

                    SetAutoObjectValueToCalculateBAFilter(item, calculatedItem);
                    wBidStateContent.CalculatedBA.BAFilter.Add(calculatedItem);
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private static void SetAutoObjectValueToCalculateBAFilter(BidAutoItem item, BidAutoItem calculatedItem)
        {
            try
            {
                Cx3Parameter cx3Parameter;
                Cx3Parameter calculateCx3Parameter;
                CxDays cxDay;
                CxDays calculateCxDays;
                switch (calculatedItem.Name)
                {

                    //-----------------------------------------------------------------------
                    case "AP":
                        var amPmConstriants = (AMPMConstriants)item.BidAutoObject;
                        var calculateAmPmConstriants = new AMPMConstriants
                        {
                            AM = amPmConstriants.AM,
                            MIX = amPmConstriants.MIX,
                            PM = amPmConstriants.PM
                        };
                        calculatedItem.BidAutoObject = calculateAmPmConstriants;
                        break;
                    //-----------------------------------------------------------------------
                    case "CL":
                        var ftCommutableLine = (FtCommutableLine)item.BidAutoObject;
                        var calculateFtCommutableLine = new FtCommutableLine
                        {
                            BaseTime = ftCommutableLine.BaseTime,
                            CheckInTime = ftCommutableLine.CheckInTime,
                            City = ftCommutableLine.City,
                            CommuteCity = ftCommutableLine.CommuteCity,
                            ConnectTime = ftCommutableLine.ConnectTime,
                            NoNights = ftCommutableLine.NoNights,
                            ToHome = ftCommutableLine.NoNights,
                            ToWork = ftCommutableLine.ToWork
                        };
                        calculatedItem.BidAutoObject = calculateFtCommutableLine;
                        break;
                    //-----------------------------------------------------------------------
                    case "DOM":
                        var daysOfMonthCx = (DaysOfMonthCx)item.BidAutoObject;
                        var calculateDaysOfMonthCx = new DaysOfMonthCx();
                        if (daysOfMonthCx.OFFDays != null)
                        {
                            calculateDaysOfMonthCx.OFFDays = new List<int>();
                            foreach (var offDay in daysOfMonthCx.OFFDays)
                            {
                                calculateDaysOfMonthCx.OFFDays.Add(offDay);
                            }
                        }
                        if (daysOfMonthCx.WorkDays != null)
                        {
                            calculateDaysOfMonthCx.WorkDays = new List<int>();
                            foreach (var workDay in daysOfMonthCx.WorkDays)
                            {
                                calculateDaysOfMonthCx.WorkDays.Add(workDay);
                            }
                        }
                        calculatedItem.BidAutoObject = calculateDaysOfMonthCx;
                        break;
                    //-----------------------------------------------------------------------
                    case "DOWA":
                        cxDay = (CxDays)item.BidAutoObject;
                        calculateCxDays = new CxDays
                        {
                            IsFri = cxDay.IsFri,
                            IsMon = cxDay.IsMon,
                            IsSat = cxDay.IsSat,
                            IsSun = cxDay.IsSun,
                            IsThu = cxDay.IsThu,
                            IsTue = cxDay.IsTue,
                            IsWed = cxDay.IsWed
                        };
                        calculatedItem.BidAutoObject = calculateCxDays;
                        break;
                    //-----------------------------------------------------------------------
                    case "DOWS":
                        cx3Parameter = (Cx3Parameter)item.BidAutoObject;
                        calculateCx3Parameter = new Cx3Parameter
                        {
                            ThirdcellValue = cx3Parameter.ThirdcellValue,
                            Type = cx3Parameter.Type,
                            Value = cx3Parameter.Value
                        };
                        calculatedItem.BidAutoObject = calculateCx3Parameter;
                        break;
                    //-----------------------------------------------------------------------
                    case "DHFL":
                        cx3Parameter = (Cx3Parameter)item.BidAutoObject;
                        calculateCx3Parameter = new Cx3Parameter
                        {
                            ThirdcellValue = cx3Parameter.ThirdcellValue,
                            Type = cx3Parameter.Type,
                            Value = cx3Parameter.Value
                        };
                        calculatedItem.BidAutoObject = calculateCx3Parameter;
                        break;
                    //-----------------------------------------------------------------------
                    case "ET":
                        cx3Parameter = (Cx3Parameter)item.BidAutoObject;
                        calculateCx3Parameter = new Cx3Parameter
                        {
                            ThirdcellValue = cx3Parameter.ThirdcellValue,
                            Type = cx3Parameter.Type,
                            Value = cx3Parameter.Value
                        };
                        calculatedItem.BidAutoObject = calculateCx3Parameter;
                        break;
                    //-----------------------------------------------------------------------
                    case "LT":
                        var lineTypeItem = (CxLine)item.BidAutoObject;
                        var calculateCxLine = new CxLine
                        {
                            Blank = lineTypeItem.Blank,
                            Hard = lineTypeItem.Hard,
                            International = lineTypeItem.International,
                            NonConus = lineTypeItem.NonConus,
                            Ready = lineTypeItem.Ready,
                            Reserve = lineTypeItem.Reserve
                        };
                        calculatedItem.BidAutoObject = calculateCxLine;
                        break;
                    //-----------------------------------------------------------------------
                    case "OC":

                        var bulkOvernightCityCx = (BulkOvernightCityCx)item.BidAutoObject;
                        var calculateBulkOvernightCityCx = new BulkOvernightCityCx();
                        if (bulkOvernightCityCx.OverNightNo != null)
                        {
                            calculateBulkOvernightCityCx.OverNightNo = new List<int>();
                            foreach (var overNightNo in bulkOvernightCityCx.OverNightNo)
                            {
                                calculateBulkOvernightCityCx.OverNightNo.Add(overNightNo);
                            }
                        }
                        if (bulkOvernightCityCx.OverNightYes != null)
                        {
                            calculateBulkOvernightCityCx.OverNightYes = new List<int>();
                            foreach (var overNightYes in bulkOvernightCityCx.OverNightYes)
                            {
                                calculateBulkOvernightCityCx.OverNightYes.Add(overNightYes);
                            }
                        }
                        calculatedItem.BidAutoObject = calculateBulkOvernightCityCx;
                        break;
                    //-----------------------------------------------------------------------
                    case "RT":
                        cx3Parameter = (Cx3Parameter)item.BidAutoObject;
                        calculateCx3Parameter = new Cx3Parameter
                        {
                            ThirdcellValue = cx3Parameter.ThirdcellValue,
                            Type = cx3Parameter.Type,
                            Value = cx3Parameter.Value
                        };
                        calculatedItem.BidAutoObject = calculateCx3Parameter;
                        break;
                    //-----------------------------------------------------------------------
                    case "SDOW":
                        cxDay = (CxDays)item.BidAutoObject;
                        calculateCxDays = new CxDays
                        {
                            IsFri = cxDay.IsFri,
                            IsMon = cxDay.IsMon,
                            IsSat = cxDay.IsSat,
                            IsSun = cxDay.IsSun,
                            IsThu = cxDay.IsThu,
                            IsTue = cxDay.IsTue,
                            IsWed = cxDay.IsWed
                        };
                        calculatedItem.BidAutoObject = calculateCxDays;
                        break;
                    //-----------------------------------------------------------------------
                    case "TBL":
                        var tripBlockLengthItem = (CxTripBlockLength)item.BidAutoObject;
                        var calculateCxTripBlockLength = new CxTripBlockLength
                        {
                            FourDay = tripBlockLengthItem.FourDay,
                            IsBlock = tripBlockLengthItem.IsBlock,
                            ThreeDay = tripBlockLengthItem.ThreeDay,
                            Turns = tripBlockLengthItem.Turns,
                            Twoday = tripBlockLengthItem.Twoday
                        };
                        calculatedItem.BidAutoObject = calculateCxTripBlockLength;
                        break;
                        //-----------------------------------------------------------------------


                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        private static void UpdateWBidStateContent()
        {
            //Save tag details to state file;


            if (GlobalSettings.Lines != null && GlobalSettings.Lines.Count > 0)
            {
                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                if (wBidStateContent != null)
                {
                    wBidStateContent.TagDetails = new TagDetails();
                    wBidStateContent.TagDetails.AddRange(GlobalSettings.Lines.ToList().Where(x => x.Tag != null && x.Tag.Trim() != string.Empty).Select(y => new Tag { Line = y.LineNum, Content = y.Tag }));

                    int toplockcount = GlobalSettings.Lines.Count(x => x.TopLock);
                    int bottomlockcount = GlobalSettings.Lines.Count(x => x.BotLock);
                    //save the top and bottom lock
                    wBidStateContent.TopLockCount = toplockcount;
                    wBidStateContent.BottomLockCount = bottomlockcount;

                    //Get the line oreder
                    List<int> lineorderlist = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
                    var lineOrders = new LineOrders();
                    int count = 1;
                    lineOrders.Orders = lineorderlist.Select(x => new LineOrder { LId = x, OId = count++ }).ToList();
                    lineOrders.Lines = lineorderlist.Count;
                    wBidStateContent.LineOrders = lineOrders;

                    //save the state of the Reserve line to botttom or blank line to bottom
                    wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
                    wBidStateContent.ForceLine.IsReverseLinetoBottom = false;


                    if (wBidStateContent.SortDetails == null)
                        wBidStateContent.SortDetails = new SortDetails();


                    wBidStateContent.SortDetails.SortColumn = "Line";


                    wBidStateContent.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
                }
            }


        }

        public static Dictionary<string, Trip> GetMissingtripFromVPS(MonthlyBidDetails bidDetails)
        {

            //var jsonData = JsonSerializer(bidDetails);
            var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<MonthlyBidDetails>(bidDetails);
            StreamReader dr = RestHelper.GetRestData("GetScrappedMissedTrips", jsonData);
            MissedTripResponseModel tripdata = SerializeHelper.ConvertJSonStringToObject<MissedTripResponseModel>(dr.ReadToEnd());
            if (tripdata.JsonTripData == null || tripdata.Message == "No such missed trips available")
            {
                return new Dictionary<string, Trip>();
            }
            Dictionary<string, Trip> tripdatas = tripdata.JsonTripData.ToDictionary(x => x.TripNum, x => x);
            return tripdatas;


        }
        public static string GetAwardAlert(UserBidDetails userbiddetails)
        {
            string alert = string.Empty;
            var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<UserBidDetails>(userbiddetails);
            StreamReader dr = RestHelper.GetRestData("GetCurrentMonthAwardData", jsonData);

            AwardDetails awarddata = SerializeHelper.ConvertJSonStringToObject<AwardDetails>(dr.ReadToEnd());
            string monthName = new DateTime(2010, userbiddetails.Month, 1).ToString("MMM", CultureInfo.InvariantCulture);
            if (awarddata.AwardedLine == 10000)//code for just to avoid alert
                return "";
            if (awarddata.AwardedLine != 0)
            {

                if (awarddata.IsPaperbid)
                {
                    alert = "You are a paper bid for the month and you were awarded line " + awarddata.AwardedLine;
                }
                else if (userbiddetails.Position == "FA")
                {
                    //You were awarded line 214 B for Jan 2020.  You will be flying with Wonder Woman (22222) postion A and SuperMan (11111) position C
                    //You are a paper bid for the month and you were awarded line 176
                    //You were awarded line 213 for Jan 2020.  You will be flying with Capt Sky King (22028)".  Then a simple OK button.  When the OK button is pushed, the Awards list will display

                    alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .";
                    if (awarddata.BuddyAwards.Count > 0)
                    {
                        alert += "\n\nYou will be flying with ";
                    }
                    foreach (var item in awarddata.BuddyAwards)
                    {
                        alert += item.BuddyName.TrimEnd() + " ( " + item.BuddyEmpNum + " ) position " + item.BuddyPosition + " and ";
                    }
                    if (alert.Length > 3 && alert.Substring(alert.Length - 4, 4) == "and ")
                    {
                        alert = alert.Substring(0, alert.Length - 4);
                    }
                }
                else if (userbiddetails.Position == "CP")
                {
                    alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
                    if (awarddata.BuddyAwards.Count > 0)
                    {
                        alert += "You will be flying with " + awarddata.BuddyAwards[0].BuddyName + " ( " + awarddata.BuddyAwards[0].BuddyEmpNum + " )";
                    }
                }
                else if (userbiddetails.Position == "FO")
                {
                    alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
                    if (awarddata.BuddyAwards.Count > 0)
                    {
                        alert += "You will be flying with Capt " + awarddata.BuddyAwards[0].BuddyName + " ( " + awarddata.BuddyAwards[0].BuddyEmpNum + " )";
                    }
                }
            }
            else
            {
                // We could not find any awarded line for 22020
                alert = "We could not find any awarded line for " + userbiddetails.EmployeeNumber;
            }
            return alert;
        }



        public static void RecalculateAMPMAndWekProperties(bool isneedToSaveLineFile)
        {
            try
            {
                if (GlobalSettings.Lines != null && GlobalSettings.Trip != null)
                {
                    Dictionary<string, Line> lines = new Dictionary<string, Line>();
                    RecalcalculateLineProperties calculateLineProperties = new RecalcalculateLineProperties();
                    foreach (Line line in GlobalSettings.Lines)
                    {
                        calculateLineProperties.RecalculateAMPMPropertiesAfterAMPMDefenitionChanges(line);
                        calculateLineProperties.RecalculateWeekProperties(line);
                        lines.Add(line.LineNum.ToString(), line);
                    }

                    LineInfo lineInfo = new LineInfo()
                    {
                        LineVersion = GlobalSettings.LineVersion,
                        Lines = lines

                    };
                    if (isneedToSaveLineFile)
                    {
                        var filename = WBidHelper.GetCurrentlyLoadedLinefileName();
                        if (filename != string.Empty)
                        {
                            var jsonLineString = JsonConvert.SerializeObject(lineInfo);
                            File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + filename, jsonLineString);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// Retrive line file  from local or from server
        /// </summary>
        /// <param name="isVacEomDrpBit"></param>
        public static string RetrieveSaveAndSetLineFiles(int isVacEomDrpBit, WBidState wBIdStateContent)
        {
            string status = string.Empty;
            try
            {
                string vacFilePath = string.Empty;
                string vacFileName = "";
                int faEomstartday = 0;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
                {

                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 1;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 2;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 3;

                    vacFileName = GetRequiredLineFileName(faEomstartday);
                }
                else
                    vacFileName = GetRequiredLineFileName();

                if (vacFileName != "NoAppDataFileName")
                {
                    vacFilePath = WBidHelper.GetAppDataPath() + "/" + vacFileName;

                    if (File.Exists(vacFilePath))
                    {

                        var compressedData = File.ReadAllText(vacFilePath);
                        string VAClinefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(compressedData);

                        //desrialise the Json
                        LineInfo wblLine = SerializeHelper.ConvertJSonStringToObject<LineInfo>(VAClinefileJsoncontent);


                        GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);
                        status = "Ok";
                        RecalculateAMPMAndWekProperties(false);
                        RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
                        objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
                    }
                    else
                    {
                        //checkBit
                        //1 For DRP
                        //2 For EOM
                        //3 For VAC
                        int chekBit = isVacEomDrpBit;
                        status = DownloadVacFilesFromServer(chekBit, vacFileName, faEomstartday);

                    }
                }
                else
                {
                    // if the users.XML dont have Filenames available for the selected Bid.
                    try
                    {
                        LineInfo oldlineInfo = null;
                        var currentBid = GlobalSettings.CurrentBidDetails;
                        string filename = currentBid.Domicile + currentBid.Postion + currentBid.Month.ToString().PadLeft(2, '0') + (Convert.ToInt16(currentBid.Year) - 2000) + currentBid.Round + "737";
                        var newFileName = filename.Substring(0, 10);
                        using (FileStream lineStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
                        {
                            LineInfo ObjlineInfo = new LineInfo();

                            oldlineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", ObjlineInfo, lineStream);

                        }
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.Lines = new Dictionary<string, Line>();

                        foreach (var item in oldlineInfo.Lines.Values)
                        {
                            Line lst = new Line();
                            var parentProperties = item.GetType().GetProperties();


                            var childProperties = lst.GetType().GetProperties();
                            foreach (var parentProperty in parentProperties)
                            {

                                foreach (var childProperty in childProperties)
                                {
                                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                                    {
                                        if (parentProperty.Name == "BlankLine")
                                        {
                                        }
                                        childProperty.SetValue(lst, parentProperty.GetValue(item));
                                        break;
                                    }
                                }
                            }
                            lineInfo.Lines.Add(lst.LineNum.ToString(), lst);
                        }


                        var jsonString = JsonConvert.SerializeObject(lineInfo);

                        //Lz compression
                        var compressedData = LZStringCSharp.LZString.CompressToUTF16(jsonString);

                        File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL", compressedData);

                        //desrialise the Json
                        LineInfo wblLine = SerializeHelper.ConvertJSonStringToObject<LineInfo>(jsonString);



                        GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);

                        wBIdStateContent.MenuBarButtonState.IsVacationCorrection = false;
                        wBIdStateContent.MenuBarButtonState.IsEOM = false;
                        wBIdStateContent.MenuBarButtonState.IsVacationDrop = false;
                        wBIdStateContent.IsVacationOverlapOverlapCorrection = false;

                        GlobalSettings.UserInfo.AppDataBidFiles.Add(new AppDataBidFileNames
                        {
                            Domicile = currentBid.Domicile,
                            Month = currentBid.Month,
                            Position = currentBid.Postion,
                            Round = currentBid.Round,
                            Year = currentBid.Year,
                            lstBidFileNames = new List<BidFileNames>() { new BidFileNames { FileName = newFileName + ".WBL", FileType = (int)BidFileType.NormalLine } }
                        });

                        WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);

                        File.Delete(filename);
                    }
                    catch (Exception ex1)
                    {
                        //throw ex1;
                    }
                }
            }
            catch (Exception ex)
            {
                status = "Something Went Wrong";
            }
            return status;
        }

        /// <summary>
        /// Download VAC,EOM,DRP Files from server
        /// </summary>
        /// <param name="checkBit">for checking whether  EOM,DRP or VAC files to donwload</param>

        private static string DownloadVacFilesFromServer(int checkBit, string vacFileName, int faEomstartday)
        {
            string status = string.Empty;
            try
            {
                BidDataFileResponse biddataresponse = new BidDataFileResponse();
                BidDataRequestDTO bidDetails = new BidDataRequestDTO();
                VacFilesRequest vacRequest = new VacFilesRequest();
                bidDetails.EmpNum = int.Parse(GlobalSettings.UserInfo.EmpNo);
                bidDetails.Base = GlobalSettings.CurrentBidDetails.Domicile;
                bidDetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                bidDetails.Month = GlobalSettings.CurrentBidDetails.Month;
                bidDetails.Year = GlobalSettings.CurrentBidDetails.Year;
                bidDetails.Round = GlobalSettings.CurrentBidDetails.Round;
                //bidDetails.Round = GlobalSettings.CurrentBidDetails.Round == "D" ? "M" : "S";

                vacRequest.bidDataRequest = bidDetails;
                //1 for DRP Files
                //2 For EOM files
                //3 for vac Files
                vacRequest.checkVACBit = checkBit;
                vacRequest.FAPositions = faEomstartday;
                vacRequest.vacFileName = vacFileName;

                try
                {
                    var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<VacFilesRequest>(vacRequest);
                    //ServiceUtility.JsonSerializer(vacRequest);
                    StreamReader dr = ServiceUtility.GetRestData("GetVacFilesDRPEOM", jsonData);
                    biddataresponse = SerializeHelper.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());
                }
                catch (Exception ex)
                {
                    status = "Server Error";
                }
                if (biddataresponse.Status == true)
                {
                    //Show alert if the  data is not available
                    if (biddataresponse.bidData.Count > 0)
                    {
                        foreach (var item in biddataresponse.bidData)
                        {
                            //Decompress the string using LZ compress.
                            string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                            File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);

                            //desrialise the Json
                            LineInfo wblVACLine = SerializeHelper.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);

                            GlobalSettings.Lines = new ObservableCollection<Line>(wblVACLine.Lines.Values);

                            status = "Ok";
                            RecalculateAMPMAndWekProperties(false);
                            RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
                            objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
                        }
                    }
                }
                else
                {
                    status = biddataresponse.Message;
                    //show alert
                    //InvokeOnMainThread(() =>
                    //{
                    //    AlertController = UIAlertController.Create(biddataresponse.Message, "WBidMax", UIAlertControllerStyle.Alert);
                    //    AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                    //    {
                    //        DismissCurrentView();
                    //    }));

                    //    this.PresentViewController(AlertController, true, null);

                    //});
                }
            }
            catch (Exception ex)
            {
                status = "Error In Download";
            }
            return status;
        }




        public static string GetCurrentlyLoadedLinefileName()
        {

            string filename = string.Empty;
            if (GlobalSettings.CurrentBidDetails != null)
            {
                int faEomstartday = 0;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
                {

                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 1;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 2;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 3;
                    filename = GetRequiredLineFileName(faEomstartday);
                }
                else
                {
                    filename = GetRequiredLineFileName();
                }
            }
            return filename;
        }
        /// <summary>
        /// Getting Line file name as per  EOM,VAC,DRP button click
        /// </summary>
        /// <param name="FAEOMStartDate">if FA Bid ,we have FA positions  like 1,2,3 </param>
        /// <returns></returns>
        private static string GetRequiredLineFileName(int FAEOMStartDate = 0)
        {
            try
            {
                var currentbiddetail = GlobalSettings.CurrentBidDetails;
                AppDataBidFileNames appDataBidFileNames = GlobalSettings.UserInfo.AppDataBidFiles.FirstOrDefault(x => x.Domicile == currentbiddetail.Domicile && x.Position == currentbiddetail.Postion && x.Round == currentbiddetail.Round && x.Month == currentbiddetail.Month && x.Year == currentbiddetail.Year);


                //List<string> lstBidFiles = new List<string>();
                //lstBidFiles = GlobalSettings.WBidStateCollection.DownlaodedbidFiles;

                string lineFileName = string.Empty;

                //string vacFileName = lstBidFiles.FirstOrDefault(x => x.Contains(".WBV"));
                //if (vacFileName != null && vacFileName.Length > 3)
                //    vacFileName = vacFileName.Split('.')[0];

                //string normalinefile = lstBidFiles.FirstOrDefault(x => x.Contains(".WBL"));
                //if (normalinefile != null && normalinefile.Length > 3)
                //    normalinefile = normalinefile.Split('.')[0];

                if (appDataBidFileNames != null)
                {
                    MenuBarButtonStatus MenuButton = GlobalSettings.MenuBarButtonStatus;

                    if (!MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //No vacation-Set WBL file name
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.NormalLine);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        // lineFileName = normalinefile + ".WBL";
                    }
                    if (MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //VAC+DRP
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Vacation);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        //lineFileName = vacFileName + ".WBV";
                    }
                    if (MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && !MenuButton.IsEOM)
                    {
                        //VAC
                        BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationDropOFF);
                        if (filename != null)
                            lineFileName = filename.FileName;
                        //lineFileName = vacFileName + ".DRP";
                    }
                    if (MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //VAC+DRP+EOM
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationEOM);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".WBE") && x.FileType == (int)BidFileType.VacationEOM);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //    lineFileName = vacFileName + "F.WBE";
                        //else
                        //    lineFileName = vacFileName + "F" + FAEOMStartDate + ".WBE";
                    }
                    if (!MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //EOM
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.EomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".DRP") && x.FileType == (int)BidFileType.EomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //    if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        //    lineFileName = vacFileName + "F.DRP";
                        //else
                        //    lineFileName = vacFileName + "F" + FAEOMStartDate + ".DRP";
                    }
                    if (!MenuButton.IsVacationCorrection && MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {
                        //EOM+DRP

                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Eom);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".WBE") && x.FileType == (int)BidFileType.Eom);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        //if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        //    lineFileName = normalinefile + "F.WBE";
                        //else
                        //    lineFileName = normalinefile + "F" + FAEOMStartDate + ".WBE";
                    }
                    if (MenuButton.IsVacationCorrection && !MenuButton.IsVacationDrop && MenuButton.IsEOM)
                    {

                        //EOM+VAC
                        if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.VacationEomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                        else
                        {
                            BidFileNames filename = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileName.Contains("F" + FAEOMStartDate + ".DRP") && x.FileType == (int)BidFileType.VacationEomDropOFF);
                            if (filename != null)
                                lineFileName = filename.FileName;
                        }
                    }

                }
                else
                {
                    lineFileName = "NoAppDataFileName";
                }
                return lineFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrive line file  from server(redownload data from server)
        /// </summary>
        /// <param name="wBIdStateContent"></param>
        public static string RedownloadBidDataFileFromServer(WBidState wBIdStateContent)
        {
            


            string status = string.Empty;
            try
            {
                string vacFileName = "";
                int faEomstartday = 0;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
                {

                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 1;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 2;
                    else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                        faEomstartday = 3;

                    vacFileName = GetRequiredLineFileName(faEomstartday);
                }
                else
                    vacFileName = GetRequiredLineFileName();

                if (vacFileName != "NoAppDataFileName")
                {
                    status = DownloadVacFilesFromServer(0, vacFileName, faEomstartday);
                }
                else
                {
                    // if the users.XML dont have Filenames available for the selected Bid.
                    try
                    {
                        LineInfo oldlineInfo = null;
                        var currentBid = GlobalSettings.CurrentBidDetails;
                        string filename = currentBid.Domicile + currentBid.Postion + currentBid.Month.ToString().PadLeft(2, '0') + (Convert.ToInt16(currentBid.Year) - 2000) + currentBid.Round + "737";
                        var newFileName = filename.Substring(0, 10);
                        using (FileStream lineStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
                        {
                           LineInfo ObjlineInfo = new LineInfo();
                            oldlineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", ObjlineInfo, lineStream);

                        }
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.Lines = new Dictionary<string, Line>();

                        foreach (var item in oldlineInfo.Lines.Values)
                        {
                            Line lst = new Line();
                            var parentProperties = item.GetType().GetProperties();


                            var childProperties = lst.GetType().GetProperties();
                            foreach (var parentProperty in parentProperties)
                            {

                                foreach (var childProperty in childProperties)
                                {
                                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                                    {
                                        if (parentProperty.Name == "BlankLine")
                                        {
                                        }
                                        childProperty.SetValue(lst, parentProperty.GetValue(item));
                                        break;
                                    }
                                }
                            }
                            lineInfo.Lines.Add(lst.LineNum.ToString(), lst);
                        }


                        var jsonString = JsonConvert.SerializeObject(lineInfo);

                        //Lz compression
                        var compressedData = LZStringCSharp.LZString.CompressToUTF16(jsonString);

                        File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL", compressedData);

                        //desrialise the Json
                        LineInfo wblLine = SerializeHelper.ConvertJSonStringToObject<LineInfo>(jsonString);
                        





                        GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);

                        wBIdStateContent.MenuBarButtonState.IsVacationCorrection = false;
                        wBIdStateContent.MenuBarButtonState.IsEOM = false;
                        wBIdStateContent.MenuBarButtonState.IsVacationDrop = false;
                        wBIdStateContent.IsVacationOverlapOverlapCorrection = false;

                        GlobalSettings.UserInfo.AppDataBidFiles.Add(new AppDataBidFileNames
                        {
                            Domicile = currentBid.Domicile,
                            Month = currentBid.Month,
                            Position = currentBid.Postion,
                            Round = currentBid.Round,
                            Year = currentBid.Year,
                            lstBidFileNames = new List<BidFileNames>() { new BidFileNames { FileName = newFileName + ".WBL", FileType = (int)BidFileType.NormalLine } }
                        });

                        WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);

                        File.Delete(filename);
                    }
                    catch (Exception ex1)
                    {
                        //throw ex1;
                    }
                }
            }
            catch (Exception ex)
            {
                status = "Something Went Wrong";
            }
            return status;

        }

        public static BidDataFileResponse RedownloadBidDataFromServer()
        {
            //Get Bid File Name

            BidDetails bidDetails = GlobalSettings.CurrentBidDetails;
            if (GlobalSettings.UserInfo.AppDataBidFiles == null)
                GlobalSettings.UserInfo.AppDataBidFiles = new List<AppDataBidFileNames>();

            AppDataBidFileNames appDataBidFileNames = GlobalSettings.UserInfo.AppDataBidFiles.FirstOrDefault(x => x.Domicile == bidDetails.Domicile && x.Position == bidDetails.Postion && x.Round == bidDetails.Round && x.Month == bidDetails.Month && x.Year == bidDetails.Year);
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            List<string> DownloadFiles = new List<string>();
            if (appDataBidFileNames != null)
            {
                DownloadFiles = GenerateRedownloadFileNames(appDataBidFileNames, DownloadFiles);

            }

            // Download multiple Bid file. Need to check multiple file downloaded service created in the service or not.
            var biddataresponse = new BidDataFileResponse();
            VacFilesRequest vacRequest = new VacFilesRequest();
            BidDataRequestDTO bidData = new BidDataRequestDTO
            {
                EmpNum = int.Parse(GlobalSettings.UserInfo.EmpNo),
                Base = bidDetails.Domicile,
                Position = bidDetails.Postion,
                Month = bidDetails.Month,
                Year = bidDetails.Year,
                Round = bidDetails.Round
            };


            vacRequest.bidDataRequest = bidData;
            vacRequest.BidFileNames = DownloadFiles;
            try
            {
                
                var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<VacFilesRequest>(vacRequest);
                StreamReader dr = RestHelper.GetRestData("GetMultipleBidDataFiles", jsonData);
                biddataresponse = SerializeHelper.ConvertJSonStringToObject<BidDataFileResponse>(dr.ReadToEnd());
                
                

                //set line and trip objects
                if (biddataresponse.bidData.Count > 0)
                {

                    foreach (var item in biddataresponse.bidData)
                    {
                        if (!item.IsError)
                        {
                            var fileExtension = item.FileName.Split('.')[1].ToString().ToLower();
                            if (fileExtension == "wbp")
                            {
                                //Decompress the string using LZ compress.
                                string tripfileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                                File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent);
                                //desrialise the Json
                                Dictionary<string, Trip> wbpLine = SerializeHelper.ConvertJSonStringToObject<Dictionary<string, Trip>>(tripfileJsoncontent);
                                GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);
                            }
                            else
                            {
                                //Decompress the string using LZ compress.
                                string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(item.FileContent);

                                File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + item.FileName, item.FileContent); //desrialise the Json
                                break;
                            }
                        }
                    }
                    string status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBIdStateContent);
                    if (status != "Ok")
                    {
                        biddataresponse.Status = false;
                    }

                    // Delete other files from local app data folder
                    IEnumerable<string> deletedFiles = new List<string>();
                    List<string> currentFiles = new List<string>();
                    foreach (var appDataFiLe in appDataBidFileNames.lstBidFileNames)
                    {
                        currentFiles.Add(appDataFiLe.FileName);
                    }
                    deletedFiles = currentFiles.Except(DownloadFiles);
                    foreach (var deleteFile in deletedFiles)
                    {
                        if (File.Exists(WBidHelper.GetAppDataPath() + "\\" + deleteFile))
                        {
                            File.Delete(WBidHelper.GetAppDataPath() + "\\" + deleteFile);

                        }
                    }
                }
                return biddataresponse;
            }
            catch (Exception ex)
            {
                
                return biddataresponse;
            }


        }

        private static List<string> GenerateRedownloadFileNames(AppDataBidFileNames appDataBidFileNames, List<string> DownloadFiles)
        {
            var normalline = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.NormalLine);
            if (normalline != null)
                DownloadFiles.Add(normalline.FileName);
            var tripdata = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Trip);
            if (tripdata != null)
                DownloadFiles.Add(tripdata.FileName);
            var vacfile = appDataBidFileNames.lstBidFileNames.FirstOrDefault(x => x.FileType == (int)BidFileType.Vacation);
            if (vacfile != null)
                DownloadFiles.Add(vacfile.FileName);

            //Set currently used line file
            int faEomstartday = 0;
            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.MenuBarButtonStatus.IsEOM)
            {

                if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) == GlobalSettings.FAEOMStartDate)
                    faEomstartday = 1;
                else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) == GlobalSettings.FAEOMStartDate)
                    faEomstartday = 2;
                else if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3) == GlobalSettings.FAEOMStartDate)
                    faEomstartday = 3;
            }

            var currentlyLoadedLineFile = GetRequiredLineFileName(faEomstartday);
            if (currentlyLoadedLineFile != null && !DownloadFiles.Contains(currentlyLoadedLineFile))
                DownloadFiles.Add(currentlyLoadedLineFile);

            return DownloadFiles;
        }

        public static NSMutableAttributedString SetInvalidCredentialAlertMessage()
        {
            string showmessage1 = "\n\n\n\nTo LOGIN, you need to use your ";
            string showmessage2 = " password! \n\nMost likely, your ";
            string showmessage3 = " password has expired. \n\nBTW, it is possible your password to LOGIN on ";
            string showmessage4 = " is valid and your ";
            string showmessage5 = " password is expired. \n\n\nTo verify your ";
            string showmessage6 = " password is still good, open a browser and try to login to ";
            string showmessage7 = " (NOT swacrew.com).";
            string Boldsletter1 = "SwaLife"; //Defines the bold field
            string Boldsletter2 = "swacrew.com"; //Defines the bold field
            string Boldsletter3 = "swalife.com"; //Defines the bold field


            nfloat size = 18.0f;
            var msg = new NSMutableAttributedString(
    showmessage1,
    font: UIFont.SystemFontOfSize(size)

);
            var msg2 = new NSMutableAttributedString(
    showmessage2,
    font: UIFont.SystemFontOfSize(size)

);
            var msg3 = new NSMutableAttributedString(
    showmessage3,
    font: UIFont.SystemFontOfSize(size)

);
            var msg4 = new NSMutableAttributedString(
    showmessage4,
    font: UIFont.SystemFontOfSize(size)

);
            var msg5 = new NSMutableAttributedString(
    showmessage5,
    font: UIFont.SystemFontOfSize(size)

);
            var msg6 = new NSMutableAttributedString(
    showmessage6,
    font: UIFont.SystemFontOfSize(size)

);
            var msg7 = new NSMutableAttributedString(
    showmessage7,
    font: UIFont.SystemFontOfSize(size)

);
            var swaBold = (new NSAttributedString(
    Boldsletter1,
 font: UIFont.BoldSystemFontOfSize(size)

));
            var swacrew = (new NSAttributedString(
   Boldsletter2,
 font: UIFont.BoldSystemFontOfSize(size)

));
            var swalife = (new NSAttributedString(
   Boldsletter3,
 font: UIFont.BoldSystemFontOfSize(size)

));

            msg.Append(swaBold);
            msg.Append(msg2);
            msg.Append(swaBold);
            msg.Append(msg3);
            msg.Append(swacrew);
            msg.Append(msg4);
            msg.Append(swaBold);
            msg.Append(msg5);
            msg.Append(swaBold);
            msg.Append(msg6);
            msg.Append(swalife);
            msg.Append(msg7);


            return msg;
        }



    }
}

