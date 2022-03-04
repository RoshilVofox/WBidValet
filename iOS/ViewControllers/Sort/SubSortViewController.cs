using System;

using UIKit;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Bidvalet.Model;
using Bidvalet.iOS.Utility;
using Bidvalet.Business;

namespace Bidvalet.iOS
{
    public partial class SubSortViewController : BaseViewController
    {
        public List<string> selectedSort = new List<string>();
        public bool isBlockSortSelected;
        public bool needTobyPassingScreen;
        public string titleSort = "";
        LoadingOverlay loadingOverlay;
        public SubSortViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

          
            // Perform any additional setup after loading the view, typically from a nib.
            Title = "Selected Sort";

            // If this correct, touch, touch "Calculate Bid List" , else go back and change your sort.

            //You can leave the Blank and Reserve lines mixed in with the other lines or you can place them at the bottom with the switches shown below

            lbCorrectBlock.Text = "If this correct, touch \"Calculate Bid List\" , else go back and change your sort.";

            SegBlankReservePriority.SelectedSegment = -1;
            if ((GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M") || (GlobalSettings.CurrentBidDetails.Postion != "FA" && GlobalSettings.CurrentBidDetails.Round == "S"))
            {
                ViewBlankAndReserve.Hidden = true;
            }
            else
            {
                string blockDesc = string.Empty;
                ViewBlankAndReserve.Hidden = false;
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    lblBlankBottom.Text = "Ready Reserve at bottom";
                    SegBlankReservePriority.SetTitle("Ready Reserve First", (nint)1);
                    blockDesc = "\n\nYou can leave the Ready Reserve and Reserve lines mixed in with the other lines or you can place them at the bottom with the switches shown below";
                }
                else
                {
                    lblBlankBottom.Text = "Blank at bottom";
                    blockDesc = "\n\nYou can leave the Blank and Reserve lines mixed in with the other lines or you can place them at the bottom with the switches shown below";

                }
                lbCorrectBlock.Text += blockDesc;

            }

            ShowNavigationBar();
            btnContinue.SetTitle("Calculate Bid List", UIControlState.Normal);
            UIHelpers.StyleForButtonsDeleteSortView(new UIButton[] { btnDelete });
            UIHelpers.StyleForButtons(new UIButton[] { btnContinue });
            btnDelete.Hidden = true;
            SwitchBlankAtBottom.On = false;
            SwitchReserveAtBottom.On = false;
            SegmentHandling();

            // ViewBlankAndReserve.Hidden = true;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
          

            if (!isBlockSortSelected)
            {
                viewBlockSort.Alpha = 1;
                lbCorrectBlock.Alpha = 1;
                lbCorrectBasic.Alpha = 1;
                btnDelete.Alpha = 1;
                lbSelectedSort.Text = "Standard Sort";
                if(selectedSort.Count>0)
                {
                    lbBlockSortFirst.Text = selectedSort[0];
                }
                else
                {
                    lbBlockSortFirst.Text = "";
                }
               
            }
            else
            {
                viewBlockSort.Alpha = 1;
                lbCorrectBlock.Alpha = 1;
                lbCorrectBasic.Alpha = 1;
                btnDelete.Alpha = 1;
                lbSelectedSort.Text = "Block Sort";
                if (!(selectedSort.Count > 0))
                {
                    lbBlockSortFirst.Text = "";
                    return;
                }

                string selectedStr = "";
                foreach (string item in selectedSort)
                {
                    if (selectedStr != string.Empty)
                        selectedStr += " , ";
                    selectedStr = selectedStr + item;// +"  , ";
                }
                //				int total = 7 - selectedSort.Count;
                //				if (total > 0) {
                //					for (int i=0;i<total;i++) selectedStr= selectedStr + "\n";
                //				}

                lbBlockSortFirst.Text = selectedStr;
            }
            if (needTobyPassingScreen == true)
            {
                CalculateBidViewController viewController = Storyboard.InstantiateViewController("CalculateBidViewController") as CalculateBidViewController;
                viewController.needTobyPassingScreen = true;
                PushViewController(viewController, false);
                needTobyPassingScreen = false;
                return;
            }
            //lbSelectedSort.Text = titleSort;
        }

        partial void OnContinueClickEvent(Foundation.NSObject sender)
        {
            //ShowActionSheet();

            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            if (SwitchBlankAtBottom.On || SwitchReserveAtBottom.On)
            {
                wBIdStateContent.BidAuto.IsBlankBottom = SwitchBlankAtBottom.On;
                wBIdStateContent.BidAuto.IsReserveBottom = SwitchReserveAtBottom.On;
                wBIdStateContent.BidAuto.IsReserveFirst = (SwitchBlankAtBottom.On && SwitchReserveAtBottom.On && SegBlankReservePriority.SelectedSegment == 0);

            }
            else
            {
                wBIdStateContent.BidAuto.IsBlankBottom = false;
                wBIdStateContent.BidAuto.IsReserveBottom = false;
                wBIdStateContent.BidAuto.IsReserveFirst = false;
            }

            CalculateBidViewController viewController = Storyboard.InstantiateViewController("CalculateBidViewController") as CalculateBidViewController;
            PushViewController(viewController, true);

            //CalculateBid();
            //if (GlobalSettings.CurrentBidDetails.Postion == "CP")
            //{
            //    BidChoicesViewController bidChoicesView = Storyboard.InstantiateViewController("BidChoicesViewController") as BidChoicesViewController;
            //   PushViewController (bidChoicesView,true);
            //}
            //else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
            //{
            //    PositionsAvoidanceViewController bidAvoidanceiew = Storyboard.InstantiateViewController("PositionsAvoidanceViewController") as PositionsAvoidanceViewController;
            //   PushViewController (bidAvoidanceiew,true);
            //}
            //else if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
            //{
            //    PostionsPrioritiesViewController positionPriorityView = Storyboard.InstantiateViewController("PostionsPrioritiesViewController") as PostionsPrioritiesViewController;
            //    PushViewController(positionPriorityView, true);
            //}
            //else if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round != "M")
            //{
            //    BidChoicesViewController bidChoicesView = Storyboard.InstantiateViewController("BidChoicesViewController") as BidChoicesViewController;
            //    PushViewController(bidChoicesView, true);
            //}
        }


        //private void CalculateBid()
        //{
        //    try
        //    {
        //        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(true, true);
        //        //this.progressCombo.Hidden = false;
        //        //lbCalculatingCombo.Hidden = false;

        //        loadingOverlay = new LoadingOverlay(this.View.Bounds, "Preparing Bid List...");
        //        this.View.AddSubview(loadingOverlay);
        //        InvokeInBackground(() =>
        //        {

        //            //StartProgress();

        //             GlobalSettings.Lines.ToList().ForEach(x =>
        //            {
        //                x.TopLock = false;
        //                x.BotLock = false;
        //                //x.WeightPoints.Reset();
        //                if (x.BAFilters != null)
        //                    x.BAFilters.Clear();
        //                x.BAGroup = string.Empty;
        //                x.IsGrpColorOn = 0;
        //            });


        //            BidAutomatorCalculations bidAutomatorCalculations = new BidAutomatorCalculations();
        //            bidAutomatorCalculations.CalculateLinePropertiesForBAFilters();
        //            //Apply COnstrint And Sort
        //            bidAutomatorCalculations.ApplyBAFilterAndSort();
        //            SetCurrentBADetailsToCalculateBAState();

        //            UpdateWBidStateContent();
        //            string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
        //            WBidHelper.SaveStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
        //            BidListViewBL.GenerateBidListIconCollection();
        //            InvokeOnMainThread(() =>
        //            {

        //                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
        //                loadingOverlay.Hide();


        //                LineViewController OBjLine = new LineViewController();
        //                this.NavigationController.PushViewController(OBjLine, true);


        //            });

        //        });
        //    }
        //    catch (Exception ex)
        //    {

        //        if (loadingOverlay != null)
        //            loadingOverlay.Hide();
        //        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
        //    }
        //}

        //public void UpdateWBidStateContent()
        //{
        //    //Save tag details to state file;


        //    if (GlobalSettings.Lines != null && GlobalSettings.Lines.Count > 0)
        //    {
        //        var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        //        wBidStateContent.TagDetails = new TagDetails();
        //        wBidStateContent.TagDetails.AddRange(GlobalSettings.Lines.ToList().Where(x => x.Tag != null && x.Tag.Trim() != string.Empty).Select(y => new Tag { Line = y.LineNum, Content = y.Tag }));

        //        int toplockcount = GlobalSettings.Lines.Where(x => x.TopLock == true).ToList().Count;
        //        int bottomlockcount = GlobalSettings.Lines.Where(x => x.BotLock == true).ToList().Count;
        //        //save the top and bottom lock
        //        wBidStateContent.TopLockCount = toplockcount;
        //        wBidStateContent.BottomLockCount = bottomlockcount;

        //        //Get the line oreder
        //        List<int> lineorderlist = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
        //        LineOrders lineOrders = new LineOrders();
        //        int count = 1;
        //        lineOrders.Orders = lineorderlist.Select(x => new LineOrder() { LId = x, OId = count++ }).ToList();
        //        lineOrders.Lines = lineorderlist.Count;
        //        wBidStateContent.LineOrders = lineOrders;

        //        //save the state of the Reserve line to botttom or blank line to bottom
        //        wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
        //        wBidStateContent.ForceLine.IsReverseLinetoBottom = false;


        //        if (wBidStateContent.SortDetails == null)
        //            wBidStateContent.SortDetails = new SortDetails();

        //        // if (currentsortmethod != "SelectedColumn")
        //        //  {
        //        wBidStateContent.SortDetails.SortColumn = "Line";

        //        //}
        //        //// Set the status of the Menu bar check box to the state file.
        //        //SetCheckBoxStateToStateFile();

        //        //if (FAEOMStartDate != null)
        //        //    GlobalSettings.WBidStateContent.FAEOMStartDate = FAEOMStartDate;
        //        //else
        //        wBidStateContent.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
        //    }
        //    //if (GlobalSettings.WBidStateContent != null)
        //    //{
        //    //    //save the main window size
        //    //    GlobalSettings.WBidStateContent.MainWindowSize.Height = WindowHeight;
        //    //    GlobalSettings.WBidStateContent.MainWindowSize.Width = WindowWidth;
        //    //    GlobalSettings.WBidStateContent.MainWindowSize.Left = WindowLeft;
        //    //    GlobalSettings.WBidStateContent.MainWindowSize.Top = WindowTop;
        //    //    GlobalSettings.WBidStateContent.MainWindowSize.IsMaximised = (WindowState == System.Windows.WindowState.Maximized) ? true : false;
        //    //}

        //    // IsStateModified = false;

        //}

        //private void SetCurrentBADetailsToCalculateBAState()
        //{
        //    try
        //    {
        //        var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

        //        if (wBidStateContent.BidAuto != null)
        //        {
        //            wBidStateContent.BidAutoOn = true;
        //            //wBidStateContent.BidAuto.BAWeight = new List<BidAutoItem>();
        //            //if (wBidStateContent.BidAuto.DailyCommuteTimes ==null)
        //            //{
        //            //    wBidStateContent.BidAuto.DailyCommuteTimes = new List<CommuteTime>();
        //            //}



        //            wBidStateContent.CalculatedBA = new BidAutomator
        //            {
        //                IsBlankBottom = wBidStateContent.BidAuto.IsBlankBottom,
        //                IsReserveBottom = wBidStateContent.BidAuto.IsReserveBottom,
        //                IsReserveFirst = wBidStateContent.BidAuto.IsReserveFirst
        //            };


        //            //wBidStateContent.CalculatedBA.BAWeight = new List<BidAutoItem>();

        //            //if (wBidStateContent.CalculatedBA.DailyCommuteTimes == null)
        //            //{
        //            //    wBidStateContent.CalculatedBA.DailyCommuteTimes = new List<CommuteTime>();
        //            //}
        //            //Ba filter
        //            //---------------------------------------------------------------------------
        //            if (wBidStateContent.BidAuto.BAFilter != null)
        //            {
        //                wBidStateContent.CalculatedBA.BAFilter = new List<BidAutoItem>();
        //                SetCurrentBAFilterDetailsToCalculateBAFilterState(wBidStateContent);

        //            }
        //            //---------------------------------------------------------------------------
        //            //Ba Group object
        //            //---------------------------------------------------------------------------
        //            if (wBidStateContent.BidAuto.BAGroup != null)
        //            {
        //                wBidStateContent.CalculatedBA.BAGroup = new List<BidAutoGroup>();
        //                SetCurrentBAGroupDetailsToCalculateBAGroupState(wBidStateContent);

        //                // GlobalSettings.WBidStateContent.BidAuto.BAGroup = null;
        //            }
        //            //---------------------------------------------------------------------------

        //            //Sort object
        //            //---------------------------------------------------------------------------
        //            if (wBidStateContent.BidAuto.BASort != null)
        //            {
        //                wBidStateContent.CalculatedBA.BASort = new SortDetails
        //                {
        //                    SortColumn = wBidStateContent.BidAuto.BASort.SortColumn,
        //                    SortColumnName = wBidStateContent.BidAuto.BASort.SortColumnName,
        //                    SortDirection = wBidStateContent.BidAuto.BASort.SortDirection
        //                };
        //                //Block sort list
        //                // if (GlobalSettings.WBidStateContent.CalculatedBA.BASort.BlokSort != null)
        //                if (wBidStateContent.BidAuto.BASort.BlokSort != null)
        //                {
        //                    wBidStateContent.CalculatedBA.BASort.BlokSort = new List<string>();
        //                    foreach (var item in wBidStateContent.BidAuto.BASort.BlokSort)
        //                    {
        //                        wBidStateContent.CalculatedBA.BASort.BlokSort.Add(item);
        //                    }
        //                }

        //            }
        //            //---------------------------------------------------------------------------




        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void SetCurrentBAGroupDetailsToCalculateBAGroupState(WBidState wBidStateContent)
        //{

        //    foreach (var item in wBidStateContent.BidAuto.BAGroup)
        //    {
        //        wBidStateContent.CalculatedBA.BAGroup.Add(new BidAutoGroup { GroupName = item.GroupName, Lines = item.Lines });
        //    }

        //}

        //private void SetCurrentBADetailsToCalculateBAState(WBidState wBidStateContent)
        //{
        //    try
        //    {
        //        if (wBidStateContent.BidAuto != null)
        //        {
        //            wBidStateContent.CalculatedBA = new BidAutomator
        //            {
        //                IsBlankBottom = wBidStateContent.BidAuto.IsBlankBottom,
        //                IsReserveBottom = wBidStateContent.BidAuto.IsReserveBottom,
        //                IsReserveFirst = wBidStateContent.BidAuto.IsReserveFirst
        //            };


        //            //Ba filter
        //            //---------------------------------------------------------------------------
        //            if (wBidStateContent.BidAuto.BAFilter != null)
        //            {
        //                wBidStateContent.CalculatedBA.BAFilter = new List<BidAutoItem>();
        //                SetCurrentBAFilterDetailsToCalculateBAFilterState(wBidStateContent);
        //            }
        //            //---------------------------------------------------------------------------
        //            //Ba Group object
        //            //---------------------------------------------------------------------------
        //            if (wBidStateContent.BidAuto.BAGroup != null)
        //            {
        //                wBidStateContent.CalculatedBA.BAGroup = new List<BidAutoGroup>();
        //                SetCurrentBAGroupDetailsToCalculateBAGroupState(wBidStateContent);

        //                // GlobalSettings.WBidStateContent.BidAuto.BAGroup = null;
        //            }
        //            //---------------------------------------------------------------------------

        //            //Sort object
        //            //---------------------------------------------------------------------------
        //            if (wBidStateContent.BidAuto.BASort != null)
        //            {
        //                wBidStateContent.CalculatedBA.BASort = new SortDetails
        //                {
        //                    SortColumn = wBidStateContent.BidAuto.BASort.SortColumn,
        //                    SortColumnName = wBidStateContent.BidAuto.BASort.SortColumnName,
        //                    SortDirection = wBidStateContent.BidAuto.BASort.SortDirection
        //                };
        //                //Block sort list
        //                // if (GlobalSettings.WBidStateContent.CalculatedBA.BASort.BlokSort != null)
        //                if (wBidStateContent.BidAuto.BASort.BlokSort != null)
        //                {
        //                    wBidStateContent.CalculatedBA.BASort.BlokSort = new List<string>();
        //                    foreach (var item in wBidStateContent.BidAuto.BASort.BlokSort)
        //                    {
        //                        wBidStateContent.CalculatedBA.BASort.BlokSort.Add(item);
        //                    }
        //                }

        //            }
        //            //---------------------------------------------------------------------------




        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void SetCurrentBAFilterDetailsToCalculateBAFilterState(WBidState wBidStateContent)
        //{
        //    try
        //    {
        //        foreach (var item in wBidStateContent.BidAuto.BAFilter)
        //        {
        //            var calculatedItem = new BidAutoItem();
        //            calculatedItem.Name = item.Name;
        //            calculatedItem.Priority = item.Priority;
        //            calculatedItem.IsApplied = item.IsApplied;

        //            SetAutoObjectValueToCalculateBAFilter(item, calculatedItem);
        //            wBidStateContent.CalculatedBA.BAFilter.Add(calculatedItem);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        //private void SetAutoObjectValueToCalculateBAFilter(BidAutoItem item, BidAutoItem calculatedItem)
        //{
        //    try
        //    {
        //        Cx3Parameter cx3Parameter;
        //        Cx3Parameter calculateCx3Parameter;
        //        CxDays cxDay;
        //        CxDays calculateCxDays;
        //        switch (calculatedItem.Name)
        //        {

        //            //-----------------------------------------------------------------------
        //            case "AP":
        //                var amPmConstriants = (AMPMConstriants)item.BidAutoObject;
        //                var calculateAmPmConstriants = new AMPMConstriants
        //                {
        //                    AM = amPmConstriants.AM,
        //                    MIX = amPmConstriants.MIX,
        //                    PM = amPmConstriants.PM
        //                };
        //                calculatedItem.BidAutoObject = calculateAmPmConstriants;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "CL":
        //                var ftCommutableLine = (FtCommutableLine)item.BidAutoObject;
        //                var calculateFtCommutableLine = new FtCommutableLine
        //                {
        //                    BaseTime = ftCommutableLine.BaseTime,
        //                    CheckInTime = ftCommutableLine.CheckInTime,
        //                    City = ftCommutableLine.City,
        //                    CommuteCity = ftCommutableLine.CommuteCity,
        //                    ConnectTime = ftCommutableLine.ConnectTime,
        //                    NoNights = ftCommutableLine.NoNights,
        //                    ToHome = ftCommutableLine.NoNights,
        //                    ToWork = ftCommutableLine.ToWork
        //                };
        //                calculatedItem.BidAutoObject = calculateFtCommutableLine;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "DOM":
        //                var daysOfMonthCx = (DaysOfMonthCx)item.BidAutoObject;
        //                var calculateDaysOfMonthCx = new DaysOfMonthCx();
        //                if (daysOfMonthCx.OFFDays != null)
        //                {
        //                    calculateDaysOfMonthCx.OFFDays = new List<int>();
        //                    foreach (var offDay in daysOfMonthCx.OFFDays)
        //                    {
        //                        calculateDaysOfMonthCx.OFFDays.Add(offDay);
        //                    }
        //                }
        //                if (daysOfMonthCx.WorkDays != null)
        //                {
        //                    calculateDaysOfMonthCx.WorkDays = new List<int>();
        //                    foreach (var workDay in daysOfMonthCx.WorkDays)
        //                    {
        //                        calculateDaysOfMonthCx.WorkDays.Add(workDay);
        //                    }
        //                }
        //                calculatedItem.BidAutoObject = calculateDaysOfMonthCx;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "DOWA":
        //                cxDay = (CxDays)item.BidAutoObject;
        //                calculateCxDays = new CxDays
        //                {
        //                    IsFri = cxDay.IsFri,
        //                    IsMon = cxDay.IsMon,
        //                    IsSat = cxDay.IsSat,
        //                    IsSun = cxDay.IsSun,
        //                    IsThu = cxDay.IsThu,
        //                    IsTue = cxDay.IsTue,
        //                    IsWed = cxDay.IsWed
        //                };
        //                calculatedItem.BidAutoObject = calculateCxDays;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "DOWS":
        //                cx3Parameter = (Cx3Parameter)item.BidAutoObject;
        //                calculateCx3Parameter = new Cx3Parameter
        //                {
        //                    ThirdcellValue = cx3Parameter.ThirdcellValue,
        //                    Type = cx3Parameter.Type,
        //                    Value = cx3Parameter.Value
        //                };
        //                calculatedItem.BidAutoObject = calculateCx3Parameter;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "DHFL":
        //                cx3Parameter = (Cx3Parameter)item.BidAutoObject;
        //                calculateCx3Parameter = new Cx3Parameter
        //                {
        //                    ThirdcellValue = cx3Parameter.ThirdcellValue,
        //                    Type = cx3Parameter.Type,
        //                    Value = cx3Parameter.Value
        //                };
        //                calculatedItem.BidAutoObject = calculateCx3Parameter;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "ET":
        //                cx3Parameter = (Cx3Parameter)item.BidAutoObject;
        //                calculateCx3Parameter = new Cx3Parameter
        //                {
        //                    ThirdcellValue = cx3Parameter.ThirdcellValue,
        //                    Type = cx3Parameter.Type,
        //                    Value = cx3Parameter.Value
        //                };
        //                calculatedItem.BidAutoObject = calculateCx3Parameter;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "LT":
        //                var lineTypeItem = (CxLine)item.BidAutoObject;
        //                var calculateCxLine = new CxLine
        //                {
        //                    Blank = lineTypeItem.Blank,
        //                    Hard = lineTypeItem.Hard,
        //                    International = lineTypeItem.International,
        //                    NonConus = lineTypeItem.NonConus,
        //                    Ready = lineTypeItem.Ready,
        //                    Reserve = lineTypeItem.Reserve
        //                };
        //                calculatedItem.BidAutoObject = calculateCxLine;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "OC":

        //                var bulkOvernightCityCx = (BulkOvernightCityCx)item.BidAutoObject;
        //                var calculateBulkOvernightCityCx = new BulkOvernightCityCx();
        //                if (bulkOvernightCityCx.OverNightNo != null)
        //                {
        //                    calculateBulkOvernightCityCx.OverNightNo = new List<int>();
        //                    foreach (var overNightNo in bulkOvernightCityCx.OverNightNo)
        //                    {
        //                        calculateBulkOvernightCityCx.OverNightNo.Add(overNightNo);
        //                    }
        //                }
        //                if (bulkOvernightCityCx.OverNightYes != null)
        //                {
        //                    calculateBulkOvernightCityCx.OverNightYes = new List<int>();
        //                    foreach (var overNightYes in bulkOvernightCityCx.OverNightYes)
        //                    {
        //                        calculateBulkOvernightCityCx.OverNightYes.Add(overNightYes);
        //                    }
        //                }
        //                calculatedItem.BidAutoObject = calculateBulkOvernightCityCx;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "RT":
        //                cx3Parameter = (Cx3Parameter)item.BidAutoObject;
        //                calculateCx3Parameter = new Cx3Parameter
        //                {
        //                    ThirdcellValue = cx3Parameter.ThirdcellValue,
        //                    Type = cx3Parameter.Type,
        //                    Value = cx3Parameter.Value
        //                };
        //                calculatedItem.BidAutoObject = calculateCx3Parameter;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "SDOW":
        //                cxDay = (CxDays)item.BidAutoObject;
        //                calculateCxDays = new CxDays
        //                {
        //                    IsFri = cxDay.IsFri,
        //                    IsMon = cxDay.IsMon,
        //                    IsSat = cxDay.IsSat,
        //                    IsSun = cxDay.IsSun,
        //                    IsThu = cxDay.IsThu,
        //                    IsTue = cxDay.IsTue,
        //                    IsWed = cxDay.IsWed
        //                };
        //                calculatedItem.BidAutoObject = calculateCxDays;
        //                break;
        //            //-----------------------------------------------------------------------
        //            case "TBL":
        //                var tripBlockLengthItem = (CxTripBlockLength)item.BidAutoObject;
        //                var calculateCxTripBlockLength = new CxTripBlockLength
        //                {
        //                    FourDay = tripBlockLengthItem.FourDay,
        //                    IsBlock = tripBlockLengthItem.IsBlock,
        //                    ThreeDay = tripBlockLengthItem.ThreeDay,
        //                    Turns = tripBlockLengthItem.Turns,
        //                    Twoday = tripBlockLengthItem.Twoday
        //                };
        //                calculatedItem.BidAutoObject = calculateCxTripBlockLength;
        //                break;
        //            //-----------------------------------------------------------------------


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //}

        partial void OnDeleteClickEvent(Foundation.NSObject sender)
        {
            selectedSort.Clear();
            PopViewController(null, true);
        }
        partial void SwitchBlankAtBottomClicked(NSObject sender)
        {
            SegmentHandling();
        }
        partial void SwitchReserveAtBottomClicked(NSObject sender)
        {
            SegmentHandling();
        }
        public void SegmentHandling()
        {
            if (SwitchBlankAtBottom.On && SwitchReserveAtBottom.On)
            {
                SegBlankReservePriority.Enabled = true;
                SegBlankReservePriority.SelectedSegment = 0;
            }
            else
            {
                SegBlankReservePriority.Enabled = false;
                SegBlankReservePriority.SelectedSegment = -1;
            }
        }
        partial void SegPriorityChanged(NSObject sender)
        {

        }

        //void HandleActionSheetEvent (object sender, UIButtonEventArgs e)
        //{
        //    Console.WriteLine(e.ButtonIndex);
        //    if (e.ButtonIndex == 0) {
        //        // cancel btn
        //        PopViewController(null, true);
        //    }else{
        //        if (e.ButtonIndex == 1) {
        //            BidChoicesViewController bidChoicesView = Storyboard.InstantiateViewController("BidChoicesViewController") as BidChoicesViewController;
        //            PushViewController (bidChoicesView,true);
        //        }
        //        if (e.ButtonIndex == 2) {
        //            PositionsAvoidanceViewController bidAvoidanceiew = Storyboard.InstantiateViewController("PositionsAvoidanceViewController") as PositionsAvoidanceViewController;
        //            PushViewController (bidAvoidanceiew,true);
        //        }
        //        if (e.ButtonIndex == 3) {
        //            PostionsPrioritiesViewController positionPriorityView = Storyboard.InstantiateViewController("PostionsPrioritiesViewController") as PostionsPrioritiesViewController;
        //            PushViewController (positionPriorityView,true);
        //        }
        //    }
        //}


        //void ShowActionSheet(){
        //    UIActionSheet actionSheet = new UIActionSheet ("Position",null,"Cancel",null,null);
        //    int countTitle = Constants.listSubmittals.Count;
        //    for (int i = 0; i < countTitle; i++) {
        //        actionSheet.AddButton (Constants.listSubmittals.ElementAt (i));
        //    }
        //    actionSheet.Clicked += HandleActionSheetEvent;
        //    // show sheet
        //    actionSheet.ShowInView (this.View);
        //}
    }
}


