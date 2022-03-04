
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
	[Activity (Label = "SelectBaseActivity", Theme="@style/Bid.ThemeTitle")]			
	public class SelectBaseActivity : BaseActivity
	{
		RecyclerView mRecyclerViewBase;
		RecyclerView.LayoutManager mLayoutManager;
		string mBaseSelected="";
		SelectBaseAdapter mBaseAdapter;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.SelectBase);
			if (savedInstanceState != null) {
				OnRestoreInstanceState (savedInstanceState);
			}
			header.SetHeader (this, null, "Select Base", Resource.Drawable.icon_arrow_left, "Back", "");
			mRecyclerViewBase = FindViewById<RecyclerView> (Resource.Id.recyclerViewBase);
			mLayoutManager = new LinearLayoutManager (this);
			mRecyclerViewBase.SetLayoutManager (mLayoutManager);
			mBaseAdapter = new SelectBaseAdapter (this, Constants.SelectBase);
			mBaseAdapter.ItemBaseClick += HandleOnItemBaseClick;
			mRecyclerViewBase.SetAdapter (mBaseAdapter);
		}

		private void HandleOnItemBaseClick (object sender, int position)
		{
			mBaseSelected = Constants.SelectBase.ElementAt (position);
			if (String.IsNullOrEmpty (mBaseSelected) || mBaseSelected.Equals ("")) {
				return;
			} else {
				var activitySeatRound = new Intent (this, typeof(SeatAndRoundActivity));
				activitySeatRound.PutExtra (Constants.BASE_SELECTED, mBaseSelected);
				StartActivity (activitySeatRound);
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutString (Constants.BASE_SELECTED, mBaseSelected);
			base.OnSaveInstanceState (outState);
		}
		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			mBaseSelected = savedInstanceState.GetString (Constants.BASE_SELECTED,"");
			//NSBidLog ("OnRestore: ",mBaseSelected);
		}
	}
}

