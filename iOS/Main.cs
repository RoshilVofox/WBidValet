#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.IO;
using Xamarin;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#endregion

namespace Bidvalet.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            //Check the BidValet directory is created to store the app data for the WBid

            ///Initializing  Xamarin Insight
            //Xamarin.Insights.Initialize("b97e108b3e99946f3bf2732bec9a3cc09d6403a9");
            InternetHelper.IsSouthWestWifiOr2wire();
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception exception)
            {
                //Insights.Report(exception);
                //write Log to insight
                //--------------------------------------------
                string empNumber = "UnKnown";
                if (GlobalSettings.UserInfo != null)
                {
                    empNumber = GlobalSettings.UserInfo.EmpNo;
                }

                //Insights.Report(exception, new Dictionary<string, string>
                  //{
                  //     { "EmpNumber", empNumber },
                  //     { "Issue", exception.Message }
                  //});

                //--------------------------------------------

                Console.WriteLine("Execption :" + exception);

                Console.WriteLine("Date :" + DateTime.Today.ToString());

                Console.WriteLine("device :" + UIDevice.CurrentDevice.LocalizedModel);

                string currentBid = FileOperations.ReadCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt");


                if (exception != null)
                {
                    Crashes.TrackError(exception);
                    Exception InnerException = exception.InnerException;
                    string message = exception.Message;
                    string where = exception.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
                    string source = exception.Source;

                    if (InnerException != null)
                    {
                        if (InnerException.Message != null)
                        {
                            message = InnerException.Message;
                        }

                        if (InnerException.StackTrace != null)
                        {
                            where = InnerException.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
                        }

                        source = InnerException.Source;

                        if (InnerException.InnerException != null)
                        {
                            if (InnerException.InnerException.Message != null)
                            {
                                message += " -> " + InnerException.InnerException.Message;
                            }

                            if (InnerException.InnerException.StackTrace != null)
                            {
                                where += "\r\n\r\n -> " + InnerException.InnerException.StackTrace.Split(new string[] { " at " },
                                    2, StringSplitOptions.None)[1];
                            }

                            if (InnerException.InnerException.Source != null)
                            {
                                source += " -> " + InnerException.InnerException.Source;
                            }
                        }
                    }

                    if (where.Length > 1024)
                    {
                        where = where.Substring(0, 1024);
                    }


                    var submitResult = "\r\n WBidValet Error Details.\r\n\r\n Error  :  " + message + "\r\n\r\n Where  :  " + where + "\r\n\r\n Source   :  " + source + "\r\n\r\n Version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\r\n\r\n Date  :" + DateTime.Now;


                    submitResult += "\r\n\r\n Data :" + currentBid + "\r\n\r\n Device :" + UIDevice.CurrentDevice.LocalizedModel;

                    // string submitResult = "\r\n\r\n\r\n Crash Report : \r\n\r\n\r\n" + "\r\n Date: " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + "\r\n\r\n Device: " + UIDevice.CurrentDevice.LocalizedModel + "\r\n\r\n Crash Details: " + ex + "\r\n\r\n Data: " + currentBid + "\r\n\r\n" + " ******************************* \r\n";


                    if (!Directory.Exists(WBidHelper.GetAppDataPath() + "/" + "Crash"))
                    {
                        Directory.CreateDirectory(WBidHelper.GetAppDataPath() + "/" + "Crash");
                    }

                    System.IO.File.AppendAllText(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log", submitResult);
                }
            }
        }
    }
}
