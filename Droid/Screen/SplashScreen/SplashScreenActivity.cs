
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
using System.Threading;

namespace Bidvalet.Droid
{
	[Activity (Label = "Bidvalet", Theme = "@style/BidTheme.Splash", Icon = "@drawable/icon", MainLauncher=true, NoHistory=true)]	
	//[Activity (Label = "Bidvalet", Theme = "@style/BidTheme.Splash")]	
	public class SplashScreenActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			Thread.Sleep(2500); // Simulate a long loading process on app startup.
			StartActivity(typeof(MainActivity));

		}
	}
}

