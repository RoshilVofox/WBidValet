using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bidvalet.Business;
using Bidvalet.Model;
using Bidvalet.Shared;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
    public partial class MultipleBidsViewController : UIViewController
    {
        private DownloadInfo _downloadFileDetails;
        private List<string> selectedBases = new List<string>();
        LoadingOverlay loadingOverlay;
        string UserId = "";
        string Password = "";
        string selectedYear = "";
        string selectedMonth = "";
        private string SessionCredential = String.Empty;
        string[] yearArray = new string[]{
                    "2020",
                    "2021"

                };
        string[] monthArray = new string[]{
                    "January",
                    "February",
                    "March",
                    "April",
                    "May",
                    "June",
                    "July",
                    "August",
                    "September",
                    "October",
                    "November",
                    "December",

                };
        public MultipleBidsViewController() : base("MultipleBidsViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ResetButtons();
            string currentYear = DateTime.Now.AddMonths(1).Year.ToString();
            int currentMonth = DateTime.Now.AddMonths(1).Month;
            btnYear.SetTitle(currentYear, UIControlState.Normal);
            string monthName = monthArray[currentMonth - 1];
            btnMonth.SetTitle(monthName, UIControlState.Normal);
            selectedYear = currentYear;
            selectedMonth = monthName;

            // Perform any additional setup after loading the view, typically from a nib.
            txtUserId.KeyboardType = UIKeyboardType.NumbersAndPunctuation;


            this.txtUserId.ShouldReturn += (textField) =>
            {
                txtPassword.BecomeFirstResponder();
                return true;
            };

            this.txtPassword.ShouldReturn += (textField) =>
            {
                textField.ResignFirstResponder();
                return true;
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        #region FO,CP
        partial void BtnALLCPClick(NSObject sender)
        {
            this.ResetButtons();
            btnALLCP.Selected = !btnALLCP.Selected;

            this.setCPFOSelected();

        }
        partial void BtnALLPilot(NSObject sender)

        {
            this.ResetButtons();
            btnALLPilot.Selected = !btnALLPilot.Selected;

            this.setCPFOSelected();

        }
        partial void BtnCPClick(NSObject sender)
        {
            this.ResetButtons();
            btnCP.Selected = !btnCP.Selected;
            this.setCPFOSelected();
        }
        partial void BtnALLFOClick(NSObject sender)
        {
            this.ResetButtons();
            btnALLFO.Selected = !btnALLFO.Selected;
            this.setCPFOSelected();
        }
        partial void BtnFOClick(NSObject sender)
        {
            this.ResetButtons();
            btnFO.Selected = !btnFO.Selected;
            this.setCPFOSelected();

        }
        #endregion

        #region FA
        partial void BtnALLFAClick(NSObject sender)
        {
            this.ResetButtons();
            btnALLFA.Selected = !btnALLFA.Selected;
            this.setFASelected();
        }
        partial void BtnFAClick(NSObject sender)
        {
            this.ResetButtons();
            btnFA.Selected = !btnFA.Selected;
            this.setFASelected();
        }
        #endregion
        #region Round Selection
        partial void BtnRound1Click(NSObject sender)
        {
            btnRound1.Selected = true;
            btnRound2.Selected = false;
        }
        partial void BtnRound2Click(NSObject sender)
        {
            btnRound1.Selected = false;
            btnRound2.Selected = true;
        }
        #endregion
        partial void BtnMonthClick(NSObject sender)
        {
            var alert = UIAlertController.Create("Select Month", null, UIAlertControllerStyle.ActionSheet);
            foreach (var monthName in monthArray)
            {
                alert.AddAction(UIAlertAction.Create(monthName, UIAlertActionStyle.Default, (UIAlertAction obj) => handleMonthTap(monthName)));
                
            }
            ShowViewController(alert, null);

        }
        public void handleMonthTap(string data)
        {
            btnMonth.SetTitle(data, UIControlState.Normal);
            selectedMonth = data;

        }
        partial void BtnYearClick(NSObject sender)
        {
            var alert = UIAlertController.Create("Select Year", null, UIAlertControllerStyle.ActionSheet);
            foreach (var year in yearArray)
            {
                alert.AddAction(UIAlertAction.Create(year, UIAlertActionStyle.Default, (UIAlertAction obj) => handleYearTap(year)));

            }
            ShowViewController(alert, null);
        }
        public void handleYearTap(string data)
        {
            btnYear.SetTitle(data, UIControlState.Normal); 
            selectedYear = data;

        }
        partial void BtnCancelClick(NSObject sender)
        {
          
            this.DismissViewController(true, null);
        }

        partial void BtnDOwnloadBidClick(NSObject sender)
        {
            bool ISAllFA, IsPilot = false, isFA=false, isFO=false, isCP=false, isALLFO=false, isALLCP=false;
            ISAllFA = btnALLFA.Selected;
            IsPilot = btnALLPilot.Selected;
            string errorinfo = string.Empty;
            string Successinfo = string.Empty;
            UserId = txtUserId.Text;
            Password = txtPassword.Text;
             isFA = btnFA.Selected;
             isFO = btnFO.Selected;
             isCP = btnCP.Selected;
             isALLFO = btnALLFO.Selected;
             isALLCP = btnALLCP.Selected;
            string Selectedround = btnRound1.Selected ? "1" : "2";
            try
            {
                getSelectedDomiciles();
                
                //Loader
                AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
                LoadingOverlay overlay = new LoadingOverlay(app.Window.Frame, "Downloading");
                app.Window.Add(overlay);
                new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    bool isAutneticated = checkAuthentication();
                    var allpositions = WBidCollection.GetPositions().ToList();
                    if (isAutneticated)
                    {

                        
                        if ((ISAllFA && !IsPilot) || isFA)
                        {
                            allpositions = allpositions.Where(x => x.LongStr == "FA").ToList();
                        }
                        else if (!ISAllFA && IsPilot)
                        {
                            allpositions = allpositions.Where(x => x.LongStr == "CP" || x.LongStr == "FO").ToList();
                        }
                        else if (isCP || isALLCP)
                        {
                            allpositions = allpositions.Where(x => x.LongStr == "CP").ToList();
                        }
                        else if (isFO || isALLFO)
                        {
                            allpositions = allpositions.Where(x => x.LongStr == "FO").ToList();
                        }
                        

                        InvokeOnMainThread(() =>
                        {

                            UIStoryboard ObjStoryboard = UIStoryboard.FromName("Main", null);
                            DownloadBidDataViewController objDownloadData = ObjStoryboard.InstantiateViewController("DownloadBidDataViewController") as DownloadBidDataViewController;

                            foreach (var position in allpositions)
                            {
                                foreach (var domicile in selectedBases)
                                {
                                    GlobalSettings.DownloadBidDetails = new BidDetails();
                                    GlobalSettings.DownloadBidDetails.Month = WBidCollection.GetBidPeriods().FirstOrDefault(x => x.Period == selectedMonth).BidPeriodId;
                                    GlobalSettings.DownloadBidDetails.Domicile = domicile;
                                    GlobalSettings.DownloadBidDetails.Postion = position.LongStr;
                                    GlobalSettings.DownloadBidDetails.Round = (Selectedround == "1") ? "D" : "B";
                                    GlobalSettings.DownloadBidDetails.Year = int.Parse(selectedYear);
                                    objDownloadData._objDownloadInfoDetails = new DownloadInfo();
                                    objDownloadData._objDownloadInfoDetails.UserId = UserId;
                                    objDownloadData._objDownloadInfoDetails.SessionCredentials = SessionCredential;
                                    bool isSuccess = objDownloadData.DownloadAndSaveBidDataFromWBid(false, true);
                                    if (isSuccess == false)
                                    {
                                        errorinfo += domicile + "- " + position.LongStr + "- Round" + Selectedround + "\r\n";
                                    }
                                    else
                                    {
                                        Successinfo += domicile + "- " + position.LongStr + "- Round" + Selectedround + "--" + DateTime.Now.ToShortTimeString() + "\r\n";
                                    }
                                }
                            }

                        });
                    }
                    InvokeOnMainThread(() =>
                    {
                        overlay.Hide();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("HandleReload", null);

                        string message = "Following datas were downloaded\n\r" + Successinfo + "\n\r\n\rFollowing datas were not downloaded\n\r" + errorinfo;
                        UIAlertController AlertController = UIAlertController.Create("Download Info", message, UIAlertControllerStyle.Alert);
                        AlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                        {

                            this.DismissViewController(true, null);
                        }));

                        this.PresentViewController(AlertController, true, null);

                    });


                })).Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private bool checkAuthentication()
        {
            try
            {
                if (Reachability.CheckVPSAvailable())
                {

                    SWAAuthentication authentication = new SWAAuthentication();
                    string authResult = authentication.CheckCredential(UserId, Password);
                    if (authResult.Contains("ERROR: ") || authResult.Contains("Exception"))
                    {
                        return false;
                    }
                    else
                    {
                        SessionCredential = authResult;
                        return true;
                    }
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getSelectedDomiciles()
        {
            if (btnATL.Selected)
                selectedBases.Add("ATL");
            if (btnAUS.Selected)
                selectedBases.Add("AUS");
            if (btnBWI.Selected)
                selectedBases.Add("BWI");
            if (btnDAL.Selected)
                selectedBases.Add("DAL");
            if (btnDEN.Selected)
                selectedBases.Add("DEN");
            if (btnFLL.Selected)
                selectedBases.Add("FLL");
            if (btnHOU.Selected)
                selectedBases.Add("HOU");
            if (btnLAS.Selected)
                selectedBases.Add("LAS");
            if (btnLAX.Selected)
                selectedBases.Add("LAX");
            if (btnMCO.Selected)
                selectedBases.Add("MCO");
            if (btnMDW.Selected)
                selectedBases.Add("MDW");
            if (btnOAK.Selected)
                selectedBases.Add("OAK");
            if (btnPHX.Selected)
                selectedBases.Add("PHX");

        }
        public void setCPFOSelected()
        {
            btnAUS.Enabled = false;
            btnFLL.Enabled = false;
            btnAUS.Selected = false;
            btnFLL.Selected = false;
            this.setSelected();
        }
        public void setFASelected()
        {
            btnAUS.Enabled = true;
            btnFLL.Enabled = true;
            btnAUS.Selected = !btnAUS.Selected;
            btnFLL.Selected = !btnFLL.Selected;
            this.setSelected();
        }

        public void setSelected()
        {

            btnATL.Selected = !btnATL.Selected;
            btnBWI.Selected = !btnBWI.Selected;
            btnHOU.Selected = !btnHOU.Selected;
            btnDAL.Selected = !btnDAL.Selected;
            btnDEN.Selected = !btnDEN.Selected;
            btnLAS.Selected = !btnLAS.Selected;
            btnLAX.Selected = !btnLAX.Selected;
            btnMCO.Selected = !btnMCO.Selected;
            btnMDW.Selected = !btnMDW.Selected;
            btnOAK.Selected = !btnOAK.Selected;
            btnPHX.Selected = !btnPHX.Selected;
        }
        public void ResetButtons()
        {
            btnAUS.Selected = false;
            btnFLL.Selected = false;
            btnAUS.Enabled = true;
            btnFLL.Enabled = true;
            btnATL.Selected = false;
            btnBWI.Selected = false;
            btnHOU.Selected = false;
            btnDAL.Selected = false;
            btnDEN.Selected = false;
            btnLAS.Selected = false;
            btnLAX.Selected = false;
            btnMCO.Selected = false;
            btnMDW.Selected = false;
            btnOAK.Selected = false;
            btnPHX.Selected = false;
            btnALLCP.Selected = false;
            btnALLFO.Selected = false;
            btnALLFA.Selected = false;
            btnALLPilot.Selected = false;
            btnFA.Selected = false;
            btnCP.Selected = false;
            btnFO.Selected = false;
        }

    }
}

