using System;
using System.Collections.Generic;
using Bidvalet.Model;
using Foundation;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.VacationDifference
{
    public class VacDiffTableViewControllerSource: UITableViewSource
    {
        private List<FlightDataChangeVacValues> _lstFlightDataChangevalues { get; set; }
        public VacDiffTableViewControllerSource(List<FlightDataChangeVacValues> lstFlightDataChangevalues)
        {
            _lstFlightDataChangevalues = lstFlightDataChangevalues;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.RegisterNibForCellReuse(UINib.FromName("VacDiffCell", NSBundle.MainBundle), "VacDiffCell");
            VacDiffCell cell = (VacDiffCell)tableView.DequeueReusableCell(new NSString("VacDiffCell"), indexPath);

            cell.LabelValues(_lstFlightDataChangevalues[indexPath.Row]);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _lstFlightDataChangevalues.Count;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
            //return base.NumberOfSections(tableView);
        }

    }
}
