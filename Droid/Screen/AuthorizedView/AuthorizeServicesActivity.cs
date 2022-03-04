
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
	[Activity (Label = "AuthorizeServicesActivity", Theme = "@style/Bid.ThemeTitle")]			
	public class AuthorizeServicesActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayoutAuthorizeServices);
		}
		protected override void OnResume ()
		{
			base.OnResume ();
			ShowDialogAuthorizeServices ();
		}

		private void ShowDialogAuthorizeServices(){
			var builder = new AlertDialog.Builder(this);
			var inflater = this.LayoutInflater;
			var dialogView = inflater.Inflate(Resource.Layout.DialogAuthorizeServices, null);
			if (dialogView != null)
			{
				ListView lvAuthors = dialogView.FindViewById<ListView> (Resource.Id.lvAuthorizes);
				lvAuthors.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1,Constants.TitleActionSheet);
				lvAuthors.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					int position = e.Position+1;
					NSBidLog("position:"+ position);
					if(position==Constants.CREATE_ACCOUNT||position==Constants.FOUND_ACCOUNT){
						var intentCreate = new Intent(this, typeof(CreateAccountActivity));
						intentCreate.PutExtra(Constants.POSITION, position);
						if(position==9){
							intentCreate.PutExtra(Constants.USER_EXIST, true);
						}
						StartActivity(intentCreate);
					}else{
						var intentShowError = new Intent(this, typeof(AuthorizeStatusActivity));
						intentShowError.PutExtra(Constants.POSITION, position);
						StartActivity(intentShowError);
					}
				};
			}
			builder.SetView(dialogView);
			//Create the builder 
			var dialog = builder.Create();
			dialog.SetCancelable (true);
			dialog.SetCanceledOnTouchOutside (true);
			dialog.Show ();
		}
	}
}

