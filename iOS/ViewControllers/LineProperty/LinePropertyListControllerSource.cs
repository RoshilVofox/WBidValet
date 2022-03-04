
using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Collections.Generic;
using System.Linq;

namespace Bidvalet.iOS
{
	public class LinePropertyListControllerSource : UITableViewSource
	{
		List<DataColumn> dataColumn;
		public LinePropertyListControllerSource ()
		{
			dataColumn=	(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)?GlobalSettings.WBidINIContent.SummaryVacationColumns: GlobalSettings.WBidINIContent.DataColumns ;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section

			return GlobalSettings.ColumnDefinition.Count;
		}

//		public override string TitleForHeader (UITableView tableView, nint section)
//		{
//			return "Header";
//		}
//
//		public override string TitleForFooter (UITableView tableView, nint section)
//		{
//			return "Footer";
//		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (LinePropertyListControllerCell.Key) as LinePropertyListControllerCell;
			if (cell == null)
				cell = new LinePropertyListControllerCell ();
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			string disp = GlobalSettings.ColumnDefinition [indexPath.Row].DisplayName;
			if (disp == "StartDOW")
				disp = "SDOW";
			if (disp == "EDomPush")
				disp = "EDOM";
			if (disp == "FA Posn")
				disp = "FaPos";
			if (disp == "LDomArr")
				disp = "LDOM";
			if (disp == "AvgLatestDomArrivalTime")
				disp = "ALDA";
			if (disp == "AvgEarliestDomPush")
				disp = "AEDP";
			if (disp == "MyValue")
				disp = "MyValue";
			cell.TextLabel.Text = disp;

			return cell;
		}
		public override UITableViewCellAccessory AccessoryForRow (UITableView tableView, NSIndexPath indexPath)
		{
			
			dataColumn=	(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)?GlobalSettings.WBidINIContent.SummaryVacationColumns: GlobalSettings.WBidINIContent.DataColumns ;
			var isexist= dataColumn.FirstOrDefault(x=>x.Id==GlobalSettings.ColumnDefinition[indexPath.Row].Id);
			if(isexist!=null)
			{
				return UITableViewCellAccessory.Checkmark;
			}
			else			return UITableViewCellAccessory.None;
		}
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			

			UITableViewCell cell= tableView.CellAt(indexPath);
			var isexist= dataColumn.FirstOrDefault(x=>x.Id==GlobalSettings.ColumnDefinition[indexPath.Row].Id);

			if (cell.Accessory == UITableViewCellAccessory.Checkmark)
			{
				if (dataColumn.Count > 1) {
					if (isexist != null)
						dataColumn.Remove (isexist);
					cell.Accessory = UITableViewCellAccessory.None;
				} 
				else 
				{
					InvokeOnMainThread(() =>
						{
							
							DisplayAlertView(GlobalSettings.ApplicationName, "You need to select atleat one column");
						});
				}
			} 
			else 
			{
				if (dataColumn.Count < 4) {
					if (isexist == null)
						dataColumn.Add (new DataColumn {
							Id = GlobalSettings.ColumnDefinition [indexPath.Row].Id,
							Width = GlobalSettings.ColumnDefinition [indexPath.Row].Width,
							Order = GlobalSettings.ColumnDefinition [indexPath.Row].Order
						});
					cell.Accessory = UITableViewCellAccessory.Checkmark;
				} 
				else 
				{
					InvokeOnMainThread(() =>
						{

							DisplayAlertView(GlobalSettings.ApplicationName, "You can't select more than 4 columns");
						});
				}
			}
		}
		private void DisplayAlertView(string caption, string message)
		{
			new UIAlertView(caption, message, null, "OK", null).Show();



		}


	}
}

