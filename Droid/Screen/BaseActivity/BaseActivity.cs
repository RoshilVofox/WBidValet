
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
using System.Reflection.Emit;
using Android.Graphics;
using Android.Support.V7.App;

namespace Bidvalet.Droid
{
	[Activity (Label = "BaseActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class BaseActivity : Activity
	{
		public BidToolbarEvent header;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			header = new BidToolbarEvent ();
			header.SetHeader (this, null, "WBid Valet", Resource.Drawable.icon_arrow_left, "Back", "");
			header.OnBackEvent += HandleBackEvent;
			header.OnLeftButtonEvent += HandleLeftEvent;
			header.OnRightButtonEvent += HandleRightEvent;
		}
		public void HandleRightEvent ()
		{
			
		}

		public void HandleLeftEvent ()
		{
			OnBackPressed ();
		}

		public void HandleBackEvent ()
		{
			OnBackPressed ();
		}


		public void ShowBidDialog (Activity activity, string Title, string message)
		{
			var builder = new Android.Support.V7.App.AlertDialog.Builder (activity);
			builder.SetTitle (Title)
				.SetMessage (message)
				.SetPositiveButton ("Yes", delegate {
					NSBidLog("","Yes");
			})
				.SetNegativeButton ("No", delegate {
					NSBidLog("","no");
			}); 
			builder.Create ().Show ();
		}

		public void SetCustomFontTextView (TextView[] txs, string fontName)
		{
			Typeface font = Typeface.CreateFromAsset (Application.Context.Assets,"Fonts/" + fontName);
			foreach (TextView tx in txs) {
				tx.SetTypeface (font, TypefaceStyle.Normal);
			}
		}
		public void SetCustomFontTextView (TextView[] txs, int which)
		{
			string fontName = "";
			switch (which){
			case 0:
				fontName = "courier.ttf";
				break;
			case 1:
				fontName = "AvenirNextLTPro-Bold.otf";
				break;
			}
			Typeface font = Typeface.CreateFromAsset (Application.Context.Assets,"Fonts/" + fontName);
			foreach (TextView tx in txs) {
				tx.SetTypeface (font, TypefaceStyle.Normal);
			}
		}
		public void BidToast(string msg){
			Toast.MakeText(this, String.Format("++++++{0}", msg),ToastLength.Short).Show();
		}

		public void NSBidLog(string msg){
			Console.WriteLine (String.Format("++++++ BidValet Log ====>>>> {0}", msg));
		}
		public void NSBidLog(string tag, string msg){
			Console.WriteLine (String.Format("++++++ BidValet Log +++++ {0} ========>{1}",tag, msg));
		}

		public void ValidatorInputField(Context context, EditText[] textFields ){
			foreach (EditText textF in textFields) {
				if (String.IsNullOrEmpty (textF.Text)) {
						Toast.MakeText (context, "Field is Empty", ToastLength.Short);
					return;
				}
			}
		}
		public List<string>lstBlockSorted(){
			List<string> lsBlockSort = new List<string> ();
			lsBlockSort.Add ("AM - PM");
			lsBlockSort.Add ("Days Off");
			lsBlockSort.Add ("Pay");
			lsBlockSort.Add ("Pay / Dutty");
			lsBlockSort.Add ("Per Diem");
			return lsBlockSort;
		}
	}
}

