#region NameSpace
using System;
using System.Globalization;
using UIKit;
using Bidvalet.Model;
using Bidvalet.iOS.Utility;
using System.Drawing; 
#endregion

namespace Bidvalet.iOS
{
	public partial class BidChoicesViewController : BaseViewController
	{
        #region Private Variables
		
		int _countSeniorities;
        int _countAllSeniorities; 
	    #endregion

		public BidChoicesViewController (IntPtr handle) : base (handle)
		{
		}

        #region Events
        public override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();

                // Perform any additional setup after loading the view, typically from a nib.
                ShowNavigationBar();
                Title = "How Many Choices";
			
               // UIHelpers.StyleForButtons(new[] { btnCalculateBid });

                edSeniority.KeyboardType = UIKeyboardType.NumberPad;
                edSeniority.Placeholder = "1 to " + GlobalSettings.Lines.Count;
                edSeniority.Enabled = false;
                _countAllSeniorities = GlobalSettings.Lines.Count;
                _countSeniorities =(GlobalSettings.UserInfo!=null)? GlobalSettings.UserInfo.SeniorityNumber:0;
                edSeniority.Text = _countAllSeniorities.ToString(CultureInfo.InvariantCulture);
                lblMaxChoice.Text = "You can submit up to " + _countAllSeniorities + " choices";
                lblMaxChoice.TextAlignment = UITextAlignment.Center;
               lblMaxChoice.Font= UIFont.FromName(lblMaxChoice.Font.Name, 14f);

                edSeniority.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    string text = textField.Text;

                    string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                    int val;
                    if (newText == "")
                        return true;
                    return Int32.TryParse(newText, out val);
                };

                edSeniority.EditingChanged += (sender, e) =>
                {

                    if (segSelectSeniority.SelectedSegment == 0)
                    {
                        if (edSeniority.Text != string.Empty)
                            _countAllSeniorities = int.Parse(edSeniority.Text);

                    }
                    else
                    {
                        if (edSeniority.Text != string.Empty)
                            _countSeniorities = int.Parse(edSeniority.Text);
                    }
                };

                //edSeniority.ShouldReturn += (textField) =>
                //{
                //    textField.ResignFirstResponder();
                //    return true;
                //};


                var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                {
                    edSeniority.ResignFirstResponder();
                });

                toolbar.Items = new[] {
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
				doneButton
			};
                edSeniority.InputAccessoryView = toolbar;



                var btnSort = new UIBarButtonItem("Submit Bid", UIBarButtonItemStyle.Plain, null);

                UITextAttributes icoFontAttribute = new UITextAttributes();

                icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(14);
                //icoFontAttribute.TextColor = UIColor.Blue;
                btnSort.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
           
              
                btnSort.Clicked += onCalculateClickedEvent;
                NavigationItem.RightBarButtonItem = btnSort;
                btnCalculateBid.SetTitle("Login to Submit Bid", UIControlState.Normal);
                UpdateUI();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void OnInputSeniorityChange(Foundation.NSObject sender)
        {

            try
            {

                if ((!edSeniority.Text.Equals("") || edSeniority.Text.Length > 0) && Convert.ToInt32(edSeniority.Text) < GlobalSettings.Lines.Count)
                {
                    _countSeniorities = Convert.ToInt32(edSeniority.Text);
                }
                else
                {
                    _countSeniorities = 0;
                }
                segSelectSeniority.SelectedSegment = -1;
                UpdateUI();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        partial void OnCalculateBidClickEvent(Foundation.NSObject sender)
        {

            try
            {

                CalculateClick();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void onCalculateClickedEvent(object sender, EventArgs e)
        {
            try
            {
                CalculateClick();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        partial void OnSegmentSeniorityChangeValue(Foundation.NSObject sender)
        {
            try
            {
                switch (segSelectSeniority.SelectedSegment)
                {
                    case 0:
                        edSeniority.Enabled = false;
                        edSeniority.Text = _countAllSeniorities.ToString(CultureInfo.InvariantCulture);
                        //_countSeniorities = 432;
                        break;
                    case 1:
                        edSeniority.Enabled = true;
                        edSeniority.Text = _countSeniorities.ToString(CultureInfo.InvariantCulture);

                        //edSeniority.Text = "57";
                        //_countSeniorities = 57;
                        break;
                }
                UpdateUI();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        partial void OnEditTextSeniorityTouchInside(Foundation.NSObject sender)
        {
            segSelectSeniority.SelectedSegment = -1;
        }
        
        #endregion

        #region Methods
        private void UpdateUI()
        {


            if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
            {

                //lbChooseSeniority.Text = String.Format("Your Seniority is: {0} of 432",_countSeniorities);
                if (GlobalSettings.UserInfo != null && GlobalSettings.UserInfo.SeniorityNumber != 0)
                {
                    lbChooseSeniority.Text = "Your Seniority is: " + GlobalSettings.UserInfo.SeniorityNumber + " of " + GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount;
                }
                else
                {
                    lbChooseSeniority.Text = "You are NOT in the seniority list for " + GlobalSettings.CurrentBidDetails.Domicile + " " + GlobalSettings.CurrentBidDetails.Postion;

                }
            }
            else
                lbChooseSeniority.Text = string.Empty;
        }

        private void CalculateClick()
        {
            if (String.IsNullOrEmpty(edSeniority.Text))
            {
                DisplayAlertView(GlobalSettings.ApplicationName, "Please choose a number.");
                return;
            }

            if (!RegXHandler.NumberValidation(edSeniority.Text))
            {
                DisplayAlertView(GlobalSettings.ApplicationName, "Invalid number.");
                return;
            }


            if (Convert.ToInt32(edSeniority.Text) < 1 || Convert.ToInt32(edSeniority.Text) > GlobalSettings.Lines.Count)
            {
                DisplayAlertView(GlobalSettings.ApplicationName,"Please choose a number that is less than or equal to "+GlobalSettings.Lines.Count+" possible choices");
                return;
            }

            GlobalSettings.SubmitBidDetails = new SubmitBid();
            GlobalSettings.SubmitBidDetails.IsSubmitAllChoices = segSelectSeniority.SelectedSegment == 0;
            if (GlobalSettings.SubmitBidDetails.IsSubmitAllChoices)
            {
                GlobalSettings.SubmitBidDetails.SeniorityNumber = _countAllSeniorities;
                GlobalSettings.SubmitBidDetails.TotalBidCount = _countAllSeniorities;
            }
            else
            {
                GlobalSettings.SubmitBidDetails.SeniorityNumber = _countSeniorities;
                GlobalSettings.SubmitBidDetails.TotalBidCount = _countSeniorities;
            }

            // GlobalSettings.SubmitBidDetails.Base = GlobalSettings.CurrentBidDetails.Domicile;
            // GlobalSettings.SubmitBidDetails.BidRound = (GlobalSettings.CurrentBidDetails.Round == "S") ? "Round 2" : "Round 1";
            // GlobalSettings.SubmitBidDetails.PacketId = WBidHelper.GenaratePacketId(GlobalSettings.CurrentBidDetails);
            // GlobalSettings.SubmitBidDetails.Seat = GlobalSettings.CurrentBidDetails.Postion;
            //GlobalSettings.SubmitBidDetails.Bid = WBidHelper.GenarateBidLineString(GlobalSettings.SubmitBidDetails.IsSubmitAllChoices,_countSeniorities);



            SubmitBidViewController viewController = Storyboard.InstantiateViewController("SubmitBidViewController") as SubmitBidViewController;
           PushViewController(viewController, true);


            //CalculateBidViewController viewController = Storyboard.InstantiateViewController("CalculateBidViewController") as CalculateBidViewController;
            //PushViewController(viewController, true);
        }

        private void DisplayAlertView(string caption, String message)
        {
            new UIAlertView(caption, message, null, "OK", null).Show();

        } 
        #endregion
        

	}

}


