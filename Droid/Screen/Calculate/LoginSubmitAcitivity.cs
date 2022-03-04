
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
	[Activity (Label = "LoginSubmitAcitivity", Theme = "@style/Bid.ThemeTitle")]			
	public class LoginSubmitAcitivity : BaseActivity
	{
		Button btnLogin;
		TextView tvTitleLoginSubmit,tvTitleUsePsd,tvCWAPsd,tvUserId,tvBidShowNext;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutLoginSubmitView);
			header.SetHeader (this, null, "Submit", Resource.Drawable.icon_arrow_left, "Back", "");
			tvTitleLoginSubmit = FindViewById<TextView> (Resource.Id.tvTitleLoginSubmit);
			tvTitleUsePsd = FindViewById<TextView> (Resource.Id.tvTitleUsePsd);
			tvCWAPsd = FindViewById<TextView> (Resource.Id.tvCWAPsd);
			tvUserId = FindViewById<TextView> (Resource.Id.tvUserId);
			tvBidShowNext = FindViewById<TextView> (Resource.Id.tvBidShowNext);
			SetCustomFontTextView (new TextView[]{ tvTitleLoginSubmit,tvTitleUsePsd,tvCWAPsd,tvUserId,tvBidShowNext}, "courier.ttf");
			btnLogin = FindViewById<Button> (Resource.Id.btnLogin);
			btnLogin.Click += (sender, e) => {
				var activityBidReceipt = new Intent(this, typeof(BidReceiptActivity));
				StartActivity(activityBidReceipt);
			};
		}
	}
}

