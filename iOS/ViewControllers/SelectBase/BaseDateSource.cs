using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.Linq;

namespace Bidvalet.iOS
{
	public class BaseDateSource: UITableViewSource
	{
		List<string> _data;
		SelectBaseViewController _parentVC;
		public BaseDateSource (List<string> data, SelectBaseViewController parentVC)
		{
			_data = data;
			_parentVC = parentVC;

            //if (GlobalSettings.UserInfo != null && !string.IsNullOrEmpty(GlobalSettings.UserInfo.Domicile))
            //{
            //    _selectedIndex = data.FindIndex(x => x == GlobalSettings.UserInfo.Domicile);
            //}
		}
		int _selectedIndex = -1;
		//ovride method set Section
		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		//overide method set TitleHeader
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Select Base";
		}
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var kCellIdentifier = new NSString ("baseCell");
			var cell = tableView.DequeueReusableCell (kCellIdentifier);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			if (indexPath.Row == _selectedIndex) {
				cell.BackgroundColor = Colors.BidRowGreen;
			} else {
				cell.BackgroundColor = UIColor.Clear;
			}
			cell.TextLabel.Text = _data.ElementAt (indexPath.Row);
			return cell;
		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (_data == null) {
				return 0;
			}
			return _data.Count;
		}


		//on Row selected
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			_selectedIndex = indexPath.Row;
			tableView.ReloadData ();
			if (_parentVC != null) {
				_parentVC.RowSelected (_data [_selectedIndex]);
			}
		}

	}
}

