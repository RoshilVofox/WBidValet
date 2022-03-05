using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using System.Linq;
using Bidvalet.Model;
using Bidvalet.Business;
using System.IO;
using System.Globalization;

namespace Bidvalet.iOS.ViewControllers.HistoryBidData
{
    public partial class HistoryBidDataRetrieve : UIViewController
    {
        #region Initialization
        static string selectedBase = string.Empty;
        string selectedPosition = string.Empty;
        string selectedRound = string.Empty;
        string selectedYear = string.Empty;
        //static int selectedMonth = 0;
        //string currentYear = DateTime.Now.Year.ToString();
        //string prevYear = (DateTime.Now.Year - 1).ToString();
        //string oldYear = (DateTime.Now.Year - 2).ToString();
        static UIColor defaultButtonColor = UIColor.SystemGray2Color;
        static UIColor selectedColor = UIColor.LinkColor;
        static string monthName;
        List<string> listDomicile;
        //string[] monthList;
        BidDetails bidDetail = new BidDetails();
        #endregion


        public HistoryBidDataRetrieve() : base("HistoryBidDataRetrieve", null)

        {
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);




        }
        public override void ViewDidLoad()
        {

            base.ViewDidLoad();
            this.NavigationController.NavigationBarHidden = false;
            this.Title = "History Bid Data";
            collectionView.RegisterNibForCell(HistoryCell.Nib, HistoryCell.Key);
            monthCollectionView.RegisterNibForCell(HistoryCell.Nib, HistoryCell.Key);
            //ResetButtons();
            setUIData();


            // Perform any additional setup after loading the view, typically from a nib.

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
        public void ItemSelected(string value)
        {

            selectedBase = value;

        }
       
        public void setUIData()
        {
            //Domicile collection 
            listDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(x => x.DomicileName).ToList();

            collectionView.Source = new HistoryBidDataSource(listDomicile);

            
            //Current Year
            string[] year = new string[] {
                    (DateTime.Now.Year - 2).ToString (),
                    (DateTime.Now.Year - 1).ToString (),
                    DateTime.Now.Year.ToString ()
                };
            foreach (var btn in btnYearCollection)
            {
                btn.SetTitleColor(UIColor.White, UIControlState.Selected);
                btn.TitleLabel.Font = UIFont.SystemFontOfSize(15);

                btn.SetTitle(year[btn.Tag], UIControlState.Normal);
            }
            changedSelectionState(btnYearCollection[2]);
            bidDetail.Year = Convert.ToInt32(btnYearCollection[2].CurrentTitle);

            setMonthData(DateTime.Now.Month);//current month UI settings
            SetUiMonthList();
            ///download button
            downloadButton.Enabled = btnMonthCollection.Any(x => x.Selected);
            downloadButton.BackgroundColor = UIColor.DarkGray;
            if (downloadButton.Enabled)
                downloadButton.BackgroundColor = UIColor.LinkColor;

        }
       
        private void setMonthData(int month)
        {
           
            var appliedMonth = WBidCollection.GetMonthName(month);
            var objdata = btnMonthCollection.Where(x => appliedMonth.Substring(0, 3).ToLower() == x.CurrentTitle.ToLower()).ToList();
            foreach (var button in objdata)
            {
                changedSelectionState(button);

                bidDetail.Month = (int)button.Tag;
            }
        }
        partial void positionButtonAction(NSObject sender)
        {

            var btn = (UIButton)sender;
            if (btn.Tag == 0)
            {

                cpButton.BackgroundColor = selectedColor;
                foButton.BackgroundColor = defaultButtonColor;
                faButton.BackgroundColor = defaultButtonColor;
                selectedPosition = "CP";
            }
            else if (btn.Tag == 1)
            {
                foButton.BackgroundColor = selectedColor;
                faButton.BackgroundColor = defaultButtonColor;
                cpButton.BackgroundColor = defaultButtonColor;
                selectedPosition = "FO";
            }
            else if (btn.Tag == 2)
            {
                faButton.BackgroundColor = selectedColor;
                cpButton.BackgroundColor = defaultButtonColor;
                foButton.BackgroundColor = defaultButtonColor;
                selectedPosition = "FA";
            }
        }
        partial void RoundButtonAction(NSObject sender)
        {

            var btn = (UIButton)sender;
            if (btn.Tag == 0)
            {
                round1Button.BackgroundColor = selectedColor;
                round2Button.BackgroundColor = defaultButtonColor;
                selectedRound = "D";
            }
            else if (btn.Tag == 1)
            {
                round2Button.BackgroundColor = selectedColor;
                round1Button.BackgroundColor = defaultButtonColor;
                selectedRound = "B";
            }
            bidDetail.Round = selectedRound;
        }
        private void SetBidYear()
        {
            var btnYr = btnYearCollection.FirstOrDefault(x => x.Selected == true);
            bidDetail.Year = int.Parse(btnYr.Title(UIControlState.Normal));

        }
        partial void btnMonthTapped(Foundation.NSObject sender)
        {
            foreach (UIButton aBtn in this.btnMonthCollection)
            {
                if (aBtn.Enabled)
                {
                    aBtn.Selected = false;
                    aBtn.BackgroundColor = UIColor.SystemGray2Color;
                }
            }

            UIButton theBtn = (UIButton)sender;
            changedSelectionState(theBtn);

            bidDetail.Month = (int)theBtn.Tag;


        }
        partial void btnYearTap(UIKit.UIButton sender)
        {
            foreach (var btn in btnYearCollection)
            {
                btn.Selected = false;
                btn.BackgroundColor = UIColor.SystemGray2Color;
            }

            changedSelectionState(btnYearCollection[sender.Tag]);
            bidDetail.Year = Convert.ToInt32(btnYearCollection[sender.Tag].CurrentTitle);

            SetUiMonthList();
            downloadButton.Enabled = btnMonthCollection.Any(x => x.Selected);
            downloadButton.BackgroundColor = UIColor.LinkColor;
        }

        void changedSelectionState(UIButton button)
        {
            button.Selected = true;
            button.BackgroundColor = UIColor.LinkColor;
        }

        void SetUiMonthList()
        {
            var monthList = WBidCollection.GetBidPeriods();
            int currentMonth = DateTime.Now.Month;
            List<int> downloadedMonths = monthList.Select(x => x.BidPeriodId).ToList();
            int year = Convert.ToInt32(bidDetail.Year);
            string domicile = bidDetail.Domicile;
            string position = bidDetail.Postion;
            int round = (bidDetail.Round == "D") ? 1 : 2;

            foreach (var button in btnMonthCollection)
            {
                button.Enabled = false;
                button.Selected = false;
                button.BackgroundColor = UIColor.DarkGray;
            }
            if (bidDetail.Year == DateTime.Now.Year)
            {
                setMonthData(DateTime.Now.Month);
                 downloadedMonths = monthList.Where(x => x.BidPeriodId <= currentMonth).Select(x => x.BidPeriodId).ToList();
            }
            else
                setMonthData(bidDetail.Month);

            var objdata = btnMonthCollection.Where(x => downloadedMonths.Any(y => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(y).Substring(0, 3).ToLower() == x.CurrentTitle.ToLower())).ToList();
            foreach (var button in objdata)
            {

                button.Enabled = true;
                button.BackgroundColor = UIColor.SystemGray2Color;
                if (button.Selected)
                    changedSelectionState(button);
            }

        }
        
        partial void downloadButtonAction(NSObject sender)
        {
            
            SetBidYear();
            bidDetail.Domicile = selectedBase;
            bidDetail.Postion = selectedPosition;
           
            monthName = WBidCollection.GetMonthName(bidDetail.Month);
            GlobalSettings.DownloadBidDetails = bidDetail;
            string roundName = selectedRound == "D" ? "Round 1" : "Round 2";
            //redirect to login page
            UIStoryboard ObjStoryboard = UIStoryboard.FromName("Main", null);
            LoginViewController loginViewController = ObjStoryboard.InstantiateViewController("LoginViewController") as LoginViewController;
            string logText = string.Format("Get {0}-{1}-{2}-{3}-{4}", selectedBase, selectedPosition, bidDetail.Year, monthName, roundName);
            loginViewController.loginTitle = logText;
            NavigationController.PushViewController(loginViewController, true);
        }
    }
}


