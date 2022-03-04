// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using System.Linq;
using Bidvalet.Business;

namespace Bidvalet.iOS
{
	public partial class SelectSeatAndRoundViewController : BaseViewController
	{

		public string baseSelected;
		string seatSelected;

		public SelectSeatAndRoundViewController (IntPtr handle) : base (handle){		
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ShowNavigationBar ();
			Title = "Select Seat & Round";
			tvSeatAndRound.Source = new SeatAndRoundSources (Constants.Seat,Constants.Round, this);
			tvSeatAndRound.ReloadData();
			tvSeatAndRound.BackgroundColor = UIColor.Clear;
			UIView footerView = new UIView ();
			footerView.BackgroundColor = Colors.BidBgTable;
			tvSeatAndRound.TableFooterView = footerView;
		}

		public void RowSelected(string seatValue, string roundValue)
		{
			if (seatValue.Equals (Constants.listSubmittals.ElementAt(0))) {
				seatSelected = "CP";
			}
			if (seatValue.Equals (Constants.listSubmittals.ElementAt(1))) {
				seatSelected = "FO";
			}
			if (seatValue.Equals (Constants.listSubmittals.ElementAt(2))) {
				seatSelected = "FA";
			}
			// for section  = 1
			LoginViewController loginViewController = Storyboard.InstantiateViewController ("LoginViewController") as LoginViewController;
			
			if ((GlobalSettings.DownloadBidDetails == null)) {
				GlobalSettings.DownloadBidDetails = new BidDetails ();

			}
			GlobalSettings.DownloadBidDetails.Domicile = baseSelected;
			GlobalSettings.DownloadBidDetails.Postion = seatSelected;
			GlobalSettings.DownloadBidDetails.Round = roundValue=="1st Round"?"D":"B";
			string monthName = WBidCollection.GetMonthName(GlobalSettings.DownloadBidDetails.Month);

			string logText = string.Format ("Get {0}-{1}-{2}-{3}", baseSelected, seatSelected, roundValue, monthName);
			loginViewController.loginTitle = logText;
			//loginViewController.isRecentBidDownload = false;
			PushViewController (loginViewController, true);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}
	}
}
