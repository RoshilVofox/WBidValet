
using System;

using Foundation;
using UIKit;
using System.IO;
using CoreGraphics;

namespace Bidvalet.iOS
{
    public partial class WebViewForLicence : UIViewController
    {
        public bool isLicence;
        public enum WebType
        {
            Licence = 1,
            Policy,
            Document,
            Video
        };

        public WebType ViewType;
        public string DocumentType;
        public WebViewForLicence()
            : base("WebViewForLicence", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override bool ShouldAutorotate()
        {
            return false;
        }
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.All;
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            return UIInterfaceOrientation.LandscapeLeft;
        }







        public override void ViewDidLoad()
        {
          
            base.ViewDidLoad();
            string localDocUrl = null;

            switch (ViewType)
            {
                case WebType.Document:
                    localDocUrl = Path.Combine(NSBundle.MainBundle.ResourcePath, GlobalSettings.Document[DocumentType]);
                    WebView.LoadRequest(new NSUrlRequest(new NSUrl(localDocUrl, false)));
                    ObjNavigationItem.Title = DocumentType;
                    ApplySettings();
                    break;
                case WebType.Licence:
                    localDocUrl = Path.Combine(NSBundle.MainBundle.ResourcePath, "License.pdf");
                    WebView.LoadRequest(new NSUrlRequest(new NSUrl(localDocUrl, false)));
                    ObjNavigationItem.Title = "License Agreement";
                    ApplySettings();
                    break;
                case WebType.Policy:
                    localDocUrl = Path.Combine(NSBundle.MainBundle.ResourcePath, " Privacy Policy.pdf");
                    WebView.LoadRequest(new NSUrlRequest(new NSUrl(localDocUrl, false)));
                    ObjNavigationItem.Title = "Privacy Policy";
                    ApplySettings();
                    break;
                case WebType.Video:
                    LoadVideo();
                    break;
            }



       
            // Perform any additional setup after loading the view, typically from a nib.
        }


        private void ApplySettings()
        {
            WebView.ScrollView.SetZoomScale(20, true);
            WebView.UserInteractionEnabled = true;
            WebView.ScalesPageToFit = true;
        }
        partial void btnDoneClicked(NSObject sender)
        {
            WebView.LoadHtmlString(string.Empty, null);
            this.DismissViewController(true, null);
        }

        private void LoadVideo()
        {
           // this.Title = "Help";
          //  this.NavigationController.NavigationItem.Title = "Help";
            ObjNavigationItem.Title = DocumentType;
            string videoId = GlobalSettings.Videos[DocumentType];

            int typeOfInternetConnection = InternetHelper.CheckInterNetConnection();

            if (typeOfInternetConnection == (int)InternetType.NoInternet)
            {

                WebView.LoadHtmlString(string.Empty, null);
                //DisplayAlertView(GlobalSettings.ApplicationName, "No Internet connection found. Please try again later.");
                DisplayAlertView(GlobalSettings.ApplicationName, Constants.VPSDownAlert);

            }

              //Grount type internet
            else if (typeOfInternetConnection == (int)InternetType.Ground || typeOfInternetConnection == (int)InternetType.AirPaid)
            {

              //  WebView.Frame = new CGRect(0, 0, 300, 300);
                string loadStr = "<iframe min-width=\"400\" width=\"100%\"   min-height=\"430\" height=\"450\" src=\"http://www.youtube.com/embed/" + videoId + "?rel=0&amp;showinfo=0\" frameborder=\"0\" allowfullscreen></iframe>";
            
                WebView.LoadHtmlString(loadStr, null);
                WebView.BackgroundColor = UIColor.Black;
                WebView.ScalesPageToFit = true;
                WebView.ScrollView.ScrollEnabled = false;
                WebView.ScrollView.SetZoomScale(0, true);
            }
            else
            {
                WebView.LoadHtmlString(string.Empty, null);
                DisplayAlertView(GlobalSettings.ApplicationName,Constants.SouthWestConnectionAlert);
            }
        }


        private void DisplayAlertView(string caption, string message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();



        }
    }
}

