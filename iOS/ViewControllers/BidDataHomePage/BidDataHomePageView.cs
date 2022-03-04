using System;
using System.Collections.Generic;
using System.IO;
using Bidvalet.Business;
using UIKit;
using Bidvalet.iOS.Utility;
using System.Linq;
using CoreGraphics;
using Bidvalet.iOS.ViewControllers.HistoryBidData;
using Foundation;

namespace Bidvalet.iOS
{
    public partial class BidDataHomePageView : UIViewController
    {
        BidDataHomeSouceViewController collectionController;
        NSObject newNotif;
       
        public BidDataHomePageView() : base("BidDataHomePageView", null)
        {
        }


        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBarHidden = false;
          
            this.Title = "Bid Data Home";
            observeNotifications();
            


            //collectionBids.Source = new BidDataHomeSouce(GetAllDownloadedBids(), this);

        }
        public override void ViewDidDisappear(bool animated)
        {

            base.ViewDidDisappear(animated);
            if (newNotif != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(newNotif);

                newNotif = null;
            }
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnDeleteBids.Hidden = true;
            btnViewHeight.Constant = 0;
            setViews();
           
            //this.collectionVw.Layout.layo = this.View.SafeAreaLayoutGuide
            this.NavigationController.NavigationBarHidden = false;

            //var barbtnreterive = new UIBarButtonItem("Retrieve", UIBarButtonItemStyle.Plain, delegate
            //{

            //    //Checking internet connection Type
            //    var typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
            //    // typeOfInternetConnection = (int)InternetType.Air;
            //    //No internet connection
            //    if (typeOfInternetConnection == (int)InternetType.NoInternet)
            //    {
            //        // DisplayAlertView(GlobalSettings.ApplicationName, "Sorry!\n\nYou do not have an Internet connection.\n\nYou will need to establish an internet connection to continue.");
            //        DisplayAlertView(GlobalSettings.ApplicationName, Constants.VPSDownAlert);

            //    }
            //    else if (typeOfInternetConnection == (int)InternetType.Air)
            //    {
            //        DisplayAlertView(GlobalSettings.ApplicationName, Constants.SouthWestConnectionAlert);
            //    }
            //    else
            //    {
            //        GlobalSettings.IsDownloadProcess = true;
            //        UIStoryboard ObjStoryboard = UIStoryboard.FromName("Main", null);
            //        SelectBaseViewController selectBaseViewController = ObjStoryboard.InstantiateViewController("SelectBaseViewController") as SelectBaseViewController;
            //        this.NavigationController.PushViewController(selectBaseViewController, true);
            //    }
            //});

            //this.NavigationItem.RightBarButtonItem = barbtnreterive;

            var bidreteriveBtn = new UIBarButtonItem();
            var icoFontAttribute = new UITextAttributes { Font = UIFont.BoldSystemFontOfSize(16) };
            bidreteriveBtn.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);

            bidreteriveBtn.Title = "Retrieve";

            bidreteriveBtn.Style = UIBarButtonItemStyle.Plain;
            bidreteriveBtn.Clicked += (sender, args) =>
            {

                try
                {

                    string[] arr = new string[]{
                    "Retrieve New Bid",
                    "Retrieve History Bid"

                };
                    UIActionSheet sheet = new UIActionSheet("Select", null, "Cancel", null, arr);
                    CGRect senderframe = bidreteriveBtn.AccessibilityFrame;
                    senderframe.X = bidreteriveBtn.AccessibilityFrame.GetMidX();
                    sheet.ShowFrom(bidreteriveBtn, true);
                    sheet.Dismissed += handleBidRetrieveTap;
                }
                catch (Exception)
                {

                }

            };
            NavigationItem.RightBarButtonItem = bidreteriveBtn;

        }

        public void handleBidRetrieveTap(object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                try
                {
                    GlobalSettings.IsHistorical = false;
                    //Checking internet connection Type
                    var typeOfInternetConnection = InternetHelper.CheckInterNetConnection();
                    // typeOfInternetConnection = (int)InternetType.Air;
                    //No internet connection
                    if (typeOfInternetConnection == (int)InternetType.NoInternet)
                    {
                        // DisplayAlertView(GlobalSettings.ApplicationName, "Sorry!\n\nYou do not have an Internet connection.\n\nYou will need to establish an internet connection to continue.");
                        DisplayAlertView(GlobalSettings.ApplicationName, Constants.VPSDownAlert);

                    }
                    else if (typeOfInternetConnection == (int)InternetType.Air)
                    {
                        DisplayAlertView(GlobalSettings.ApplicationName, Constants.SouthWestConnectionAlert);
                    }
                    else
                    {
                        GlobalSettings.IsDownloadProcess = true;
                        UIStoryboard ObjStoryboard = UIStoryboard.FromName("Main", null);
                        SelectBaseViewController selectBaseViewController = ObjStoryboard.InstantiateViewController("SelectBaseViewController") as SelectBaseViewController;
                        this.NavigationController.PushViewController(selectBaseViewController, true);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else if (e.ButtonIndex == 1)
            {
                //
                UIAlertController alert = UIAlertController.Create("WBidMax", "When viewing Historical Bid Data, Vacation Correction will not be available.\n\nNor will you be able to accidentally submit a bid using the Historical Bid Data.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (actionCancel) =>
                {
                    GlobalSettings.IsHistorical = true;

                    HistoryBidDataRetrieve ObjhistoryData = new HistoryBidDataRetrieve();
                    NavigationController.PushViewController(ObjhistoryData, true);
                }));
                this.PresentViewController(alert, true, null);
            }



            UIActionSheet obj = (UIActionSheet)sender;
            obj.Dispose();
        }
        
        private void setViews()
        {
            var layout = new UICollectionViewFlowLayout();
            layout.SectionInset = new UIEdgeInsets(20, 20, 20, 20);
            layout.MinimumInteritemSpacing = 20;
            layout.MinimumLineSpacing = 20;
            layout.ItemSize = new CGSize(this.View.Frame.Width-90, 60);

            collectionController = new BidDataHomeSouceViewController(layout);

            //collectionController.View.Frame = containerView.Frame;
            collectionController.overlayFrame= new CGRect(containerView.Frame.X, containerView.Frame.Y - 100, containerView.Frame.Width, containerView.Frame.Height);
            collectionController.View.Frame = new CGRect(containerView.Frame.X, containerView.Frame.Y-90, containerView.Frame.Width, containerView.Frame.Height);

            this.AddChildViewController(collectionController);
            this.containerView.Add(collectionController.View);
        }
        
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void btnDeleteBidsClicked(UIButton sender)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName("HandleBidDelete", null);
            
        }
        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();
        }
        private void observeNotifications()
        {         
            newNotif = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("showDeleteButton"), showDeleteButton);
        }
        private void showDeleteButton(NSNotification obj)
        {
            btnDeleteBids.Hidden = true;
            btnViewHeight.Constant = 0;
            int status = Convert.ToInt32(obj.Object.ToString());
            if (status == 1)
            {
                btnViewHeight.Constant = 50;
                btnDeleteBids.Hidden = false;
            }
            //collectionController.View.Frame = new CGRect(0, 150, this.View.Frame.Width, (viewHeight - 150) - this.View.SafeAreaInsets.Bottom);








        }
    }
}

