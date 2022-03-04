
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
	[Activity (Label = "EmailSentViewActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class EmailSentViewActivity : BaseActivity
	{
		Button btnDone;
		TextView tvBidSent;
		bool isEmail = true;
		string titleShow = "";
		string textShow = "";
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			isEmail = Intent.GetBooleanExtra (Constants.KEY_SENT, true);
			if (isEmail) {
				titleShow = "Email Sent";
				textShow = "Your bid receipt was emailed to: \n\nexample@example.com";
			} else {
				titleShow = "Print Sent";
				textShow = "Your bid receipt was sent to: \n\nHP8600 for printing";
			}
			SetContentView (Resource.Layout.LayoutEmailSentView);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			tvBidSent = FindViewById<TextView> (Resource.Id.tvBidSent);

			SetCustomFontTextView (new TextView[]{ tvBidSent}, "courier.ttf");
			btnDone.Click += (sender, e) => {
				IntentToHomeView();
			};
			tvBidSent.Text = textShow;
			header.SetHeader (this, null, titleShow, Resource.Drawable.icon_arrow_left, "Back", "Done");
			header.OnRightButtonEvent += HandleDoneEvent;
		}
		public void HandleDoneEvent ()
		{
			IntentToHomeView ();
		}

		private void IntentToHomeView(){
			var activityMain = new Intent (this, typeof(MainActivity));
			StartActivity (activityMain);
		}
	}
}

