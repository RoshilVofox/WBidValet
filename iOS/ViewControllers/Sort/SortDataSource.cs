using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace Bidvalet.iOS
{
	public class SortDataSource:UITableViewSource
	{
		SortViewController _parentVC;
		public string cellIdentifier ="constraintCell";
		public List<string> lstCurrentConstraint;
		int _selectedItem= -1;
		bool _canEdit = false;
		public SortDataSource (List<string> data,SortViewController viewConstraint)
		{
			_parentVC = viewConstraint;
			lstCurrentConstraint = data;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Order and / or delete sort keys";
		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (lstCurrentConstraint == null) {
				return 0;
			}
			return lstCurrentConstraint.Count;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			UIView headerView = new UIView ();
			headerView.Frame = new CoreGraphics.CGRect (0, 0, tableView.Bounds.Width, 32);
			headerView.BackgroundColor = Colors.BidLightGray;
			UILabel title = new UILabel ();
			title.Frame = new CoreGraphics.CGRect (0, -3, headerView.Frame.Width - 10, headerView.Frame.Height);
			title.Text = "Order and / or delete sort keys";
			UIButton btn = new UIButton (headerView.Bounds);
			btn.BackgroundColor = UIColor.Clear;

			btn.TouchUpInside += HandleSectionClickedEvent;
			headerView.AddSubview (title);
			headerView.AddSubview (btn);
			return headerView;
		}

		void HandleSectionClickedEvent (object sender, EventArgs e)
		{
			if (_canEdit) {
				_canEdit = false;
			}else{
				_canEdit = true;
			}
			_parentVC.SetTableViewEditable (_canEdit);

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
			cell.TextLabel.Text = string.Format("{0}", lstCurrentConstraint.ElementAt (indexPath.Row));//indexPath.Row+1, 
			if (indexPath.Row == _selectedItem ) {
				cell.BackgroundColor = Colors.BidRowGreen;
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			return cell;
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			_selectedItem= indexPath.Row;
			tableView.ReloadData();
			//			if (_parentVC != null && _selectedItem >= 0) {
			//				_parentVC.RowSelected (selectedItems);
			//			}
		}
		public override void RowDeselected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
			tableView.SeparatorColor = Colors.BidRowSeparator;
			tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.BackgroundColor = UIColor.Clear;
			//			selectedItems.Remove(_dataConstraints.ElementAt(_selectedItem));
		}

		public override bool CanEditRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return true;
		}
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle) {
			case UITableViewCellEditingStyle.Delete:
				lstCurrentConstraint.RemoveAt (indexPath.Row);
				tableView.DeleteRows (new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Fade);
				_parentVC.ReloadTV ();
				break;

			case UITableViewCellEditingStyle.None:
				Console.WriteLine ("CommitEditingStyle:None called");
				break;
			}
		}
		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{
			return "Delete";
		}

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			//---- get a reference to the item
			var item = lstCurrentConstraint [sourceIndexPath.Row];
			int deleteAt = sourceIndexPath.Row;

			//---- if we're moving within the same section, and we're inserting it before
			if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row)) {
				//---- add one to where we delete, because we're increasing the index by inserting
				deleteAt = sourceIndexPath.Row + 1;
			}
			//---- copy the item to the new location
			lstCurrentConstraint.Insert(destinationIndexPath.Row, item);

			//---- remove from the old
			lstCurrentConstraint.RemoveAt(deleteAt);
		}
		public void WillBeginTableEditing (UITableView tableView)
		{
			//---- start animations
			tableView.BeginUpdates ();

			//---- insert a new row in the table
			tableView.InsertRows (new NSIndexPath[] { NSIndexPath.FromRowSection (1, 1) }, UITableViewRowAnimation.Fade);
			//---- create a new item and add it to our underlying data
			lstCurrentConstraint.Insert (1, "");

			//---- end animations
			tableView.EndUpdates ();
		}
		/// <summary>
		/// Called manually when the table leaves edit mode
		/// </summary>
		public void DidFinishTableEditing (UITableView tableView)
		{
			//---- start animations
			tableView.BeginUpdates ();
			//---- remove our row from the underlying data
			lstCurrentConstraint.RemoveAt (1);
			//---- remove the row from the table
			tableView.DeleteRows (new NSIndexPath[] { NSIndexPath.FromRowSection (1, 1) }, UITableViewRowAnimation.Fade);
			//---- finish animations
			tableView.EndUpdates ();
		}
	}
}

