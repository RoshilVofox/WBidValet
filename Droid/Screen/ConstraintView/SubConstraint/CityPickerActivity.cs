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
	[Activity (Label = "CityPickerActivity",Theme ="@style/Bid.ThemeTitle")]			
	public class CityPickerActivity : BaseActivity
	{
		GridView grdCities;
		CitiesPickerAdapter cityAdapter;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutPickerCity);
			header.SetHeader (this, null, "Cities", Resource.Drawable.icon_arrow_left, "Back", "");
			grdCities = FindViewById<GridView> (Resource.Id.grdCityPicker);
			cityAdapter = new CitiesPickerAdapter (this,Constants.listCities);
			grdCities.Adapter = cityAdapter;
			grdCities.ItemClick += GrdCities_ItemClicked;
		}
		void GrdCities_ItemClicked (object sender, AdapterView.ItemClickEventArgs e)
		{
			var cityItem = Constants.listCities.ElementAt (e.Position);
			//BidToast (cityItem);
			Intent myIntent = new Intent (this, typeof(CommuteInforActivity));
			myIntent.PutExtra (Constants.KEY_SENT, cityItem);
			SetResult (Result.Ok, myIntent);
			Finish();
		}
	}
}

