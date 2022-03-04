
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
using Android.Graphics;
using Java.Lang;
using Android.Util;

namespace Bidvalet.Droid
{
	[Activity (Label = "DayOfMonthViewActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class DayOfMonthViewActivity : BaseActivity
	{
		int _numberItemPerRow = 7;
		int _numberRows = 6;
		TextView tvMonthYear;
		Button btnDone, btnClearAll;
		DaysOfMonthCx dom;
		GridView grdCalendar;
		CalendarAdapter adCalendar;
		List<CalendarButton> lsDaysInMonths = new List<CalendarButton>();

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutDayOfMonth);
			header.SetHeader (this, null, "Filters", Resource.Drawable.icon_arrow_left, "Back", "");
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			tvMonthYear = FindViewById<TextView> (Resource.Id.tvMonthYear);
			btnClearAll = FindViewById<Button> (Resource.Id.btnClearAll);
			if (dom == null) {
				dom = new DaysOfMonthCx ();
			}
			setupView ();
			grdCalendar = FindViewById<GridView> (Resource.Id.grdCalendar);
			adCalendar = new CalendarAdapter (this, lsDaysInMonths);
			grdCalendar.Adapter = adCalendar;
			// anh sua dung roi day
			adCalendar.OnItemDayClick += (sender, sd1) => {
				//CalendarButton sd = (CalendarButton)sender;
				CalendarButton sd =  lsDaysInMonths.ElementAt(sd1.Position);
				if (sd.State == CalendarButton.CalendarButtonState.WHITE) {
					return;
				} else if (sd.State ==CalendarButton.CalendarButtonState.GRAY) {
					sd.State =CalendarButton.CalendarButtonState.GREEN;
					sd.SetBackgroundResource (Resource.Drawable.bidvalet_green_border_oval);
				} else if (sd.State ==CalendarButton.CalendarButtonState.GREEN) {
					sd.State =CalendarButton.CalendarButtonState.RED;
					sd.SetBackgroundResource (Resource.Drawable.bidvalet_red_border_oval);
				} else {
					sd.State = CalendarButton.CalendarButtonState.GRAY;
					sd.SetBackgroundResource (Resource.Drawable.bidvalet_gray_border_oval);
				}
				adCalendar.NotifyDataSetChanged();
			};

			btnDone.Click += (sender, e) => {
				Intent myIntent = new Intent (this, typeof(ConstraintViewActivity));
				if(dom.OFFDays==null){
					dom.OFFDays = new List<int>();
				}
				if(dom.WorkDays==null){
					dom.WorkDays = new List<int>();
				}
				dom.OFFDays = GetListOffDays();
				dom.WorkDays = GetListWorkDays();
				myIntent.PutExtra(Constants.KEY_DAY_OFF_MONTH, dom.OFFDays.ToArray());
				myIntent.PutExtra(Constants.KEY_DAY_WORK_MONTH, dom.WorkDays.ToArray());
				SetResult (Result.Ok, myIntent);
				Finish();
			};
			btnClearAll.Click += (sender, e) => {
				ClearAll();
			};
		}
		private void setupView(){
			//text month year
			DateTime currentTime = DateTime.Now;
			string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames;
			tvMonthYear.Text = string.Format ("{0} {1}", monthNames[currentTime.Month-1], currentTime.Year);
			//create default layout
			int tag = 0;
			for (int row = 0; row < _numberRows; row++) {
				for (int col = 0; col < _numberItemPerRow; col++) {
					CalendarButton btn;
					btn = new CalendarButton (this);
					btn.SetBackgroundResource (Resource.Drawable.bidvalet_white_border_oval);
					btn.State = CalendarButton.CalendarButtonState.WHITE;
					btn.Position = tag;
					tag++;
					lsDaysInMonths.Add (btn);
				}
			}

			// get the first and last date of current month;
			DateTime nexMonth = currentTime.AddMonths(1);
			var startDate = new DateTime(nexMonth.Year, nexMonth.Month, 1);
			var endDate = startDate.AddMonths(1).AddDays(-1);
			int currentDateOfWeek = (int)startDate.Date.DayOfWeek;
			for (int i = 0; i < endDate.Day; i++) {
//				CalendarButton btnItem = ;
				lsDaysInMonths.ElementAt(currentDateOfWeek + i).SetBackgroundResource(Resource.Drawable.bidvalet_gray_border_oval);
				lsDaysInMonths.ElementAt(currentDateOfWeek + i).ID = i + 1;
				lsDaysInMonths.ElementAt(currentDateOfWeek + i).State = CalendarButton.CalendarButtonState.GRAY;
				lsDaysInMonths.ElementAt(currentDateOfWeek + i).SetText(string.Format ("{0}", i + 1),TextView.BufferType.Normal);

			}
			//add 3 days next month

			var btn1 = lsDaysInMonths.ElementAt(currentDateOfWeek + endDate.Day);
			btn1.ID = currentDateOfWeek + endDate.Day;
			btn1.Text = "1";
			btn1.State = CalendarButton.CalendarButtonState.WHITE;
			var btn2 = lsDaysInMonths.ElementAt(currentDateOfWeek + endDate.Day+1);
			btn2.ID = currentDateOfWeek + endDate.Day+1;
			btn2.Text = "2";
			btn2.State = CalendarButton.CalendarButtonState.WHITE;
			var btn3 = lsDaysInMonths.ElementAt(currentDateOfWeek + endDate.Day+2);
			btn3.ID = currentDateOfWeek + endDate.Day+2;
			btn3.Text = "3";
			btn3.State = CalendarButton.CalendarButtonState.WHITE;
		}

		public List<int> GetListOffDays(){
			List<int> lsOffs = new List<int> ();
			foreach (CalendarButton btn in lsDaysInMonths) {
				if (btn.State == CalendarButton.CalendarButtonState.RED) {
					lsOffs.Add (btn.ID);
				}
			}
			return lsOffs;
		}


		/// <summary>
		///  Gets the list work days => green
		/// </summary>
		/// <returns>The list work days.</returns>
		public List<int> GetListWorkDays()
		{
			List<int> lsWors = new List<int> ();
			foreach (CalendarButton btn in lsDaysInMonths) {
				if (btn.State == CalendarButton.CalendarButtonState.GREEN) {
					lsWors.Add (btn.ID);
				}
			}
			return lsWors;
		}

		public void ClearAll()
		{
			// get the first and last date of current month;
			DateTime nexMonth = DateTime.Now.AddMonths(1);
			var startDate = new DateTime(nexMonth.Year, nexMonth.Month, 1);
			var endDate = startDate.AddMonths(1).AddDays(-1);
			int currentDateOfWeek = (int)startDate.Date.DayOfWeek;
			for (int i = 0; i < endDate.Day; i++) {
				CalendarButton btn = lsDaysInMonths.ElementAt (currentDateOfWeek + i);
				btn.State = CalendarButton.CalendarButtonState.GRAY;
			}
			adCalendar.NotifyDataSetChanged ();
		}
	}
}

