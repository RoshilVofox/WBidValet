
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
	[Activity (Label = "SubmitalPriorityActivity",Theme = "@style/Bid.ThemeTitle")]			
	public class SubmitalPriorityActivity : BaseActivity
	{
		Button btnAll,btnSeniority,btnCalculate,btnLineThenPosition,btnPositionThenLine;
		EditText edtSeniority,edtFirstAvoid,edtSecondAvoid,edtThirdAvoid;
		private enum SeniorityType{
			ALL, SOME, NONE
		}
		private enum PositionType{
			LINE, POSITION
		}
		SeniorityType typeSeniority;
		PositionType mPosition;
		Android.Graphics.Color seniorityWhite, seniorityBlue;
		int senioritiesCount=0;
		TextView tvHowMany,tvPosition,tvLine,tvSenioryCount,tvTitle4th,tvTitle3rd,tvTitle2nd,tvTitle1st,tvTitlePriority;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutSubmitalPriority);
			header.SetHeader (this, null, "Position", Resource.Drawable.icon_arrow_left, "Back", "Calculate");
			header.OnRightButtonEvent += HandleCalculateEvent;

			seniorityWhite = Android.Graphics.Color.White;
			seniorityBlue = Android.Graphics.Color.ParseColor("#0082FF");
			btnAll = FindViewById<Button> (Resource.Id.btnAll);
			btnSeniority = FindViewById<Button> (Resource.Id.btnSeniority);
			btnCalculate = FindViewById<Button> (Resource.Id.btnCalculate);
			btnLineThenPosition = FindViewById<Button> (Resource.Id.btnLineThenPosition);
			btnPositionThenLine = FindViewById<Button> (Resource.Id.btnPositionThenLine);
			edtSeniority = FindViewById<EditText> (Resource.Id.edtSeniority);
			edtFirstAvoid = FindViewById<EditText> (Resource.Id.edtFirstAvoid);
			edtSecondAvoid = FindViewById<EditText> (Resource.Id.edtSecondAvoid);
			edtThirdAvoid = FindViewById<EditText> (Resource.Id.edtThirdAvoid);
			tvHowMany = FindViewById<TextView> (Resource.Id.tvHowMany);
			tvPosition = FindViewById<TextView> (Resource.Id.tvPosition);
			tvLine = FindViewById<TextView> (Resource.Id.tvLine);
			tvSenioryCount = FindViewById<TextView> (Resource.Id.tvSenioryCount);
			tvTitle4th = FindViewById<TextView> (Resource.Id.tvTitle4th);
			tvTitle3rd = FindViewById<TextView> (Resource.Id.tvTitle3rd);
			tvTitle2nd = FindViewById<TextView> (Resource.Id.tvTitle2nd);
			tvTitle1st = FindViewById<TextView> (Resource.Id.tvTitle1st);
			tvTitlePriority = FindViewById<TextView> (Resource.Id.tvTitlePriority);
			SetCustomFontTextView (new TextView[]{tvHowMany,tvPosition,tvLine,tvSenioryCount,tvTitle4th,tvTitle3rd,tvTitle2nd,tvTitle1st,tvTitlePriority},0);
			typeSeniority = SeniorityType.ALL;
			mPosition = PositionType.LINE;
			ChangLayoutView ();
			ChangPosionView ();
			btnLineThenPosition.Click += (sender, e) => {
				mPosition = PositionType.LINE;
				ChangPosionView ();
			};
			btnPositionThenLine.Click += (sender, e) => {
				mPosition = PositionType.POSITION;
				ChangPosionView ();
			};

			btnAll.Click += (sender, e) => {
				senioritiesCount = 432;
				edtSeniority.Text = "432";
				//tvSenioryCount.Text = String.Format("Your seniority is: {0} of 432",senioritiesCount);
				typeSeniority = SeniorityType.ALL;
				ChangLayoutView ();
			};
			btnSeniority.Click += (sender, e) => {
				edtSeniority.Text = "57";
				senioritiesCount = 57;
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
					senioritiesCount = val;
				}
			};
			edtSeniority.AfterTextChanged += (sender, e) => {
				if(edtSeniority.Text.Length>0&& Convert.ToInt32(edtSeniority.Text.ToString()) < 432){
					int val = Convert.ToInt32(edtSeniority.Text.ToString());
					//tvSenioryCount.Text = String.Format("Your seniority is: {0} of 432",val);
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
		private void ChangPosionView(){
			switch(mPosition){
			case PositionType.LINE:
				btnLineThenPosition.SetBackgroundResource (Resource.Drawable.bidvalet_button_standard_pressed);
				btnLineThenPosition.SetTextColor (seniorityWhite);
				btnPositionThenLine.SetBackgroundResource (Resource.Drawable.bidvalet_button_block_normal);
				btnPositionThenLine.SetTextColor (seniorityBlue);
				break;
			case PositionType.POSITION:
				btnLineThenPosition.SetBackgroundResource (Resource.Drawable.bidvalet_button_standard_normal);
				btnLineThenPosition.SetTextColor (seniorityBlue);
				btnPositionThenLine.SetBackgroundResource (Resource.Drawable.bidvalet_button_block_pressed);
				btnPositionThenLine.SetTextColor (seniorityWhite);
				break;
			}
		}
	}
}

