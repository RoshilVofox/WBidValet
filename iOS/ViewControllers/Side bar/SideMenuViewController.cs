
using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
	public partial class SideMenuViewController : UIViewController
	{
		public SideMenuViewController () : base ("SideMenuViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

       

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.View.BackgroundColor=UIColor.Clear;
			tableView.Source = new HelpMenuSource (this);
		
			//this.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			UITapGestureRecognizer ObjGuesture= new UITapGestureRecognizer (this, new  ObjCRuntime.Selector ("ViewTapped"));
			ObjGuesture.NumberOfTapsRequired = 1;
			this.ViewDismiss.AddGestureRecognizer (ObjGuesture);
			// Perform any additional setup after loading the view, typically from a nib.
		}
		[Export("ViewTapped")]
		void ViewTapped()
		{
			this.DismissViewController(false,null);
		}

	}
}

