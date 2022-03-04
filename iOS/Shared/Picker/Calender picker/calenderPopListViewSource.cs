using System;
using CoreGraphics;
using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Collections.ObjectModel;
using Bidvalet.iOS.Utility;

namespace Bidvalet.iOS
{
	public class calenderPopListViewSource : UICollectionViewSource
	{
		ObservableCollection<CalendarData> calData;
		public LineDetailedCalenderView _ParentView;
		public calenderPopListViewSource (ObservableCollection<CalendarData> trip,LineDetailedCalenderView View)
		{
			_ParentView = View;
			calData = trip;
		}


		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			return calData.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("CalenderPopCell", NSBundle.MainBundle),new NSString ("CalenderPopCell"));
			var cell = collectionView.DequeueReusableCell (CalenderPopCell.Key, indexPath) as CalenderPopCell;

			// TODO: populate the cell with the appropriate data based on the indexPath
			cell.BackgroundColor = UIColor.White;
			cell.bindData (calData [indexPath.Row]);
			return cell;
		}
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			string trip = calData [indexPath.Row].TripNumber;
			//string ss = trip;
			if (calData [indexPath.Row].TripNumber != null) {
				
				_ParentView.selectedTrip = trip;
               // _ParentView.se
				//_ParentView.SelectedLine = indexPath.Row;
				_ParentView.isLastTrip = calData [indexPath.Row].IsLastTrip;
				_ParentView.NAvigateToTrip ();

			} else {

			}
			collectionView.ReloadData ();
		}

	}
}

