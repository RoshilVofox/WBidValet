
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
	[Activity (Label = "SubmitalPositionActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class SubmitalPositionActivity : BaseActivity
	{
		Button btnAll,btnSeniority,btnCalculate;
		EditText edtSeniority,edtFirstAvoid,edtSecondAvoid,edtThirdAvoid;
		TextView tvSenioryCount,tvTitlePosition,tvBidAvoidTwo,
		tvBidAvoidOne,tvBidAvoidThree,tvSubmital;
		private enum SeniorityType{
			ALL, SOME, NONE
		}
		SeniorityType typeSeniority;
		Android.Graphics.Color seniorityWhite, seniorityBlue;
		int senioritiesCount=0;
	

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutSubmitalPosition);
			header.SetHeader (this, null, "Position", Resource.Drawable.icon_arrow_left, "Back", "Calculate");
			header.OnRightButtonEvent += HandleCalculateEvent;
		
			seniorityWhite = Android.Graphics.Color.White;
			seniorityBlue = Android.Graphics.Color.ParseColor("#0082FF");
			btnAll = FindViewById<Button> (Resource.Id.btnAll);
			btnSeniority = FindViewById<Button> (Resource.Id.btnSeniority);
			btnCalculate = FindViewById<Button> (Resource.Id.btnCalculate);
			edtSeniority = FindViewById<EditText> (Resource.Id.edtSeniority);
			edtFirstAvoid = FindViewById<EditText> (Resource.Id.edtFirstAvoid);
			edtSecondAvoid = FindViewById<EditText> (Resource.Id.edtSecondAvoid);
			edtThirdAvoid = FindViewById<EditText> (Resource.Id.edtThirdAvoid);
			tvSenioryCount = FindViewById<TextView> (Resource.Id.tvSenioryCount);
			tvTitlePosition = FindViewById<TextView> (Resource.Id.tvTitlePosition);
			tvBidAvoidTwo = FindViewById<TextView> (Resource.Id.tvBidAvoidTwo);
			tvBidAvoidOne = FindViewById<TextView> (Resource.Id.tvBidAvoidOne);
			tvBidAvoidThree = FindViewById<TextView> (Resource.Id.tvBidAvoidThree);
			tvSubmital = FindViewById<TextView> (Resource.Id.tvSubmital);
			SetCustomFontTextView (new TextView[]{tvSenioryCount,tvTitlePosition,tvBidAvoidTwo,
				tvBidAvoidOne,tvBidAvoidThree,tvSubmital},0);
			typeSeniority = SeniorityType.ALL;
			ChangLayoutView ();
			btnAll.Click += (sender, e) => {
				senioritiesCount = 432;
				edtSeniority.Text = "432";
				tvSenioryCount.Text = String.Format("Your seniority is: {0} of 432",senioritiesCount);
				typeSeniority = SeniorityType.ALL;
				ChangLayoutView ();
			};
			btnSeniority.Click += (sender, e) => {
				edtSeniority.Text = "57";
				senioritiesCount = 57;
				tvSenioryCount.Text = String.Format("Your seniority is: {0} of 432",senioritiesCount);
				typeSeniority = SeniorityType.SOME;
				ChangLayoutView ();
			};
			edtSeniority.Click += (sender, e) => {
				typeSeniority = SeniorityType.NONE;
				ChangLayoutView ();
			};
			edtSeniority.TextChanged += (sender, e) => {
				if(edtSeniority.Text.Length>0&& Convert.ToInt32(edtSeniority.Text.ToString()) < 432){
					int val = Convert.ToInt32(edtSeniority.Text.ToString());
					tvSenioryCount.Text = String.Format("Your seniority is: {0} of 432",val);
				}
			};
			edtSeniority.AfterTextChanged += (sender, e) => {
				if(edtSeniority.Text.Length>0&& Convert.ToInt32(edtSeniority.Text.ToString()) < 432){
					int val = Convert.ToInt32(edtSeniority.Text.ToString());
					tvSenioryCount.Text = String.Format("Your seniority is: {0} of 432",val);
					senioritiesCount = val;
				}
			};

			btnCalculate.Click += (sender, e) => {
				IntentView();
			};
		}
		private void IntentView(){
			var activityCalculate = new Intent(this,typeof(CalculateBidViewActivity));
			StartActivity(activityCalculate);
		}
		public void HandleCalculateEvent ()
		{
			IntentView ();
		}

		private void ChangLayoutView(){
			switch(typeSeniority){
			case SeniorityType.ALL:
				btnAll.SetBackgroundResource (Resource.Drawable.bidvalet_button_standard_pressed);
				btnAll.SetTextColor (seniorityWhite);
				btnSeniority.SetBackgroundResource (Resource.Drawable.bidvalet_button_block_normal);
				btnSeniority.SetTextColor (seniorityBlue);
				break;
			case SeniorityType.SOME:
				btnAll.SetBackgroundResource (Resource.Drawable.bidvalet_button_standard_normal);
				btnAll.SetTextColor (seniorityBlue);
				btnSeniority.SetBackgroundResource (Resource.Drawable.bidvalet_button_block_pressed);
				btnSeniority.SetTextColor (seniorityWhite);
				break;
			case SeniorityType.NONE:
				btnAll.SetBackgroundResource (Resource.Drawable.bidvalet_button_standard_normal);
				btnAll.SetTextColor (seniorityBlue);
				btnSeniority.SetBackgroundResource (Resource.Drawable.bidvalet_button_block_normal);
				btnSeniority.SetTextColor (seniorityBlue);
				break;
			}
		}
	}
}

