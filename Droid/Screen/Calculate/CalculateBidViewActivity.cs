
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
using System.Threading;

namespace Bidvalet.Droid
{
	[Activity (Label = "Calculate Bid",Theme ="@style/Bid.ThemeTitle")]			
	public class CalculateBidViewActivity : BaseActivity
	{
		const int MAX_PROGRESS = 64;
		Button btnSubmit;
		ProgressBar progressBid;
		TextView tvComboCalculate,tvTitleCalculate;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutCalculateBidView);
			header.SetHeader (this, null, "Calculate Bid", Resource.Drawable.icon_arrow_left, "Back", "");
			btnSubmit = FindViewById<Button> (Resource.Id.btnSubmit);
			tvComboCalculate = FindViewById<TextView> (Resource.Id.tvComboCalculate);
			tvTitleCalculate = FindViewById<TextView> (Resource.Id.tvTitleCalculate);
			SetCustomFontTextView (new TextView[]{tvComboCalculate,tvTitleCalculate}, "courier.ttf");
			progressBid = FindViewById<ProgressBar> (Resource.Id.progressBid);
			progressBid.Max = MAX_PROGRESS;
			btnSubmit.Click += (sender, e) => {
				var acitivityLoginSubmit = new Intent(this, typeof(LoginSubmitAcitivity));
				StartActivity(acitivityLoginSubmit);
			};
			new Thread(new ThreadStart(() => {
				for (int i = 0; i <= MAX_PROGRESS; i++) {
					this.RunOnUiThread ( () => {
						progressBid.Progress = i;
						tvComboCalculate.Text = String.Format("Calculating Combo #{0}",i);
					});
					Thread.Sleep(30);
				}
			})).Start();
		}

	}
}

