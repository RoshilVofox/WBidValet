#region NameSpace

using System;
using UIKit;
using System.Linq; 
#endregion


namespace Bidvalet.iOS
{
    public partial class AuthorizationServiceViewController : BaseViewController
    {

     
        public AuthorizationServiceViewController(IntPtr handle)
            : base(handle)
        {
        }

        #region Events
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ShowActionSheet();
        }

        /// <summary>
        /// On Done Button Clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        partial void OnDoneButtonClickedEvent(Foundation.NSObject sender)
        {
            PopViewController(null, true);
        }


        void HandleActionSheetEvent(object sender, UIButtonEventArgs e)
        {
            Console.WriteLine(e.ButtonIndex);
            if (e.ButtonIndex == 0)
            {
                // cancel btn
                PopViewController(null, false);
            }
            else
            {
                var createAccountView = Storyboard.InstantiateViewController("CreateAccountTableViewController") as CreateAccountTableViewController;
                var testCaseViewController = Storyboard.InstantiateViewController("AuthorizationTestCaseViewController") as AuthorizationTestCaseViewController;
                if (testCaseViewController != null)
                {
                    testCaseViewController.messageError = Constants.ErrorMessages.ElementAt((int)e.ButtonIndex - 1);
                    testCaseViewController.topBarTitle = Constants.listTitleTopBar.ElementAt((int)e.ButtonIndex - 1);
                    testCaseViewController.numberRow = (int)(e.ButtonIndex);
                    if (e.ButtonIndex == Constants.EXPIRED_SUBSCRIPTION)
                    {
                        testCaseViewController.isShowPurchaseButton = true;
                    }
                    if (e.ButtonIndex == Constants.NEW_CB_WB_USER || e.ButtonIndex == Constants.VALID_SUBSCRIPTION)
                    {
                        testCaseViewController.buttonTitle = Constants.GO_TO_CONSTRAINTS;
                    }
                    if (e.ButtonIndex == Constants.FOUND_ACCOUNT || e.ButtonIndex == Constants.CREATE_ACCOUNT)
                    {
                        if (e.ButtonIndex == Constants.FOUND_ACCOUNT)
                        {
                            if (createAccountView != null) createAccountView.IsFoundAccount = true;
                        }
                        NavigationController.NavigationItem.HidesBackButton = true;
                        PushViewController(createAccountView, true);
                    }
                    else
                    {
                        PushViewController(testCaseViewController, true);
                    }
                }
            }
        } 
        #endregion

        #region Methods
        void ShowActionSheet()
        {
            var actionSheet = new UIActionSheet(Constants.AUTHORIZATION, null, Constants.CANCEL, null, null);
            int countTitle = Constants.TitleActionSheet.Count;
            for (int i = 0; i < countTitle; i++)
            {
                actionSheet.AddButton(Constants.TitleActionSheet.ElementAt(i));
            }
            actionSheet.Clicked += HandleActionSheetEvent;
            // show sheet
            actionSheet.ShowInView(View);
        } 
        #endregion

       
    }
}

