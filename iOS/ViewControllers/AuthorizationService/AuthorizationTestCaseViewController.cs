// This file has been autogenerated from a class added in the UI designer.

#region NameSpace
using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using Bidvalet.Business;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Bidvalet.Business.BusinessLogic;
#endregion
namespace Bidvalet.iOS
{
    public partial class AuthorizationTestCaseViewController : BaseViewController
    {
        #region Public Variables

        public string messageError;
        public NSMutableAttributedString InvalidCredentialError;
        public bool isShowPurchaseButton = false;
        public string buttonTitle = "Done";
        public int numberRow = -1;
        public string topBarTitle = string.Empty;
        public string FileName = string.Empty;
        public DateTime ExpirationDate;
        public bool needTobyPassingScreen;
        #endregion

        #region Private Variables
        private LoadingOverlay _loadingOverlay;
        #endregion

        public AuthorizationTestCaseViewController(IntPtr handle)
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
            base.ViewDidLoad();
        
            try
            {

                tvMessageError.Font = UIFont.FromName("Helvetica-Bold", 16f);
                Title = topBarTitle;
                ShowNavigationBar();
                SetUpView();
                if (topBarTitle == "Invalid Login")
                {
                    tvMessageError.AttributedText = InvalidCredentialError;
                }
                else
                {
                    tvMessageError.Text = messageError;
                }

                if (needTobyPassingScreen == true)
                {
                    var constraintView = Storyboard.InstantiateViewController("ConstraintsChangeViewController") as ConstraintsChangeViewController;
                    constraintView.needTobyPassingScreen = true;
                    PushViewController(constraintView, false);

                    return;
                }

                if (Title == "Filters")
                {


                    if (GlobalSettings.WBidINIContent == null || GlobalSettings.WBidINIContent.Cities == null)
                    {
                        Constants.listCities = new List<string>();
                    }
                    else
                    {
                        Constants.listCities = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                    }
                    if (FileName != string.Empty)
                    {
                        _loadingOverlay = new LoadingOverlay(View.Bounds, "Loading...");
                        View.Add(_loadingOverlay);
                        InvokeInBackground(() =>
                        {
                            LoadExistingBidDetails(FileName);
                            InvokeOnMainThread(() => _loadingOverlay.Hide());
                        });

                    }
                    else
                    {
                        SetCurrentBidDetails();
                    }
                   
                }


            }
            catch (Exception ex)
            {

                throw;
            }


        }

        /// <summary>
        /// Button Done clicked Event
        /// </summary>
        /// <param name="sender">Sender.</param>

        partial void OnDoneButtonClickedEvent(Foundation.NSObject sender)
        {
            try
            {

                if (numberRow == (int)AuthStaus.Filters || numberRow == Constants.FOUND_ACCOUNT || numberRow == Constants.CREATE_ACCOUNT || numberRow == Constants.NEW_CB_WB_USER || numberRow == Constants.VALID_SUBSCRIPTION)
                {
                    var constraintView = Storyboard.InstantiateViewController("ConstraintsChangeViewController") as ConstraintsChangeViewController;
                    GenerateDynamicOverNightCitiesList();
                    PushViewController(constraintView, true);
                }
                else
                {
                    PopViewController(null, true);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// On Purchase In App Button Clicked Event
        /// </summary>
        /// <param name="sender">Sender.</param>
        partial void OnPurchaseInAppEvent(Foundation.NSObject sender)
        {
            NSLogBidValet("purchase clicked");
            var expiredViewController = Storyboard.InstantiateViewController("ExpiredViewController") as ExpiredViewController;
            if (expiredViewController != null)
            {
                expiredViewController.expiredTime = ExpirationDate;
                NavigationController.PushViewController(expiredViewController, true);
            }
            //PopViewController(null, true);
        }
        #endregion

        #region Private Methods


        private void GenerateDynamicOverNightCitiesList()
        {
            GlobalSettings.OverNightCitiesInBid = new List<City>();
            foreach (Line line in GlobalSettings.Lines)
            {
               
                foreach (var pairing in line.Pairings)
                {                 //Get trip
                    Trip trip = GetTrip(pairing);
                 
                    List<string> overNightCities = trip.DutyPeriods.Select(x => x.ArrStaLastLeg).Where(y => y.ToString(CultureInfo.InvariantCulture) != GlobalSettings.CurrentBidDetails.Domicile).ToList();

                    foreach (string city in overNightCities)
                    {
                        if (!GlobalSettings.OverNightCitiesInBid.Any(x => x.Name == city))
                        {
                            var inicity = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city);
                            if (inicity == null)
                            {
                                var cityid = GlobalSettings.WBidINIContent.Cities.Max(x => x.Id) + 1;
                                //SendMailToAdmin("Below city is added into the INI file for this user .Check the Wbupdate file.+\nc City= " + city + "Id =" + cityid, GlobalSettings.WbidUserContent.UserInformation.Email);

                                GlobalSettings.WBidINIContent.Cities.Add(new City { Id = cityid, Name = city, Code = 6 });
                                inicity = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city);
                            }


                            if (inicity != null)
                                GlobalSettings.OverNightCitiesInBid.Add(new City
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

        private Trip GetTrip(string pairing)
        {
            Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4)) ??
                        GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
			if (trip == null && pairing.Length>6)
            {
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0,6)).FirstOrDefault();
            }
            return trip;

        }

        private void SetUpView()
        {
            UIHelpers.StyleForButtons(new[] { btnPurchaseInApp, btnDone });
            btnDone.SetTitle(buttonTitle, UIControlState.Normal);
            if (isShowPurchaseButton)
            {
                btnPurchaseInApp.Alpha = 1;
                btnPurchaseInApp.SetTitle("In-app $5.99 / month", UIControlState.Normal);
            }
            else
            {
                btnPurchaseInApp.Alpha = 0;
            }
        }

        public void LoadExistingBidDetails(string fileName)
        {



            try
            {
                WBidHelper.SetCurrentBidInformationfromStateFileName(fileName);



                TripInfo tripInfo;
                using (FileStream tripStream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBP"))
                {
                    //int a = 1;
                    var objTripInfo = new TripInfo();
                    tripInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBP", objTripInfo, tripStream);
                }

                GlobalSettings.Trip = new ObservableCollection<Trip>(tripInfo.Trips.Values);
                if (tripInfo.TripVersion == GlobalSettings.TripVersion)
                {
                    LineInfo lineInfo;
                    using (FileStream linestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBL"))
                    {
                        //int a = 1;


                        var objineinfo = new LineInfo();
                        lineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBL", objineinfo, linestream);

                    }

                    if (lineInfo.LineVersion == GlobalSettings.LineVersion)
                    {
                        GlobalSettings.Lines = new ObservableCollection<Line>(lineInfo.Lines.Values);
                    }
                }


                foreach (Line line in GlobalSettings.Lines)
                {
                    line.ConstraintPoints = new ConstraintPoints();
                    line.WeightPoints = new WeightPoints();
                }
                GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");

				foreach (var item in GlobalSettings.WBidStateCollection.StateList)
				{
					//remove 300 and 500 equipments
					if (item.BidAuto != null && item.BidAuto.BAFilter != null && item.BidAuto.BAFilter.Count > 0)
					{
						item.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
						item.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
						item.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "35");
					}
					if (item.CalculatedBA != null && item.CalculatedBA.BAFilter != null && item.CalculatedBA.BAFilter.Count > 0)
					{
						item.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
						item.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
						item.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "35");

					}

				}
                GlobalSettings.IsHistorical = (GlobalSettings.WBidStateCollection.DataSource == "HistoricalData");


                if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
                {
                    GlobalSettings.UserInfo.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0 ? 0 : GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber;
                }

               
               
                WBidHelper.GenerateDynamicOverNightCitiesList();
                //  GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
                GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList();
                var statemanagement = new StateManagement();
                //statemanagement.ReloadDataFromStateFile();
                WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

                if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileName + ".VAC"))
                {

                    using (
                        FileStream vacstream =
                        File.OpenRead(WBidHelper.GetAppDataPath() + "/" + fileName + ".VAC"))
                    {

                        var objineinfo = new Dictionary<string, TripMultiVacData>();
                        GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + fileName + ".VAC", objineinfo, vacstream);

                    }
                }
                else
                {
                    if (wBidStateContent != null && wBidStateContent.MenuBarButtonState != null)
                    {
                        wBidStateContent.MenuBarButtonState.IsEOM = false;
                        wBidStateContent.MenuBarButtonState.IsVacationCorrection = false;
                        wBidStateContent.MenuBarButtonState.IsVacationDrop = false;
                    }
                }

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
                GlobalSettings.IsFVVacation = (GlobalSettings.WBidStateCollection.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

                GlobalSettings.FVVacation = GlobalSettings.WBidStateCollection.FVVacation;

                statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
                //Setting  status to Global variables
                statemanagement.SetVacationOrOverlapExists(wBidStateContent);

                PerformFVVacation();
                //St the line order based on the state file.
                statemanagement.ReloadStateContent(wBidStateContent);



                statemanagement.RecalculateLineProperties(wBidStateContent);


                SetCurrentBidDetails();
            }
            catch (Exception ex)
            {
                InvokeOnMainThread(() =>
                {

                    throw ex;
                });
            }

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
        public void SetCurrentBidDetails()
        {

            string bidDetails = "\n\n" + GlobalSettings.CurrentBidDetails.Domicile + " - " + GlobalSettings.CurrentBidDetails.Postion + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(GlobalSettings.CurrentBidDetails.Month) + " " + GlobalSettings.CurrentBidDetails.Year + "\n" + ((GlobalSettings.CurrentBidDetails.Round == "M") ? "1st Round" : "2nd Round");
            if (GlobalSettings.IsHistorical)
            {
                bidDetails += "  (Historical)";
            }
            InvokeOnMainThread(() =>
            {

                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    messageError += "\n\nFlt Attendants don't worry about position just yet, we do that just before submitting your bid";
                }

                messageError += bidDetails;
            
                var attributedString = new NSMutableAttributedString(messageError);
                attributedString.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Helvetica-Bold", 16f), new NSRange(0, messageError.Length));
                attributedString.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Red, new NSRange(messageError.Length - bidDetails.Length, bidDetails.Length));
                if(tvMessageError!=null)
                tvMessageError.AttributedText = attributedString;


               
            });
        }
        #endregion
    }
}