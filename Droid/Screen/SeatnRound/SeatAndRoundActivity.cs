
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
using Android.Support.V7.Widget;

namespace Bidvalet.Droid
{
	[Activity (Label = "Select Seat & Round", Theme = "@style/Bid.ThemeTitle")]			
	public class SeatAndRoundActivity : BaseActivity
	{
		RecyclerView mRecyclerViewSeat, mRecyclerViewRound;
		RecyclerView.LayoutManager mSeatLayoutManager,mRoundLayoutManager;
		string mSeatSelected="", mRoundSelected="";
		string mBaseSelected="";

		SelectBaseAdapter mSeatRoundAdapter;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutSeatAndRound);
			header.SetHeader (this, null, "Select Seat & Round", Resource.Drawable.icon_arrow_left, "Back", "");
			mBaseSelected = Intent.GetStringExtra(Constants.BASE_SELECTED);
			//NSBidLog (mBaseSelected);
			if (savedInstanceState != null) {
				OnRestoreInstanceState (savedInstanceState);
			}
			// setup seat
			mRecyclerViewSeat = FindViewById<RecyclerView> (Resource.Id.recyclerViewSeat);
			mSeatLayoutManager = new LinearLayoutManager (this);
			mRecyclerViewSeat.SetLayoutManager (mSeatLayoutManager);
			mSeatRoundAdapter = new SelectBaseAdapter (this, Constants.SelectSeat);
			mSeatRoundAdapter.ItemBaseClick += HandleOnItemSeatClick;
			mRecyclerViewSeat.SetAdapter (mSeatRoundAdapter);

			//setup round
			mRecyclerViewRound = FindViewById<RecyclerView> (Resource.Id.recyclerViewRound);
			mRoundLayoutManager = new LinearLayoutManager (this);
			mRecyclerViewRound.SetLayoutManager (mRoundLayoutManager);
			mSeatRoundAdapter = new SelectBaseAdapter (this, Constants.SelectRound);
			mSeatRoundAdapter.ItemBaseClick += HandleOnItemRoundClick;
			mRecyclerViewRound.SetAdapter (mSeatRoundAdapter);
		}

		private void HandleOnItemSeatClick (object sender, int position)
		{
			mSeatSelected = Constants.SelectSeat.ElementAt (position);
			if (String.IsNullOrEmpty (mSeatSelected)||mSeatSelected.Equals("")||String.IsNullOrEmpty(mRoundSelected)||mRoundSelected.Equals("")) {
				return;
			}
			IntentLoginView ();
		}

		private void HandleOnItemRoundClick (object sender, int position)
		{
			mRoundSelected = Constants.SelectRound.ElementAt (position);
			if (String.IsNullOrEmpty (mSeatSelected)||mSeatSelected.Equals("")||String.IsNullOrEmpty(mRoundSelected)||mRoundSelected.Equals("")) {
				return;
			}
			IntentLoginView ();
		}

		private void IntentLoginView(){
			var intentLoginView = new Intent (this, typeof(LoginViewActivity));
			intentLoginView.PutExtra (Constants.BASE_SELECTED,mBaseSelected);
			intentLoginView.PutExtra (Constants.SEAT_SELECTED, mSeatSelected);
			intentLoginView.PutExtra (Constants.ROUND_SELECTED, mRoundSelected);
			StartActivity (intentLoginView);
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutString (Constants.SEAT_SELECTED, mSeatSelected);
			outState.PutString (Constants.ROUND_SELECTED, mRoundSelected);
			base.OnSaveInstanceState (outState);
		}
		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			mSeatSelected = savedInstanceState.GetString (Constants.SEAT_SELECTED,"");
			mRoundSelected = savedInstanceState.GetString (Constants.ROUND_SELECTED,"");
			//NSBidLog ("Restored: Seat = ", mSeatSelected);
			//NSBidLog ("Restored: Round = ", mRoundSelected);
		}
	}
}

