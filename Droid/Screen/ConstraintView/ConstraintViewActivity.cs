
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
using Android.Text;
using Android.Text.Style;
//using Bidvalet.Droid.SortView;
using Bidvalet.Droid.SortView;

namespace Bidvalet.Droid
{
	[Activity (Label = "Constraints", Theme="@style/Bid.ThemeTitle")]
	//[Activity (Label = "Constraints", Theme="@style/Bid.ThemeTitle",MainLauncher=true )]
	public class ConstraintViewActivity : Activity
	{
//		public delegate void EventListViewDrag();
//
//		public event EventListViewDrag DragListView;
//		public void 


		private DraggableListView lvConstraint;
		private ConstraintsAdapter mConstraintsAdapter;
		BidToolbarEvent header;

		TextView tvNoConstraint;
		RelativeLayout rlNoConstraint;

		CommutableLineCx cm;
		DaysOfMonthCx dom;
		OvernightCitiesCx ov;
		RestCx re;
		EquirementConstraint eq;
		DHFristLastConstraint dhfl;
		DaysOfWeekSome dowS;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.LayoutConstraintView);
			lvConstraint = FindViewById<DraggableListView>(Resource.Id.lvConstraint);
			tvNoConstraint = FindViewById<TextView> (Resource.Id.tvNoConstraint);
			rlNoConstraint= FindViewById<RelativeLayout> (Resource.Id.rlNoConstraint);
			if (SharedObject.Instance.ListConstraint.Count > 0) {
				rlNoConstraint.Visibility = ViewStates.Gone;
			} else {
				rlNoConstraint.Visibility = ViewStates.Visible;
			}
			header = new BidToolbarEvent ();
			header.SetHeader (this, null, "Filters", Resource.Drawable.icon_plus, "Clear All", "Done");
			header.OnBackEvent += HandleAddConstraintEvent;
			header.OnLeftButtonEvent += HandleClearAllEvent;
			header.OnRightButtonEvent += HandleDoneEvent;

			SpannableStringBuilder builder = new SpannableStringBuilder();
			builder.Append ("Touch the + sign top left to add your constraints.\n\n The constraints with a light green background can be added multiple times.\n\nFor example: Days of the Week could be Mo-Tu-We and then Tu-We-Th\n\nBe sure to put your most important constraints at the top.\n\nYou can use the  " +
			" icon to drag a line up or down in the order.");
			tvNoConstraint.Text = String.Format("{0}",builder);
			var imageSpan = new ImageSpan(this, Resource.Drawable.icon_small_menu); //Find your drawable.
			var spannableString = new SpannableString(tvNoConstraint.Text); //Set text of SpannableString from TextView
			spannableString.SetSpan(imageSpan, tvNoConstraint.Text.Length -46, tvNoConstraint.Text.Length-45, 0); //Add image at end of string
			tvNoConstraint.TextFormatted = spannableString;
			mConstraintsAdapter = new ConstraintsAdapter (this);
			lvConstraint.Adapter = mConstraintsAdapter;
			mConstraintsAdapter.CmtLineTitleClick += HandlerConstraintItemClicked;
			mConstraintsAdapter.DaysOfMonthClick += HandleConstraintItemDaysOfMonthClicked;
			mConstraintsAdapter.TitleOvernightCityClick += HandlerTitleOvernightCityClick;
			mConstraintsAdapter.RestClick += HandlerRestCxClicked;
			mConstraintsAdapter.RestValueClick+= HandlerValueRestCxClicked;
			mConstraintsAdapter.LessMoreRestClick+= HandlerRestCxLessMoreClicked;
			mConstraintsAdapter.DaysOfWeekClick += HandlerDaysOfWeekClick;
			mConstraintsAdapter.DaysOfWeekLessMoreClick += HandlerDaysOfWeekLessMoreClick;
			mConstraintsAdapter.DaysOfWeekValueClick += HandlerDaysOfWeekValueClick;
			mConstraintsAdapter.DHFirstLastClick += HandlerDHFirstLastClick;
			mConstraintsAdapter.DHLessMoreClick += HandlerDHLessMoreClick;
			mConstraintsAdapter.DHFirstLastValueClick += HandlerDHFirstLastValueClick;
			mConstraintsAdapter.EquipmentClick += HandlerEquipmentClick;
			mConstraintsAdapter.EquipmentLessMoreClick += HandlerEquipmentLessMoreClick;
			mConstraintsAdapter.EquipmentValueClick += HandlerEquipmentValueClick;
		}

		void HandlerEquipmentValueClick (object sender, EquirementConstraint eq)
		{
			int[]values = new int[21];
			for (int i = 0; i <= 20; i++) {
				values [i] = i;
			}
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,values);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					eq.Value = values[agr.Position];
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerEquipmentLessMoreClick (object sender, EquirementConstraint eq)
		{
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.lsLessMore);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					eq.LessMore = Constants.lsLessMore.ElementAt(agr.Position);
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerEquipmentClick (object sender, EquirementConstraint eq)
		{
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.listEquipments);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					eq.Equirement = Constants.listEquipments.ElementAt(agr.Position);
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerDHFirstLastValueClick (object sender, DHFristLastConstraint dhfl)
		{
			int[]values = new int[7];
			for (int i = 0; i <= 6; i++) {
				values [i] = i;
			}
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,values);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					dhfl.Value = values[agr.Position];
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerDHLessMoreClick (object sender, DHFristLastConstraint dhfl)
		{
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.lsLessMore);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					dhfl.LessMore = Constants.lsLessMore.ElementAt(agr.Position);
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerDHFirstLastClick (object sender, DHFristLastConstraint dhfl)
		{
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.lsFirstLast);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					dhfl.DH = Constants.lsFirstLast.ElementAt(agr.Position);
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerDaysOfWeekLessMoreClick (object sender, DaysOfWeekSome e)
		{
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.lsLessMore);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					dowS.LessOrMore = Constants.lsLessMore.ElementAt(agr.Position);
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerDaysOfWeekValueClick (object sender, DaysOfWeekSome e)
		{
			int[]values = new int[7];
			for (int i = 0; i <= 6; i++) {
				values [i] = i;
			}
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,values);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					dowS.Value = values[agr.Position];
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerDaysOfWeekClick (object s, DaysOfWeekSome dowS)
		{
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.DaysInWeek);
				lvAuthors.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					dowS.Date = Constants.DaysInWeek.ElementAt(e.Position);
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}

		void HandlerRestCxLessMoreClicked (object sender, RestCx e)
		{
			ShowRestDialogLessMore ();
		}

		void HandlerRestCxClicked (object sender, RestCx e)
		{
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.lsRest);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					re.Dom = Constants.lsRest.ElementAt(agr.Position);
					mConstraintsAdapter.NotifyDataSetChanged();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}
		void HandlerValueRestCxClicked (object sender, RestCx e)
		{
			int[] restValues = new int[49];
			for (int i = 0; i <= 48; i++) {
				restValues [i] = i;
			}
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,restValues);
				lvAuthors.ItemClick += (object s, AdapterView.ItemClickEventArgs agr) => {
					re.Value = restValues[agr.Position];
					ReloadData();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}


		public void ReloadData()
		{
			mConstraintsAdapter.NotifyDataSetChanged ();
		}

		AlertDialog dialogLessMore;

		public void ShowRestDialogLessMore(){
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.lsLessMore);
				lvAuthors.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					re.LessMore = Constants.lsLessMore.ElementAt(e.Position);
					ReloadData();
					dialogLessMore.Dismiss();
				};
			}
			builder.SetView(dialogView);
			dialogLessMore = builder.Create();
			dialogLessMore.SetCancelable (true);
			dialogLessMore.SetCanceledOnTouchOutside (true);
			dialogLessMore.Show ();
		}
		//handle item click
		void HandlerConstraintItemClicked (object sender, CommutableLineCx e)
		{
			var activityCmt = new Intent (this, typeof(CommuteInforActivity));
			StartActivityForResult (activityCmt, Constants.CMUT_LINE_REQUEST);
		}
		void HandleConstraintItemDaysOfMonthClicked (object sender, DaysOfMonthCx e)
		{
			var activityCmt = new Intent (this, typeof(DayOfMonthViewActivity));
			StartActivityForResult (activityCmt, Constants.CMUT_DAY_OF_MONTH_REQUEST);
		}
		void HandlerTitleOvernightCityClick (object sender, OvernightCitiesCx e)
		{
			var activityCity = new Intent (this, typeof(CitiesViewActivity));
			StartActivityForResult (activityCity, Constants.CMUT_OVER_NIGHT_REQUEST);

		}

		public void HandleDoneEvent ()
		{
			if (SharedObject.Instance.ListConstraint.Count == 0) {
				return;
			}
			var activitySort = new Intent (this, typeof(SortViewActivity));
			StartActivity(activitySort);
		}

		public void HandleClearAllEvent ()
		{
			SharedObject.Instance.ListConstraint.Clear ();
			mConstraintsAdapter.NotifyDataSetChanged ();
			if (SharedObject.Instance.ListConstraint.Count > 0) {
				rlNoConstraint.Visibility = ViewStates.Gone;
			} else {
				rlNoConstraint.Visibility = ViewStates.Visible;
			}
		}

		public void HandleAddConstraintEvent ()
		{
			ShowDialogConstraints();
		}

		AlertDialog dialog;
		private void ShowDialogConstraints(){
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.listConstraints);
				lvAuthors.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					switch (e.Position) {
					case 0:
						AMPMConstriants ampm = new AMPMConstriants();
						ampm.AM = false;
						ampm.MIX = false;
						ampm.PM = false;
						SharedObject.Instance.ListConstraint.Add(ampm);
						break;
					case 1:
						var activityCmutLine = new Intent(this, typeof(CommuteInforActivity));
						cm = new CommutableLineCx();
						cm.City = "AUS";
						cm.IsAny =true;
						cm.IsRonBoth = !cm.IsAny;
						cm.IsHome=false;
						cm.IsWork = false;
						SharedObject.Instance.ListConstraint.Add(cm);
						StartActivityForResult(activityCmutLine,Constants.CMUT_LINE_REQUEST);
						break;
					case 2:
						var activityCmt = new Intent (this, typeof(DayOfMonthViewActivity));
						dom = new DaysOfMonthCx();
						dom.OFFDays = new List<int>();
						dom.WorkDays = new List<int>();
						SharedObject.Instance.ListConstraint.Add(dom);
						StartActivityForResult (activityCmt, Constants.CMUT_DAY_OF_MONTH_REQUEST);
						break;
					case 3:
						DaysOfWeekAll dowA = new DaysOfWeekAll();
						dowA.Su = false;
						dowA.Mo = false;
						dowA.Tu = false;
						dowA.We = false;
						dowA.Th = false;
						dowA.Fr = false;
						dowA.Sa = false;
						SharedObject.Instance.ListConstraint.Add(dowA);
						break;
					case 4:
						dowS = new DaysOfWeekSome();
						dowS.Date = "Sun";
						dowS.LessOrMore="Less than";
						dowS.Value=1;
						SharedObject.Instance.ListConstraint.Add(dowS);
						break;
					case 5:
						dhfl = new DHFristLastConstraint();
						dhfl.DH = "first";
						dhfl.LessMore="Less than";
						dhfl.Value=1;
						SharedObject.Instance.ListConstraint.Add(dhfl);
						break;
					case 6:
						eq = new EquirementConstraint();
						eq.Equirement = 700;
						eq.LessMore="Less than";
						eq.Value=1;
						SharedObject.Instance.ListConstraint.Add(eq);
						break;
					case 7:
						LineTypeConstraint lt = new LineTypeConstraint();
						lt.Blank = false;
						lt.Res = false;
						lt.Hard = false;
						lt.Int = false;
						lt.NonCon = false;
						SharedObject.Instance.ListConstraint.Add(lt);
						break;
					case 8:
						var activityOver = new Intent (this, typeof(CitiesViewActivity));
						ov = new OvernightCitiesCx();
						ov.No= new List<string>();
						ov.Yes = new List<string>();
						SharedObject.Instance.ListConstraint.Add(ov);
						StartActivityForResult(activityOver,Constants.CMUT_OVER_NIGHT_REQUEST);
						break;
					case 9:
						re = new RestCx();
						re.Dom= "inDom";
						re.LessMore = "Less than";
						re.Value = 10;
						SharedObject.Instance.ListConstraint.Add(re);
						break;
					case 10:
						StartDayOfWeek sdow = new StartDayOfWeek();
						sdow.Su = false;
						sdow.Mo = false;
						sdow.Tu = false;
						sdow.We = false;
						sdow.Th = false;
						sdow.Fr = false;
						sdow.Sa = false;
						SharedObject.Instance.ListConstraint.Add(sdow);
						break;
					case 11:
						TripBlockLenghtCx tr = new TripBlockLenghtCx();
						tr.Turn=false;
						tr.TwoDay = false;
						tr.ThreeDay =false;
						tr.FoDay=false;
						tr.IsTrip=true;
						tr.IsBlock=!tr.IsTrip;
						SharedObject.Instance.ListConstraint.Add(tr);
						break;
					default:
					break;
					}
					if (SharedObject.Instance.ListConstraint.Count > 0) {
						rlNoConstraint.Visibility = ViewStates.Gone;
					} else {
						rlNoConstraint.Visibility = ViewStates.Visible;
					}
					mConstraintsAdapter.NotifyDataSetChanged();
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
			base.OnActivityResult (requestCode, resultCode, data);
			if (resultCode == Result.Ok) {
				if (requestCode == Constants.CMUT_LINE_REQUEST && data != null) {
					cm.City=data.GetStringExtra(Constants.KEY_CITY_NAME);
					ReloadData();
				}
				if (requestCode == Constants.CMUT_DAY_OF_MONTH_REQUEST && data != null) {
					dom.OFFDays = data.GetIntArrayExtra (Constants.KEY_DAY_OFF_MONTH).OfType<int>().ToList();
					dom.WorkDays = data.GetIntArrayExtra (Constants.KEY_DAY_WORK_MONTH).OfType<int>().ToList();
					ReloadData ();
				}
				if (requestCode == Constants.CMUT_OVER_NIGHT_REQUEST && data != null) {
					ov.No = data.GetStringArrayExtra (Constants.KEY_OVERNIGHT_NO).OfType<string>().ToList();
					ov.Yes = data.GetStringArrayExtra (Constants.KEY_OVERNIGHT_YES).OfType<string>().ToList();
					ReloadData ();
				}
			}
		}
		private void ShowLessMoreDialog(){
			string[] lessmore = new string[]{"Less than", "More than"};
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogConstraints, null);
			if (dialogView != null) {
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvConstraint);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,lessmore);
				lvAuthors.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					switch (e.Position) {
					case 0:
						AMPMConstriants ampm = new AMPMConstriants ();
						ampm.AM = false;
						ampm.MIX = false;
						ampm.PM = false;
						SharedObject.Instance.ListConstraint.Add (ampm);
						break;
					case 1:
						var activityCmutLine = new Intent (this, typeof(CommuteInforActivity));
						cm = new CommutableLineCx ();
						cm.City = "";
						cm.IsAny = true;
						cm.IsRonBoth = !cm.IsAny;
						cm.IsHome = false;
						cm.IsWork = false;
						SharedObject.Instance.ListConstraint.Add (cm);
						StartActivityForResult (activityCmutLine, Constants.CMUT_LINE_REQUEST);
						break;
					}
				};
				dialog.Dismiss();
			}
			builder.SetView(dialogView);
			dialog = builder.Create();
			dialog.SetCancelable (true);
			dialog.SetCanceledOnTouchOutside (true);
			dialog.Show ();
		}

	}
}

