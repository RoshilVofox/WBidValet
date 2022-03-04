
using System;

using Foundation;
using UIKit;
using Bidvalet.Business;

namespace Bidvalet.iOS
{
	public class LinePropertyListControllerController : UITableViewController
	{
		public LineViewController lineView;
		public LinePropertyListControllerController () : base (UITableViewStyle.Plain)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.Title =" Line Property";

			UIBarButtonItem BackButton = new UIBarButtonItem();
			UITextAttributes icoFontAttribute = new UITextAttributes();

			icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(15);
			//icoFontAttribute.TextColor = UIColor.Red;
			BackButton.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
			BackButton.Title = "Reset";

			BackButton.Style = UIBarButtonItemStyle.Plain;
			BackButton.Clicked += (sender, args) =>
			{
				//ToDO:Load reset
				if(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
				{
					GlobalSettings.WBidINIContent.SummaryVacationColumns=WBidCollection.GenerateDefaultVacationColumns();
				}
				else
				{
					GlobalSettings.WBidINIContent.DataColumns=WBidCollection.genarateDefaultColumns();
				}

				TableView.ReloadData();
			};

			this.NavigationItem.LeftBarButtonItem = BackButton;


			UIBarButtonItem DoneButton = new UIBarButtonItem();


			icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(15);
			//icoFontAttribute.TextColor = UIColor.Red;
			DoneButton.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
			DoneButton.Title = "Done";

			DoneButton.Style = UIBarButtonItemStyle.Plain;
			DoneButton.Clicked += (sender, args) =>
			{
				//Check line property
				WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				// Register the TableView's data source
				this.NavigationItem.RightBarButtonItem = DoneButton;

				TableView.Source = new LinePropertyListControllerSource();
				this.DismissViewController(true, null);
				lineView.TableView.ReloadData();// Added for reloading line view controller after line property changed
			};

			this.NavigationItem.RightBarButtonItem = DoneButton;

			// Register the TableView's data source
			TableView.Source = new LinePropertyListControllerSource ();

			
		}
	}
}

