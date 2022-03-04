
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

namespace Bidvalet.Droid
{
	[Activity (Label = "Login",Theme ="@style/Bid.ThemeTitle")]			
	public class LoginViewActivity : BaseActivity
	{
		TextView tvTitleLogin;
		string mBaseSelected = "", mSeatSelected="", mRoundSelected="";
		Button btnLogin;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutLoginView);
			mBaseSelected = Intent.GetStringExtra (Constants.BASE_SELECTED);
			mSeatSelected = Intent.GetStringExtra (Constants.SEAT_SELECTED);
			mRoundSelected = Intent.GetStringExtra (Constants.ROUND_SELECTED);
			if (savedInstanceState != null) {
				OnRestoreInstanceState (savedInstanceState);
			}
			header.SetHeader (this, null, "Login", Resource.Drawable.icon_arrow_left, "Back", "");
			GetViewControll ();
		}
		private void GetViewControll(){
			btnLogin = FindViewById<Button> (Resource.Id.btnLogin);
			tvTitleLogin = FindViewById<TextView> (Resource.Id.tvTitleLogin);
			tvTitleLogin.Text = String.Format ("Get {0} - {1} - {2}", mBaseSelected, mSeatSelected, mRoundSelected);
			btnLogin.Click += (object sender, EventArgs e) => {
				var activityAuthor = new Intent(this, typeof(AuthorizeServicesActivity));
				StartActivity(activityAuthor);
			};
		}

		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			mBaseSelected = savedInstanceState.GetString (Constants.BASE_SELECTED, "");
			mSeatSelected = savedInstanceState.GetString (Constants.SEAT_SELECTED, "");
			mRoundSelected = savedInstanceState.GetString (Constants.ROUND_SELECTED, "");
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutString (Constants.BASE_SELECTED, mBaseSelected);
			outState.PutString (Constants.SEAT_SELECTED, mSeatSelected);
			outState.PutString (Constants.ROUND_SELECTED, mRoundSelected);
			base.OnSaveInstanceState (outState);
		}
	}
}

