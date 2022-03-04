
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
	[Activity (Label = "CitiesViewActivity",Theme ="@style/Bid.ThemeTitle")]			
	public class CitiesViewActivity : Activity
	{
		GridView grdCities;
		CitiesAdapter cityAdapter;
		OvernightCitiesCx _data;
		RelativeLayout tileLayout;
		TextView tileText;
		Button btnDone, btnClear;
		string _city ;
		List<CitiesButton> listCityName = new List<CitiesButton> ();

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutCitiesView);
			grdCities = FindViewById<GridView> (Resource.Id.grdCityPicker);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			btnClear = FindViewById<Button> (Resource.Id.btnClear);
			this.grdCities.SetGravity(GravityFlags.Center);
			if (_data == null) {
				_data = new OvernightCitiesCx();
			}
			listCityName = new List<CitiesButton> ();
			listCityName = getData ();
			cityAdapter = new CitiesAdapter (this,listCityName);
			grdCities.Adapter = cityAdapter;
			grdCities.ItemClick += GrdCities_ItemClicked;
			btnDone.Click += (sender, e) => {
				Intent myIntent = new Intent (this, typeof(ConstraintViewActivity));
				//_data.No = new List<string>{"AUS", "BLH", "DLI", "TBC" };
				//_data.Yes = new List<string>{ "VNE", "GEN", "KEN", "BND" }; 
				if (_data.Yes == null) {
					_data.Yes = new List<string>();
				}
				if (_data.No == null) {
					_data.No = new List<string>();
				}
				myIntent.PutExtra(Constants.KEY_OVERNIGHT_NO, _data.No.ToArray());
				myIntent.PutExtra(Constants.KEY_OVERNIGHT_YES, _data.Yes.ToArray());
				SetResult (Result.Ok, myIntent);

				Finish();
			};
			btnClear.Click += (sender, e) => {
				if (_data != null) {
					_data.No.Clear();
					_data.Yes.Clear();
//					if (_data.No != null) {
//						_data.No.Clear();
//					}
//					if (_data.Yes != null) {
//						_data.Yes.Clear();
//					}
				}
				listCityName = getData ();
				cityAdapter.NotifyDataSetChanged();
			};
		}

		private List<CitiesButton> getData() {
			List<CitiesButton> cityItems = new List<CitiesButton>();
			int count = Constants.listCities.Count;
			for (int i = 0; i < count; i++) {
				cityItems.Add(new CitiesButton(Constants.listCities.ElementAt(i), 0));
			}
			return cityItems;
		}
		 
		void GrdCities_ItemClicked (object sender, AdapterView.ItemClickEventArgs e)
		{
			var cityItem = listCityName.ElementAt (e.Position);
			_city = cityItem.CityName;
			tileLayout = e.View.FindViewById<RelativeLayout> (Resource.Id.tile_layout);
			tileText = e.View.FindViewById<TextView> (Resource.Id.tile_text);
			cityItem.CountClicked++;
			if (_data == null) {
				_data = new OvernightCitiesCx();
			}
			if (cityItem.CountClicked%3 == 0) {
				// remove
				if (_data != null && _data.No != null && _data.No.Contains(_city)) {
					_data.No.Remove(_city);
				}

				if (_data != null && _data.Yes != null && _data.Yes.Contains(_city)) {
					_data.Yes.Remove(_city);
				}
			}
			if (cityItem.CountClicked%3 == 1) {
				if (_data.Yes!= null && !_data.Yes.Contains(_city)) {
					_data.Yes.Add(_city);
				}
				if (_data.Yes == null) {
					_data.Yes = new List<string>();
					_data.Yes.Add(_city);
				}
				if (_data.No != null && _data.No.Contains(_city)) {
					_data.No.Remove(_city);
				}
			}
			if (cityItem.CountClicked %3  == 2) {
				// no
				if (_data.Yes!= null && _data.Yes.Contains(_city)) {
					_data.Yes.Remove(_city);
				}
				if (_data.No != null && !_data.No.Contains(_city)) {
					_data.No.Add(_city);
				}
				if (_data.No == null) {
					_data.No = new List<string>();
					_data.No.Add(_city);
				}
			}
			UpdateUI();
		}
		private void UpdateUI(){
			tileText.Text = _city;
			if (_data == null) {
				tileLayout.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#FFFFFF"));
				tileText.Tag = 0;
			} else {
				bool isFound = false;
				if (_data.No != null && _data.No.Contains(_city)) {
					tileText.Tag = 2;
					tileLayout.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#BE000A"));
					isFound = true;
				}
				if (_data.Yes != null && _data.Yes.Contains(_city)) {
					tileText.Tag = 1;
					tileLayout.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#B6F3B7"));
					isFound = true;
				}
				if (!isFound) {
					tileLayout.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#FFFFFF"));
					tileText.Tag = 0;// green=> 1touch/YES, red => 2touch/NO
				}
			}
		}
	}
}

