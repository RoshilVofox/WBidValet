#region NameSpace
using Bidvalet.Business;
using Bidvalet.iOS.Utility;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using Bidvalet.Model;
#endregion

namespace Bidvalet.iOS
{
    public partial class CalculateBidViewController : BaseViewController
    {

        #region Variables
        double _incrementValue;
        List<NSObject> arrObserver = new List<NSObject>();
        LoadingOverlay _loadingOverlay;
        public bool needTobyPassingScreen;
        #endregion

        public CalculateBidViewController(IntPtr handle)
            : base(handle)
        {
        }

        #region Events
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ShowNavigationBar();
            progressCombo.Progress = 0.0f;
            Title = "Calculate Bid";

            if (needTobyPassingScreen == true)
            {
                LineViewController ObjLine = new LineViewController();
                needTobyPassingScreen = false;
                this.NavigationController.PushViewController(ObjLine, false);
                return;

            }
            btnSubmitBid.SetTitle("Prepare Bid List", UIControlState.Normal);
            observeNotifications();
            UIHelpers.StyleForButtons(new UIButton[] { btnSubmitBid });

            int count = SharedObject.Instance.ListConstraint == null ? 0 : SharedObject.Instance.ListConstraint.Count;

            lbCalculate.Text = string.Format("You have set {0} filters so that means there {1} different combinations to consider.", count, Math.Pow(2, count));
            _incrementValue = 1 / Math.Pow(2, count);


        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            progressCombo.Hidden = true;
            lbCalculatingCombo.Text = string.Empty;
            lbCalculatingCombo.Hidden = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);


        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void OnSubmitBidClickEvent(Foundation.NSObject sender)
        {


            try
            {
                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(true, true);
                this.progressCombo.Hidden = false;
                lbCalculatingCombo.Hidden = false;

                _loadingOverlay = new LoadingOverlay(this.View.Bounds, "Preparing Bid List...");
                this.View.AddSubview(_loadingOverlay);
                InvokeInBackground(() =>
                {

                    StartProgress();




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
                    InvokeOnMainThread(() =>
                    {

                        //string msg = string.Empty;
                        //int i = 0;
                        //foreach (var item in GlobalSettings.Lines)
                        //{
                        //    ////if (item.LineNum >= 398)
                        //    ////{
                        //    //    msg += item.LineNum + " " + item.BAGroup + " " + item.Points + "\r\n";
                        //    ////}
                        //    //if (i == 25) break;
                        //    //i++;
                        //}

                        //DisplayAlertView("", msg);
                        //this.NavigationController.NavigationItem.LeftBarButtonItem.Enabled = true;
                        this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
                        _loadingOverlay.Hide();
                        LineViewController ObjLine = new LineViewController();
                        this.NavigationController.PushViewController(ObjLine, true);


                        //viewLine();


                        //                        SubmitBidViewController viewController = Storyboard.InstantiateViewController("SubmitBidViewController") as SubmitBidViewController;
                                           // PushViewController(viewController, true);
                    });

                });
            }
            catch (Exception ex)
            {

                if (_loadingOverlay != null)
                    _loadingOverlay.Hide();
                this.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(false, true);
            }



        }
        #endregion

        #region Methods
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

        private void SetCurrentBADetailsToCalculateBAState()
        {
            try
            {
                var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                if (wBidStateContent.BidAuto != null)
                {
                    wBidStateContent.BidAutoOn = true;
                    //wBidStateContent.BidAuto.BAWeight = new List<BidAutoItem>();
                    //if (wBidStateContent.BidAuto.DailyCommuteTimes ==null)
                    //{
                    //    wBidStateContent.BidAuto.DailyCommuteTimes = new List<CommuteTime>();
                    //}



                    wBidStateContent.CalculatedBA = new BidAutomator
                    {
                        IsBlankBottom = wBidStateContent.BidAuto.IsBlankBottom,
                        IsReserveBottom = wBidStateContent.BidAuto.IsReserveBottom,
                        IsReserveFirst = wBidStateContent.BidAuto.IsReserveFirst
                    };


                    //wBidStateContent.CalculatedBA.BAWeight = new List<BidAutoItem>();

                    //if (wBidStateContent.CalculatedBA.DailyCommuteTimes == null)
                    //{
                    //    wBidStateContent.CalculatedBA.DailyCommuteTimes = new List<CommuteTime>();
                    //}
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

        private void SetCurrentBAGroupDetailsToCalculateBAGroupState(WBidState wBidStateContent)
        {

            foreach (var item in wBidStateContent.BidAuto.BAGroup)
            {
                wBidStateContent.CalculatedBA.BAGroup.Add(new BidAutoGroup { GroupName = item.GroupName, Lines = item.Lines });
            }

        }

        private void SetCurrentBADetailsToCalculateBAState(WBidState wBidStateContent)
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

        private void SetCurrentBAFilterDetailsToCalculateBAFilterState(WBidState wBidStateContent)
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

        private void SetAutoObjectValueToCalculateBAFilter(BidAutoItem item, BidAutoItem calculatedItem)
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
                            ToWork = ftCommutableLine.ToWork,
                            isNonStop= ftCommutableLine.isNonStop
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

        private void observeNotifications()
        {
            arrObserver.Add(NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("IncrementBidProgress"), IncrementProgress));

        }

        public void StartProgress()
        {
            InvokeOnMainThread(() =>
            {
                progressCombo.Progress = 0.0f;

            });
        }

        void IncrementProgress(NSNotification n)
        {

            InvokeOnMainThread(() =>
            {
                lbCalculatingCombo.Text = "Calculating Combo #Grp " + n.Object;

                progressCombo.SetProgress(this.progressCombo.Progress + float.Parse(_incrementValue.ToString()), true);
            });

        }
        //void viewLine()
        //{

        //    var ObjLine = new LineViewController();
        //    this.NavigationController.PushViewController(ObjLine, true);
        //}
        private void DisplayAlertView(string caption, String message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        }
        #endregion
    }
}


