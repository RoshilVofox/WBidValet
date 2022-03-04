using System;
using Android.Widget;
using System.Collections.Generic;
using Android.App;
using System.Linq;
using Android.Views;
using Android.Util;
using Android.Graphics;

namespace Bidvalet.Droid
{
	public class CalendarAdapter:BaseAdapter
	{
		Activity activity; 
		List<CalendarButton> lstCities;
		public EventHandler<CalendarButton> OnItemDayClick;
		public CalendarAdapter (Activity activity, List<CalendarButton> list)
		{
			this.activity = activity;
			lstCities = list;
		}

		public override int Count {
			get {
				return lstCities.Count;
			}
		}
		public override Java.Lang.Object GetItem (int position)
		{
			return lstCities.ElementAt(position);
		}
		public override long GetItemId (int position)
		{
			return lstCities.ElementAt(position).ID;
		}
		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			// position luon la 0?
			View view ;//= convertView;

			view = activity.LayoutInflater.Inflate (Resource.Layout.DayItem, parent, false);	
			CalendarButton sd = view.FindViewById<CalendarButton> (Resource.Id.tvDayItem);
			sd.Gravity = GravityFlags.Center;
			sd.Position = position;
			var item = lstCities.ElementAt (sd.Position);
			// cai sd nay luon la view dau tien co idDayItem => luon la item co white va id =0.

			sd.Click += (sender, agrs) => {
				if(OnItemDayClick!=null){
					OnItemDayClick(this, sd);
				}
			};
			sd.Text = item.Text;
			if (item.State == CalendarButton.CalendarButtonState.WHITE) {
				sd.SetBackgroundResource (Resource.Drawable.bidvalet_white_border_oval);
			} else if (item.State == CalendarButton.CalendarButtonState.GRAY) {
				sd.SetBackgroundResource (Resource.Drawable.bidvalet_gray_border_oval);
			} else if (item.State == CalendarButton.CalendarButtonState.GREEN) {
				sd.SetBackgroundResource (Resource.Drawable.bidvalet_green_border_oval);
			} else {
				sd.SetBackgroundResource (Resource.Drawable.bidvalet_red_border_oval);
			}

			return view;
		}

	}
}

