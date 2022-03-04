
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Java.Lang; //for the reflection
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Graphics;


namespace Bidvalet.Droid
{
	[Android.App.Activity (Label = "ExpriedSubscriptionActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class ExpriedSubscriptionActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.ExpiredSubscription);
			header.SetHeader (this, null, "Subscribe", Resource.Drawable.icon_arrow_left, "Back", "");
		}
	}
}

