
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
	[Activity (Label = "CommuteInforActivity")]			
	public class CommuteInforActivity : BaseActivity
	{
		public CommutableLineCx _data;
		const int CmtTime =1;
		const int CheckIn = 2;
		const int BackBase = 3;

			
		ImageView imgCmutCityInfo,imgCmutCityTime,imgPadCheckInnfo,imgPadBackToBaseInfo,imgViewCommuteInfo;
		TextView tvCmutCity,tvCmutTime,tvPadCheckIn,tvPadBackToBase,tvViewCommuteTime,tvDoneSetting;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutCommuteInformation);
			header.SetHeader (this, null, "Filters", Resource.Drawable.icon_arrow_left, "Back", "");
			imgCmutCityInfo = FindViewById<ImageView> (Resource.Id.imgCmutCityInfo); 
			imgCmutCityTime = FindViewById<ImageView> (Resource.Id.imgCmutCityTime); 
			imgPadCheckInnfo = FindViewById<ImageView> (Resource.Id.imgPadCheckInnfo); 
			imgPadBackToBaseInfo = FindViewById<ImageView> (Resource.Id.imgPadBackToBaseInfo); 
			imgViewCommuteInfo = FindViewById<ImageView> (Resource.Id.imgViewCommuteInfo);
			tvCmutCity = FindViewById<TextView> (Resource.Id.tvCmutCity);
			tvCmutTime = FindViewById<TextView> (Resource.Id.tvCmutTime);
			tvPadCheckIn = FindViewById<TextView> (Resource.Id.tvPadCheckIn);
			tvPadBackToBase = FindViewById<TextView> (Resource.Id.tvPadBackToBase);
			tvViewCommuteTime = FindViewById<TextView> (Resource.Id.tvViewCommuteTime);
			tvDoneSetting = FindViewById<TextView> (Resource.Id.tvDoneSetting);
			if (_data ==null){
				_data = new CommutableLineCx ();
				_data.City = "";
				_data.IsAny =true;
				_data.IsRonBoth = !_data.IsAny;
				_data.IsHome=false;
				_data.IsWork = false;
			}
			string city = _data.City;
			if (city == null || city == "") {
				tvCmutCity.SetText("AUS",TextView.BufferType.Normal);	
			} else {
				tvCmutCity.SetText(_data.City,TextView.BufferType.Normal);
			}
			if (_data.PadForBackToBaseInMinutes < 1) {
				_data.PadForBackToBaseInMinutes = 5;
			}
			if (_data.ConnectTimeInMinutes < 1) {
				_data.ConnectTimeInMinutes = 5;
			}
			if (_data.PadForCheckInInMinutes < 1) {
				_data.PadForCheckInInMinutes = 5;
			}
			SetTitleForTextView (_data.PadForBackToBaseInMinutes, tvPadBackToBase);
			SetTitleForTextView (_data.ConnectTimeInMinutes, tvCmutTime);
			SetTitleForTextView (_data.PadForCheckInInMinutes, tvPadCheckIn);

			imgCmutCityInfo.Click += (object sender, EventArgs e) => {
				ShowDialogMessage(Constants.Set_Commute_City, Constants.Set_Commute_City_Message);
			};
			imgCmutCityTime.Click += (object sender, EventArgs e) => {
				ShowDialogMessage(Constants.Set_Connect_Times, Constants.Set_Connect_Times_Message);
			};
			imgPadCheckInnfo.Click += (object sender, EventArgs e) => {
				ShowDialogMessage(Constants.Pad_For_Checkin, Constants.Pad_For_Checkin_Msg);
			};
			imgPadBackToBaseInfo.Click += (object sender, EventArgs e) => {
				ShowDialogMessage(Constants.Pad_For_Back_To_Base, Constants.Pad_For_Back_To_Base_Msg);
			};
			imgViewCommuteInfo.Click += (object sender, EventArgs e) => {
				ShowDialogMessage(Constants.Get_Commute_Times, Constants.Get_Commute_Times_Message);
			};
			tvViewCommuteTime.Click += (object sender, EventArgs e) => {
				ShowDialogMessage(Constants.AppName, "Not yet functional");
			};
			tvCmutTime.Click += (sender, e) => {
				ShowDialogCmtTimes(CmtTime);
			};
			tvPadBackToBase.Click += (sender, e) => {
				ShowDialogCmtTimes(BackBase);
			};
			tvPadCheckIn.Click += (sender, e) => {
				ShowDialogCmtTimes(CheckIn);
			};
			tvDoneSetting.Click += (sender, e) => {
				Intent myIntent = new Intent (this, typeof(ConstraintViewActivity));
				myIntent.PutExtra (Constants.KEY_CITY_NAME, _data.City);
				SetResult (Result.Ok, myIntent);
				Finish();
			};
			tvCmutCity.Click += (sender, e) => {
				var activityPickCity = new Intent (this, typeof(CityPickerActivity));
				StartActivityForResult(activityPickCity,Constants.CMUT_PICK_CITY_REQUEST);
			};
		}

		void SetTitleForTextView (double minutes, TextView btn)
		{
			int hour = (int)minutes/60;
			int mins = (int)minutes%60;
			string minStr = mins.ToString();
			if (mins < 10) {
				minStr = string.Format("0{0}",mins);
			}
			btn.Text = string.Format("{0}:{1}", hour , minStr);
		}

		private void ShowDialogMessage(string title, string msg){
			var builder = new AlertDialog.Builder(this);
			builder.SetIconAttribute(Android.Resource.Attribute.AlertDialogIcon);
			builder.SetMessage(msg);
			builder.SetTitle (title);
			builder.SetPositiveButton(Resource.String.button_done, OkClicked);
			builder.Create();
			builder.Show ();
		}
		private void OkClicked(object sender, DialogClickEventArgs args)
		{
			var dialog = (AlertDialog) sender;
			dialog.Dismiss ();
		}
		AlertDialog dialog;

		protected void ShowDialogCmtTimes(int id)
		{
			
			List<string> lstTimes = new List<string> ();
			string title = "";
			switch (id) {
			case CmtTime:
			case BackBase:
				for (int i = 5; i <= 60; i+=5) {
					title = string.Format("{0}:{1}", i/60, i%60);
					lstTimes.Add (title);
				}
				break;
			case CheckIn:
				for (int i = 5; i <= 120; i+=5) {
					title = string.Format("{0}:{1}", i/60, i%60);
					lstTimes.Add (title);
				}
				break;
			}
			var builder = new AlertDialog.Builder (this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate (Resource.Layout.DialogConstraints, null);
			if (dialogView != null) {
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1, lstTimes);
				lvAuthors.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					double minutes =(e.Position+1)*5;
					_data.PadForBackToBaseInMinutes = minutes;
					switch (id) {
					case CmtTime:
						SetTitleForTextView(minutes,tvCmutTime);
						break;
					case BackBase:
						SetTitleForTextView(minutes,tvPadBackToBase);
						break;
					case CheckIn:
						SetTitleForTextView(minutes,tvPadCheckIn);
						break;
					}
					dialog.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialog = builder.Create();
			dialog.SetCancelable (true);
			dialog.SetCanceledOnTouchOutside (true);
			dialog.Show ();
		}
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (resultCode==Result.Ok) {
				if (requestCode == Constants.CMUT_PICK_CITY_REQUEST && data != null) {
					_data.City = data.GetStringExtra(Constants.KEY_SENT);
					tvCmutCity.SetText (_data.City,TextView.BufferType.Normal);
				}
			}
			base.OnActivityResult (requestCode, resultCode, data);
		}
	}
}

