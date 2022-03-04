#region NameSpace
using System;
using UIKit; 
#endregion

namespace Bidvalet.iOS
{
	public class BaseViewController: UIViewController
	{
		public BaseViewController (IntPtr handle) : base (handle)
		{		
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.NavigationController.SetNavigationBarHidden (true, false);
		}

		public void ShowNavigationBar()
		{
			this.NavigationController.SetNavigationBarHidden (false, false);
			NavigationController.NavigationBar.BarTintColor = UIColor.White;
			NavigationController.NavigationBar.Translucent = false;
			NavigationItem.BackBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain,null);
		}
		public void ShowPopUpInfo(string title, string message){
			new UIAlertView(title, message, null, Constants.OK, null).Show();
		}
		public void PushViewController(UIViewController viewController, bool animated){
			NavigationController.PushViewController (viewController, animated);
		}
		public void PopViewController(UIViewController bidViewController, bool anim){
			if (bidViewController != null) {
				NavigationController.PopToViewController (bidViewController, anim);
			} else {
				NavigationController.PopViewController (anim);
			}
		}
		public void NSLogBidValet(string msg){
			Console.WriteLine(msg);
		}
	}
}

