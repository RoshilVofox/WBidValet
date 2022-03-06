using System;

using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Bidvalet.Model;
using Bidvalet.Business;
using System.IO;
using System.Linq;
//using WBid.WBidClient.Models;
namespace Bidvalet.iOS
{
    public partial class ConstraintsChangeViewController : UITableViewController
    {
        int _heightRow = 30;
        UIView noConstraintView;
        UIBarButtonItem objClearButton;
        public bool needTobyPassingScreen;
        public ConstraintsChangeViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LoadDefaultOrExistingFilters();
            Title = "Filters";
            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = UIColor.White;
            this.NavigationItem.StandardAppearance = appearance;
            this.NavigationItem.ScrollEdgeAppearance = this.NavigationItem.StandardAppearance;

            if (needTobyPassingScreen == true)
            {
                SortViewController sortView = Storyboard.InstantiateViewController("SortViewController") as SortViewController;
                sortView.needTobyPassingScreen = true;
                needTobyPassingScreen = false;
                NavigationController.PushViewController(sortView, false);
                return;
            }
            if (File.Exists(WBidHelper.WBidQuickFilter))
            {
                var qfInfo = (QuickFilterInfo)XmlHelper.DeserializeFromXml<QuickFilterInfo>(WBidHelper.WBidQuickFilter);
                if (qfInfo != null)
                {
                    GlobalSettings.QuickFilters = (qfInfo.QuickFilters.Count == 0) ? null : qfInfo.QuickFilters;
					if (GlobalSettings.QuickFilters != null)
					{
						GlobalSettings.QuickFilters.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
						GlobalSettings.QuickFilters.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
						GlobalSettings.QuickFilters.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "35");
					}
                }

            }

         
        }

        public void PushViewControllView(FtCommutableLine cl)
        {
            CommuteLinesViewController cmtView = Storyboard.InstantiateViewController("CommuteLinesViewController") as CommuteLinesViewController;
            cmtView.data = cl;
            NavigationController.PushViewController(cmtView, true);
        }

        public override void ViewDidAppear(bool animated)
        {
            if (SharedObject.Instance.ListConstraint != null && SharedObject.Instance.ListConstraint.Count > 0)
            {
                int total=SharedObject.Instance.ListConstraint.Count;
                for (int count = 0; count < total; count++)
                {
                    if (SharedObject.Instance.ListConstraint[count].GetType().Name == "DaysOfMonthCx")
                    {
                        var daysOfMnth = (DaysOfMonthCx)SharedObject.Instance.ListConstraint[count];
                        if ((daysOfMnth.OFFDays == null && daysOfMnth.WorkDays == null)||(daysOfMnth.OFFDays.Count == 0 && daysOfMnth.WorkDays.Count == 0))
                            SharedObject.Instance.ListConstraint.Remove(daysOfMnth);
                    }

                }
            }

            ReLoadData();
            base.ViewDidAppear(animated);
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1; // we only have on section
        }
        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return SharedObject.Instance.ListConstraint.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var currentElement = SharedObject.Instance.ListConstraint.ElementAt(indexPath.Row);
            if (currentElement is AMPMConstriants)
            {
                AMPMCell cellAmPm = AMPMCell.Create();
                cellAmPm.Filldata(this, (AMPMConstriants)currentElement);
                return cellAmPm;
            }
            if (currentElement is BlankReserveConstraint)
            {
                BlankReserveCell cell = BlankReserveCell.Create();
                cell.Filldata(this, (BlankReserveConstraint)currentElement);
                return cell;
            }
            if (currentElement is FtCommutableLine)
            {
                CommutableLinesCell cell = CommutableLinesCell.Create();
                cell.Filldata(this, (FtCommutableLine)currentElement);
                return cell;
            }
            if (currentElement is DaysOfMonthCx)
            {
                DaysOfMonthCell cell = DaysOfMonthCell.Create();
                cell.Filldata(this, (DaysOfMonthCx)currentElement);
                return cell;
            }
            if (currentElement is DaysOfWeekAll)
            {
                DaysOfWeekAllCell cell = DaysOfWeekAllCell.Create();
                cell.Filldata(this, (DaysOfWeekAll)currentElement);
                return cell;
            }
            if (currentElement is DaysOfWeekSome)
            {
                DaysOfWeekSomeCell cell = DaysOfWeekSomeCell.Create();
                cell.Filldata(this, (DaysOfWeekSome)currentElement);
                return cell;
            }
            if (currentElement is DHFristLastConstraint)
            {
                DHFirstLastCell cell = DHFirstLastCell.Create();
                cell.Filldata(this, (DHFristLastConstraint)currentElement);
                return cell;
            }
            if (currentElement is EquirementConstraint)
            {
                EquipmentCell cell = EquipmentCell.Create();
                cell.Filldata(this, (EquirementConstraint)currentElement);
                return cell;
            }
            if (currentElement is LineTypeConstraint)
            {
                LinesTypeCell cell = LinesTypeCell.Create();
                cell.Filldata(this, (LineTypeConstraint)currentElement);
                return cell;
            }
            if (currentElement is OvernightCitiesCx)
            {
                OvernightsCityCell cell = OvernightsCityCell.Create();
                cell.Filldata(this, (OvernightCitiesCx)currentElement);
                return cell;
            }
            if (currentElement is RestCx)
            {
                RestCell cell = RestCell.Create();
                cell.Filldata(this, (RestCx)currentElement);
                return cell;
            }
            if (currentElement is StartDayOfWeek)
            {
                StartDayOfWeekCell cell = StartDayOfWeekCell.Create();
                cell.Filldata(this, (StartDayOfWeek)currentElement);
                return cell;
            }
            if (currentElement is CxTripBlockLength)
            {
                TripBlockLengthCell cellTripBlock = TripBlockLengthCell.Create();
                cellTripBlock.Filldata(this, (CxTripBlockLength)currentElement);
                return cellTripBlock;
            }

            return null;
        }

        public void DeleteObject(object obj)
        {
            if (obj.GetType().Name == "FtCommutableLine")
            {
                var wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                if (wBIdStateContent != null && wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.DailyCommuteTimes != null)
                {
                    wBIdStateContent.BidAuto.DailyCommuteTimes.Clear();
                }
            }

            SharedObject.Instance.ListConstraint.Remove(obj);
            ReLoadData();
        }


        public void AddConstraintAtIndex(int row)
        {

            switch (row)
            {
                case Constants.Am_Pm_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new AMPMConstriants() { AM = true, MIX = true, PM = true });
                    ReLoadData();
                    break;
                //			case Constants.Blank_Reserve_Constraint:
                //				isFound = false;
                //				foreach (var item in SharedObject.Instance.ListConstraint) {
                //					if (item is BlankReserveConstraint) {
                //						isFound = true;
                //						break;
                //					}
                //				}
                //				if (!isFound) {
                //					SharedObject.Instance.ListConstraint.Add (new BlankReserveConstraint ());	
                //				}
                //				break;
                case Constants.Commutable_Lines_Constraint:


                    if (!CheckFilterAlreadyAdded("FtCommutableLine"))
                    {
                        var cl = new FtCommutableLine() { ToHome = true };

                        noConstraintView.Hidden = true;
                        TableView.Hidden = false;
                        PushViewControllView(cl);
                    }
                    //				ReLoadData ();
                    break;
                case Constants.Days_of_Month_Constraint:
                    //if (!CheckFilterAlreadyAdded("DaysOfMonthCx"))
                    //{
                    var dom = new DaysOfMonthCx();
                    SharedObject.Instance.ListConstraint.Add(dom);
                    noConstraintView.Hidden = true;
                    TableView.Hidden = false;
                    ShowDaysOfMonthVC(dom);
                    // }
                    break;
                case Constants.Days_Week_All_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new DaysOfWeekAll() { Fr = true, Mo = true, Sa = true, Su = true, Th = true, Tu = true, We = true });
                    ReLoadData();
                    break;
                case Constants.Days_of_Week_Some_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new DaysOfWeekSome() { Date = "Mon", LessOrMore = "Less than", Value = 0 });
                    ReLoadData();
                    break;
                case Constants.DH_First_Last_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new DHFristLastConstraint() { DH = "first", LessMore = "Less than", Value = 0 });
                    ReLoadData();
                    break;
                case Constants.Equipment_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new EquirementConstraint() { Equipment = 700, LessMore = "Less than", Value = 0 });
                    ReLoadData();
                    break;
                case Constants.Line_Type_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new LineTypeConstraint() { Blank = true, Hard = true, Int = true, NonCon = true, Res = true });
                    ReLoadData();
                    break;
                case Constants.Overnight_Cities_Constraint:
                    var ovn = new OvernightCitiesCx();
                    SharedObject.Instance.ListConstraint.Add(ovn);
                    noConstraintView.Hidden = true;
                    TableView.Hidden = false;
                    ShowOvernightCities(ovn);
                    //				ReLoadData ();
                    break;
                case Constants.Rest_Constraint:
                    if (!CheckFilterAlreadyAdded("RestCx"))
                    {
                        SharedObject.Instance.ListConstraint.Add(new RestCx() { Dom = "All", LessMore = "Less than", Value = 8 });
                        ReLoadData();
                    }
                    break;
                case Constants.Start_Day_Of_Week_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new StartDayOfWeek());
                    ReLoadData();
                    break;
                case Constants.Trip_Work_Block_Length_Constraint:
                    SharedObject.Instance.ListConstraint.Add(new CxTripBlockLength() { IsBlock = true });
                    ReLoadData();
                    break;
                default:
                    break;
            }
        }


        private bool CheckFilterAlreadyAdded(string filterName)
        {
            bool exist = false;

            if (SharedObject.Instance.ListConstraint != null && SharedObject.Instance.ListConstraint.Count > 0)
                exist = SharedObject.Instance.ListConstraint.Any(x => x.GetType().Name == filterName);
            return exist;
        }

        void ShowOvernightCities(OvernightCitiesCx ovn)
        {
            CitiesOvernightsViewController viewController = Storyboard.InstantiateViewController("CitiesOvernightsViewController") as CitiesOvernightsViewController;

            viewController.data = ovn;
            NavigationController.PushViewController(viewController, true);
        }

        void ShowDaysOfMonthVC(DaysOfMonthCx dom)
        {
            ConstraintDaysMonthViewController viewController = Storyboard.InstantiateViewController("ConstraintDaysMonthViewController") as ConstraintDaysMonthViewController;
            viewController.data = dom;
            NavigationController.PushViewController(viewController, true);
        }

        void ReLoadData()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            NavigationController.NavigationBar.BarTintColor = UIColor.White;
            NavigationController.NavigationBar.Translucent = false;
            NavigationItem.HidesBackButton = true;

			UIBarButtonItem objSpace = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);
			objSpace.Width = 20;

            //Clear Button 
            objClearButton = new UIBarButtonItem();
            UITextAttributes icoFontAttribute = new UITextAttributes();

            icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(18);
            icoFontAttribute.TextColor = UIColor.Red;
            objClearButton.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
            objClearButton.Title = "Clear";

            objClearButton.Style = UIBarButtonItemStyle.Plain;
            objClearButton.Clicked += (sender, args) =>
            {

                string message = "Are you sure you want to Clear all of your Filters?";

                var alertVW = new UIAlertView(GlobalSettings.ApplicationName, message, null, "No", new string[] { "Yes" });
                alertVW.Clicked += (object senderObj, UIButtonEventArgs ee) =>
                {
                    int index = (int)ee.ButtonIndex;
                    //No
                    if (index == 1)
                    {
                        SharedObject.Instance.ListConstraint.Clear();
                        SetDefaultScreen();
                        TableView.ReloadData();

                    }
                };
                alertVW.Show();

                // remove all items in list

            };


            UIBarButtonItem addButton = new UIBarButtonItem();
            UITextAttributes icoFontAddAttribute = new UITextAttributes();
            //icoFontAttribute.Font = UIFont.FromName("Courier", 18f);
            //icoFontAddAttribute.TextColor = UIColor.Blue;
            icoFontAddAttribute.Font = UIFont.BoldSystemFontOfSize(22);
            addButton.SetTitleTextAttributes(icoFontAddAttribute, UIControlState.Normal);
            addButton.Title = "+";
            addButton.Style = UIBarButtonItemStyle.Plain;
            addButton.Clicked += (sender, args) =>
            {
                ConstraintModalView modal = new ConstraintModalView(this);
                modal.ProvidesPresentationContextTransitionStyle = false;
                modal.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
                NavigationController.PresentViewController(modal, true, () => { });
            };


            NavigationItem.RightBarButtonItems = new UIBarButtonItem[]{
			new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, ((sender, e) =>
					{
						if (SharedObject.Instance.ListConstraint != null && SharedObject.Instance.ListConstraint.Count > 0)
						{


							BidAutoHelper.SetSelectedFiltersToStateObject(SharedObject.Instance.ListConstraint);
							string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
							WBidHelper.SaveStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");

							// GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
							WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
							// if (wBIdStateContent.BidAuto.BAFilter != GlobalSettings.QuickFilters)
							if (GlobalSettings.QuickFilters == null || CompareHelper.CompareStates(GlobalSettings.QuickFilters, wBIdStateContent.BidAuto.BAFilter))
							{

                                string message = "Would you like to save the selected filters as a default? \n\n (Note, we will not save   Commutable line,Days of the Month and Overnight cities, as that filters are specific to a month)";
								//var dom = wBIdStateContent.BidAuto.BAFilter.FirstOrDefault(x => x.Name == "DOM");
								//if (dom != null)
									//message += "\r\nDays of Month filter does not include for default list";
								var alertVW = new UIAlertView(GlobalSettings.ApplicationName, message, null, "No", new string[] { "Yes" });
								alertVW.Clicked += (object senderObj, UIButtonEventArgs ee) =>
								{
									
									int index = (int)ee.ButtonIndex;
									//No
									if (index == 1)
									{


                                        //QuickFilterInfo qfInfo = new QuickFilterInfo
                                        //{
                                        //    QuickFilterVersion = GlobalSettings.QuickFilterVersion,
                                        //    QuickFilters = wBIdStateContent.BidAuto.BAFilter
                                        //};
                                        QuickFilterInfo qfInfo = new QuickFilterInfo();

                                        qfInfo.QuickFilterVersion = GlobalSettings.QuickFilterVersion;

                                        qfInfo.QuickFilters = new List<BidAutoItem>();

                                        foreach (var item in wBIdStateContent.BidAuto.BAFilter)
                                        {
                                            if (item.Name != "CL" && item.Name != "DOM" && item.Name != "OC")
                                            {
                                                qfInfo.QuickFilters.Add(item);
                                            }
                                        }
                                       
										GlobalSettings.QuickFilters=qfInfo.QuickFilters;
										WBidHelper.SaveQuickFilterFile(qfInfo, WBidHelper.WBidQuickFilter);

									}
									else
									{
									}
									SortViewController sortView = Storyboard.InstantiateViewController("SortViewController") as SortViewController;
									NavigationController.PushViewController(sortView, true);
								};
								alertVW.Show();

							}
							else
							{


								SortViewController sortView = Storyboard.InstantiateViewController("SortViewController") as SortViewController;
								NavigationController.PushViewController(sortView, true);
							}
						}
						else
						{
							DisplayAlertView(GlobalSettings.ApplicationName, "Please Select a filter");

						}
					}))
			,
				objSpace,
                //new UIBarButtonItem ("Clear", UIBarButtonItemStyle.Plain,(sender,args) => {
                //    // remove all items in list
                //    SharedObject.Instance.ListConstraint.Clear();
                //    SetDefaultScreen();
                //    TableView.ReloadData();
                //}
			//	)

				objClearButton,objSpace
            };
            //

            //            NavigationItem.RightBarButtonItem = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, ((sender, e) =>
            //            {
            //                if (SharedObject.Instance.ListConstraint != null && SharedObject.Instance.ListConstraint.Count > 0)
            //                {
            //
            //
            //                    BidAutoHelper.SetSelectedFiltersToStateObject(SharedObject.Instance.ListConstraint);
            //                    string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
            //                    WBidHelper.SaveStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
            //
            //                    // GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
            //                    WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //                    // if (wBIdStateContent.BidAuto.BAFilter != GlobalSettings.QuickFilters)
            //                    if (GlobalSettings.QuickFilters == null || CompareHelper.CompareStates(GlobalSettings.QuickFilters, wBIdStateContent.BidAuto.BAFilter))
            //                    {
            //
            //                        string message = "Are you want to save the selected filters as a   default filters?";
            //                        var dom = wBIdStateContent.BidAuto.BAFilter.FirstOrDefault(x => x.Name == "DOM");
            //                        if (dom != null)
            //                            message += "\r\nDays of Month filter does not include for default list";
            //                        var alertVW = new UIAlertView(GlobalSettings.ApplicationName, message, null, "No", new string[] { "Yes" });
            //                        alertVW.Clicked += (object senderObj, UIButtonEventArgs ee) =>
            //                        {
            //                            int index = (int)ee.ButtonIndex;
            //                            //No
            //                            if (index == 1)
            //                            {
            //
            //
            //                                QuickFilterInfo qfInfo = new QuickFilterInfo
            //                                {
            //                                    QuickFilterVersion = GlobalSettings.QuickFilterVersion,
            //                                    QuickFilters = wBIdStateContent.BidAuto.BAFilter
            //                                };
            //									GlobalSettings.QuickFilters=qfInfo.QuickFilters;
            //                                WBidHelper.SaveQuickFilterFile(qfInfo, WBidHelper.WBidQuickFilter);
            //
            //                            }
            //                            else
            //                            {
            //                            }
            //                            SortViewController sortView = Storyboard.InstantiateViewController("SortViewController") as SortViewController;
            //                            NavigationController.PushViewController(sortView, true);
            //                        };
            //                        alertVW.Show();
            //
            //                    }
            //                    else
            //                    {
            //
            //
            //                        SortViewController sortView = Storyboard.InstantiateViewController("SortViewController") as SortViewController;
            //                        NavigationController.PushViewController(sortView, true);
            //                    }
            //                }
            //                else
            //                {
            //                    DisplayAlertView(GlobalSettings.ApplicationName, "Please Select a filter");
            //                    
            //                }
            //            }));
            NavigationItem.LeftBarButtonItems = new UIBarButtonItem[]{
				new UIBarButtonItem ("< Back", UIBarButtonItemStyle.Plain,(sender,args) => {
					if (noConstraintView != null)
					{
						noConstraintView.RemoveFromSuperview();

					}
					NavigationController.PopViewController(true);
				}),
				objSpace,
                //new UIBarButtonItem ("+", UIBarButtonItemStyle.Plain,(sender,args) => {
                //    ConstraintModalView modal = new ConstraintModalView(this);
                //    modal.ProvidesPresentationContextTransitionStyle = true;
                //    modal.ModalPresentationStyle  = UIModalPresentationStyle.OverCurrentContext;
                //    NavigationController.PresentViewController(modal,true, ()=>{});
                //}
                //)
				addButton,objSpace
            };
            //





            TableView.TableFooterView = new UIView();
            TableView.SetEditing(true, true);
            SetDefaultScreen();
            TableView.ReloadData();
        }



        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        }
        private void SetDefaultScreen()
        {
            if (noConstraintView == null)
            {
                NoConstraintVC noConstraintViewVC = Storyboard.InstantiateViewController("NoConstraintVC") as NoConstraintVC;
                noConstraintView = noConstraintViewVC.View;

                this.NavigationController.View.InsertSubviewBelow(noConstraintView, this.NavigationController.NavigationBar);
            }
            if (SharedObject.Instance.ListConstraint.Count > 0)
            {
                // hide intro view
                noConstraintView.Hidden = true;
                TableView.Hidden = false;
            }
            else
            {
                // show intro view;
                noConstraintView.Hidden = false;
                TableView.Hidden = true;
            }
            objClearButton.Enabled = (SharedObject.Instance.ListConstraint.Count > 0);

            if (objClearButton.Enabled)
            {
                UITextAttributes uiAttribute = objClearButton.GetTitleTextAttributes(UIControlState.Disabled);
                uiAttribute.TextColor = UIColor.Red;
                objClearButton.SetTitleTextAttributes(uiAttribute, UIControlState.Normal);
            }
            else
            {
                UITextAttributes uiAttribute = objClearButton.GetTitleTextAttributes(UIControlState.Normal);
                uiAttribute.TextColor = UIColor.LightGray;
                objClearButton.SetTitleTextAttributes(uiAttribute, UIControlState.Disabled);
            }

        }
        /*
         * Tableview methods
         */
        public override bool CanEditRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return true;
        }
        public override bool CanMoveRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return true;
        }
        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }
        public override bool ShouldIndentWhileEditing(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return false;
        }
        public override void MoveRow(UITableView tableView, Foundation.NSIndexPath sourceIndexPath, Foundation.NSIndexPath destinationIndexPath)
        {
            //base.MoveRow (tableView, sourceIndexPath, destinationIndexPath);

            //---- get a reference to the item
            var item = SharedObject.Instance.ListConstraint[sourceIndexPath.Row];
            int deleteAt = sourceIndexPath.Row;

            //---- if we're moving within the same section, and we're inserting it before
            if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row))
            {
                //---- add one to where we delete, because we're increasing the index by inserting
                deleteAt = sourceIndexPath.Row + 1;
            }
            //---- copy the item to the new location
            SharedObject.Instance.ListConstraint.Insert(destinationIndexPath.Row, item);
            //---- remove from the old
            SharedObject.Instance.ListConstraint.RemoveAt(deleteAt);
        }
        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return _heightRow;
        }

        private void LoadDefaultOrExistingFilters()
        {
            if (GlobalSettings.IsDownloadProcess)
            {
                if (GlobalSettings.QuickFilters != null)
                {
                    BidAutoHelper.LoadFilters(GlobalSettings.QuickFilters);
                    ReLoadData();
                }
            }
            else
            {

                WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                //  wBIdStateContent.BidAuto = null;
                if (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.BAFilter != null)
                {
                    BidAutoHelper.LoadFilters(wBIdStateContent.BidAuto.BAFilter);
                    ReLoadData();
                }
            }
        }



    }
}


