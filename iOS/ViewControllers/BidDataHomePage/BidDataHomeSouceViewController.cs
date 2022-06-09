
using System;
using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Collections.ObjectModel;
using Bidvalet.iOS.Utility;
using Bidvalet.Business;
using CoreGraphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bidvalet.iOS
{
    public class BidDataHomeSouceViewController : UICollectionViewController
    {
        List<RecentFile> recentFiles;
        public bool shouldJiggle = false;
        LoadingOverlay loadingOverlay;
        NSObject newNotif;
        NSObject notif1;
        NSObject newNotifDelete;
        public List<int> deleteIndexList;
        public UIViewController ObjParentController;
        public CGRect overlayFrame;
        public List<int> deletedBidIndexList;
        public BidDataHomeSouceViewController(UICollectionViewLayout layout)
            : base(layout)
        {

        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            recentFiles = GetExistingDataInAppData();
            
            CollectionView.RegisterClassForCell(typeof(BidDataHomeCell), BidDataHomeCell.Key);
            this.CollectionView.BackgroundColor =UIColorExtensions.FromHex(this.CollectionView.BackgroundColor, 0xFEEAB1);
            this.CollectionView.ShowsHorizontalScrollIndicator = false;
            this.CollectionView.ShowsVerticalScrollIndicator = false;
            //this.View.Layer.BorderWidth = 1;
            //this.View.Layer.BorderColor = ColorClass.SummaryHeaderBorderColor.CGColor;
            this.View.Layer.CornerRadius = 3;
            deleteIndexList = new List<int>();
            deletedBidIndexList = new List<int>();
            
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            collectionView.RegisterNibForCell(UINib.FromName("BidDataHomeCell", NSBundle.MainBundle), new NSString("BidDataHomeCell"));
            var cell = collectionView.DequeueReusableCell(BidDataHomeCell.Key, indexPath) as BidDataHomeCell;
            RecentFile aFile = recentFiles[indexPath.Row];
            bool isSelected = false;
            if (deleteIndexList!=null)
             isSelected = (deleteIndexList.Contains(indexPath.Row)) ? true : false;
            cell.BindData(aFile, indexPath, shouldJiggle, isSelected);
            cell.BackgroundColor = UIColor.LightGray;
            cell.Layer.CornerRadius = 3;
            cell.tag = indexPath.Row;
            
            return cell;
        }
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            //// TODO: return the actual number of items in the section
            //if (recentFiles.Count == 0)
            //	NSNotificationCenter.DefaultCenter.PostNotificationName("EditEnableDisable", new NSString("disable"));
            //else
            //	NSNotificationCenter.DefaultCenter.PostNotificationName("EditEnableDisable", new NSString("enable"));
            return recentFiles.Count;

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            recentFiles = GetExistingDataInAppData();
            // this.ParentViewController.View.UserInteractionEnabled = true;
            observeNotifications();
            deleteIndexList.Clear();
            NSNumber nsNum = 0;
            NSNotificationCenter.DefaultCenter.PostNotificationName("showDeleteButton", nsNum);
            this.CollectionView.ReloadData();
            // NSNotificationCenter.DefaultCenter.PostNotificationName("HandleReload", null);
            //HandleReload();

        }

        public override void ViewDidDisappear(bool animated)
        {

            base.ViewDidDisappear(animated);
            if (newNotif != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(newNotif);

                newNotif = null;
            }
            if (newNotifDelete != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(newNotifDelete);

                newNotifDelete = null;
            }
            if (notif1!=null)
            {
                
                NSNotificationCenter.DefaultCenter.RemoveObserver(notif1);
                notif1 = null;
            }

        }
        private void observeNotifications()
        {
            //notif = NSNotificationCenter.DefaultCenter.AddObserver("homeCollectionCellDelateButtonTapped", cellDeleteButtonTapped);
            newNotif  = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("HandleMultipleDelete"), HandleMultipleDelete);
            newNotifDelete = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("HandleBidDelete"), HandleDelete);
            notif1 = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("HandleReload"), HandleReload);

        }
        void HandleReload(NSNotification obj)
        {
            recentFiles = GetExistingDataInAppData();
            this.CollectionView.ReloadData();
        }

        void HandleMultipleDelete(NSNotification obj)
        {
            int strBid = Convert.ToInt32(obj.Object.ToString());
            if (deleteIndexList.Contains(strBid))
                deleteIndexList.Remove(strBid);
            else
                deleteIndexList.Add(strBid);
            NSIndexPath nsPath = NSIndexPath.FromRowSection(strBid, 0);
            NSIndexPath[] nsIndex = { nsPath };
            this.CollectionView.ReloadItems(nsIndex);
            NSNumber nsNum = 1;
            if (deleteIndexList.Count == 0)
                nsNum = 0;
            
            NSNotificationCenter.DefaultCenter.PostNotificationName("showDeleteButton", nsNum);
        }

        void HandleDelete(NSNotification n)
        {
            recentFiles = GetExistingDataInAppData();
            //int strBid = Convert.ToInt32(obj.Object.ToString());
            //int strBid = 0;
            //for (int i = 0; i < deleteIndexList.Count; i++)
            //{
               // strBid = deleteIndexList[i];
                if (recentFiles.Count >= deleteIndexList.Count)
                {
                //RecentFile fileTodelete = recentFiles[strBid];
                //string message = fileTodelete.MonthDisplay + " " + fileTodelete.Year + "\n" + fileTodelete.Domcile + "-" + fileTodelete.Position + "-" + fileTodelete.Round + "\n Do you want to delete this Bid?";
                string fileMsg = deleteIndexList.Count > 1 ? " files" : " file";
                string message = "Are you sure want to delete " + deleteIndexList.Count + fileMsg;
                    UIAlertController alert = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("NO", UIAlertActionStyle.Cancel, (actionCancel) =>
                    {

                    }));

                    alert.AddAction(UIAlertAction.Create("YES", UIAlertActionStyle.Default, (actionOK) =>
                    {
                        //LoadingOverlay loadingOverlay;
                        //loadingOverlay = new LoadingOverlay(overlayFrame, "");
                        //this.View.Add(loadingOverlay);
                        //DeleteBidPeriod(fileTodelete.Domcile, fileTodelete.Position, fileTodelete.Round, fileTodelete.Month, Convert.ToInt32(fileTodelete.Year));
                        try
                        {
                            DeleteBidPeriod();
                            var tempData=new List<RecentFile>();
                            foreach (int data in deleteIndexList)
                            {

                                tempData.Add(recentFiles[data]);
                            }
                            foreach(var temp in tempData)
                            {
                                recentFiles.Remove(temp);
                            }
                            deleteIndexList.Clear();
                            deletedBidIndexList.Clear();
                            NSNumber nsNum =  0;
                            NSNotificationCenter.DefaultCenter.PostNotificationName("showDeleteButton", nsNum);
                            this.CollectionView.ReloadData();
                            
                        }
                        catch(Exception ex)
                        {
                            new UIAlertView("Error", "Some bids are not deleted.Please try again", null, "OK", null).Show();
                            
                            foreach (int data in deleteIndexList)
                            {
                                recentFiles.Remove(recentFiles[data]);
                            }
                            foreach (int data in deletedBidIndexList)
                            {
                                deleteIndexList.Remove(data);
                            }
                            this.CollectionView.ReloadData();
                        }
                        
                        WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);
                        
                        //loadingOverlay.Hide();

                    }));
                
                this.PresentViewController(alert, true, null);
                

            }
            //}
        }
        private static List<RecentFile> GetExistingDataInAppData()
        {
            string path = WBidHelper.GetAppDataPath();
            List<RecentFile> lstRecentFiles = new RecentFiles();
            //get all the  files in the root folder(look for wbl)
            List<string> linefilenames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
                .Where(s => s.ToLower().EndsWith(".wbl")).ToList();
            foreach (string filenames in linefilenames)
            {
                string filename = filenames.Substring(0, filenames.Length - 3);
                if (filenames.Length < 14)
                    continue;

                if (File.Exists(path + "/" + filename + "WBP"))
                {
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    RecentFile recentfile = new RecentFile();
                    recentfile.Domcile = filename.Substring(0, 3);
                    recentfile.Position = filename.Substring(3, 2);
                    recentfile.Month = Convert.ToInt32(filename.Substring(5, 2));
                    recentfile.MonthDisplay = mfi.GetMonthName(Convert.ToInt32(filename.Substring(5, 2))).Substring(0, 3).ToUpper();
                    recentfile.Year = (Convert.ToInt16(filename.Substring(7, 2)) + 2000).ToString();
                    recentfile.Round = (filename.Substring(9, 1) == "M") ? "1st Round" : "2nd Round";
                    
                    lstRecentFiles.Add(recentfile);
                }
            }
            lstRecentFiles = lstRecentFiles.OrderByDescending(x => x.Year).ThenByDescending(y => y.Month).ThenByDescending(z => z.Round).ThenBy(a => a.Domcile).ToList();
            return lstRecentFiles;

        }
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            try
            {
                NSNumber nsNum = 0;
                deleteIndexList.Clear();
                this.CollectionView.ReloadData();
                NSNotificationCenter.DefaultCenter.PostNotificationName("showDeleteButton", nsNum);
    
                this.NavigationController.NavigationBar.UserInteractionEnabled = false;

                this.NavigationController.NavigationBar.TintColor = UIColor.Clear;
                //this.ParentViewController.View.UserInteractionEnabled = false;
                //collectionView.AllowsSelection = false;
                Console.WriteLine("Collection ItemSelected");
                RecentFile aFile = recentFiles[indexPath.Row];

                string round = (aFile.Round == "1st Round") ? "M" : "S";
                //genarate the filename
                string filename = aFile.Domcile + aFile.Position + aFile.Month.ToString().PadLeft(2, '0') + (Convert.ToInt16(aFile.Year) - 2000) + round + "737";
                WBidHelper.SetCurrentBidInformationfromStateFileName(filename);

                //Write to currentBidDetailsfile for Error log
                FileOperations.WriteCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData());

                string zipFilename = WBidHelper.GenarateZipFileName();


                UICollectionViewCell selectedCell = collectionView.CellForItem(indexPath);
                LoadingOverlay loadingOverlay;
                //CGRect frame = new CGRect(this.View.Frame.X, this.View.Frame.Y-30, this.View.Frame.Height+90, this.View.Frame.Width);
                loadingOverlay = new LoadingOverlay(overlayFrame, "");
                this.View.Add(loadingOverlay);

                //load the line file data
                Task task = Task.Run(() =>
                {

                    if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS"))
                    {
                        WBidIntialState wbidintialState = null;
                        try
                        {
                            wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

                        }
                        catch (Exception ex)
                        {

                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                            XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                            //WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");
                            //WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                            //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");


                        }
                        GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS", 400, 1, wbidintialState);
                        WBidHelper.SaveStateFile(filename);

                    }
                    else
                    {

                        try
                        {
                            //Read State file content and Stored it into an object
                            GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");

                            //XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");
                        }
                        catch (Exception ex)
                        {
                            //Recreate state file
                            //--------------------------------------------------------------------------------
                            WBidIntialState wbidintialState = null;
                            //WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                            try
                            {

                                wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                            }
                            catch (Exception exx)
                            {
                                wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                                XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                                //WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");

                                //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0", "");

                            }
                            GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS", 400, 1, wbidintialState);
                            WBidHelper.SaveStateFile(filename);
                            //WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"wbsRecreate","0","0");


                            //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"wbsRecreate","0","0");
                            //GlobalSettings.WBidStateCollection =XmlHelper.ReCreateStateFile(filename,GlobalSettings.Lines.Count,GlobalSettings.Lines.First ().LineNum);
                        }
                    }

                    //if (GlobalSettings.WBidStateCollection.SubmittedResult != null && GlobalSettings.WBidStateCollection.SubmittedResult != string.Empty)
                    //{


                    //    //update submit result
                    //    if (Reachability.CheckVPSAvailable())
                    //    {
                    //        RestServiceUtil RestService;
                    //        BidSubmittedData objbiddetails = new BidSubmittedData();
                    //        objbiddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    //        objbiddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    //        objbiddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    //        objbiddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    //        objbiddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    //        objbiddetails.EmpNum = Convert.ToInt32(GlobalSettings.UserInfo.EmpNo);
                    //        ObjOdata.GetBidSubmittedData(objbiddetails);
                    //    }

                    //}
                    //Check the WBL file contains 737
                    //if it contrains 737, desealize using protbug
                    //seralize using lz compression. remove 737 file.
                    //load WBL and Load WBP using LZ compression.

                    TripInfo tripInfo = null;
                    LineInfo lineInfo = null;
                    Model.OldLine.OldLineInfo oldlineInfo = null;
                    var newFileName = filename.Substring(0, 10);
                    string filePathOldWBP = WBidHelper.GetAppDataPath() + "/" + filename + ".WBP";
                    string filePathNewWBP = WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBP";
                    string filePathOldWBL = WBidHelper.GetAppDataPath() + "/" + filename + ".WBL";
                    string filePathNewWBL = WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL";

                    if (File.Exists(filePathOldWBP) || File.Exists(filePathNewWBP))
                    {
                        try
                        {
                            var compressedData = File.ReadAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBP");
                            string tripfileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(compressedData);

                            //desrialise the Json
                            Dictionary<string, Trip> wbpLine = SerializeHelper.ConvertJSonStringToObject<Dictionary<string, Trip>>(tripfileJsoncontent);

                            GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                using (FileStream tripStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBP"))
                                {
                                    TripInfo objTripInfo = new TripInfo();
                                    tripInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBP", objTripInfo, tripStream);

                                }
                                var jsonString = JsonConvert.SerializeObject(tripInfo.Trips);

                                //Lz compression
                                var compressedData = LZStringCSharp.LZString.CompressToUTF16(jsonString);

                                File.WriteAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBP", compressedData);

                                //desrialise the Json
                                Dictionary<string, Trip> wbpLine = SerializeHelper.ConvertJSonStringToObject<Dictionary<string, Trip>>(jsonString);

                                GlobalSettings.Trip = new ObservableCollection<Trip>(wbpLine.Values);

                                File.Delete(filePathOldWBP);
                            }
                            catch (Exception e)
                            {
                                throw ex;
                            }

                        }

                    }



                    if (File.Exists(filePathOldWBL) || File.Exists(filePathNewWBL))
                    {
                        try
                        {

                            var compressedData = File.ReadAllText(WBidHelper.GetAppDataPath() + "/" + newFileName + ".WBL");
                            string linefileJsoncontent = LZStringCSharp.LZString.DecompressFromUTF16(compressedData);

                            //desrialise the Json
                            LineInfo wblLine = SerializeHelper.ConvertJSonStringToObject<LineInfo>(linefileJsoncontent);


                            GlobalSettings.Lines = new ObservableCollection<Line>(wblLine.Lines.Values);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                using (FileStream lineStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
                                {
                                    Model.OldLine.OldLineInfo ObjlineInfo = new Model.OldLine.OldLineInfo();
                                    oldlineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", ObjlineInfo, lineStream);

                                }
                                lineInfo = new LineInfo();
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
                                                if (parentProperty.Name == "FASecondRoundPositions")
                                                {

                                                }
                                                childProperty.SetValue(lst, parentProperty.GetValue(item));



                                                break;
                                            }
                                        }
                                    }

                                    if (item.FASecondRoundPositions != null || item.FASecondRoundPositions.Count > 0)
                                    {
                                        var fasecondroundpositionsOld = item.FASecondRoundPositions;
                                        List<FASecondRoundPositions> lstnewFaseconroudnposition = new List<FASecondRoundPositions>();
                                        FASecondRoundPositions objFaposition;
                                        foreach (var singleitem in fasecondroundpositionsOld)
                                        {
                                            objFaposition = new FASecondRoundPositions();
                                            objFaposition.key = singleitem.Key;
                                            objFaposition.Value = singleitem.Value;
                                            lstnewFaseconroudnposition.Add(objFaposition);
                                        }
                                        lst.FASecondRoundPositions = lstnewFaseconroudnposition;
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
                                var currentbiddetail = GlobalSettings.CurrentBidDetails;
                                AppDataBidFileNames appDataBidFileNames = GlobalSettings.UserInfo.AppDataBidFiles.FirstOrDefault(x => x.Domicile == currentbiddetail.Domicile && x.Position == currentbiddetail.Postion && x.Round == currentbiddetail.Round && x.Month == currentbiddetail.Month && x.Year == currentbiddetail.Year);
                                if (appDataBidFileNames == null)
                                {
                                    GlobalSettings.UserInfo.AppDataBidFiles.Add(new AppDataBidFileNames
                                    {
                                        Domicile = currentbiddetail.Domicile,
                                        Month = currentbiddetail.Month,
                                        Position = currentbiddetail.Postion,
                                        Round = currentbiddetail.Round,
                                        Year = currentbiddetail.Year,
                                        lstBidFileNames = new List<BidFileNames>() { new BidFileNames { FileName = newFileName + ".WBL", FileType = (int)BidFileType.NormalLine } }
                                    });

                                    WBidHelper.SaveUserFile(GlobalSettings.UserInfo, WBidHelper.WBidUserFilePath);
                                }
                                File.Delete(filePathOldWBL);
                            }
                            catch (Exception ex1)
                            {
                                throw ex1;
                            }

                        }

                    }



                    GlobalSettings.CompanyVA = GlobalSettings.WBidStateCollection.CompanyVA;
                    if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
                    {
                        if (GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0)
                            GlobalSettings.UserInfo.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount;
                        else
                            GlobalSettings.UserInfo.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber;
                    }

                    WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                    if (wBIdStateContent.Weights.NormalizeDaysOff == null)
                    {
                        wBIdStateContent.Weights.NormalizeDaysOff = new Wt2Parameter() { Type = 1, Weight = 0 };

                    }
                    if (wBIdStateContent.CxWtState.NormalizeDays == null)
                    {
                        wBIdStateContent.CxWtState.NormalizeDays = new StateStatus() { Cx = false, Wt = false };

                    }
                    if (wBIdStateContent.Constraints.Rest.ThirdcellValue == "")
                        wBIdStateContent.Constraints.Rest.ThirdcellValue = "1";

                    if (wBIdStateContent.Constraints.PerDiem.Value == 300)
                        wBIdStateContent.Constraints.PerDiem.Value = 18000;
                    if (wBIdStateContent.CxWtState.CLAuto == null)
                        wBIdStateContent.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
                    if (wBIdStateContent.CxWtState.CitiesLegs == null)
                        wBIdStateContent.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
                    if (wBIdStateContent.Constraints.StartDayOftheWeek == null)
                    {
                        //Need to add secondcellValue
                        //wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue = "1";
                        //foreach (var item in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters)
                        //{
                        //	if (item.SecondcellValue == null)
                        //	{
                        //		item.SecondcellValue = "1";
                        //	}
                        //}
                    }
                    if (wBIdStateContent.CxWtState.ETOPS == null)
                        wBIdStateContent.CxWtState.ETOPS = new StateStatus { Cx = false, Wt = false };
                   
                    if (wBIdStateContent.Theme == null)
                        wBIdStateContent.Theme = new Theme { FontSize = "", FontType = "" };
                    WBidState state = new WBidState(wBIdStateContent);



                    //if (wBIdStateContent.MenuBarButtonState.IsMIL && GlobalSettings.WBidINIContent.User.MIL)
                    //{
                    //	MILData milData;
                    //	if (File.Exists(WBidHelper.MILFilePath))
                    //	{
                    //		using (FileStream milStream = File.OpenRead(WBidHelper.MILFilePath))
                    //		{
                    //			MILData milDataobject = new MILData();
                    //			milData = ProtoSerailizer.DeSerializeObject(WBidHelper.MILFilePath, milDataobject, milStream);
                    //		}

                    //		GlobalSettings.MILData = milData.MILValue;
                    //		GlobalSettings.MILDates = WBidCollection.GenarateOrderedMILDates(wBIdStateContent.MILDateList);
                    //	}

                    //}
                    //if (wBIdStateContent.TagDetails != null)
                    //{
                    //	GlobalSettings.TagDetails = new TagDetails();
                    //	wBIdStateContent.TagDetails.ForEach(x => GlobalSettings.TagDetails.Add(new Tag { Line = x.Line, Content = x.Content }));
                    //}
                    //}
                    GlobalSettings.MenuBarButtonStatus = wBIdStateContent.MenuBarButtonState;
                    GlobalSettings.IsVacationCorrection = wBIdStateContent.IsVacationOverlapOverlapCorrection;
                    //GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
                    GlobalSettings.OrderedVacationDays = new List<Absense>();
                    GlobalSettings.TempOrderedVacationDays = new List<Absense>();

                    WBidHelper.GenerateDynamicOverNightCitiesList();
                    GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList();
                    if (GlobalSettings.WBidStateCollection.Vacation != null)
                    {

                        if (GlobalSettings.WBidStateCollection.Vacation.Count > 0)
                        {

                            GlobalSettings.SeniorityListMember = GlobalSettings.SeniorityListMember ?? new SeniorityListMember();
                            GlobalSettings.SeniorityListMember.Absences = new List<Absense>();
                            GlobalSettings.WBidStateCollection.Vacation.ForEach(x => GlobalSettings.SeniorityListMember.Absences.Add(new Absense
                            {
                                AbsenceType = "VA",
                                StartAbsenceDate = Convert.ToDateTime(x.StartDate),
                                EndAbsenceDate = Convert.ToDateTime(x.EndDate)
                            }));
                            GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
                            GlobalSettings.TempOrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
                        }
                    }





                    List<int> lstint = new List<int>() { 1, 2, 3 };

                    StateManagement statemanagement = new StateManagement();



                    WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);


                    GlobalSettings.IsFVVacation = (GlobalSettings.WBidStateCollection.FVVacation != null && GlobalSettings.WBidStateCollection.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

                    GlobalSettings.FVVacation = GlobalSettings.WBidStateCollection.FVVacation;


                    if (wBidStateContent != null)
                    {
                        if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (wBidStateContent.FAEOMStartDate != null && wBidStateContent.FAEOMStartDate != DateTime.MinValue && wBidStateContent.FAEOMStartDate != DateTime.MinValue.ToUniversalTime()))
                            GlobalSettings.FAEOMStartDate = wBidStateContent.FAEOMStartDate;
                        else
                            GlobalSettings.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();

                        try
                        {
                            WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
                        }
                        catch (Exception ex)
                        {
                            //WBidHelper.AddDetailsToMailCrashLogs(ex);
                        }

                        foreach (Line line in GlobalSettings.Lines)
                        {
                            line.ConstraintPoints = new ConstraintPoints();
                            line.WeightPoints = new WeightPoints();
                        }
                        //if (GlobalSettings.IsFVVacation)
                        //{
                        //	FVVacation objvac = new FVVacation();
                        //	GlobalSettings.Lines = new ObservableCollection<Line>(objvac.SetFVVacationValuesForAllLines(GlobalSettings.Lines.ToList()));

                        //	//wBidStateContent.MenuBarButtonState.IsVacationCorrection = true;
                        //}
                        ////statemanagement.RecalculateLineProperties(wBidStateContent);
                        //if (GlobalSettings.WBidINIContent.RatioValues != null)
                        //{
                        //	SetRatioValues(wBidStateContent);
                        //}
                        //Setting Button status to Global variables
                        statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
                        //Setting  status to Global variables
                        statemanagement.SetVacationOrOverlapExists(wBidStateContent);
                        //St the line order based on the state file.
                        statemanagement.ReloadStateContent(wBidStateContent);

                        //SetManualMovementShadowForLines(wBidStateContent);
                        //SetBidAutoGroupNumberFromStateFile(wBidStateContent);
                    }

                    InvokeOnMainThread(() =>
                    {
                        loadingOverlay.Hide();

                        this.NavigationController.NavigationBar.TintColor = UIColor.Blue;
                        NSNotificationCenter.DefaultCenter.RemoveObserver(newNotif);
                        newNotif = null;

                        //Need to redirect login view
                        UIStoryboard ObjStoryboard = UIStoryboard.FromName("Main", null);
                        //LoginViewController loginView = ObjStoryboard.InstantiateViewController("LoginViewController") as LoginViewController;

                        GlobalSettings.DownloadBidDetails = new BidDetails
                        {
                            Domicile= GlobalSettings.CurrentBidDetails.Domicile,
                            Postion= GlobalSettings.CurrentBidDetails.Postion,
                            Year= GlobalSettings.CurrentBidDetails.Year,
                            Month= GlobalSettings.CurrentBidDetails.Month,
                            Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B",
                            BidPeriodEndDate= GlobalSettings.CurrentBidDetails.BidPeriodEndDate,
                            BidPeriodStartDate= GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
                            Equipment= GlobalSettings.CurrentBidDetails.Equipment
                            
                        };

                        //string logText = string.Format("Get {0}-{1}-{2}", GlobalSettings.DownloadBidDetails.Domicile, GlobalSettings.DownloadBidDetails.Postion, GlobalSettings.DownloadBidDetails.Round);

                        //loginView.loginTitle = logText;
                        //loginView.isRecentBidDownload = true;
                        //this.NavigationController.PushViewController(loginView, true);
                        AuthorizationTestCaseViewController ConstraintsViewController = ObjStoryboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
                        ConstraintsViewController.buttonTitle = Constants.GO_TO_CONSTRAINTS;
                        ConstraintsViewController.messageError = Constants.ErrorMessages.ElementAt((int)AuthStaus.Filters - 1);
                        ConstraintsViewController.topBarTitle = Enumerable.ElementAt(Constants.listTitleTopBar, (int)AuthStaus.Filters - 1);
                        ConstraintsViewController.numberRow = (int)AuthStaus.Filters;
                        this.NavigationController.PushViewController(ConstraintsViewController, true);

                    });

                });


            }
            catch (Exception ex)
            {


                InvokeOnMainThread(() =>
                {
                    loadingOverlay.Hide();


                    this.NavigationController.NavigationBar.TintColor = UIColor.Blue;
                    throw ex;
                });

            }



        }
        private void DeleteBidPeriod()
        {
            try
            {
                //domicle==bwi
                //position==Cp
                //round=D
                //bidperiod=1

                //string message = "Are you sure you want to delete the" + currentOpenBid + " WBid data files \nfor " + SelectedDomicile.DomicileName + " " + SelectedPosition.LongStr + " " + SelectedEquipment.EquipmentNumber.ToString() + " ";
                // message += SelectedBidRound.RoundDescription + " for " + SelectedBidPeriod.Period + " ?";

                //if (MessageBox.Show(message, "WBidMax", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.Yes)
                // {
                foreach (var deleteFileIndex in deleteIndexList)
                {
                    RecentFile fileTodelete = recentFiles[deleteFileIndex];
                    string domcile = fileTodelete.Domcile;
                    string position=fileTodelete.Position;
                    string round=fileTodelete.Round;
                    int bidperiod=fileTodelete.Month;
                    int year=Convert.ToInt32(fileTodelete.Year);

                    string bidround = (round == "1st Round" ? "M" : "S");
                    AppDataBidFileNames appbiddata = GlobalSettings.UserInfo.AppDataBidFiles.FirstOrDefault(x => x.Year == year && x.Month == bidperiod && x.Domicile == domcile && x.Position == position && x.Round == bidround);
                    if (appbiddata != null)
                    {
                        var bidfiles = appbiddata.lstBidFileNames;
                        if (bidfiles != null && bidfiles.Count > 0)
                        {
                            foreach (var item in bidfiles)
                            {
                                var file = WBidHelper.GetAppDataPath() + "/" + item.FileName;
                                if (File.Exists(file))
                                {
                                    File.Delete(file);
                                }
                            }
                        }
                        GlobalSettings.UserInfo.AppDataBidFiles.Remove(appbiddata);

                    }

                    //Delete Old formatted files
                    string fileName = domcile + position + bidperiod.ToString("d2") + (year - 2000) + (round == "1st Round" ? "M" : "S") ;
                    string CmtfileName = domcile + position + bidperiod.ToString("d2") + (year - 2000) + (round == "1st Round" ? "M" : "S") + "Cmt.COM";

                    string folderName = WBidCollection.GetPositions().FirstOrDefault(x => x.LongStr == fileName.Substring(3, 2)).ShortStr + (round == "1st Round" ? "D" : "B") + fileName.Substring(0, 3) + WBidCollection.GetBidPeriods().FirstOrDefault(x => x.BidPeriodId == bidperiod).HexaValue;
                    //Delete WBL file
                    if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBL"))
                    {
                        File.Delete(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBL");

                    }

                    //Delete WBP file
                    if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBP"))
                    {
                        File.Delete(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBP");

                    }

                    //Delete WBS file
                    if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS"))
                    {
                        File.Delete(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");

                    }

                    //Delete VAC file
                    if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileName + ".VAC"))
                    {
                        File.Delete(WBidHelper.GetAppDataPath() + "/" + fileName + ".VAC");

                    }
                    //delete the folder.
                    if (Directory.Exists(WBidHelper.GetAppDataPath() + "/" + folderName))
                    {
                        Directory.Delete(WBidHelper.GetAppDataPath() + "/" + folderName, true);
                    }

                    if (File.Exists(WBidHelper.GetAppDataPath() + "/" + CmtfileName))
                    {
                        File.Delete(WBidHelper.GetAppDataPath() + "/" + CmtfileName);
                    }
                    //NSNotificationCenter.DefaultCenter.PostNotificationName("HandleReload", null);

                    deletedBidIndexList.Add(deleteFileIndex);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }
}
