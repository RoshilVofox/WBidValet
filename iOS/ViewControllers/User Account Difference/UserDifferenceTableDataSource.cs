using UIKit;

using System.Collections.Generic;
using System.Linq;

using System;
using Foundation;
using System.Reflection;

namespace Bidvalet.iOS
{
	public class UserDifferenceTableDataSource : UITableViewSource
	{
		
		public UserInformation LocalSelectedUser;
		public UserDifferenceTableDataSource ( List<KeyValuePair<string, string>> DifferenceListt)
		{
			DifferenceList = DifferenceListt;
			//LocalSelectedUser = localSelectedUser;

			LocalSelectedUser = new UserInformation ();
			LocalSelectedUser.FirstName = GlobalSettings.UserInfo.FirstName;
			LocalSelectedUser.LastName = GlobalSettings.UserInfo.LastName;
			LocalSelectedUser.EmpNo = GlobalSettings.UserInfo.EmpNo;
			LocalSelectedUser.Position = GlobalSettings.UserInfo.Position;
			LocalSelectedUser.Email = GlobalSettings.UserInfo.Email;
			LocalSelectedUser.CellNumber = GlobalSettings.UserInfo.CellNumber;
			LocalSelectedUser.CellCarrier = GlobalSettings.UserInfo.CellCarrier;
			LocalSelectedUser.isAcceptMail = GlobalSettings.UserInfo.isAcceptMail;

		}
		public List<KeyValuePair<string, string>> DifferenceList = new List<KeyValuePair<string, string>> ();
		//public List<KeyValuePair<string, string>> SelectedList = new List<KeyValuePair<string, string>> ();
		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			//return 4;
			return DifferenceList.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			
			tableView.RegisterNibForCellReuse (UINib.FromName ("UserAccountDifferenceCell", NSBundle.MainBundle), "UserAccountDifferenceCell");
			UserAccountDifferenceCell cell = (UserAccountDifferenceCell)tableView.DequeueReusableCell (new NSString ("UserAccountDifferenceCell"), indexPath);
			cell.Tag = indexPath.Row;
			cell.BackgroundColor = UIColor.Clear;
			cell.Objsource = this;
			string[] values = DifferenceList [indexPath.Row].Value.Split (',');
			cell.SetDetailCell (DifferenceList [indexPath.Row].Key.ToString (), values [0], values [1]);
			//cell.SetDetailCell ("test", "test", "test");
			return cell;

		}




		public void Changesegment(int row, int SegValue)
		{
			string[] values = DifferenceList [row].Value.Split (',');
			if (DifferenceList [row].Key == "CellCarrier") {
				LocalSelectedUser.CellCarrier = Constants.ListCarrier.IndexOf (values [SegValue]) + 1;
			} 
			else {
				PropertyInfo propertyInfo = LocalSelectedUser.GetType ().GetProperty (DifferenceList [row].Key);
				propertyInfo.SetValue (LocalSelectedUser, Convert.ChangeType (values [SegValue], propertyInfo.PropertyType), null);
			}


		}
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 80;
		}
	}
}
