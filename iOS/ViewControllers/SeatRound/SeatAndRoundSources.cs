using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.Linq;
using CoreGraphics;

namespace Bidvalet.iOS
{
	public class SeatAndRoundSources:UITableViewSource
	{
		static int NUMBER_SECTION = 2;
		static string SELECT_SEAT = "SELECT SEAT";
		static string SELECT_ROUND = "SELECT ROUND";
		List<string> _dataSeat;
		List<string> _dataRound;
		SelectSeatAndRoundViewController _parentVC;
		public string cellIdentifier ="baseCell";

		int _selectedItemInSection1 = -1, _selectedItemInSection2 = -1;

		public SeatAndRoundSources (List<string> data, List<string> dataRound,SelectSeatAndRoundViewController parentVC)
		{
			_dataSeat = data;
			_dataRound = dataRound;
			_parentVC = parentVC;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return NUMBER_SECTION;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			if (section == 0) {
				return SELECT_SEAT;
			} else {
				return SELECT_ROUND;
			}
		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (section == 0) {
				return _dataSeat.Count;
			} else {
				return _dataRound.Count;
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
			tableView.SeparatorColor = Colors.BidRowSeparator;
			tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			}
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.BackgroundColor = UIColor.Clear;
			if (indexPath.Section == 0) {
				cell.TextLabel.Text = _dataSeat.ElementAt (indexPath.Row);
			} else{
				cell.TextLabel.Text = _dataRound.ElementAt (indexPath.Row);
			
			}
			if (indexPath.Section == 0 && indexPath.Row == _selectedItemInSection1 ) {
				cell.BackgroundColor = Colors.BidRowGreen;
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			if (indexPath.Section == 1 && indexPath.Row == _selectedItemInSection2 ) {
				cell.BackgroundColor = Colors.BidRowGreen;
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == 0) {
				_selectedItemInSection1 = indexPath.Row;
			} else {
				_selectedItemInSection2 = indexPath.Row;
			}
			// update cell
			tableView.ReloadData();
			if (_parentVC != null && _selectedItemInSection1 >= 0 && _selectedItemInSection2 >= 0) {
				_parentVC.RowSelected (_dataSeat.ElementAt(_selectedItemInSection1), _dataRound.ElementAt(_selectedItemInSection2));
			}

		}

		void UpdateCellStyle(UITableView tableView)
		{
			// update for section 0

		}


		void ResetCellInSection(UITableView tableView, int section)
		{
			int length = _dataSeat.Count;
			if (section == 1) {
				length = _dataRound.Count;
			} 
			for (int i = 0; i < length; i++) {
				NSIndexPath indexpath = NSIndexPath.FromRowSection (i, section);

				UITableViewCell cell = tableView.CellAt (indexpath);
				if (cell != null) {
					cell.BackgroundColor = UIColor.Clear;
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}
			}
		}
	}
}


