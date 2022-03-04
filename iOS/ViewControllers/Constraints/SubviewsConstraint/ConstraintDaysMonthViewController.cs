﻿using System;

using UIKit;
using System.Collections.Generic;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
	public partial class ConstraintDaysMonthViewController : BaseViewController
	{
		private int SCREEN_HEIGHT = 568;
		private int BORDER_CONNER = 10;
		private int BORDER_WIDTH = 1;
		public DaysOfMonthCx data;
		private PickerView _view;

		public ConstraintDaysMonthViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ShowNavigationBar ();
			Title = "Days Of Month";
			// Perform any additional setup after loading the view, typically from a nib.
			BorderBtns (btnOff);
			BorderBtns (btnDefault);
			BorderBtns (btnOn);
			View.BackgroundColor = UIColor.FromRGB((nfloat)(255.0/255.0),(nfloat)(228.0/255.0),(nfloat)(154.0/255.0));
			UIHelpers.StyleForButtons (new UIButton[]{ btnDone, btnClear });
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (UIScreen.MainScreen.Bounds.Height > SCREEN_HEIGHT) {
				ctHeight.Constant = UIScreen.MainScreen.Bounds.Height - 64;
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			_view = new PickerView(UIColor.Gray, UIColor.Red, Colors.BidGreen, data);
			_view.BackgroundColor = UIColor.FromRGB((nfloat)(255.0/255.0),(nfloat)(228.0/255.0),(nfloat)(154.0/255.0));
			_view.Frame = viewCalendarShow.Bounds;

            
			viewCalendarShow.AddSubview (_view);

			base.ViewDidAppear (animated);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		void BorderBtns (UIButton btn)
		{
			btn.Layer.BorderColor = UIColor.Black.CGColor;
			btn.Layer.BorderWidth = BORDER_WIDTH;
			btn.Layer.CornerRadius = BORDER_CONNER;
		}

		partial void OnClearEvent (Foundation.NSObject sender)
		{
			_view.ClearAll ();
		}

		partial void OnDoneEvent (Foundation.NSObject sender)
		{
			data.OFFDays = _view.GetListOffDays ();
			data.WorkDays = _view.GetListWorkDays ();
      
            if(data.OFFDays.Count==0 && data.WorkDays.Count==0)
                SharedObject.Instance.ListConstraint.Remove(data);

			this.NavigationController.PopViewController (true);
		}
	}
}


