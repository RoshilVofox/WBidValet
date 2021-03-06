using System;
using Android.Widget;
using Android.App;
using System.Collections.Generic;
using System.Linq;
using Android.Views;

namespace Bidvalet.Droid
{
	public class CitiesAdapter:BaseAdapter
	{
		Activity activity; 
		List<CitiesButton> lstCities;
		public CitiesAdapter (Activity activity, List<CitiesButton> list)
		{
			this.activity = activity;
			lstCities = list;
		}

		public override int Count {
			get {
				return lstCities.Count>0?lstCities.Count:0;
			}
		}
		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}
//		public override CitiesButton GetItem (int position)
//		{
//			return lstCities.ElementAt (position);
//		}
		public override long GetItemId (int position)
		{
			return 0;
		}
		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var view = convertView ?? activity.LayoutInflater.Inflate (Resource.Layout.CityItem, parent, false);
			var tileText = view.FindViewById<TextView> (Resource.Id.tile_text);
			var item = lstCities.ElementAt (position);
			tileText.SetText (item.CityName, TextView.BufferType.Normal);
			return view;
		}
	}
}

