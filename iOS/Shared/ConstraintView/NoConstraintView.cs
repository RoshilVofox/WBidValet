using System;

using UIKit;
using Foundation;

namespace Bidvalet.iOS
{
	public partial class NoConstraintView : UIViewController
	{
		ConstraintsChangeViewController _parentVC;
		public NoConstraintView (ConstraintsChangeViewController parentVC) : base ("NoConstraintView", null)
		{
			_parentVC = parentVC;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public void AddConstraintView()
		{
			_parentVC.DismissViewController(true,()=>{
				//_parentVC.AddNoConstraintView(true);	
			});
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null)
			{
				_parentVC.DismissViewController (true, null);
			}
		}
	}
}


