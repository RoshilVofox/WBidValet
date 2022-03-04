
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
	[Activity (Label = "BidReceiptActivity",Theme ="@style/Bid.ThemeTitle")]			
	public class BidReceiptActivity : BaseActivity
	{
		Button btnDone, btnEmail, btnPrint;
		TextView tvBidReceiptTitle;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutBidReceiptView);

			tvBidReceiptTitle = FindViewById<TextView> (Resource.Id.tvBidReceiptTitle);

			SetCustomFontTextView (new TextView[]{ tvBidReceiptTitle}, "courier.ttf");

			btnEmail = FindViewById<Button> (Resource.Id.btnEmail);
			btnPrint = FindViewById<Button> (Resource.Id.btnPrint);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			btnDone.Click += (sender, e) => {
				IntentToHomeView();
			};
			btnEmail.Click += (sender, e) => {
				IntentToSentView(true);
			};
			btnPrint.Click += (sender, e) => {
				IntentToSentView(false);
			};
			header.SetHeader (this, null, "Bid Receipt", Resource.Drawable.icon_arrow_left, "Back", "Done");
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
		private void IntentToSentView(bool iEmail){
			var activitySent = new Intent (this, typeof(EmailSentViewActivity));
			activitySent.PutExtra (Constants.KEY_SENT,iEmail);
			StartActivity (activitySent);
		}
	}
}

