#region NameSpace
using System;
using UIKit;
using Foundation;
using Bidvalet.iOS.Utility;
using Bidvalet.Model;
using System.Drawing; 
#endregion

namespace Bidvalet.iOS
{
    public partial class PositionsAvoidanceViewController : BaseViewController
    {
        #region Private Variables
        int _countSeniorities = 0;
        int _countAllSeniorities = 0; 
        #endregion

        public PositionsAvoidanceViewController(IntPtr handle)
            : base(handle)
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
                Title = "Positions";
                UIHelpers.StyleForButtons(new[] { btnCalculateBid });

                var btnSort = new UIBarButtonItem("Submit Bid", UIBarButtonItemStyle.Plain, null);
                btnSort.Clicked += onCalculateClickedEvent;
                NavigationItem.RightBarButtonItem = btnSort;


                edSeniority.KeyboardType = UIKeyboardType.NumberPad;
                //edSeniority.EditingDidBegin+= EdSeniority_EditingDidBegin;
                //edSeniority.EditingDidEnd += EdSeniority_EditingDidEnd;
                edBidAvoidOne.Placeholder = "Employee Number";
                edBidAvoidTwo.Placeholder = "Employee Number";
                edBidAvoidThree.Placeholder = "Employee Number";
                edSeniority.Placeholder = "1 to " + GlobalSettings.Lines.Count;

                edBidAvoidOne.Text = string.Empty;
                edBidAvoidTwo.Text = string.Empty;
                edBidAvoidThree.Text = string.Empty;
                if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.AvoidanceBids != null)
                {
                    if (GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1 != string.Empty && GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1 != "0")
                    {
                        edBidAvoidOne.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1;
                    }

                    if (GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2 != string.Empty && GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2 != "0")
                    {
                        edBidAvoidTwo.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2;
                    }
                    if (GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3 != string.Empty && GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3 != "0")
                    {
                        edBidAvoidThree.Text = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3;
                    }

                }


                edSeniority.Enabled = false;
                _countAllSeniorities = GlobalSettings.Lines.Count;
                _countSeniorities = GlobalSettings.UserInfo.SeniorityNumber;
                edSeniority.Text = _countAllSeniorities.ToString();

                edSeniority.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    string text = textField.Text;

                    string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                    int val;
                    if (newText == "")
                        return true;
                    else
                        return Int32.TryParse(newText, out val);
                };

                edSeniority.EditingChanged += (object sender, EventArgs e) =>
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

                var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                {
                    edSeniority.ResignFirstResponder();
                });

                toolbar.Items = new UIBarButtonItem[] {
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
				doneButton
			};
                edSeniority.InputAccessoryView = toolbar;

                //edSeniority.ShouldReturn += (textField) =>
                //{
                //    textField.ResignFirstResponder();
                //    return true;
                //};

                edBidAvoidOne.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    string text = textField.Text;
                    string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                    int val;

                    if (newText == "" && newText.Length < 10)
                        return true;
                    else
                        return Int32.TryParse(newText, out val);
                };
                edBidAvoidTwo.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    string text = textField.Text;
                    string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                    int val;

                    if (newText == "" && newText.Length < 10)
                        return true;
                    else
                        return Int32.TryParse(newText, out val);
                };
                edBidAvoidThree.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    string text = textField.Text;
                    string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                    int val;

                    if (newText == "" && newText.Length < 10)
                        return true;
                    else
                        return Int32.TryParse(newText, out val);
                };

                UpdateUI();
                btnCalculateBid.SetTitle("Login to Submit Bid", UIControlState.Normal);

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        void EdSeniority_EditingDidEnd(object sender, EventArgs e)
        {
            UIView.Animate(0.1f, () =>
            {
                this.View.Frame = new CoreGraphics.CGRect(View.Frame.X, View.Frame.Y + 200, View.Frame.Width, View.Frame.Height);
            });
        }

        void EdSeniority_EditingDidBegin(object sender, EventArgs e)
        {
            UIView.Animate(0.3f, () =>
            {
                this.View.Frame = new CoreGraphics.CGRect(View.Frame.X, View.Frame.Y - 200, View.Frame.Width, View.Frame.Height);
            });
        }

        void onCalculateClickedEvent(object sender, EventArgs e)
        {
            CalculateClick();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        partial void OnCalculateBidClickEvent(Foundation.NSObject sender)
        {
            CalculateClick();
            // onCalculateClickedEvent(sender, null);
        }

        partial void OnEDSeniorityChangedValue(Foundation.NSObject sender)
        {
            if ((!edSeniority.Text.Equals("") || edSeniority.Text.Length > 0) && Convert.ToInt32(edSeniority.Text) < 432)
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

        partial void OnSegmentSeniorityChangeValue(Foundation.NSObject sender)
        {
            switch (segSelectSeniority.SelectedSegment)
            {
                case 0:
                    edSeniority.Enabled = false;
                    edSeniority.Text = _countAllSeniorities.ToString();
                    //_countSeniorities = 432;
                    break;
                case 1:
                    edSeniority.Enabled = true;
                    edSeniority.Text = _countSeniorities.ToString();

                    //edSeniority.Text = "57";
                    //_countSeniorities = 57;
                    break;
            }
            UpdateUI();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;
            if (touch != null)
            {
                View.EndEditing(true);
            }
        } 
        #endregion

        #region Methods
        private void UpdateUI()
        {
            if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
            {


                if (GlobalSettings.UserInfo != null && GlobalSettings.UserInfo.SeniorityNumber != 0)
                {
                    lbSeniority.Text = "Your Seniority is: " + GlobalSettings.UserInfo.SeniorityNumber + " of " + GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount;
                }
                else
                {
                    lbSeniority.Text = "You are NOT in the seniority list for " + GlobalSettings.CurrentBidDetails.Domicile + " " + GlobalSettings.CurrentBidDetails.Postion;

                }
            }
            else
                lbSeniority.Text = string.Empty;
        }

        private void CalculateClick()
        {

            if (edBidAvoidOne.Text.Length != 0)
            {
                if (!RegXHandler.EmployeeNumberValidation(edBidAvoidOne.Text))
                {
                    DisplayAlertView(GlobalSettings.ApplicationName, "Invalid Employee Number in first field.");

                    return;
                }
            }
            if (edBidAvoidTwo.Text.Length != 0)
            {
                if (!RegXHandler.EmployeeNumberValidation(edBidAvoidTwo.Text))
                {
                    DisplayAlertView(GlobalSettings.ApplicationName, "Invalid Employee Number in second field.");

                    return;
                }
            }
            if (edBidAvoidThree.Text.Length != 0)
            {
                if (!RegXHandler.EmployeeNumberValidation(edBidAvoidThree.Text))
                {
                    DisplayAlertView(GlobalSettings.ApplicationName, "Invalid Employee Number in third field.");
                    return;
                }
            }



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
                DisplayAlertView(GlobalSettings.ApplicationName, "Please choose a number that is less than or equal to " + GlobalSettings.Lines.Count + " possible choices");
                return;
            }

            //if (edBidAvoidOne.Text.Length != 0 || edBidAvoidTwo.Text.Length != 0 || edBidAvoidThree.Text.Length != 0)
            //{


            GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1 = (edBidAvoidOne.Text.Trim() == string.Empty) ? "0" : edBidAvoidOne.Text.Trim();
            GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2 = (edBidAvoidTwo.Text.Trim() == string.Empty) ? "0" : edBidAvoidTwo.Text.Trim();
            GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3 = (edBidAvoidThree.Text.Trim() == string.Empty) ? "0" : edBidAvoidThree.Text.Trim();
            WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
            // }


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

            //GlobalSettings.SubmitBidDetails.Base = GlobalSettings.CurrentBidDetails.Domicile;
            //GlobalSettings.SubmitBidDetails.BidRound = (GlobalSettings.CurrentBidDetails.Round == "S") ? "Round 2" : "Round 1";
            //GlobalSettings.SubmitBidDetails.PacketId = WBidHelper.GenaratePacketId(GlobalSettings.CurrentBidDetails);
            //GlobalSettings.SubmitBidDetails.Seat = GlobalSettings.CurrentBidDetails.Postion;
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


