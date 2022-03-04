
using System;

using Foundation;
using UIKit;
using System.IO;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
	public partial class FirstViewController : BaseViewController
	{
		
		public FirstViewController (IntPtr handle) : base (handle)
		{		
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//File.Delete (WBidHelper.WBidUserFilePath);

			//PushViewController (true, DateTime.Now);GlobalSettings.UserInfo = null;
			//RedirectUserDifferenceScreen();
			PushViewController(true, DateTime.Now);

			//SetUpInitialView ();

		}

		private void RedirectTodownloadView()
		{
			DownloadBidDataViewController DownloadData = Storyboard.InstantiateViewController("DownloadBidDataViewController") as DownloadBidDataViewController;

			this.NavigationController.NavigationItem.HidesBackButton = false;

			this.PresentViewController (DownloadData, true, null);
			//PushViewController (DownloadData, true);

		}


		private void RedirectUserDifferenceScreen()
		{
			UserAccountDifferenceScreen ObjUserAccount = Storyboard.InstantiateViewController("UserAccountDifferenceScreen") as UserAccountDifferenceScreen;

			this.NavigationController.NavigationItem.HidesBackButton = false;

			this.PresentViewController (ObjUserAccount, true, null);
			//PushViewController (DownloadData, true);

		}

		private void SetUpInitialView()
		{
			//File.Delete (WBidHelper.WBidUserFilePath);
			if (File.Exists (WBidHelper.WBidUserFilePath)) {
				
				GlobalSettings.UserInfo = (UserInformation)XmlHelper.DeserializeFromXml<UserInformation> (WBidHelper.WBidUserFilePath);
				//File.Delete (WBidHelper.WBidUserFilePath);

				if (GlobalSettings.UserInfo != null) {
					PushViewController(false, GlobalSettings.UserInfo.PaidUntilDate??DateTime.MinValue);


				}

			} else {

				PushViewController(true, DateTime.Now);
				//Fresh User



			}
			
		}
		// events

		partial void OnExpiredBtnEvent (Foundation.NSObject sender)
		{
			PushViewController(false, DateTime.Now.AddDays(-10));
		}

		partial void OnNewDownloadBtnEvent (Foundation.NSObject sender){
			PushViewController(true, DateTime.Now);
		}

		partial void OnValidBtnEvent (Foundation.NSObject sender)
		{
			PushViewController(false, DateTime.Now.AddDays(5));
		}

		/*
		 * Internal functions
		 */


		void PushViewController (bool isFirstTimeUsing, DateTime expiredDate)
		{
			SubScriptionViewController subScriptionViewController = Storyboard.InstantiateViewController ("SubScriptionViewController") as SubScriptionViewController;
			subScriptionViewController.isFirstTimeUsing = isFirstTimeUsing;
			subScriptionViewController.expiredTime = expiredDate;
			PushViewController(subScriptionViewController, true);
		}
	}
}

