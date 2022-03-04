using System;

using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.App;

namespace Bidvalet.Droid
{
	[Activity (Label = "MainActivity",Theme = "@style/Bid.Theme")]
	public class MainActivity : Activity
	{
		int _expiredDay = 0;	
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			Button btnNewDownload = FindViewById<Button> (Resource.Id.btnNewDownload);
			Button btnValidSubscription = FindViewById<Button> (Resource.Id.btnValidSubscription);
			Button btnExpiredSubsciption = FindViewById<Button> (Resource.Id.btnExpiredSubscription);
			
			btnNewDownload.Click += delegate {
				IntentViewWithParams(BidEnum.BidStartState.NEW_DOWNLOAD);
			};
			btnValidSubscription.Click += delegate {
				IntentViewWithParams(Bidvalet.Droid.BidEnum.BidStartState.VALID_SUB);
			};
			btnExpiredSubsciption.Click += delegate {
				IntentViewWithParams(Bidvalet.Droid.BidEnum.BidStartState.EXPIRED_SUB);
			};
		}

		private void IntentViewWithParams(Bidvalet.Droid.BidEnum.BidStartState state){
			var activityHome = new Intent (this, typeof(HomeActivity));
			switch (state) {
			case Bidvalet.Droid.BidEnum.BidStartState.NEW_DOWNLOAD:
				activityHome.PutExtra (Constants.START_STATE, Constants.STARE_NEW_DOWNLOAD);
				_expiredDay = 0;
				break;
			case Bidvalet.Droid.BidEnum.BidStartState.VALID_SUB:
				activityHome.PutExtra (Constants.START_STATE, Constants.STARE_VALID_SUBSCRIPTION);
				_expiredDay = 6;
				break;
			case Bidvalet.Droid.BidEnum.BidStartState.EXPIRED_SUB:
				activityHome.PutExtra (Constants.START_STATE, Constants.STARE_EXPIRED_SUBSCRIPTION);
				_expiredDay = -10;
				break;
			}
			activityHome.PutExtra (Constants.DATE_EXPIRED, _expiredDay);
			StartActivity (activityHome);
		}

	}
}


