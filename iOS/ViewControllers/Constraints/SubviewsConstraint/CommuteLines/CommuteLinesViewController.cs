using System;
using UIKit;
using Bidvalet.Model;
using System.Linq;
using Foundation;
using Bidvalet.iOS.Utility;

namespace Bidvalet.iOS
{
	public partial class CommuteLinesViewController : BaseViewController
	{

        bool _isFirstTime;
	    private string _cityName;
		LoadingOverlay loadingOverlay;
		bool isNonStopSelected = false;
		public FtCommutableLine data {
			get;
			set;
		}

		public CommuteLinesViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			btnNonStop.Selected = data.isNonStop;
			// Perform any additional setup after loading the view, typically from a nib.
			ShowNavigationBar();
			Title = Constants.CONSTRAINTS;
//			UIHelpers.StyleForButtonsBorderBlackRectange (new UIButton[]{btnViewCommuteTime, btnDoneSetting});
			UIHelpers.StyleForButtonsBorderBlackRectangeThin (new UIButton[]{
				btnCityName, btnBackToBase, btnTimeCheckIn, btnTimeConnect
			});
			UIHelpers.StyleForButtonsWithBorder (new UIButton[]{ btnViewCommuteTime, btnDoneSetting });
			string city = data.City;
			if (city == null || city == "") {
                _isFirstTime = true;
				btnCityName.SetTitle ("Select", UIControlState.Normal);	
			} else {
                _isFirstTime = false; ;
				btnCityName.SetTitle(data.City,UIControlState.Normal);
			    _cityName = data.City;
			}
			if (data.BaseTime < 1) {
				data.BaseTime = 10;
			}

			if (data.ConnectTime < 1) {
				data.ConnectTime = btnNonStop.Selected ? 0 : 30;
				
			}
			if (data.CheckInTime < 1) {
				data.CheckInTime = 60;
			}
			
			
			SetTitleForButton (data.BaseTime, btnBackToBase);
			SetTitleForButton (data.ConnectTime, btnTimeConnect);
			SetTitleForButton (data.CheckInTime, btnTimeCheckIn);


		}
		
		void SetTitleForButton (double minutes, UIButton btn)
		{
			//int hour = (int)minutes/60;
			//int mins = (int)minutes%60;
			//string minStr = mins.ToString();
			//if (mins < 10) {
			//	minStr = string.Format("0{0}",mins);
			//}
			//btn.SetTitle(string.Format("{0}:{1}", hour , minStr), UIControlState.Normal);


			int hour = (int)minutes / 60;
			int mins = (int)minutes % 60;
			string minStr = mins.ToString("00");
			string hourStr = hour.ToString("00");

			if (mins < 10 && minutes > 0)
			{
				minStr = string.Format("0{0}", mins);
			}
			else if (minutes == 0)
			{
				hourStr = "--";
				minStr = "--";

			}

			btn.SetTitle(string.Format("{0}:{1}", hourStr, minStr), UIControlState.Normal);
		}

	    public override void ViewWillAppear(bool animated)
	    {
	        base.ViewWillAppear(animated);

	        var wBIdStateContent =
	            GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
	                x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
	        btnViewCommuteTime.Enabled =
	            btnDoneSetting.Enabled =
	                (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.DailyCommuteTimes != null &&
	                 wBIdStateContent.BidAuto.DailyCommuteTimes.Count > 0 &&
	                 wBIdStateContent.BidAuto.DailyCommuteTimes.Any(
	                     x => x.EarliestArrivel != DateTime.MinValue || x.LatestDeparture != DateTime.MinValue));
	       
            
            //if (btnViewCommuteTime.Enabled)
            //{
            //    data.City = _cityName;
            //    btnCityName.SetTitle(_cityName, UIControlState.Normal);
              
               
            //}
            //else
            //{
            //    btnCityName.SetTitle("Select", UIControlState.Normal);

            //}

          
	    }
        partial void btnNonStopClick(UIButton sender)
        {
			btnNonStop.Selected = this.data.isNonStop = !sender.Selected;
			isNonStopSelected = btnNonStop.Selected;
			data.ConnectTime = btnNonStop.Selected ? 0 : 30;
			SetTitleForButton(data.ConnectTime, btnTimeConnect);
			if (_cityName != null && _cityName != "Select")
			{

				calculateDialyCommuteLines();

			}
		}
        
		public void calculateDialyCommuteLines()
		{
			

				loadingOverlay = new LoadingOverlay(new CoreGraphics.CGRect(View.Bounds.X, 1, View.Bounds.Width, View.Bounds.Height), "Loading...");
				View.Add(loadingOverlay);

				InvokeInBackground(() =>
				{

					BidAutoCalculateCommuteTimes bidAutoCalculateCommuteTimes = new BidAutoCalculateCommuteTimes();
					bidAutoCalculateCommuteTimes.CalculateDailyCommutableTimes(_cityName, isNonStopSelected);

					InvokeOnMainThread(() => {

						if (bidAutoCalculateCommuteTimes == null) return;

						if (bidAutoCalculateCommuteTimes.ErrorMessage != string.Empty)
						{
							DisplayAlertView("WBidMax", bidAutoCalculateCommuteTimes.ErrorMessage);
						}

						loadingOverlay.Hide();

					});
				});
			
		}

		private void DisplayAlertView(string caption, string message)
		{
			loadingOverlay.Hide();

			UIWindow WindowAlert = new UIWindow(UIScreen.MainScreen.Bounds);
			WindowAlert.RootViewController = new UIViewController();
			UIAlertController okAlertController = UIAlertController.Create(caption, message, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) => { WindowAlert.Dispose(); }));
			WindowAlert.MakeKeyAndVisible();
			WindowAlert.RootViewController.PresentViewController(okAlertController, true, null);
			//WindowAlert.Dispose();


		}
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		partial void OnBackToBaseEvent (Foundation.NSObject sender){
			UIActionSheet sheet = new UIActionSheet("");
			for (int i = 5; i <= 60; i+=5) {
				string title = string.Format("{0}:{1}", (i/60).ToString("00"), (i%60).ToString("00"));
				sheet.AddButton(title);
			}
			sheet.Clicked += (object sd, UIButtonEventArgs e) => {
				double minutes =(e.ButtonIndex+1)*5;
				data.BaseTime =(int) minutes;
				SetTitleForButton(minutes,btnBackToBase );
			};
			sheet.ShowInView(this.View);
		}

		partial void OnDoneSettingEvent (Foundation.NSObject sender){

           var  wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
           if (wBIdStateContent.BidAuto.DailyCommuteTimes != null)
           {

               if (_isFirstTime)
               {
                   SharedObject.Instance.ListConstraint.Add(this.data);
               }

               data.City = _cityName;
           }
			NavigationController.PopViewController(true);
		}

		partial void OnPadCheckInEvent (Foundation.NSObject sender){
			UIActionSheet sheet = new UIActionSheet("");
			for (int i = 5; i <= 120; i+=5) {
				string title = string.Format("{0}:{1}", (i/60).ToString("00"), (i%60).ToString("00"));
				sheet.AddButton(title);
			}
			sheet.Clicked += (object sd, UIButtonEventArgs e) => {
				double minutes =(e.ButtonIndex+1)*5;
				data.CheckInTime =(int) minutes;
				SetTitleForButton(minutes,btnTimeCheckIn);
			};
			sheet.ShowInView(this.View);
		}
		CitiesPickerVC _picker;
		partial void OnSetCityNameEvent (Foundation.NSObject sender){
			// show picker
			if (_picker == null)
			{
				_picker = Storyboard.InstantiateViewController("CitiesPickerVC") as CitiesPickerVC;
				_picker.isNonStop = btnNonStop.Selected;
				_picker.PickedItem += (string value) =>
				{
					_cityName = value;
					//data.City = value;
					btnCityName.SetTitle(value, UIControlState.Normal);
				};
			}
			else
			{
				_picker.isNonStop = btnNonStop.Selected;
			}
			NavigationController.PushViewController (_picker, true);
		}

		partial void OnSetConnectTimeEvent (Foundation.NSObject sender){
			// show alert to select time
			UIActionSheet sheet = new UIActionSheet("");
			for (int i = 5; i <= 60; i+=5) {
				string title = string.Format("{0}:{1}", (i/60).ToString("00"), (i%60).ToString("00"));
				sheet.AddButton(title);
			}
			sheet.Clicked += (object sd, UIButtonEventArgs e) => {
				double minutes =(e.ButtonIndex+1)*5;
				data.ConnectTime =(int) minutes;
				SetTitleForButton(minutes, btnTimeConnect);
			};
			sheet.ShowInView(this.View);
		}

		partial void OnViewCommuteTime (Foundation.NSObject sender){
			CommutableTimeViewController _time = Storyboard.InstantiateViewController ("CommutableTimeViewController")as CommutableTimeViewController;
			_time.data=data;
		    _time.CityName = _cityName;
			this.PresentViewController(_time,true,null);
//			UIAlertView alert =new UIAlertView(Constants.AppName, "Not yet functional", null, "OK",null);
//			alert.Show();
		}

		partial void OnInfoCommuteCityEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Set_Commute_City, Constants.Set_Commute_City_Message);
		}

		partial void OnInfoConnectTimeEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Set_Connect_Times, Constants.Set_Connect_Times_Message);
		}

		partial void OnInfoPadCheckInEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Pad_For_Checkin, Constants.Pad_For_Checkin_Msg);
		}
		partial void OnInfoBackToBaseEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Pad_For_Back_To_Base, Constants.Pad_For_Back_To_Base_Msg);
		}

		partial void OnInfoViewCommuteTimeEvent (Foundation.NSObject sender){
			ShowPopUpInfo(Constants.Get_Commute_Times, Constants.Get_Commute_Times_Message);
		}


	}
}


