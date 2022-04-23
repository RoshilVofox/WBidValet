using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bidvalet.Business;
using Bidvalet.iOS.Utility;
using Bidvalet.Model;
using Foundation;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.VacationDifference
{
    public partial class VacationDifferenceViewController : UIViewController
    {
        LoadingOverlay ActivityIndicator;
        public List<VacationValueDifferenceOutputDTO> lstVacationDifferencedata { get; set; }
        public List<FlightDataChangeVacValues> lstFlightDataChangevalues { get; set; }
        VacationValueDifferenceInputDTO input;
       
        public VacationDifferenceViewController() : base("VacationDifferenceViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnOk.Hidden = true;
            // Perform any additional setup after loading the view, typically from a nib.
            UIBarButtonItem BackButton = new UIBarButtonItem();
            UITextAttributes icoFontAttribute = new UITextAttributes();

            icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(18);
            //icoFontAttribute.TextColor = UIColor.Blue;
            BackButton.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
            BackButton.Title = "< Back";

            BackButton.Style = UIBarButtonItemStyle.Plain;
            BackButton.Clicked += (sender, args) =>
            {
                this.NavigationController.PopViewController(true);
            };
            this.NavigationItem.LeftBarButtonItem = BackButton;
            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = UIColor.White;
            this.NavigationItem.StandardAppearance = appearance;
            this.NavigationItem.ScrollEdgeAppearance = this.NavigationItem.StandardAppearance;


            ActivityIndicator = new LoadingOverlay(View.Bounds, "Retrieving data. \n Please wait..");
            this.View.Add(ActivityIndicator);

            tblVacDifference.Source = new VacDiffTableViewControllerSource(lstFlightDataChangevalues);
            tblVacDifference.ReloadData();
            ActivityIndicator.Hide();

        }


        partial void btnOkClick(NSObject sender)
        {
           // PopViewController(null, true);
            this.DismissViewController(true, null);
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

