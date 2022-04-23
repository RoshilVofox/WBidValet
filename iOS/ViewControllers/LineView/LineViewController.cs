#region NameSpace

using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using Bidvalet.Model;
using System.Linq;
using Bidvalet.Business.BusinessLogic;
using Bidvalet.Business;
using CoreGraphics;
using Bidvalet.iOS.Utility;
using Bidvalet.iOS.ViewControllers;
using System.Globalization;
using System.IO;
using Bidvalet.iOS.ViewControllers.VacationDifference;


#endregion
namespace Bidvalet.iOS
{
    public class LineViewController : UITableViewController
    {
        #region Variables
        //public UINavigationController Objnav;
        private UIActionSheet _actionSheet;
        NSObject _showLineProperty;
        NSObject _nsShowCalender;
        NSObject _nsObserver;
        NSObject _nsObserverSelectedLine;
        NSNumber _selectedRowNumber;
        LoadingOverlay _loadingOverlay;
        UIButton btnVacCorrect;
        List<NSObject> arrObserver = new List<NSObject>();
        UIButton btnEOM;
        bool FirstTime;
        string[] arr;
        // List<int> _selectedLines; 
        #endregion

        public LineViewController()
            : base(UITableViewStyle.Plain)
        {
        }

        #region Events

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            var columndefinition = GlobalSettings.ColumnDefinition;
            TableView.Source = new LineViewControllerSource(columndefinition, this);

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            //Show Month to Month vacation alert
            

        }

        private void ShowMonthToMonthVacationAlert()
        {
            if (GlobalSettings.CurrentBidDetails.Postion != "FA" && GlobalSettings.iSNeedToShowMonthtoMonthAlert == true)
            {
                List<Weekday> lstweekdays = GetWeekDays(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);
                List<Vacation> uservacation = GlobalSettings.WBidStateCollection.Vacation;
                List<Weekday> vacationweeks = lstweekdays.Where(x => uservacation.Any(y => DateTime.Parse(y.StartDate) == x.StartDate && DateTime.Parse(y.EndDate) == x.EndDate)).ToList();
                bool isneedtoShowAlert = vacationweeks.Any(x => x.Code.Contains("A") || x.Code.Contains("E"));
                if (isneedtoShowAlert == true)
                {
                    var startDateA = "";
                    var endDateA = "";
                    var startDateE = "";
                    var endDateE = "";

                    string AlertText = string.Empty;
                    var codeArray = vacationweeks.Select(x => x.Code);

                    if (codeArray.Contains("A") && codeArray.Contains("E"))
                    {
                        //AE Vacation
                        startDateA = vacationweeks.Find(x => x.Code == "A").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "A").StartDate.ToString("MMM");
                        endDateA = vacationweeks.Find(x => x.Code == "A").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "A").EndDate.ToString("MMM");

                        startDateE = vacationweeks.Find(x => x.Code == "E").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "E").StartDate.ToString("MMM");
                        endDateE = vacationweeks.Find(x => x.Code == "E").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "E").EndDate.ToString("MMM");

                        AlertText = "You have  'A & E' week vacation: \n\n" + startDateA + " - " + endDateA + " and " + startDateE + " - " + endDateE;
                        AlertText += "\n\nA weeks generally are the lead out month and E weeks generally are the lead-in month of a month-to-month vacation..";
                        AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";
                    }
                    else if (codeArray.Contains("A"))
                    {
                        //A Vacation
                        startDateA = vacationweeks.Find(x => x.Code == "A").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "A").StartDate.ToString("MMM");
                        endDateA = vacationweeks.Find(x => x.Code == "A").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "A").EndDate.ToString("MMM");
                        AlertText = "You have  'A' week vacation: \n\n" + startDateA + " - " + endDateA;
                        AlertText += "\n\nA weeks generally are the lead out month of a month-to - month vacation.";
                        AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";

                    }
                    else if (codeArray.Contains("E"))
                    {
                        //E Vacation
                        startDateE = vacationweeks.Find(x => x.Code == "E").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "E").StartDate.ToString("MMM");
                        endDateE = vacationweeks.Find(x => x.Code == "E").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "E").EndDate.ToString("MMM");
                        AlertText = "You have  'E' week vacation: \n\n" + startDateE + " - " + endDateE;
                        AlertText += "\n\nE weeks generally are the lead-in month of a month-to-month vacation..";
                        AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";
                    }

                    AlertText += "\n\nWe suggest you read the following documents to improve your bidding knowledge";

                    ShowMonthToMonthAlerView(AlertText);
                }
            }
        }
        private void ShowMonthToMonthAlerView(string AlertText)
        {
            MonthToMonthAlertView monthtomonthalert = new MonthToMonthAlertView();
            monthtomonthalert.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            UINavigationController nav = new UINavigationController(monthtomonthalert);
            monthtomonthalert.alert = AlertText;
            nav.NavigationBarHidden = true;
            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(nav, true, null);
        }

        public void LoadTableView()
        {
            GlobalSettings.SelectedLines = new List<int>();
            //nsObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("showActionSheet"), ShowActionSheet);
            //nsObserverSelectedLine = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("BidListRowSelected"), BidListRowSelected);
            //nsShowCalender = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowCalender"), ShowCalenderFunction);
            NavigationController.SetNavigationBarHidden(false, false);
            Title = "Bid List - " + GlobalSettings.Lines.Count + " Lines";
            // Register the TableView's data source
            TableView.SetEditing(true, true);

            var backButton = new UIBarButtonItem();
            var icoFontAttribute = new UITextAttributes { Font = UIFont.BoldSystemFontOfSize(18) };

            //icoFontAttribute.TextColor = UIColor.Blue;
            backButton.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
            backButton.Title = "< Back";

            backButton.Style = UIBarButtonItemStyle.Plain;
            backButton.Clicked += (sender, args) =>
            {
                //foreach (NSObject obj in arrObserver)
                //{
                //    NSNotificationCenter.DefaultCenter.RemoveObserver(obj);
                //}
                //StateManagement stateManagement = new StateManagement();
                //stateManagement.UpdateWBidStateContent();
                //WBidHelper.PushToUndoStack();
                if (NavigationController != null)
                {

                    NavigationController.PopViewController(true);
                }
            };
            NavigationItem.LeftBarButtonItem = backButton;

            var columndefinition = (List<ColumnDefinition>)XmlHelper.DeserializeFromXml<ColumnDefinitions>(WBidHelper.GetWBidColumnDefinitionFilePath());
            //TableView.Style = UITableViewStyle.Plain;

            TableView.Source = new LineViewControllerSource(columndefinition, this);
        }

        public override void ViewDidLoad()
       {
            base.ViewDidLoad();

            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = UIColor.White;
            this.NavigationItem.StandardAppearance = appearance;
            this.NavigationItem.ScrollEdgeAppearance = this.NavigationItem.StandardAppearance;

            //Submit Button 
            var bidStuffBtn = new UIBarButtonItem();
            var icoFontAttribute = new UITextAttributes { Font = UIFont.BoldSystemFontOfSize(16) };
            //observeNotification();
            //icoFontAttribute.TextColor = UIColor.Blue;
            bidStuffBtn.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);


            string dynamicTitle = "Bid Stuff";

            bidStuffBtn.Title = dynamicTitle;

            bidStuffBtn.Style = UIBarButtonItemStyle.Plain;
            bidStuffBtn.Clicked += (sender, args) =>
            {
                
                //if (GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
                //{
                //    arr = new string[]{
                //    "Redownload Bid Data"
                //    };
                //}
                //else
                //{
                    arr = new string[] {
                            "Submit Bid",
                            // "Redownload Vacation"
                            "Redownload Bid Data",
                            };
                //}
                try
                {




                    UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
                    CGRect senderframe = bidStuffBtn.AccessibilityFrame;
                    senderframe.X = bidStuffBtn.AccessibilityFrame.GetMidX();
                    sheet.ShowFrom(bidStuffBtn, true);
                    
                    sheet.Dismissed += handleBidStuffTap;

                    //_loadingOverlay = new LoadingOverlay(View.Bounds, "Loading...");

                    //View.AddSubview(_loadingOverlay);
                    //InvokeInBackground(() =>
                    //{

                    //    UpdateWBidStateContent();
                    //    string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
                    //    WBidHelper.SaveStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                    //    InvokeOnMainThread(() =>
                    //    {
                    //        _loadingOverlay.Hide();
                    //        NavigateToNextView();
                    //    });
                    //});

                }
                catch (Exception)
                {
                    //  this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                    if (_loadingOverlay != null)
                    {
                        _loadingOverlay.Hide();
                    }


                }

            };
            NavigationItem.RightBarButtonItem = bidStuffBtn;
            LoadTableView();
            ShowMonthToMonthVacationAlert();
            
        }
        public void handleBidStuffTap(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                try
                {
                    if (GlobalSettings.WBidStateCollection.DataSource != "HistoricalData")
                    {
                        _loadingOverlay = new LoadingOverlay(View.Bounds, "Loading...");

                        View.AddSubview(_loadingOverlay);
                        InvokeInBackground(() =>
                        {

                            UpdateWBidStateContent();
                            string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
                            WBidHelper.SaveStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                            InvokeOnMainThread(() =>
                            {
                                _loadingOverlay.Hide();
                                NavigateToNextView();
                            });
                        });
                    }
                    else
                    {
                        UIAlertController alert = UIAlertController.Create("WBidMax", "You cannot submit historic bid data", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                        {
                            
                        }));
                        this.PresentViewController(alert, true, null);
                    }
                }
                catch (Exception ex)
                {
                    if (_loadingOverlay != null)
                    {
                        _loadingOverlay.Hide();
                    }
                }
            }
            else if (e.ButtonIndex==1)
            {
                //RedownloadVacation();
                RedownloadBidData();

            }
            



            UIActionSheet obj = (UIActionSheet)sender;
            obj.Dispose();
        }
        private void DisplayVacationDifferenceData()
        {
            VacationValueDifferenceInputDTO input = new VacationValueDifferenceInputDTO();



            input.BidDetails = new UserBidDetails();
            input.BidDetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
            input.BidDetails.Position = GlobalSettings.CurrentBidDetails.Postion;
            input.BidDetails.Round = GlobalSettings.CurrentBidDetails.Round == "M" ? 1 : 2;
            input.BidDetails.Year = GlobalSettings.CurrentBidDetails.Year;
            input.BidDetails.Month = GlobalSettings.CurrentBidDetails.Month;


            input.IsDrop = true;
            input.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
            input.IsVAC = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
            input.FAEOMStartDate = GlobalSettings.FAEOMStartDate.Date.Day;
            input.FromApp = 3;
            input.lstVacation = new List<VacationInfo>();


            var vavacation = GlobalSettings.WBidStateCollection.Vacation;
            if (vavacation != null && vavacation.Count > 0)
            {
                foreach (var item in vavacation)
                {

                    var startdate = DateTime.Parse(item.StartDate, CultureInfo.InvariantCulture);
                    var enddate = DateTime.Parse(item.EndDate, CultureInfo.InvariantCulture);
                    var vacationstring = startdate.Month + "/" + startdate.Day + "-" + enddate.Month + "/" + enddate.Day;
                    input.lstVacation.Add(new VacationInfo { Type = "VA", VacDate = vacationstring });

                }
            }
            var Fvvavacation = GlobalSettings.WBidStateCollection.FVVacation;
            if (Fvvavacation != null && Fvvavacation.Count > 0)
            {
                foreach (var item in Fvvavacation)
                {
                    var vacationstring = item.StartAbsenceDate.Month + "/" + item.StartAbsenceDate.Day + "-" + item.EndAbsenceDate.Month + "/" + item.EndAbsenceDate.Day;
                    input.lstVacation.Add(new VacationInfo { Type = item.AbsenceType, VacDate = vacationstring });
                }
            }

            var jsonData = SerializeHelper.JsonObjectToStringSerializerMethod<VacationValueDifferenceInputDTO>(input);
            StreamReader dr = ServiceUtility.GetRestData("GetVacationDifferenceData", jsonData);
            var biddataresponse = SerializeHelper.ConvertJSonStringToObject<List<VacationValueDifferenceOutputDTO>>(dr.ReadToEnd()).FirstOrDefault();

            if (biddataresponse!=null && biddataresponse.lstFlightDataChangeVacValues.Count >0)
            {
               
                var objvacdiff = new VacationDifferenceViewController();
                objvacdiff.lstFlightDataChangevalues = biddataresponse.lstFlightDataChangeVacValues;
                NavigationController.PushViewController(objvacdiff, true);
            }
            else
            {
                InvokeOnMainThread(() => {
                    //ActivityIndicator.Hide();
                    string message = string.Empty;
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        message = "There are no differences in pay for your vacation with the new Flight Data.";
                    }
                    else
                    {
                        message = "There are no differences in pay with the new Flight Data. But if you have vacation, please turn ON vacation and check the vacation difference";
                    }
                    UIAlertController okAlertController = UIAlertController.Create("WBidMax", message, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => {

                        this.DismissViewController(true, null);
                    }));
                    this.PresentViewController(okAlertController, true, null);
                });
            }



        }
        public void observeNotification()
        {
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("DataReload"), handleDataReload));

        }
        public void handleDataReload(NSNotification n)
        {

            TableView.ReloadData();


        }
        public override void ViewWillAppear(bool animated)
        {
            _nsObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("showActionSheet"), ShowActionSheet);
            _nsObserverSelectedLine = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("BidListRowSelected"), BidListRowSelected);
            _nsShowCalender = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowCalender"), ShowCalenderFunction);
            _showLineProperty = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("ShowLineProperty"), ShowLinePropertyFunction);
        }

        public override void ViewWillDisappear(bool animated)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(_nsObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_nsObserverSelectedLine);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_nsShowCalender);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_showLineProperty);
        }
        #endregion

        #region Private Methods
        public void UpdateWBidStateContent()
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

                    // if (currentsortmethod != "SelectedColumn")
                    //  {
                    wBidStateContent.SortDetails.SortColumn = "Line";

                    //}
                    //// Set the status of the Menu bar check box to the state file.
                    //SetCheckBoxStateToStateFile();

                    //if (FAEOMStartDate != null)
                    //    GlobalSettings.WBidStateContent.FAEOMStartDate = FAEOMStartDate;
                    //else
                    wBidStateContent.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
                }
            }
            //if (GlobalSettings.WBidStateContent != null)
            //{
            //    //save the main window size
            //    GlobalSettings.WBidStateContent.MainWindowSize.Height = WindowHeight;
            //    GlobalSettings.WBidStateContent.MainWindowSize.Width = WindowWidth;
            //    GlobalSettings.WBidStateContent.MainWindowSize.Left = WindowLeft;
            //    GlobalSettings.WBidStateContent.MainWindowSize.Top = WindowTop;
            //    GlobalSettings.WBidStateContent.MainWindowSize.IsMaximised = (WindowState == System.Windows.WindowState.Maximized) ? true : false;
            //}

            // IsStateModified = false;

        }


        private void NavigateToNextView()
        {
            UIStoryboard Storyboard = UIStoryboard.FromName("Main", null);
            if (GlobalSettings.CurrentBidDetails.Postion == "CP")
            {
                var bidChoicesView = Storyboard.InstantiateViewController("BidChoicesViewController") as BidChoicesViewController;
                NavigationController.PushViewController(bidChoicesView, true);
            }
            else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
            {
                var bidAvoidanceiew = Storyboard.InstantiateViewController("PositionsAvoidanceViewController") as PositionsAvoidanceViewController;
                NavigationController.PushViewController(bidAvoidanceiew, true);
            }
            else if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
            {
                var positionPriorityView = Storyboard.InstantiateViewController("PostionsPrioritiesViewController") as PostionsPrioritiesViewController;
                NavigationController.PushViewController(positionPriorityView, true);
            }
            else if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round != "M")
            {
                var bidChoicesView = Storyboard.InstantiateViewController("BidChoicesViewController") as BidChoicesViewController;
                NavigationController.PushViewController(bidChoicesView, true);
            }
        }


        public void SrollDown()
        {
            NSIndexPath[] arrVisibleIndex = TableView.IndexPathsForVisibleRows;

            if (arrVisibleIndex.Length > 0)
            {
                NSIndexPath nextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 50, 0);
                if (nextIndex.Row >= GlobalSettings.Lines.Count)
                {
                    nextIndex = NSIndexPath.FromRowSection(TableView.NumberOfRowsInSection(0) - 1, 0);
                    //	this.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
                }
                //else
                TableView.ScrollToRow(nextIndex, UITableViewScrollPosition.Top, true);
            }
        }

        public void SingleTapSrollDown()
        {
            NSIndexPath[] arrVisibleIndex = TableView.IndexPathsForVisibleRows;

            if (arrVisibleIndex.Length > 0)
            {
                NSIndexPath nextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 50, 0);
                if (nextIndex.Row >= GlobalSettings.Lines.Count)
                {
                    nextIndex = NSIndexPath.FromRowSection(TableView.NumberOfRowsInSection(0) - 1, 0);
                    //this.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
                }
                //else
                TableView.ScrollToRow(nextIndex, UITableViewScrollPosition.Top, true);
            }
        }

        public void DoubleTapSrollDown()
        {
            NSIndexPath[] arrVisibleIndex = TableView.IndexPathsForVisibleRows;

            if (arrVisibleIndex.Length > 0)
            {
                NSIndexPath nextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row + 100, 0);
                if (nextIndex.Row >= GlobalSettings.Lines.Count)
                {
                    nextIndex = NSIndexPath.FromRowSection(TableView.NumberOfRowsInSection(0) - 1, 0);
                    //this.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
                }
                //else
                TableView.ScrollToRow(nextIndex, UITableViewScrollPosition.Top, true);
            }
        }

        public void SingleTapUp()
        {
            NSIndexPath[] arrVisibleIndex = TableView.IndexPathsForVisibleRows;
            if (arrVisibleIndex.Length > 0)
            {
                NSIndexPath nextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 50, 0);
                if (nextIndex.Row < 0)
                {

                    nextIndex = NSIndexPath.FromRowSection(0, 0);
                    //this.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
                }
                //else
                TableView.ScrollToRow(nextIndex, UITableViewScrollPosition.Top, true);
            }
        }

        public void DoubleTapUp()
        {
            NSIndexPath[] arrVisibleIndex = TableView.IndexPathsForVisibleRows;
            if (arrVisibleIndex.Length > 0)
            {
                NSIndexPath nextIndex = NSIndexPath.FromRowSection(arrVisibleIndex[0].Row - 100, 0);
                if (nextIndex.Row < 0)
                {
                    nextIndex = NSIndexPath.FromRowSection(0, 0);
                    //this.TableView.ScrollToRow(NextIndex,UITableViewScrollPosition.Top, true);
                }
                //else
                TableView.ScrollToRow(nextIndex, UITableViewScrollPosition.Top, true);
            }
        }

        public void LongTapUp()
        {
            if (GlobalSettings.Lines.Count > 0)
            {
                NSIndexPath nextIndex = NSIndexPath.FromRowSection(0, 0);
                TableView.ScrollToRow(nextIndex, UITableViewScrollPosition.Top, true);

            }

        }

        public void LongTapDown()
        {
            if (GlobalSettings.Lines.Count > 0)
            {
                NSIndexPath nextIndex = NSIndexPath.FromRowSection(GlobalSettings.Lines.Count - 1, 0);
                TableView.ScrollToRow(nextIndex, UITableViewScrollPosition.Bottom, true);

            }

        }

        public void GoToLine(string line)
        {
            TableView.EndEditing(true);
            if (line.Trim() == string.Empty)
            {
                // this.View.EndEditing(true);
                DisplayAlertView(GlobalSettings.ApplicationName, "Please Enter a Line Number");
                //this.TableView.EndEditing(true);
                return;
            }
            int lineNo = Convert.ToInt32(line);
            var lineObj = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineNo);
            if (lineObj == null)
            {

                DisplayAlertView(GlobalSettings.ApplicationName, "Invalid Line Number");
                //this.NavigationItem.LeftBarButtonItem.s
                // this.TableView.EndEditing(true);
                return;
            }
            int index = GlobalSettings.Lines.IndexOf(lineObj);
            NSIndexPath lPath = NSIndexPath.FromRowSection(index, 0);
            //foreach (Line ln in GlobalSettings.Lines) {
            //    int index;
            //    if (ln.LineNum == lineNo) {
            //        index = GlobalSettings.Lines.IndexOf (ln);
            //        lPath = NSIndexPath.FromRowSection (index, 0);
            //    }
            //}
            if (lPath != null)
            {

                TableView.ScrollToRow(lPath, UITableViewScrollPosition.Top, true);


            }
        }


        private void ShowActionSheet(NSNotification ns)
        {
            _selectedRowNumber = (NSNumber)ns.Object;
            _actionSheet = new UIActionSheet("Options", null, "CANCEL", null, null);
            _actionSheet.AddButton("Move selected lines - above");
            _actionSheet.AddButton("Move selected lines - below");
            _actionSheet.AddButton("Deselect lines");
            _actionSheet.Clicked += HandleActionSheetEvent;
            _actionSheet.ShowInView(View);
        }

        private void HandleActionSheetEvent(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                _actionSheet.Dispose();
            }
            else
            {
                if (GlobalSettings.SelectedLines == null || GlobalSettings.SelectedLines.Count == 0)
                {
                    DisplayAlertView(GlobalSettings.ApplicationName, "Please select atleast one line.");
                }
                else
                {
                    if (e.ButtonIndex == 1)
                    {
                        bool isNeedToShowMessage = LineOperations.MoveSelectedLineAbove(GlobalSettings.SelectedLines, _selectedRowNumber.Int32Value);
                    }
                    else if (e.ButtonIndex == 2)
                    {
                        bool isNeedToShowMessage = LineOperations.MoveSelectedLineBelow(GlobalSettings.SelectedLines, _selectedRowNumber.Int32Value);
                    }
                    GlobalSettings.SelectedLines = new List<int>();
                    TableView.ReloadData();

                }
            }
        }


        private void BidListRowSelected(NSNotification ns)
        {
            var row = (NSNumber)ns.Object;
            if (!GlobalSettings.SelectedLines.Contains(row.Int32Value))
            {

                GlobalSettings.SelectedLines.Add(row.Int32Value);
            }
            else
            {
                GlobalSettings.SelectedLines.Remove(row.Int32Value);
            }
        }

        public void ShowCalenderFunction(NSNotification n)
        {
            int selectedLine = Int32.Parse(n.Object.ToString());
            var ObjLineProperty = new LineDetailedCalenderView();
            ObjLineProperty.ViewSelectedLine = selectedLine;
            //Objnav.PushViewController(ObjLineProperty, true);
            NavigationController.PushViewController(ObjLineProperty, true);

        }

        public void ShowLinePropertyFunction(NSNotification n)
        {
            LinePropertyListControllerController ObjLineProperty = new LinePropertyListControllerController();
            ObjLineProperty.lineView = this;// Added for passing lineview controller instance for reloading table from property popup

            UINavigationController Objnav = new UINavigationController(ObjLineProperty);


            this.PresentViewController(Objnav, true, null);



        }
        public void btnVacDifftap()
        {
            DisplayVacationDifferenceData();
        }
        public void btnVacCorrectTap(UIKit.UIButton sender, UIKit.UIButton btnEom)
        {
            try
            {
                btnEOM = btnEom;
                btnVacCorrect = sender;

                //==============New Code starts: Roshil
                string overlayTxt = "";
                bool IsFileAvailable = false;
                string status = string.Empty;
                sender.Selected = !sender.Selected;
                WBidHelper.PushToUndoStack();
                SetVACButton(sender);

                WBidCollection.GenarateTempAbsenceList();
                string oàerlayTxt = string.Empty;


                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                {
                    overlayTxt = "Applying Vacation Correction";

                    //btnVacDrop.Enabled = true;
                }
                else
                {
                    overlayTxt = "Removing Vacation Correction";

                }
                AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
                LoadingOverlay overlay = new LoadingOverlay(app.Window.Frame, overlayTxt);
                app.Window.Add(overlay);

                InvokeInBackground(() =>
                {
                    try
                    {
                        WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
                        //status = RetrieveAndSaveVACLineFiles(3);
                        IsFileAvailable = (status == "Ok") ? true : false;
                        if (IsFileAvailable)
                        {
                            StateManagement statemanagement = new StateManagement();

                            statemanagement.ApplyCSW(wBidStateContent);
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                                sender.Selected = !sender.Selected;
                                SetVACButton(sender);
                                overlay.Hide();
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        InvokeOnMainThread(() =>
                        {

                            throw ex;
                        });
                    }

                    InvokeOnMainThread(() =>
                    {
                        overlay.Hide();

                        BidListViewBL.GenerateBidListIconCollection();
                        //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                        //loadSummaryListAndHeader();
                        //Reload
                        //TableView.Source = new LineViewControllerSource(columndefinition, this);
                        TableView.ReloadData();




                    });
                });
                WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBidStateCont.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;

                //============== New Code ends




            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void SetVACButton(UIButton sender)
        {
            if (sender.Selected)
            {
                //vacation button selected.
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;
                //reload data
                //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                //TableView.ReloadData();

            }

            else
            {

                //vacation button unselected.
                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;

                if (GlobalSettings.MenuBarButtonStatus.IsEOM == false)
                {
                    //btnVacDrop.Selected = false;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    //reload data
                    //NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                    //TableView.ReloadData();
                }

            }

            SetVacButtonStates();
        }

        public void SetVacButtonStates()
        {

            // Configuring Modern view property lists.
            if (GlobalSettings.MenuBarButtonStatus == null)
                GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();


            if (GlobalSettings.IsVacationCorrection || GlobalSettings.IsFVVacation)
            {
                btnVacCorrect.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL);
                btnVacCorrect.BackgroundColor = UIColor.DarkGray;

            }
            else
            {
                btnVacCorrect.Enabled = false;
                btnVacCorrect.BackgroundColor = UIColor.LightGray;
            }



            btnEOM.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL && (GlobalSettings.CurrentBidDetails.Postion == "FA" || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3).DayOfWeek == 0));

            if (btnEOM.Enabled)
                btnEOM.BackgroundColor = UIColor.DarkGray;
            else
                btnEOM.BackgroundColor = UIColor.LightGray;
            /// btnVacDrop.Enabled = ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.OrderedVacationDays!=null&&  GlobalSettings.OrderedVacationDays.Count>0) || GlobalSettings.MenuBarButtonStatus.IsEOM);
            //btnVacDrop.Enabled = ((GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.IsVacationCorrection) || GlobalSettings.MenuBarButtonStatus.IsEOM);



            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            }


            btnEOM.Selected = GlobalSettings.MenuBarButtonStatus.IsEOM;
            btnVacCorrect.Selected = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;

            if (btnEOM.Selected)
                btnEOM.BackgroundColor = UIColor.SystemBlueColor;

            if (btnVacCorrect.Selected)
                btnVacCorrect.BackgroundColor = UIColor.SystemBlueColor;
            if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
            {
                if (GlobalSettings.WBidStateCollection != null)
                {
                    if (GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
                    {
                        btnEOM.Enabled = false;
                        btnVacCorrect.Enabled = false;
                    }
                    WBidState WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                }

            }
        }


        public void btnEOMTapped(UIKit.UIButton sender, UIKit.UIButton btnVAC)
        {
            //=================New code starts Roshil
            try
            {
                btnEOM = sender;
                btnVacCorrect = btnVAC;
                WBidHelper.PushToUndoStack();


                if (!sender.Selected)
                {
                    GlobalSettings.MenuBarButtonStatus.IsEOM = true;
                    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;

                    SetVacButtonStates();

                    if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                    {
                        DateTime defDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
                        defDate.AddMonths(1);
                        string[] strParams = {
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (1)),
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (2)),
                            String.Format ("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays (3))
                        };
                        UIActionSheet sheet = new UIActionSheet("Where does your vacation start on next month?", null, null, null, strParams);
                        CGRect senderframe = sender.Frame;
                        senderframe.X = sender.Frame.GetMidX();
                        sheet.ShowFrom(senderframe, this.View, true);
                        sheet.Clicked += handleEOMOptions;

                    }
                    else
                    {
                        WBidCollection.GenarateTempAbsenceList();
                        sender.Selected = true;
                        string overlayTxt = string.Empty;
                        if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                            overlayTxt = "Applying EOM";
                        else
                        {
                            //if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                            //{
                            //    btnVacDrop.Selected = false;
                            //    btnVacDrop.Enabled = false;
                            //    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                            //}
                            overlayTxt = "Removing EOM";
                        }



                        LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                        this.View.Add(overlay);
                        InvokeInBackground(() =>
                        {

                            try
                            {
                                WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                                var status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
                                if (status == "Ok")
                                {
                                    StateManagement statemanagement = new StateManagement();

                                    statemanagement.ApplyCSW(wBidStateContent);
                                }


                                InvokeOnMainThread(() =>
                                {
                                    if (status == "Ok")
                                    {

                                        // NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
                                        TableView.ReloadData();
                                        overlay.Hide();

                                    }
                                    else
                                    {
                                        overlay.Hide();
                                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                        this.PresentViewController(okAlertController, true, null);
                                        sender.Selected = false;
                                        GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                                        UnselectEOM();
                                    }
                                });
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
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM && GlobalSettings.CurrentBidDetails.Postion != "FA")
                    {

                        var eomstartdate = GetnextSunday();
                        List<Weekday> vacationweeks = new List<Weekday>();

                        vacationweeks.Add(new Weekday() { StartDate = eomstartdate, EndDate = eomstartdate.AddDays(6), Code = "EOM" });

                        var startDateEOM = "";
                        var endDateEOM = "";
                        string AlertText = string.Empty;

                        //EOM Vacation
                        startDateEOM = vacationweeks.Find(x => x.Code == "EOM").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "EOM").StartDate.ToString("MMM");
                        endDateEOM = vacationweeks.Find(x => x.Code == "EOM").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "EOM").EndDate.ToString("MMM");
                        AlertText = "You have an 'EOM'  vacation: \n\n" + startDateEOM + " - " + endDateEOM;
                        AlertText += "\n\nEOM weeks can affect the vacation pay in the current bid period and also the next month.";
                        AlertText += "\n\nWe have two documents regarding Month-to-Month vacations that also apply to EOM vacation weeks.";
                        AlertText += "\n\nWe suggest you read the following documents to improve your bidding knowledge";

                        //ShowMonthToMonthAlerView(AlertText);

                    }
                }
                else
                {

                    sender.Selected = false;
                    GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                    WBidCollection.GenarateTempAbsenceList();
                    UnselectEOM();

                    string overlayTxt = string.Empty;
                    if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                        overlayTxt = "Applying EOM";
                    else
                    {
                        //if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                        //{
                        //    btnVacDrop.Selected = false;
                        //    btnVacDrop.Enabled = false;
                        //    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                        //}
                        overlayTxt = "Removing EOM";
                    }

                    LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                    this.View.Add(overlay);

                    InvokeInBackground(() =>
                    {
                        string status = string.Empty;
                        try
                        {
                            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                            status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBidStateContent);
                            //status = RetrieveAndSaveVACLineFiles(3);
                            if (status == "Ok")
                            {
                                StateManagement statemanagement = new StateManagement();

                                statemanagement.ApplyCSW(wBidStateContent);
                            }
                        }
                        catch (Exception ex)
                        {
                            InvokeOnMainThread(() =>
                            {
                                throw ex;
                            });
                        }

                        InvokeOnMainThread(() =>
                        {
                            if (status == "Ok")
                            {
                                overlay.Hide();
                            }
                            else
                            {
                                overlay.Hide();
                                UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                                GlobalSettings.MenuBarButtonStatus.IsEOM = true;
                                sender.Selected = true;
                                UnselectEOM();
                            }

                        });
                    });
                }
                WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                wBIdStateContent.MenuBarButtonState.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;



                //=================New code ends

            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        public static DateTime GetnextSunday()
        {
            DateTime date = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
            for (int count = 1; count <= 3; count++)
            {
                date = date.AddDays(1);
                if (date.DayOfWeek.ToString() == "Sunday")
                    break;
            }


            return date;
        }



        private void UnselectEOM()
        {


            //if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
            //{
            //    btnVacDrop.Selected = false;
            //    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            //}
            //if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop == false)
            //{
            //    btnVacDrop.SetTitle("FLY", UIControlState.Normal);
            //    btnVacDrop.SetTitleColor(UIColor.White, UIControlState.Normal);
            //    this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonRed.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);


            //    this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            //}
            //else
            //{
            //    btnVacDrop.SetTitle("DRP", UIControlState.Selected);
            //    btnVacDrop.SetTitle("DRP", UIControlState.Normal);
            //    btnVacDrop.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
            //    this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("menuGreenNormal.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Normal);
            //    this.btnVacDrop.SetBackgroundImage(UIImage.FromBundle("activeButtonOrange.png").CreateResizableImage(new UIEdgeInsets(5, 5, 5, 5)), UIControlState.Selected);
            //}
            SetVacButtonStates();
        }


        void handleEOMOptions(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                btnEOM.Selected = true;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1);
            }
            else if (e.ButtonIndex == 1)
            {
                btnEOM.Selected = true;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2);

            }
            else if (e.ButtonIndex == 2)
            {
                btnEOM.Selected = true;
                GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3);

            }
            else
            {
                btnEOM.Selected = false;
                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                //if (!btnEOM.Selected && !btnVacCorrect.Selected)
                //{
                //   btnVacDrop.Enabled = false;
                //    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                //}

            }
            WBidCollection.GenarateTempAbsenceList();


            CreateEOMforFA();
        }
        private void CreateEOMforFA()
        {
            if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue)
            {
                //btnVacDrop.Enabled = true;
                string overlayTxt = string.Empty;
                if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                    overlayTxt = "Applying EOM";
                else
                {
                    //if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                    //{
                    //    btnVacDrop.Selected = false;
                    //    btnVacDrop.Enabled = false;
                    //    GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
                    //}
                    overlayTxt = "Removing EOM";
                }

                //RetrieveAndSaveVACLineFiles(3);
                LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
                this.View.Add(overlay);
                InvokeInBackground(() =>
                {
                    WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                    var status = WBidHelper.RetrieveSaveAndSetLineFiles(3, wBIdStateContent);

                    if (status == "Ok")
                    {
                        StateManagement statemanagement = new StateManagement();
                        WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        statemanagement.ApplyCSW(wBidStateContent);
                    }
                    InvokeOnMainThread(() =>
                    {
                        overlay.Hide();
                        if (status != "Ok")
                        {
                            overlay.Hide();
                            UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            this.PresentViewController(okAlertController, true, null);
                            btnEOM.Selected = false;
                            GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                            UnselectEOM();
                        }
                    });

                });
            }
        }

        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();
        }


        private List<Weekday> GetWeekDays(int year, int month)
        {

            List<Weekday> dates = new List<Weekday>();
            CultureInfo ci = new CultureInfo("en-US");

            for (int i = 1; i <= ci.Calendar.GetDaysInMonth(year, month); i++)
            {

                if (new DateTime(year, month, i).DayOfWeek == DayOfWeek.Saturday)
                {
                    dates.Add(new Weekday { Day = new DateTime(year, month, i).AddDays(-6).Day, StartDate = new DateTime(year, month, i).AddDays(-6).Date, EndDate = new DateTime(year, month, i) });
                }

            }
            //need to add one extra sunday 
            dates.Add(new Weekday { Day = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(7).Day, StartDate = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(7).Date, EndDate = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(13).Date });
            for (int i = 0; i < dates.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        dates[0].Code = "A";
                        break;
                    case 1:
                        dates[1].Code = "B";
                        break;
                    case 2:
                        dates[2].Code = "C";
                        break;
                    case 3:
                        dates[3].Code = "D";
                        break;
                    case 4:
                        dates[4].Code = "E";
                        break;
                    case 5:
                        dates[5].Code = "F";
                        break;
                }
            }
            return dates;

        }

        //Redownload Bid Data
        private void RedownloadBidData()
        {
            try
            {
                string appfolderpath = WBidHelper.GetAppDataPath();
                string status = string.Empty;
                string overlayTxt = string.Empty;
                WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                overlayTxt = "Redownloading Bid Data...";
                AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
                LoadingOverlay overlay = new LoadingOverlay(app.Window.Frame, overlayTxt);
                app.Window.Add(overlay);


                InvokeInBackground(() =>
                {
                    BidDataFileResponse bidDataResponse = new BidDataFileResponse();
                    //if (!GlobalSettings.IsHistorical)
                    //{
                        bidDataResponse = WBidHelper.RedownloadBidDataFromServer();
                        if (bidDataResponse.bidData != null && bidDataResponse.Status == true)
                        {
                            StateManagement statemanagement = new StateManagement();

                            statemanagement.ApplyCSW(wBidStateContent);
                            InvokeOnMainThread(() =>
                            {

                                UIAlertController okAlertController = UIAlertController.Create("Success !", "Bid Data redownloaded. Please check.", UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                                //ReloadData
                                BidListViewBL.GenerateBidListIconCollection();
                                TableView.ReloadData();
                                overlay.Hide();
                            });
                        }
                        else
                        {
                            string msg = (bidDataResponse != null && bidDataResponse.Message != null) ? bidDataResponse.Message : string.Empty;
                            if (msg == string.Empty)
                                msg = "Something went wrong. please try again or contact Administrator";
                            InvokeOnMainThread(() =>
                            {
                                overlay.Hide();
                                UIAlertController okAlertController = UIAlertController.Create("Error!", msg, UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);

                            });
                        }
                    //}
                });
            }
            catch (Exception ex)
            {

            }
        }

        //Redownload Vacation
        private void RedownloadVacation()
        {
            try
            {
                string status = string.Empty;
                string overlayTxt = string.Empty;
                WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                if (GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                {
                    overlayTxt = "Redownloading vacation...";
                    AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
                    LoadingOverlay overlay = new LoadingOverlay(app.Window.Frame, overlayTxt);
                    app.Window.Add(overlay);
                    InvokeInBackground(() =>
                    {
                        status = WBidHelper.RedownloadBidDataFileFromServer(wBidStateContent);
                        if (status == "Ok")
                        {
                            StateManagement statemanagement = new StateManagement();

                            statemanagement.ApplyCSW(wBidStateContent);

                            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                            {
                                RecalcalculateLineProperties objrecalculate = new RecalcalculateLineProperties();
                                objrecalculate.CalculateDropTemplateForBidLines(GlobalSettings.Lines);
                            }
                            InvokeOnMainThread(() =>
                            {
                                WBidHelper.RecalculateAMPMAndWekProperties(false);
                                //ReloadData
                                BidListViewBL.GenerateBidListIconCollection();
                                TableView.ReloadData();

                                overlay.Hide();
                            });
                        }
                        else
                        {


                            InvokeOnMainThread(() =>
                            {
                                overlay.Hide();
                                UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                                this.PresentViewController(okAlertController, true, null);
                                GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                                //UnselectEOM();
                                BidListViewBL.GenerateBidListIconCollection();
                                TableView.ReloadData();
                            });

                        }
                    });

                }
                else
                {
                    status = "You have no vacation.. Please check the senioity list!!!";

                    InvokeOnMainThread(() =>
                    {
                        UIAlertController okAlertController = UIAlertController.Create("WBidMax", status, UIAlertControllerStyle.Alert);
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                        this.PresentViewController(okAlertController, true, null);
                        GlobalSettings.MenuBarButtonStatus.IsEOM = false;
                    //UnselectEOM();
                    //NSNotificationCenter.DefaultCenter.PostNotificationName("setButtonStates", null);
                    BidListViewBL.GenerateBidListIconCollection();
                        TableView.ReloadData();
                    });

                }
            }
            catch (Exception ex)
            {
            }
        }


        #endregion
    }
}

