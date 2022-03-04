
using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Linq;

namespace Bidvalet.iOS
{
    public partial class CommutableTimeViewController : UIViewController
    {
        private CommutablePickerView _view;
        public FtCommutableLine data;
        public string CityName { get; set; }
        public CommutableTimeViewController()
            : base("CommutableTimeViewController", null)
        {
        }
        public CommutableTimeViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidAppear(bool animated)
        {
            _view = new CommutablePickerView(data);
            _view.BackgroundColor = UIColor.FromRGB((nfloat)(255.0 / 255.0), (nfloat)(228.0 / 255.0), (nfloat)(154.0 / 255.0));
            _view.Frame = Viewcalender.Bounds;


            Viewcalender.AddSubview(_view);

            base.ViewDidAppear(animated);
        }
        partial void DismissCalendarView(NSObject sender)
        {
            this.DismissViewController(true, null);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            lblCommuteCity.Text = string.Empty;
            lblBase.Text = GlobalSettings.CurrentBidDetails.Domicile;

            //if (data != null) lblCommuteCity.Text = data.City;
            if (data != null) lblCommuteCity.Text = CityName;

            //WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            ////  wBIdStateContent.BidAuto = null;
            //if (wBIdStateContent.BidAuto != null && wBIdStateContent.BidAuto.BAFilter != null)
            //{
            //    var sl = wBIdStateContent.BidAuto.BAFilter.FirstOrDefault(x => x.Name == "CL");
            //    lblCommuteCity.Text = ((FtCommutableLine)sl.BidAutoObject).City;
            //}
            //else
            //{
            //    lblCommuteCity.Text = string.Empty;
            //}


            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}

