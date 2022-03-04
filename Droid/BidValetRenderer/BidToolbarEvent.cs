using System;
using Android.App;
using Android.Widget;
using Android.Views;

namespace Bidvalet.Droid
{
	public class BidToolbarEvent
	{
		public BidToolbarEvent ()
		{
		}
		public delegate void BackEventHandler ();
		public event BackEventHandler OnBackEvent;

		public delegate void LeftBtnEventHandler ();
		public event LeftBtnEventHandler OnLeftButtonEvent;

		public delegate void RightBtnEventHandler ();
		public event RightBtnEventHandler OnRightButtonEvent;

		public void SetHeader(Activity act,View view, string title,int icLeft, string leftText, string rightText){
			
			ImageView BackButton = null;
			TextView RightButton=null, LeftButton = null;
			TextView AppNameView = null;
			if (view!=null) {
				BackButton = view.FindViewById<ImageView> (Resource.Id.BackButton);
				RightButton = view.FindViewById<TextView> (Resource.Id.RightButton);
				LeftButton = view.FindViewById<TextView> (Resource.Id.LeftButton);
				AppNameView = view.FindViewById<TextView> (Resource.Id.AppNameView);
			}else{
				BackButton = act.FindViewById<ImageView>(Resource.Id.BackButton);
				RightButton = act.FindViewById<TextView> (Resource.Id.RightButton);
				LeftButton = act.FindViewById<TextView> (Resource.Id.LeftButton);
				AppNameView = act.FindViewById<TextView> (Resource.Id.AppNameView);
			}
			if (AppNameView != null) {
				AppNameView.Text = title;
			}
			if (BackButton != null) {
				if (icLeft != 0) {
					BackButton.Visibility = ViewStates.Visible;
					BackButton.SetImageResource (icLeft);
					BackButton.Click += (sender, e) => {
						if (OnBackEvent != null) {
							OnBackEvent ();
						}
					};
				} else {
					BackButton.Visibility = ViewStates.Gone;
				}
			}
			if (LeftButton != null) {
				LeftButton.Visibility = ViewStates.Visible;
				LeftButton.Text = leftText;
				LeftButton.Click += (sender, e) => {
					if (OnLeftButtonEvent != null) {
						OnLeftButtonEvent ();
					}
				};
			}
			if (RightButton != null) {
				RightButton.Visibility = ViewStates.Visible;
				RightButton.Text = rightText;
				RightButton.Click += (sender, e) => {
					if (OnRightButtonEvent != null) {
						OnRightButtonEvent ();
					}
				};
			}
		}
	}
}

