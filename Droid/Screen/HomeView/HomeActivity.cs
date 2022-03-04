
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Text;
using Java.Util;

namespace Bidvalet.Droid
{
	[Activity (Label = "HomeActivity", Theme="@style/Bid.Theme")]	
	public class HomeActivity : BaseActivity
	{
		Bidvalet.Droid.BidEnum.BidStartState _bidState;
		int _currentState= 0;
		int _dateExpired=0;
		TextView lbSubExpiredOn, lbSubDateExpired, lbGoSub, lbAppName;
		Button btnStart, btnViewBidReceipt, btnEditAccount;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.Home);
			_dateExpired = Intent.GetIntExtra (Constants.DATE_EXPIRED, 0);
			_currentState = Intent.GetIntExtra (Constants.START_STATE, 0);
			if (bundle != null) {
				OnRestoreInstanceState (bundle);
			}
			GetInitView ();
		}

		private void GetInitView(){
			lbAppName = FindViewById<TextView>(Resource.Id.lbAppName);
			lbSubExpiredOn = FindViewById<TextView>(Resource.Id.lbSubExpiredOn);
			lbSubDateExpired = FindViewById<TextView> (Resource.Id.lbSubDateExpired);
			lbGoSub = FindViewById<TextView> (Resource.Id.lbGoWebsite);
			btnStart = FindViewById<Button> (Resource.Id.btnStart);
			btnViewBidReceipt = FindViewById<Button> (Resource.Id.btnViewBidReceipt);
			btnEditAccount = FindViewById<Button> (Resource.Id.btnEditAccount);
			SetCustomFontTextView (new TextView[]{lbAppName}, "ZapfinoExtraLT-One.otf");
			UpdateControllHomeView ();
			btnStart.Click += delegate {
				NSBidLog("onStartClick");
				switch (_bidState) {
				case Bidvalet.Droid.BidEnum.BidStartState.NEW_DOWNLOAD:
				case Bidvalet.Droid.BidEnum.BidStartState.VALID_SUB:
					var activityBase = new Intent (this, typeof(SelectBaseActivity));
					StartActivity(activityBase);
					break;
				case Bidvalet.Droid.BidEnum.BidStartState.EXPIRED_SUB:
					var activityExpired = new Intent (this, typeof(ExpriedSubscriptionActivity));
					StartActivity(activityExpired);
					break;
				}
			};
			btnViewBidReceipt.Click += delegate {
				var activityBidReceipt = new Intent(this, typeof(BidReceiptActivity));
				StartActivity(activityBidReceipt);
			};
			btnEditAccount.Click += delegate {
				var editAccount = new Intent (this, typeof(CreateAccountActivity));
				editAccount.PutExtra(Constants.USER_EXIST, true);
				StartActivity(editAccount);
			};
		}

		private void UpdateControllHomeView(){
			string format = "dd MMM yyyy";
			lbSubDateExpired.Text = String.Format("{0}",DateTime.Now.AddDays (_dateExpired).ToString(format));
			switch (_currentState) {
			case 0:
				_bidState = Bidvalet.Droid.BidEnum.BidStartState.NEW_DOWNLOAD;
				lbSubExpiredOn.Visibility = ViewStates.Gone;
				lbGoSub.Visibility = ViewStates.Gone;
				lbSubDateExpired.Visibility = ViewStates.Gone;
				btnEditAccount.Visibility = ViewStates.Invisible;
				btnViewBidReceipt.Visibility = ViewStates.Invisible;
				break;
			case 1:
				_bidState = Bidvalet.Droid.BidEnum.BidStartState.VALID_SUB;
				lbSubExpiredOn.Visibility = ViewStates.Visible;
				lbGoSub.Visibility = ViewStates.Gone;
				lbSubDateExpired.Visibility = ViewStates.Visible;
				btnEditAccount.Visibility = ViewStates.Visible;
				btnViewBidReceipt.Visibility = ViewStates.Visible;
				lbSubDateExpired.SetTextColor (Android.Graphics.Color.ParseColor("#379B00"));
				break;
			case 2:
				_bidState = Bidvalet.Droid.BidEnum.BidStartState.EXPIRED_SUB;
				lbSubExpiredOn.Visibility = ViewStates.Visible;
				lbGoSub.Visibility = ViewStates.Visible;
				lbSubDateExpired.Visibility = ViewStates.Visible;
				btnEditAccount.Visibility = ViewStates.Visible;
				btnViewBidReceipt.Visibility = ViewStates.Visible;
				lbSubDateExpired.SetTextColor (Android.Graphics.Color.ParseColor("#DE0000"));
				break;
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			switch (_bidState) {
			case Bidvalet.Droid.BidEnum.BidStartState.NEW_DOWNLOAD:
				_currentState = Constants.STARE_NEW_DOWNLOAD;
				break;
			case Bidvalet.Droid.BidEnum.BidStartState.VALID_SUB:
				_currentState = Constants.STARE_VALID_SUBSCRIPTION;
				break;
			case Bidvalet.Droid.BidEnum.BidStartState.EXPIRED_SUB:
				_currentState = Constants.STARE_EXPIRED_SUBSCRIPTION;
				break;
			}
			outState.PutInt(Constants.START_STATE, _currentState);
		}
		protected override void OnRestoreInstanceState (Bundle bundle)
		{
			base.OnRestoreInstanceState (bundle);
			_currentState = bundle.GetInt (Constants.START_STATE, 0);
		}

	}
}

