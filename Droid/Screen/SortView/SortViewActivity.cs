
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
using Bidvalet.Droid.SortView;

namespace Bidvalet.Droid
{
	[Activity (Label = "SortViewActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class SortViewActivity : BaseActivity
	{
		Bidvalet.Droid.BidEnum.SortedOfKey keySorted;
		Bidvalet.Droid.BidEnum.TypeOfSort typeSorted;
		TextView tvScreenSort, tvMostSortPay;
		Button btnStandardSort, btnBlockSort, btnSinglePay, btnAdvancePay, btnAdvancePayDay, btnAdvancePayDutty, btnAdvancePayBlock, btnAdvancePayFDP, btnAdvancePayTAFB,
			btnBlockAMPM, btnBlockDayOff, btnBlockPay, btnBlockPayDutty, btnBlockPerDiem, btnBlockPMAM, btnBlockVacPay, btnBlockWeekday, btnDone;
		ScrollView scrStandardSort;
		LinearLayout lnSingleSort, lnAdvanceSort;
		Switch switchAdvance;
		RelativeLayout rlBlockSort;

		DraggableListView lvBlockSort;

		Android.Graphics.Color colorWhite, colorBlue;
		List<string> lsBlockSort = new List<string> ();
		BlockSortAdapter blockAdapter;
		bool isBockSort = false;
		string selectSortKey="";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutSortView);
			colorWhite = Android.Graphics.Color.White; 
			colorBlue = Android.Graphics.Color.ParseColor ("#0082FF"); 
			GetViewControll ();
			if (typeSorted == null) {
				typeSorted = Bidvalet.Droid.BidEnum.TypeOfSort.Standard;
			}
			if (keySorted == null) {
				keySorted = Bidvalet.Droid.BidEnum.SortedOfKey.Single;
			}
			SetChangeView ();
		}

		private void GetViewControll ()
		{
			tvScreenSort = FindViewById<TextView> (Resource.Id.tvScreenSort);
			tvMostSortPay= FindViewById<TextView> (Resource.Id.tvMostSortPay);
			SetCustomFontTextView (new TextView[]{tvScreenSort, tvMostSortPay},0);
			btnStandardSort = FindViewById<Button> (Resource.Id.btnStandardSort);
			btnBlockSort = FindViewById<Button> (Resource.Id.btnBlockSort);
			btnSinglePay = FindViewById<Button> (Resource.Id.btnSinglePay);
			btnAdvancePay = FindViewById<Button> (Resource.Id.btnAdvancePay);
			btnAdvancePayDay = FindViewById<Button> (Resource.Id.btnAdvancePayDay);
			btnAdvancePayDutty = FindViewById<Button> (Resource.Id.btnAdvancePayDutty);
			btnAdvancePayBlock = FindViewById<Button> (Resource.Id.btnAdvancePayBlock);
			btnAdvancePayFDP = FindViewById<Button> (Resource.Id.btnAdvancePayFDP);
			btnAdvancePayTAFB = FindViewById<Button> (Resource.Id.btnAdvancePayTAFB);
			btnBlockAMPM = FindViewById<Button> (Resource.Id.btnBlockAMPM);
			btnBlockDayOff = FindViewById<Button> (Resource.Id.btnBlockDayOff);
			btnBlockPay = FindViewById<Button> (Resource.Id.btnBlockPay);
			btnBlockPayDutty = FindViewById<Button> (Resource.Id.btnBlockPayDutty);
			btnBlockPerDiem = FindViewById<Button> (Resource.Id.btnBlockPerDiem);
			btnBlockPMAM = FindViewById<Button> (Resource.Id.btnBlockPMAM);
			btnBlockVacPay = FindViewById<Button> (Resource.Id.btnBlockVacPay);
			btnBlockWeekday = FindViewById<Button> (Resource.Id.btnBlockWeekday);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			scrStandardSort = FindViewById<ScrollView> (Resource.Id.scrStandardSort);
			lnSingleSort = FindViewById<LinearLayout> (Resource.Id.lnSingleSort);
			lnAdvanceSort = FindViewById<LinearLayout> (Resource.Id.lnAdvanceSort);
			switchAdvance = FindViewById<Switch> (Resource.Id.switchAdvance);
			rlBlockSort = FindViewById<RelativeLayout> (Resource.Id.rlBlockSort);
			lvBlockSort = FindViewById<DraggableListView> (Resource.Id.lvBlockSort);
			blockAdapter = new BlockSortAdapter (this, lsBlockSort);
			blockAdapter.DeletedItem += HandleItemDeleted;
			lvBlockSort.Adapter = blockAdapter;
			btnStandardSort.Click += (sender, e) => {
				typeSorted = Bidvalet.Droid.BidEnum.TypeOfSort.Standard;
				SetChangeView ();
			};
			btnBlockSort.Click += (sender, e) => {
				typeSorted = Bidvalet.Droid.BidEnum.TypeOfSort.Block;
				SetChangeView ();
			};
			switchAdvance.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e) {
				if (e.IsChecked) {
					keySorted = Bidvalet.Droid.BidEnum.SortedOfKey.Advance;
				} else {
					keySorted = Bidvalet.Droid.BidEnum.SortedOfKey.Single;
				}
				ChangeStandardSort ();
			};
			btnSinglePay.Click += (object sender, EventArgs e) => {
				isBockSort = false;
				selectSortKey = "Pay";
				ActivityIntent ();
			};
			btnAdvancePay.Click += (object sender, EventArgs e) => {
				isBockSort = false;
				selectSortKey = "Pay";
				ActivityIntent ();
			};
			btnAdvancePayDay.Click += (object sender, EventArgs e) => {
				isBockSort = false;
				selectSortKey = "Pay Day";
				ActivityIntent ();
			};
			btnAdvancePayDutty.Click += (object sender, EventArgs e) => {
				isBockSort = false;
				selectSortKey = "Pay / Dutty";
				ActivityIntent ();
			};
			btnAdvancePayBlock.Click += (object sender, EventArgs e) => {
				isBockSort = false;
				selectSortKey = "Pay / Block";
				ActivityIntent ();
			};
			btnAdvancePayFDP.Click += (object sender, EventArgs e) => {
				isBockSort = false;
				selectSortKey = "Pay / FDP";
				ActivityIntent ();
			};
			btnAdvancePayTAFB.Click += (object sender, EventArgs e) => {
				isBockSort = false;
				selectSortKey = "Pay / TAFB";
				ActivityIntent ();
			};

			//block sort
			btnBlockAMPM.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("AM - PM") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("AM - PM");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnBlockDayOff.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("Days Off") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("Days Off");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnBlockPay.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("Pay") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("Pay");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnBlockPayDutty.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("Pay / Dutty") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("Pay / Dutty");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnBlockPerDiem.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("Per Diem") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("Per Diem");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnBlockPMAM.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("PM - AM") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("PM - AM");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnBlockVacPay.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("Vac Pay") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("Vac Pay");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnBlockWeekday.Click += (object sender, EventArgs e) => {
				if (!lsBlockSort.Contains ("Weekday") && lsBlockSort.Count < 8) {
					lsBlockSort.Add ("Weekday");
					blockAdapter.NotifyDataSetChanged ();
				}
			};
			btnDone.Click += (object sender, EventArgs e) => {
				isBockSort = true;
				ActivityIntent ();
			};
		}

		void HandleItemDeleted (string item)
		{
			if (lsBlockSort.Count > 0) {
				Console.WriteLine ("run");
				lsBlockSort.Remove (item);
				blockAdapter.NotifyDataSetChanged ();
			}
		}

		private void SetChangeView ()
		{
			string titleSort = "";
			switch (typeSorted) {
			case Bidvalet.Droid.BidEnum.TypeOfSort.Standard:
				tvScreenSort.SetText (Resource.String.label_title_single_sort);
				btnStandardSort.SetBackgroundResource (Resource.Drawable.bidvalet_button_standard_pressed);
				btnStandardSort.SetTextColor (colorWhite);
				btnBlockSort.SetBackgroundResource (Resource.Drawable.bidvalet_button_block_normal);
				btnBlockSort.SetTextColor (colorBlue);
				scrStandardSort.Visibility = ViewStates.Visible;
				rlBlockSort.Visibility = ViewStates.Gone;
				titleSort = "Sort";
				ChangeStandardSort ();
				break;
			case Bidvalet.Droid.BidEnum.TypeOfSort.Block:
				tvScreenSort.SetText (Resource.String.label_title_block_sort);
				btnStandardSort.SetBackgroundResource (Resource.Drawable.bidvalet_button_standard_normal);
				btnStandardSort.SetTextColor (colorBlue);
				btnBlockSort.SetBackgroundResource (Resource.Drawable.bidvalet_button_block_pressed);
				btnBlockSort.SetTextColor (colorWhite);
				scrStandardSort.Visibility = ViewStates.Gone;
				titleSort = "Block Sort";
				rlBlockSort.Visibility = ViewStates.Visible;
				break;
			}
			header.SetHeader (this, null, titleSort, Resource.Drawable.icon_arrow_left, "Back", "");
		}

		private void ChangeStandardSort ()
		{
			switch (keySorted) {
			case Bidvalet.Droid.BidEnum.SortedOfKey.Single:
				lnSingleSort.Visibility = ViewStates.Visible;
				lnAdvanceSort.Visibility = ViewStates.Gone;
				break;
			case Bidvalet.Droid.BidEnum.SortedOfKey.Advance:
				lnSingleSort.Visibility = ViewStates.Gone;
				lnAdvanceSort.Visibility = ViewStates.Visible;
				break;
			}
		}

		private void ActivityIntent ()
		{
			var activitySorted = new Intent (this, typeof(SubSortViewActivity));
			activitySorted.PutExtra (Constants.BLOCK_SORT, isBockSort);
			if (isBockSort) {
				activitySorted.PutStringArrayListExtra (Constants.SORT_KEYS, lsBlockSort);
			} else {
				activitySorted.PutExtra (Constants.SORT_KEYS, selectSortKey);
			}
			StartActivity (activitySorted);
		}
	}
}

