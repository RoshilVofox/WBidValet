
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
	[Activity (Label = "AuthorizeStatusActivity", Theme ="@style/Bid.ThemeTitle")]			
	public class AuthorizeStatusActivity : BaseActivity
	{
		public string messageError ="";
		public bool isShowPurchaseButton = false;
		public string buttonTitle = "Done";
		public int numberRow = 1;
		public string topBarTitle = "";
		Button btnDone, btnPurchase;
		TextView tvMessageError;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutAuthorizeStatus);
			//Get all data intent
			numberRow = Intent.GetIntExtra(Constants.POSITION,1);
			if (savedInstanceState != null) {
				OnRestoreInstanceState (savedInstanceState);
			}
			tvMessageError = FindViewById<TextView> (Resource.Id.tvMessageError);
			SetCustomFontTextView(new TextView[]{tvMessageError},1);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			btnPurchase = FindViewById<Button> (Resource.Id.btnPurchaseInApp);
			SetupView ();
			header.SetHeader (this, null, topBarTitle, Resource.Drawable.icon_arrow_left, "Back", "");
			btnDone.Click += (object sender, EventArgs e) => {
				if(numberRow==Constants.VALID_SUBSCRIPTION||numberRow ==Constants.NEW_CB_WB_USER){
					//Toast.MakeText(this,"Go to CONSTRAINTS", ToastLength.Short).Show();
					var activityConstraint = new Intent(this, typeof(ConstraintViewActivity));
					StartActivity(activityConstraint);

				}else{
					OnBackPressed();
				}
			};

		}
		private void SetupView(){
			if(numberRow==Constants.VALID_SUBSCRIPTION||numberRow ==Constants.NEW_CB_WB_USER){
				buttonTitle = "Go to CONSTRAINTS";
			} else {
				buttonTitle = "Done";
			}
			if (numberRow == Constants.EXPIRED_SUBSCRIPTION) {
				isShowPurchaseButton = true;
			}
			messageError = Constants.ErrorMessages.ElementAt (numberRow-1);
			topBarTitle = Constants.listTitleTopBar.ElementAt (numberRow-1);
			btnDone.Text = buttonTitle;
			tvMessageError.Text = messageError;
			if (isShowPurchaseButton) {
				btnPurchase.Alpha = 1;
			} else {
				btnPurchase.Alpha = 0;
			}
		}
		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			numberRow = savedInstanceState.GetInt (Constants.POSITION);
			messageError = savedInstanceState.GetString (Constants.MESSAGE_ERROR);
			buttonTitle = savedInstanceState.GetString (Constants.BUTTON_TITLE);
			isShowPurchaseButton = savedInstanceState.GetBoolean (Constants.SHOW_PURCHASE);
		}
		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutInt (Constants.POSITION, numberRow);
			outState.PutString (Constants.MESSAGE_ERROR, messageError);
			outState.PutString (Constants.BUTTON_TITLE, buttonTitle);
			outState.PutBoolean (Constants.SHOW_PURCHASE, isShowPurchaseButton);
			base.OnSaveInstanceState (outState);
		}
	}
}

