#region NameSpace
using System;
using UIKit; 
#endregion

namespace Bidvalet.iOS
{
	public partial class EmailSentViewController : BaseViewController
	{
        #region Variable
        public bool isEmailSent = true;
        
        #endregion

		public EmailSentViewController (IntPtr handle) : base (handle)
		{
		}

        #region Events
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (isEmailSent)
            {
                Title = Constants.TITLE_SENT_EMAIL;
                lbSentTo.Text = Constants.TITLE_SENT_EMAIL_TEXT;
                lbDepricationSent.Text = GlobalSettings.UserInfo.Email;
                //lbDepricationSent.Text = Constants.TITLE_EMAIL;
            }
            else
            {
                lbSentTo.Text = Constants.TITLE_SENT_PRINT_TEXT;
                Title = Constants.TITLE_SENT_PRINT;
                lbDepricationSent.Text = Constants.TITLE_PRINT;
            }
            ShowNavigationBar();
            UIHelpers.StyleForButtons(new UIButton[] { btnDone });
            UIBarButtonItem btnSort = new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, null);
            btnSort.Clicked += onSortClickedEvent;
            NavigationItem.RightBarButtonItem = btnSort;
        }

        void onSortClickedEvent(object sender, EventArgs e)
        {
            SubScriptionViewController viewController = Storyboard.InstantiateViewController("SubScriptionViewController") as SubScriptionViewController;
            PushViewController(viewController, true);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void OnDoneClickEvent(Foundation.NSObject sender)
        {
            //SubScriptionViewController viewController = Storyboard.InstantiateViewController("SubScriptionViewController") as SubScriptionViewController;
            this.NavigationController.PopToRootViewController(true);//PushViewController(viewController,true);
        } 
        #endregion
	}
}


