
using System;

using Foundation;
using UIKit;
using System.Drawing;

namespace Bidvalet.iOS
{
    public partial class BidListCustomHeader : UITableViewHeaderFooterView
    {
        public static readonly UINib Nib = UINib.FromName("BidListCustomHeader", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("BidListCustomHeader");
        public LineViewController _parent;
        NSObject _nsObserver;
        public BidListCustomHeader(IntPtr handle) : base(handle)
        {
        }


        public static BidListCustomHeader Create()
        {
            return (BidListCustomHeader)Nib.Instantiate(null, null)[0];
        }


        public void DownNavigation()
        {

            //	SearchBar.SearchButtonClicked+= SearchBar_SearchButtonClicked;


            SearchBar.ShouldChangeCharacters = (textField, range, replacement) =>
            {
                string text = textField.Text;
                string newText = text.Substring(0, (int)range.Location) + replacement + text.Substring((int)range.Location + (int)range.Length);
                int val;
                if (newText == "")
                    return true;
                else
                    return Int32.TryParse(newText, out val);
            };

            //			SearchBar.ShouldChangeTextInRange= (UITextField textField, NSRange range, string replace) => {
            //				string text = textField.Text;
            //				string newText = text.Substring(0, (int)range.Location) + replace + text.Substring((int)range.Location + (int)range.Length);
            //				int val;
            //				if (newText == "")
            //					return true;
            //				else
            //					return Int32.TryParse(newText, out val);
            //			};



            SearchBar.KeyboardType = UIKeyboardType.NumberPad;

            UIButton menuButton = new UIButton();
            menuButton.SetTitle("Search", UIControlState.Normal);
            menuButton.Frame = new RectangleF(0.0f, 0.0f, 60.0f, 44.0f);
            menuButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            var doneButton = new UIBarButtonItem(menuButton);


            UIButton menuCancelButton = new UIButton();
            menuCancelButton.SetTitle("Cancel", UIControlState.Normal);
            menuCancelButton.Frame = new RectangleF(0.0f, 0.0f, 60.0f, 44.0f);
            menuCancelButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            var cancelButton = new UIBarButtonItem(menuCancelButton);

            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 60.0f, 44.0f));

            menuCancelButton.TouchUpInside += (sender, e) =>
            {
                SearchBar.ResignFirstResponder();
            };
            menuButton.TouchUpInside += (sender, e) =>
            {
                SearchBar.ResignFirstResponder();

                _parent.GoToLine(SearchBar.Text);
            };

            //            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Search, delegate
            //            {
            //                SearchBar.ResignFirstResponder();
            //
            //                _parent.GoToLine(SearchBar.Text);
            //            });

            toolbar.Items = new[] {cancelButton,
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            SearchBar.InputAccessoryView = toolbar;



            UITapGestureRecognizer DownsingleTap;
            DownsingleTap = new UITapGestureRecognizer(() =>
                {

                    _parent.SingleTapSrollDown();


                });

            DownsingleTap.NumberOfTapsRequired = 1;
            btnDownScroll.AddGestureRecognizer(DownsingleTap);


            UITapGestureRecognizer DownDoubleTap;
            DownDoubleTap = new UITapGestureRecognizer(() =>
                {


                    _parent.DoubleTapSrollDown();

                });

            DownDoubleTap.NumberOfTapsRequired = 2;
            btnDownScroll.AddGestureRecognizer(DownDoubleTap);

            DownsingleTap.RequireGestureRecognizerToFail(DownDoubleTap);
            btnVacDiff.Hidden = !GlobalSettings.IsNeedToEnableVacDiffButton;
        }

        //void SearchBar_SearchButtonClicked (object sender, EventArgs e)
        //{
        //    UISearchBar ObjSearchBar=(UISearchBar)sender;
        //    _parent.GoToLine(ObjSearchBar.Text);
        //    //ObjSearchBar.ResignFirstResponder();

        //}
  
        
        public override void AwakeFromNib()
        {
            //NSNotificationCenter.DefaultCenter.RemoveObserver(;
            //_nsObserver = NSNotificationCenter.DefaultCenter.AddObserver(new Foundation.NSString("setButtonStates"), setVACButtonStates);
            btnVacDiff.Hidden = !GlobalSettings.IsNeedToEnableVacDiffButton;
        }
        public void UpNavigation()
        {
            this.SearchBar.EndEditing(true);
            UITapGestureRecognizer UpsingleTap;
            UpsingleTap = new UITapGestureRecognizer(() =>
                {
                    _parent.SingleTapUp();
                });

            UpsingleTap.NumberOfTapsRequired = 1;
            btnUpScroll.AddGestureRecognizer(UpsingleTap);

            UITapGestureRecognizer UpDoubleTap;
            UpDoubleTap = new UITapGestureRecognizer(() =>
                {
                    _parent.DoubleTapUp();
                });

            UpDoubleTap.NumberOfTapsRequired = 2;
            btnUpScroll.AddGestureRecognizer(UpDoubleTap);

            UpsingleTap.RequireGestureRecognizerToFail(UpDoubleTap);
        }

        public void LongPressHandling()
        {
            UILongPressGestureRecognizer longPressUp = new UILongPressGestureRecognizer(() =>
                {
                    _parent.LongTapUp();
                });

            btnUpScroll.AddGestureRecognizer(longPressUp);
            longPressUp.DelaysTouchesBegan = true;



            UILongPressGestureRecognizer longPressDown = new UILongPressGestureRecognizer(() =>
                {
                    _parent.LongTapDown();
                });

            btnDownScroll.AddGestureRecognizer(longPressDown);
            longPressUp.DelaysTouchesBegan = true;
           

        }


        partial void btnVACTapped(UIKit.UIButton sender)
        {
            _parent.btnVacCorrectTap(sender,btnEOM);
        }
        partial void btnEOMTapped(UIButton sender, UIEvent @event)
        {
            _parent.btnEOMTapped(sender, btnVAC);
        }
       
        public void setVACButtonStates(NSNotification n)
        {
            setButtonStates();
        }
        partial void btnVacDiffClicked(UIButton sender)
        {
            _parent.btnVacDifftap();
        }
        
        public void setButtonStates()
        {
            if (GlobalSettings.IsVacationCorrection || GlobalSettings.IsFVVacation)
            {
                btnVAC.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL);
                btnVAC.BackgroundColor = UIColor.DarkGray;
            }
            else
            {
                btnVAC.Enabled = false;
                btnVAC.BackgroundColor = UIColor.LightGray;

            }

            btnEOM.Enabled = (!GlobalSettings.MenuBarButtonStatus.IsOverlap && !GlobalSettings.MenuBarButtonStatus.IsMIL && (GlobalSettings.CurrentBidDetails.Postion == "FA" || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3).DayOfWeek == 0));
            if(btnEOM.Enabled)
                btnEOM.BackgroundColor = UIColor.DarkGray;
            else
                btnEOM.BackgroundColor = UIColor.LightGray;
            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
            }


            btnEOM.Selected = GlobalSettings.MenuBarButtonStatus.IsEOM;
            if(btnEOM.Selected)
                btnEOM.BackgroundColor = UIColor.SystemBlueColor;
            btnVAC.Selected = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
            if (btnVAC.Selected)
                btnVAC.BackgroundColor = UIColor.SystemBlueColor;
        }
        
    }
}

