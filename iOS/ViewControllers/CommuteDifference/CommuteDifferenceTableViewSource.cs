using System;
using System.Collections.Generic;
using Bidvalet.Model;
using Foundation;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.CommuteDifference
{
    public class CommuteDifferenceTableViewSource : UITableViewSource
    {
        List<CommuteFltChangeValues> lstCommuteDifference { get; set; }
        public CommuteDifferenceTableViewSource(List<CommuteFltChangeValues> _lstCommuteDifference)
        {
            lstCommuteDifference = _lstCommuteDifference;
        }

       

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return lstCommuteDifference.Count;
        }




        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
            //return base.NumberOfSections(tableView);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName("ComuteDiffTableViewCell", NSBundle.MainBundle), "ComuteDiffTableViewCell");
            ComuteDiffTableViewCell cell = (ComuteDiffTableViewCell)tableView.DequeueReusableCell(new NSString("ComuteDiffTableViewCell"), indexPath);

            cell.LabelValues(lstCommuteDifference[indexPath.Row]);
            return cell;
        }
    }
}
