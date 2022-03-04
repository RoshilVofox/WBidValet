
using System;

using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
	public partial class ConstraintModalView : UIViewController
	{
		ConstraintsChangeViewController _parentVC;
		public ConstraintModalView (ConstraintsChangeViewController parentVC) : base ("ConstraintModalView", null)
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
            this.View.BackgroundColor = UIColor.DarkGray;
			tvConstraint.TableFooterView = new UIView ();
			tvConstraint.Source = new ConstraintTableSource (this);
		}

		public void AddConstraintAtIndex (int row)
		{
			_parentVC.DismissViewController(true,()=>{
				_parentVC.AddConstraintAtIndex(row);	
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

