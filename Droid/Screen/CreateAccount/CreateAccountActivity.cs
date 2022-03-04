
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
	[Activity (Label = "Create Account", Theme ="@style/Bid.Theme")]			
	public class CreateAccountActivity : BaseActivity
	{
		private enum BidPosition{
			PILOT,FITART
		}
		BidPosition bidPosition;
		private bool isFoundAccount=false;
		Switch swAcceptEmail, swAcceptTerm;
		TextView tvPrivacy, tvLicense, tvTitleScreen;
		Button btnPilot, btnFitArt, btnSetCarrier, btnCancel, btnSave;
		EditText edtFirstName, edtLastName, edtEmpNumber, edtEmail, edtEmailConfirm, edtCellphone, edtPassword, edtRePassword;
		string Title_Position="",_setCarrier="";
		UserInformation userInfo = new UserInformation();
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.LayoutCreateAccount);
			if (savedInstanceState != null) {
				OnRestoreInstanceState (savedInstanceState);
			}
			isFoundAccount = Intent.GetBooleanExtra (Constants.USER_EXIST,false);
			bidPosition = BidPosition.PILOT;
			GetView ();
			UpdatePosition(bidPosition);
		}
		private void GetView(){
			swAcceptEmail = FindViewById<Switch> (Resource.Id.accept_email_switch);
			swAcceptTerm =  FindViewById<Switch> (Resource.Id.accept_term_switch);
			tvTitleScreen = FindViewById<TextView> (Resource.Id.tvTitleScreen);
			tvPrivacy = FindViewById<TextView> (Resource.Id.tvPrivacy);
			tvLicense = FindViewById<TextView> (Resource.Id.tvLicense);
			btnPilot = FindViewById<Button> (Resource.Id.btnPilot); 
			btnFitArt = FindViewById<Button> (Resource.Id.btnFitArt); 
			btnSetCarrier = FindViewById<Button> (Resource.Id.btnSetCarrier); 
			btnCancel = FindViewById<Button> (Resource.Id.btnCancel); 
			btnSave = FindViewById<Button> (Resource.Id.btnSave);
			edtFirstName = FindViewById<EditText> (Resource.Id.edtFirstName);
			edtLastName = FindViewById<EditText> (Resource.Id.edtLastName); 
			edtEmpNumber = FindViewById<EditText> (Resource.Id.edtEmployeeNumber); 
			edtEmail = FindViewById<EditText> (Resource.Id.edtEmail); 
			edtEmailConfirm = FindViewById<EditText> (Resource.Id.edtEmailConfirm); 
			edtCellphone = FindViewById<EditText> (Resource.Id.edtCellphone); 
			edtPassword = FindViewById<EditText> (Resource.Id.edtPassword); 
			edtRePassword = FindViewById<EditText> (Resource.Id.edtRePassword);
			if (isFoundAccount) {
				tvTitleScreen.Text = Constants.LABEL_EDIT_ACCOUNT;
				btnSave.Text = Constants.LABEL_SAVE_CHANGE;
				userInfo = defaultUserInformation ();
				//_setCarrier = userInfo.Carrier;
				pushUserInfoOnControl (userInfo);
			} else {
				tvTitleScreen.Text = Constants.LABEL_CREATE_ACCOUNT;
				btnSave.Text = Constants.LABEL_CREATE;
			}

			btnSetCarrier.Click += (object sender, EventArgs e) => {
				ShowDialogSetCarrier();
			};
			btnPilot.Click += (object sender, EventArgs e) => {
				Title_Position = "Pilot";
				bidPosition = BidPosition.PILOT;
				UpdatePosition(bidPosition);
			};
			btnFitArt.Click += (object sender, EventArgs e) => {
				Title_Position = "FitArt";
				bidPosition = BidPosition.FITART;
				UpdatePosition(bidPosition);
			};
			btnCancel.Click += (object sender, EventArgs e) => {
				OnBackPressed();
			};
			btnSave.Click += (object sender, EventArgs e) => {
				saveUserInfomationChanged();
				var validAccount = new Intent (this, typeof(AuthorizeStatusActivity));
				validAccount.PutExtra(Constants.POSITION, Constants.VALID_SUBSCRIPTION);
				StartActivity(validAccount);
			};
		}

		//
		//set up default user information
		private UserInformation defaultUserInformation ()
		{
			UserInformation user = new UserInformation ();
			user.FirstName = "Thang";
			user.LastName = "Pham";
			user.EmpNo = "1233456";
			user.Domicile = "Domicile";
			user.Position = "Fit Art";//Pilot or Fit Art
			user.SeniorityNumber = 1;
			user.Email = "phamthangnd@gmail.com";
			//user.CellPhone = "0987654321";
			//user.Carrier = "T-Mobile";
			//user.Password = "123456789";
			//user.AcceptEmail = true;
			//user.AcceptTerms = false;
			return user;
		}
		//publish on controll
		private void pushUserInfoOnControl (UserInformation user)
		{
			//btnSetCarrier.Text =user.Carrier;
			tvTitleScreen.Text = "Edit and change account details as needed.";
			//edtPassword.Text = user.Password;
			edtEmail.Text = user.Email;
			edtEmailConfirm.Text = user.Email;
			edtFirstName.Text = user.FirstName;
			edtLastName.Text = user.LastName;
			edtEmpNumber.Text = user.EmpNo;
			//edtPassword.Text = user.Password;
			//edtCellphone.Text = user.CellPhone;
			if (user.isAcceptMail) {
				swAcceptEmail.Checked = true;
			} else {
				swAcceptEmail.Checked = false;
			}
			if (user.isAcceptMail) {
				swAcceptTerm.Checked= true;
			} else {
				swAcceptTerm.Checked = false;
			}
//			if (user.Position.Equals (Title_Position)) {
//				segUserPosition.SelectedSegment = PILOT;
//			} else {
//				segUserPosition.SelectedSegment = FIT_ART;
//			}
//			UpdatePosition (pos);
		}
		private void UpdatePosition(BidPosition pos){
			switch (pos) {
			case BidPosition.PILOT:
				Title_Position = "Pilot";
				btnPilot.SetBackgroundResource (Resource.Drawable.bidvalet_button_pilot_pressed);
				btnPilot.SetTextColor(Android.Graphics.Color.White);
				btnFitArt.SetBackgroundResource (Resource.Drawable.bidvalet_button_fitart_normal);
				btnFitArt.SetTextColor(Android.Graphics.Color.ParseColor("#2AB20B"));
				break;
			case BidPosition.FITART:
				Title_Position = "FitArt";
				btnFitArt.SetBackgroundResource (Resource.Drawable.bidvalet_button_fitart_pressed);
				btnFitArt.SetTextColor(Android.Graphics.Color.White);
				btnPilot.SetBackgroundResource (Resource.Drawable.bidvalet_button_pilot_normal);
				btnPilot.SetTextColor(Android.Graphics.Color.ParseColor("#2AB20B"));
				break;
			}
		}
		// save user after change
		private void saveUserInfomationChanged ()
		{
			ValidatorInputField (this, new EditText[] {
				edtRePassword,
				edtEmail,
				edtEmailConfirm,
				edtFirstName ,
				edtLastName,
				edtEmpNumber ,
				edtPassword,
				edtCellphone
			});
			userInfo.FirstName = edtFirstName.Text;
			userInfo.LastName = edtLastName.Text;
			userInfo.EmpNo = edtEmpNumber.Text;
			userInfo.Domicile = "Domicile";
			userInfo.SeniorityNumber = 1;
			userInfo.Email = edtEmail.Text;
			userInfo.CellNumber = edtCellphone.Text;
			if (_setCarrier.Equals ("")) {
				//BidToast ("Set Carrier!");//ShowDialogSetCarrier ();
				return;
			}
			//userInfo.Carrier = _setCarrier;
			//userInfo.Password = edtPassword.Text;
			if (bidPosition == BidPosition.PILOT) {
				//
				userInfo.Position = Title_Position;
			} else {
				userInfo.Position = Title_Position;
			}
			if (swAcceptTerm.Checked) {
				userInfo.isAcceptUserTermsAndCondition = true;
			} else {
				userInfo.isAcceptUserTermsAndCondition = false;
			}
			if (swAcceptEmail.Checked) {
				userInfo.isAcceptMail = true;
			} else {
				userInfo.isAcceptMail = false;
			}
			userInfo.Email = edtEmail.Text;
		}

		AlertDialog dialog;

		private void ShowDialogSetCarrier(){
			var builder = new AlertDialog.Builder(this);
			//var dialog = builder.Create();
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogAuthorizeServices, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvAuthorizes);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.ListCarrier);
				lvAuthors.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					_setCarrier = Constants.ListCarrier.ElementAt(e.Position);
					btnSetCarrier.Text = _setCarrier;
					//var dialogShow = (AlertDialog)sender;
					//dialog.Dismiss();
					if (dialog != null) {
						dialog.Dismiss();
					}
				};
			}
			builder.SetView(dialogView);
			//Create the builder 
			dialog = builder.Create ();
			dialog.SetCancelable (true);
			dialog.SetCanceledOnTouchOutside (true);
			dialog.Show ();
		}
		//
		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutString ("CARRIER", _setCarrier);
			base.OnSaveInstanceState (outState);
		}
		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			_setCarrier = savedInstanceState.GetString ("CARRIER","");
		}
	}
}

