using System;
using System.Collections.Generic;
using System.IO;
using Bidvalet.Business;
using UIKit;
using Bidvalet.iOS.Utility;
using System.Linq;

namespace Bidvalet.iOS
{
	public partial class BidAwardView : UIViewController
	{
		public SubScriptionViewController objSubscriptionView;
		string pagetitle;
		List<Data> lstdata;
		
		public BidAwardView() : base("BidAwardView", null)
		{
		}
		 public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			this.NavigationController.NavigationBarHidden = false;
            this.Title = "Bid Awards";
			collectionviewAwards.Source = new BidAwardSource(GetAvailableAwardfile(), this);
			
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//GetAvailableAwardfile();
			CommonClass.objAward = this;
   //         this.Title = "Bid Awards";
			//collectionviewAwards.Source = new BidAwardSource(GetAvailableAwardfile(),this);
			
			this.NavigationController.NavigationBarHidden = false;

			var barbtnreterive = new  UIBarButtonItem("Retrieve", UIBarButtonItemStyle.Plain, delegate
			   {
				   
				   GlobalSettings.isAwardDownload = true;
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
			   });
			this.NavigationItem.RightBarButtonItem = barbtnreterive;
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		private List<Data> GetAvailableAwardfile()
		{
			lstdata = new List<Data>();
			if (!Directory.Exists(WBidHelper.GetBidAwardPath()))
			{
				Directory.CreateDirectory(WBidHelper.GetBidAwardPath());
			}
			DirectoryInfo directory = new DirectoryInfo(WBidHelper.GetBidAwardPath());
			pagetitle="Bid Awards";
			var title=directory.GetDirectories().Select(x => x.Name);
			if (title.FirstOrDefault() != null )
            {
				pagetitle = pagetitle + " in " + title.FirstOrDefault();
                this.Title = "Bid Awards(" + title.FirstOrDefault().Substring(0,3) + ")";
            }
			
			
			
			FileInfo[] files = directory.GetFiles("*.TXT",SearchOption.AllDirectories);

			foreach (FileInfo file in files)
			{
				string round = file.Name.Substring(5, 1) == "M" ? "Round 1" : "Round 2";
				lstdata.Add(new Data { filename = file.Name, Name = file.Name.Substring(0, 3) + "-" + file.Name.Substring(3, 2) + " " + round ,Round=(round=="Round1")?1:2});
			}lstdata.Sort((Data x, Data y) => x.Round);
			return lstdata;
		}
		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
		partial void btnCancelClicked(Foundation.NSObject sender)
		{
			
			this.NavigationController.PopViewController(true);
		}
		partial void btnRetriveclicked(Foundation.NSObject sender)
		{
			
			GlobalSettings.isAwardDownload = true;
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

			//this.NavigationController.PushViewController
			////this.DismissViewController(true, null);
			//objSubscriptionView.showBaseSelectionView();
		
		}
		public void ShowBidAward(string fileName)
		{ 
			//InvokeOnMainThread(() =>
			//			{
			//	webPrint fileViewer = new webPrint();
			//				//fileViewer.strUrl = fileName;
			//				fileViewer.loadFileFromUrl(fileName);
			//				this.NavigationController.PushViewController(fileViewer, true);
							
			//			});
			InvokeOnMainThread(() =>
			{
				webPrint fileViewer = new webPrint();
				fileViewer.strUrl = fileName;
				this.NavigationController.PushViewController(fileViewer, true);

			});

		}
		private void DisplayAlertView(string caption, string message)
		{
			new UIAlertView(caption, message, null, "OK", null).Show();
		}
		public class Data
		{
			public string filename
		{
			get;
			set;
		}
			public string Name
			{
				get;
				set;
			}
			public int Round
			{
				get;
				set;
			}
		}
	}
}

