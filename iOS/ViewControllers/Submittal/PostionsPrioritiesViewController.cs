#region NameSpace
using System;
using UIKit;
using System.Drawing;
using Foundation;
using Bidvalet.iOS.Utility;
using Bidvalet.Model;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Bidvalet.iOS
{
	public partial class PostionsPrioritiesViewController : BaseViewController
	{
        #region Private Variables
        int _countAllSeniorities = 0;
        int _countSeniorities = 0;
        string[] availablePositions = { "A", "B", "C", "D", "None" };
        
        #endregion

		public PostionsPrioritiesViewController (IntPtr handle ) : base (handle)
		{
		}

        #region Events
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            ShowNavigationBar();
            UIHelpers.StyleForButtons(new UIButton[] { btCalculateBid });
            UIHelpers.StyleForButtonsBorderBlackRectange(new UIButton[] { btNonePriority, btPriorityPositionFirst, btPriorityPositionSecond, btPriorityPositionThird });
            Title = "Positions";

            UIBarButtonItem btnSort = new UIBarButtonItem("Submit Bid", UIBarButtonItemStyle.Plain, null);
            btnSort.Clicked += onCalculateClickedEvent;
            NavigationItem.RightBarButtonItem = btnSort;

            edChoosePriority.KeyboardType = UIKeyboardType.NumberPad;
            edChoosePriority.Enabled = true;

            int totalABCChoices = GlobalSettings.Lines.Where(x => string.Join("", x.FAPositions) == "ABC").Count();

            int totalDChoices = GlobalSettings.Lines.Where(x => string.Join("", x.FAPositions) == "D").Count();


            _countAllSeniorities = totalABCChoices * 3 + totalDChoices;
                //GlobalSettings.Lines.Count;
            _countSeniorities = GlobalSettings.UserInfo.SeniorityNumber > _countAllSeniorities ? 0 : GlobalSettings.UserInfo.SeniorityNumber;

            edChoosePriority.Text = _countSeniorities.ToString();

            edChoosePriority.Placeholder = "1 to " + GlobalSettings.Lines.Count;
            edChoosePriority.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                string text = textField.Text;

                string newText = text.Substring(0, (int)range.Location) + replacementString + text.Substring((int)range.Location + (int)range.Length);
                int val;
                if (newText == "")
                    return true;
                else
                    return Int32.TryParse(newText, out val);
            };

            edChoosePriority.EditingChanged += (object sender, EventArgs e) =>
            {
                //Console.WriteLine("start");
                if (segBidChoices.SelectedSegment == 0)
                {
                    if (edChoosePriority.Text != string.Empty)
                        _countAllSeniorities = int.Parse(edChoosePriority.Text);

                }
                else
                {
                    if (edChoosePriority.Text != string.Empty)
                        _countSeniorities = int.Parse(edChoosePriority.Text);
                }
            };

            //edChoosePriority.ShouldReturn += (textField) =>
            //{
            //    textField.ResignFirstResponder();
            //    return true;
            //};

            //edChoosePriority.EditingDidBegin += EdChoosePriority_EditingDidBegin;
            //edChoosePriority.EditingDidEnd += EdChoosePriority_EditingDidEnd;
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                edChoosePriority.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            edChoosePriority.InputAccessoryView = toolbar;
            btPriorityPositionFirst.SetTitle("A", UIControlState.Normal);
            btPriorityPositionSecond.SetTitle("B", UIControlState.Normal);
            btPriorityPositionThird.SetTitle("C", UIControlState.Normal);
            btNonePriority.SetTitle("None", UIControlState.Normal);
            btCalculateBid.SetTitle("Login to Submit Bid", UIControlState.Normal);

        }

        void EdChoosePriority_EditingDidEnd(object sender, EventArgs e)
        {
            UIView.Animate(0.1f, () =>
            {
                View.Frame = new CoreGraphics.CGRect(View.Frame.X, View.Frame.Y + 200, View.Frame.Width, View.Frame.Height);
            });
        }

        void EdChoosePriority_EditingDidBegin(object sender, EventArgs e)
        {
            UIView.Animate(0.3f, () =>
            {
                View.Frame = new CoreGraphics.CGRect(View.Frame.X, View.Frame.Y - 200, View.Frame.Width, View.Frame.Height);
            });
        }
        void onCalculateClickedEvent(object sender, EventArgs e)
        {
            HandleOnCalculateBidEvent();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void OnAddReserveChangeValue(Foundation.NSObject sender)
        {
            NSLogBidValet("OnAddReserveChangeValue");
        }

        partial void OnCalculateBidClickEvent(Foundation.NSObject sender)
        {
            HandleOnCalculateBidEvent();
        }

        partial void OnNonePriorityClickEvent(Foundation.NSObject sender)
        {
            int index = Array.IndexOf(availablePositions, btNonePriority.TitleLabel.Text);
            if (index == 3)
            {
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);
               // this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
                return;
            }
            if (btPriorityPositionThird.TitleLabel.Text == "None")
            {
                btNonePriority.TitleLabel.Text = "None";
                ApplyRedColor(btNonePriority);
              
                DisplayAlertView(GlobalSettings.ApplicationName, "You cannot set the 4th position while the 3rd position is “NONE”.  Change the 3rd position first");
               // this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
                //this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);
                return;
            }
            for (int i = 0; i < 5; i++)
            {
                if (index == 4)
                    index = 0;
                else
                    index++;

                if ((btPriorityPositionFirst.TitleLabel.Text != availablePositions[index]) && (btPriorityPositionSecond.TitleLabel.Text != availablePositions[index]) && (btPriorityPositionThird.TitleLabel.Text != availablePositions[index]) && (btNonePriority.TitleLabel.Text != availablePositions[index]))
                {
                    btNonePriority.SetTitle(availablePositions[index], UIControlState.Normal);
                    if (index == 4)
                    {
                        ApplyRedColor(btNonePriority);
                    }
                    else
                    {
                        ApplyGreenColor(btNonePriority);
                    }
                  
                }

            }

            //this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
            //SetSubmitButtonStatus();
            // this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);

            NSLogBidValet("OnNonePriorityClickEvent");
        }

        partial void OnPriorityFirstClickEvent(Foundation.NSObject sender)
        {


            int index = Array.IndexOf(availablePositions, btPriorityPositionFirst.TitleLabel.Text);
            if (index == 3)
                index = 0;
            else
                index++;
            btPriorityPositionFirst.SetTitle(availablePositions[index], UIControlState.Normal);
           

            if (btPriorityPositionSecond.TitleLabel.Text == availablePositions[index])
            {
                btPriorityPositionSecond.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btPriorityPositionSecond);
                btPriorityPositionThird.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btPriorityPositionThird);
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);
            }
            if (btPriorityPositionThird.TitleLabel.Text == availablePositions[index])
            {
                btPriorityPositionThird.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btPriorityPositionThird);
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);

            }
            if (btNonePriority.TitleLabel.Text == availablePositions[index])
            {
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);
            }
            // this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);


            //this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
            NSLogBidValet("OnPriorityFirstClickEvent");
        }

        //[Export("SetBackGroundColor")]
        private void SetBackGroundColor()
        {
            if (btPriorityPositionSecond.TitleLabel.Text == "None")
                btPriorityPositionSecond.BackgroundColor = UIColor.Red;

            else
                btPriorityPositionSecond.BackgroundColor = UIColor.Green;

            if (btPriorityPositionThird.TitleLabel.Text == "None")
                btPriorityPositionThird.BackgroundColor = UIColor.Red;
            //btPriorityPositionThird.BackgroundColor = UIColor.FromRGB(172, 255, 139);
            else
                btPriorityPositionThird.BackgroundColor = UIColor.Green;
                //btPriorityPositionThird.BackgroundColor = UIColor.FromRGB(251, 74, 8);

            if (btNonePriority.TitleLabel.Text=="None" )
                btNonePriority.BackgroundColor = UIColor.Red;
            else
                btNonePriority.BackgroundColor = UIColor.Green;
                
        }

        private void ApplyRedColor(UIButton uIButton)
        {
            uIButton.BackgroundColor = UIColor.FromRGB(251, 74, 8);
        }

        private void ApplyGreenColor(UIButton uIButton)
        {
            uIButton.BackgroundColor = UIColor.FromRGB(172, 255, 139);
            
        }

        partial void OnPrioritySecondClickEvent(Foundation.NSObject sender)
        {

            int index = Array.IndexOf(availablePositions, btPriorityPositionSecond.TitleLabel.Text);
            if (index == 3)
            {
                btPriorityPositionThird.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btPriorityPositionThird);
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);
                btPriorityPositionSecond.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btPriorityPositionSecond);
                // this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);
             //   this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
                return;
            }



            for (int i = 0; i < 5; i++)
            {
                if (index == 4)
                    index = 0;
                else
                    index++;

                if (btPriorityPositionFirst.TitleLabel.Text != availablePositions[index] && (btPriorityPositionSecond.TitleLabel.Text != availablePositions[index]))
                {
                    btPriorityPositionSecond.SetTitle(availablePositions[index], UIControlState.Normal);
                    if (index == 4)
                    {
                        ApplyRedColor(btPriorityPositionSecond);
                    }
                    else
                    {
                        ApplyGreenColor(btPriorityPositionSecond);
                    }
                    break;
                }
            }


            if (btPriorityPositionThird.TitleLabel.Text == availablePositions[index])
            {
                btPriorityPositionThird.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btPriorityPositionThird);
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);
            }
            if (btNonePriority.TitleLabel.Text == availablePositions[index])
            {
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);
            }

           // this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
            // this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);
            NSLogBidValet("OnPrioritySecondClickEvent");
        }

        partial void OnPriorityThirdClickEvent(Foundation.NSObject sender)
        {
            int index = Array.IndexOf(availablePositions, btPriorityPositionThird.TitleLabel.Text);
            if (index == 3)
            {
                btNonePriority.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btNonePriority);
                btPriorityPositionThird.SetTitle("None", UIControlState.Normal);
                ApplyRedColor(btPriorityPositionThird);
                // this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);
              //  this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
                return;
            }

            if (btPriorityPositionSecond.TitleLabel.Text == "None")
            {
                btPriorityPositionThird.TitleLabel.Text = "None";
                ApplyRedColor(btPriorityPositionThird);
                DisplayAlertView(GlobalSettings.ApplicationName, "You cannot set the 3rd or 4th position while the 2nd position is “NONE”.  Change the 2nd position first");
                //UIAlertView alert = new UIAlertView();
                //alert.Title = "You cannot set the 3rd or 4th position while the 2nd position is “NONE”.  Change the 2nd position first";
                ////				alert.AddButton("No");
                //alert.AddButton("OK");
                ////				alert.Clicked += confirmClicked;
                //alert.Show();
                //this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);
               // this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                if (index == 4)
                    index = 0;
                else
                    index++;

                if ((btPriorityPositionFirst.TitleLabel.Text != availablePositions[index]) && (btPriorityPositionSecond.TitleLabel.Text != availablePositions[index]) && (btPriorityPositionThird.TitleLabel.Text != availablePositions[index]))
                {
                    btPriorityPositionThird.SetTitle(availablePositions[index], UIControlState.Normal);
                    if (index == 4)
                    {
                        ApplyRedColor(btPriorityPositionThird);
                    }
                    else
                    {
                        ApplyGreenColor(btPriorityPositionThird);
                    }
                    break;
                }
            }
            if (btNonePriority.TitleLabel.Text == availablePositions[index])
            {
                btNonePriority.SetTitle("None", UIControlState.Normal);
            }
            //this.PerformSelector(new ObjCRuntime.Selector("SetBackGroundColor"), null, 0); 

            //SetSubmitButtonStatus();
            // this.PerformSelector(new ObjCRuntime.Selector("SetSubmitButtonStatus"), null, 0.5);
        }

        partial void OnReserveOnlyChangeValue(Foundation.NSObject sender)
        {
           
            NSLogBidValet("OnReserveOnlyChangeValue");
        }

        partial void OnSegBidChoicesChangeValue(Foundation.NSObject sender)
        {
            switch (segBidChoices.SelectedSegment)
            {
                case 0:
                    edChoosePriority.Enabled = false;
                    edChoosePriority.Text = _countAllSeniorities.ToString();
                    //_countSeniorities = 432;
                    break;
                case 1:
                    edChoosePriority.Enabled = true;
                    edChoosePriority.Text = _countSeniorities.ToString();

                    //edSeniority.Text = "57";
                    //_countSeniorities = 57;
                    break;
            }
            UpdateUI();
        }




        partial void OnSegPrioritiesChangeValue(Foundation.NSObject sender)
        {

           // GlobalSettings.SubmitBidDetails.IsRepeatline
         
            NSLogBidValet("OnSegPrioritiesChangeValue");
        }
        
        #endregion

        #region Methods
        private void UpdateUI()
        {
            //lbSeniority.Text = String.Format("Your Seniority is: {0} of 432",_countSeniorities);
        }

        private void HandleOnCalculateBidEvent()
        {


            if (String.IsNullOrEmpty(edChoosePriority.Text))
            {
                DisplayAlertView(GlobalSettings.ApplicationName, " Seniority Number cannot be empty.");
                return;
            }

            if (!RegXHandler.NumberValidation(edChoosePriority.Text))
            {
                DisplayAlertView(GlobalSettings.ApplicationName, "Invalid Seniority Number.");
                return;
            }


            if (Convert.ToInt32(edChoosePriority.Text) < 1 || Convert.ToInt32(edChoosePriority.Text) > _countAllSeniorities)
            {
                DisplayAlertView(GlobalSettings.ApplicationName, "Please choose a Seniority Number between 1 and " + _countAllSeniorities);
                return;
            }


            GlobalSettings.SubmitBidDetails = new SubmitBid();
            GlobalSettings.SubmitBidDetails.IsSubmitAllChoices = segBidChoices.SelectedSegment == 0;
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


            GlobalSettings.SubmitBidDetails.IsAddReserveToEnd = swAddReserve.On;
            GlobalSettings.SubmitBidDetails.IsReserveOnly = swReserveOnly.On;


            GlobalSettings.SubmitBidDetails.PositionChoices = new List<string>();
            GlobalSettings.SubmitBidDetails.PositionChoices.Add(btPriorityPositionFirst.TitleLabel.Text);
            if (btPriorityPositionSecond.TitleLabel.Text != "None")
            {
                GlobalSettings.SubmitBidDetails.PositionChoices.Add(btPriorityPositionSecond.TitleLabel.Text);
            }

            if (btPriorityPositionThird.TitleLabel.Text != "None")
            {
                GlobalSettings.SubmitBidDetails.PositionChoices.Add(btPriorityPositionThird.TitleLabel.Text);
            }


            if (btNonePriority.TitleLabel.Text != "None")
            {
                GlobalSettings.SubmitBidDetails.PositionChoices.Add(btNonePriority.TitleLabel.Text);
            }

            GlobalSettings.SubmitBidDetails.IsRepeatline = segPriorities.SelectedSegment == 0;


           GlobalSettings.SubmitBidDetails.IsAddReserveToEnd = swAddReserve.On ;

           GlobalSettings.SubmitBidDetails.IsReserveOnly = swReserveOnly.On;

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


