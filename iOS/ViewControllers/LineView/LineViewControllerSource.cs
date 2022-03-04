
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using Bidvalet.Model;
using Bidvalet.Business;

namespace Bidvalet.iOS
{
	public class LineViewControllerSource : UITableViewSource
	{
		List<ColumnDefinition> columndefinition;
		public LineViewController ObjLine;

      
		public LineViewControllerSource  (List<ColumnDefinition> columndef, LineViewController line)
		{
			columndefinition=columndef;
			ObjLine = line;
		}
		UILongPressGestureRecognizer CellLogPress;
		UITapGestureRecognizer DoubleTap;
		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return GlobalSettings.Lines.Count;
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
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}
		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
            if (GlobalSettings.Lines != null && GlobalSettings.Lines.Count>0)
            {

				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();
				WBidHelper.PushToUndoStack();
				if (sourceIndexPath.Row != destinationIndexPath.Row)
                {

                    //---- get a reference to the item
                    var item = GlobalSettings.Lines[sourceIndexPath.Row];
                    int deleteAt = 0;
                    int insertAt = 0;
                    //if (destinationIndexPath.Row < GlobalSettings.Lines.Count - 1)
                    //{
                    //    if()
 
                    //}

                    if (destinationIndexPath.Row < sourceIndexPath.Row)
                    {
                        deleteAt = sourceIndexPath.Row+1;
                        insertAt = destinationIndexPath.Row;
                        var itemDestination = GlobalSettings.Lines[insertAt];
                        item.TopLock = itemDestination.TopLock;
                    }
                    else if (destinationIndexPath.Row > sourceIndexPath.Row && destinationIndexPath.Row < GlobalSettings.Lines.Count - 1)
                    {
                        deleteAt = sourceIndexPath.Row;
                        insertAt = destinationIndexPath.Row+1;
                        var itemDestination = GlobalSettings.Lines[insertAt];
                        item.TopLock = itemDestination.TopLock;
                    }


                    ////---- if we're moving within the same section, and we're inserting it before
                    //if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row))
                    //{
                    //    //---- add one to where we delete, because we're increasing the index by inserting
                    //    deleteAt = sourceIndexPath.Row + 1;
                    //}
                    //---- copy the item to the new location
                    GlobalSettings.Lines.Insert(insertAt, item);
                    //---- remove from the old

                    GlobalSettings.Lines.RemoveAt(deleteAt);
					//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
					 tableView.ReloadData();
				}
            }
		}
		public void reloadTable(UITableView tableView)
        {
			tableView.ReloadData();
		}

		public override bool CanEditRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return true;
		}
		public override bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}
		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.None;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (LineViewControllerCell.Key) as LineViewControllerCell;
			if (cell == null)
				cell = LineViewControllerCell.Create ();
			//cell.ShouldIndentWhileEditing = false;

			cell.BindData (columndefinition,indexPath.Row);
			cell.LineRow = indexPath.Row;
			//CellLogPress=new UILongPressGestureRecognizer (this, new  ObjCRuntime.Selector ("CellLongPressed"));

			//cell.AddGestureRecognizer (CellLogPress);
           // CellLogPress.DelaysTouchesBegan = true;

            //cell.AddGestureRecognizer(new UILongPressGestureRecognizer((longPress) =>
            //{
            //    if (longPress.State == UIGestureRecognizerState.Began)
            //    {
            //        Console.WriteLine("Long pressed");

            //        NSNumber row = new NSNumber(GlobalSettings.Lines[indexPath.Row].LineNum);
             

            //        NSNotificationCenter.DefaultCenter.PostNotificationName("showActionSheet", row);
            //    }
            //}));

			DoubleTap=new UITapGestureRecognizer (this, new  ObjCRuntime.Selector ("DoubleTapped"));
			DoubleTap.NumberOfTapsRequired = 2;
			cell.AddGestureRecognizer (DoubleTap);
			//cell.reloadCalenderView(indexPath.Row);
			// TODO: populate the cell with the appropriate data based on the indexPath
			//cell.DetailTextLabel.Text = "DetailsTextLabel";

			return cell;
		}


        //[Export("CellLongPressed")]
        //void CellLongPressed()
        //{
        //    if (CellLogPress.State == UIGestureRecognizerState.Possible)
        //    {
        //        Console.WriteLine("Long pressed");
        //        NSNotificationCenter.DefaultCenter.PostNotificationName("showActionSheet", null);
        //    }
        //}
		[Export("DoubleTapped")]
		void DoubleTapped()
		{
			
			NSNotificationCenter.DefaultCenter.PostNotificationName("ShowLineProperty", null);
			Console.WriteLine("Double Tapped");
		}
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 49;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{

			var cell = tableView.DequeueReusableHeaderFooterView (BidListCustomHeader.Key) as BidListCustomHeader;
			if (cell == null)
				cell = BidListCustomHeader.Create ();
			cell._parent = ObjLine;
			cell.LongPressHandling ();
			cell.DownNavigation ();
			cell.UpNavigation ();
			cell.setButtonStates();
			return cell;

		}


		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return (nfloat)44.0;
		}


       
	}
}

