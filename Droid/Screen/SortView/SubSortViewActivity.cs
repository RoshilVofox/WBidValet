
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
	[Activity (Label = "SubSortViewActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class SubSortViewActivity : BaseActivity
	{
		string valueSortKeys="";
		Button btnDelete, btnContinue;
		TextView tvTitleSub, tvSortKey,tvTitleCorrect;
		IList<string> lstKeySort = new List<string> ();
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			bool isBlock = Intent.GetBooleanExtra (Constants.BLOCK_SORT,false);
			if (isBlock) {
				lstKeySort = Intent.GetStringArrayListExtra(Constants.SORT_KEYS);
			} else {
				valueSortKeys = Intent.GetStringExtra (Constants.SORT_KEYS);
			}
			SetContentView (Resource.Layout.LayoutSubSortView);
			header.SetHeader (this, null,"Selected Sort",Resource.Drawable.icon_arrow_left, "Back", "");

			btnDelete = FindViewById<Button> (Resource.Id.btnDelete);
			btnContinue = FindViewById<Button> (Resource.Id.btnContinue);
			tvSortKey = FindViewById<TextView> (Resource.Id.tvSortSubKey);
			tvTitleSub= FindViewById<TextView> (Resource.Id.tvTitleSubSort);
			tvTitleCorrect= FindViewById<TextView> (Resource.Id.tvTitleCorrect);
			SetCustomFontTextView (new TextView[]{tvTitleSub, tvSortKey,tvTitleCorrect}, 0);
			UpdateView (isBlock);
			btnDelete.Click += (sender, e) => {
				OnBackPressed();
			};
			btnContinue.Click += (sender, e) => {
				ShowDialogTestingPurposes();
			};
		}
		private void UpdateView(bool blockSort){
			if (!blockSort) {
				tvTitleSub.Text = String.Format("OK, you have selected \"{0}\" as your sort method.",valueSortKeys);
				tvSortKey.Visibility = ViewStates.Gone;
				btnDelete.Visibility = ViewStates.Visible;
			} else {
				tvTitleSub.Text ="OK, you have selected \"Block Sort\" with the following keys:";
				string textKeys = "";
				for (int i=0; i<lstKeySort.Count;i++){
					textKeys = textKeys + "\n" + lstKeySort.ElementAt (i);
				}
				tvSortKey.Text = textKeys;
				tvSortKey.Visibility = ViewStates.Visible;
				btnDelete.Visibility = ViewStates.Gone;
			}
		}
		AlertDialog alert = null;
		private void ShowDialogTestingPurposes(){
			var builder = new AlertDialog.Builder (this);
			builder.SetTitle ("Position:");
			builder.SetItems (Constants.arrPositions, ListItemClicked);
			builder.SetNegativeButton("CANCEL", CancelClicked);
			alert = builder.Create();
			alert.Show();
		}

		private void ListItemClicked(object sender, DialogClickEventArgs args)
		{
			switch (args.Which){
			case Constants.CAPITAIN:
				var activityChoices = new Intent (this, typeof(SubmitalChoicesActivity));
				StartActivity (activityChoices);
				break;
			case Constants.FIRST_OFFICER: 
				var activityPosition = new Intent (this, typeof(SubmitalPositionActivity));
				StartActivity ( activityPosition);
				break;
			case Constants.FLIGHT_ATTENDANT:
				var activityPriority = new Intent (this, typeof(SubmitalPriorityActivity));
				StartActivity (activityPriority);
				break;
			}
			//BidToast(string.Format("You've selected: {0}, {1}", args.Which, Constants.arrPositions[args.Which]));
		}
		private void CancelClicked (object sender, DialogClickEventArgs e)
		{
			if (alert != null&&alert.IsShowing) {
				alert.Dismiss ();
			}
		}
	}
}

