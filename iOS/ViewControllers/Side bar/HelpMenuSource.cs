using UIKit;

using System.Collections.Generic;
using System.Linq;

using System;
using Foundation;
using System.Reflection;

namespace Bidvalet.iOS
{
	public class HelpMenuSource : UITableViewSource
	{
		
		public UserInformation LocalSelectedUser;
		SideMenuViewController _parent;
		NSDictionary DicHelpITems=null;
		List<string>  Documents;
		List<string>  Videos;
		public HelpMenuSource (SideMenuViewController objParent )
		{
			_parent = objParent;

			Documents= new List<string>{"How to use", "Filters", "Logic of WBidValet"};
			Videos =new List<string>(new string[] {"How to use", "Filters"});	
		}


	
		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 2;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			//return 4;
			NSArray arr =null;
			switch (section) {
			case 0:
				return GlobalSettings.Document.Count;

			case 1:
				return GlobalSettings.Videos.Count;

			}
			return 0;
		}
        public override void WillDisplayHeaderView(UITableView tableView, UIView headerView, nint section)
        {
          //  base.WillDisplayHeaderView(tableView, headerView, section);
			((UITableViewHeaderFooterView)headerView).TextLabel.TextColor = UIColor.Yellow;

		}
        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			
			tableView.RegisterNibForCellReuse (UINib.FromName ("HelpCell", NSBundle.MainBundle), "HelpCell");
			HelpCell cell = (HelpCell)tableView.DequeueReusableCell (new NSString ("HelpCell"), indexPath);
			string data="";

			switch (indexPath.Section) {
			case 0:
				var arrayOfAllKeys = GlobalSettings.Document.Keys.ToArray();
				data =arrayOfAllKeys[indexPath.Row];
				break;
			case 1:
				var arrayOfAllKeys1 = GlobalSettings.Videos.Keys.ToArray();
				data =arrayOfAllKeys1[indexPath.Row];
				break;
			}
			cell.BackgroundColor = UIColor.Clear;
			cell.ContentView.BackgroundColor = UIColor.Clear;
			cell.bindData (data);
		//	cell.SetDetailCell ("");
			//cell.SetDetailCell ("test", "test", "test");
			return cell;

		}


		public override string TitleForHeader (UITableView tableView, nint section)
		{
			switch (section) {
			case 0:
				return "Documents";

			case 1:
				return "Videos";

			}
			return "";
		}

		public override nfloat EstimatedHeightForHeader (UITableView tableView, nint section)
		{
			return 30;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{

			string data="";
			switch (indexPath.Section) {
			case 0:
				var arrayOfAllKeys = GlobalSettings.Document.Keys.ToArray();
				data =arrayOfAllKeys[indexPath.Row];
				NSNotificationCenter.DefaultCenter.PostNotificationName ("CallHelpDocument", (NSString)data);
				break;
			case 1:
				var arrayOfAllKeys1 = GlobalSettings.Videos.Keys.ToArray();
				data =arrayOfAllKeys1[indexPath.Row];
				NSNotificationCenter.DefaultCenter.PostNotificationName ("CallHelpVideo", (NSString)data);
				break;
			}
			_parent.DismissViewController (false, null);

		}

//
//
//		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
//		{
//			return 80;
//		}
	}
}
