using System;
using CoreGraphics;
using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Collections.ObjectModel;
using Bidvalet.iOS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Bidvalet.iOS
{
	public class LineViewLinePropertySource : UICollectionViewSource
	{
		List<ColumnDefinition>  columndefinition ;
		public int mainRow;
		public LineViewLinePropertySource (List<ColumnDefinition> columndef , int row)
		{
			mainRow = row;
			columndefinition = columndef;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			//return CommonClass.calData.Count;
			return (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)?GlobalSettings.WBidINIContent.SummaryVacationColumns.Count: GlobalSettings.WBidINIContent.DataColumns.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("LinePropertyCell", NSBundle.MainBundle),new NSString ("LinePropertyCell"));
			var cell = collectionView.DequeueReusableCell (LinePropertyCell.Key, indexPath) as LinePropertyCell;

//			// TODO: populate the cell with the appropriate data based on the indexPath
			cell.BackgroundColor = UIColor.Clear;
			cell.ContentView.BackgroundColor = UIColor.Clear;

			DataColumn dataColumn =(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)?GlobalSettings.WBidINIContent.SummaryVacationColumns[indexPath.Row]: GlobalSettings.WBidINIContent.DataColumns [indexPath.Row];
			ColumnDefinition columDefinition = columndefinition.Where (x => x.Id == dataColumn.Id).FirstOrDefault ();
			cell.BindData (columDefinition, mainRow);
        
			return cell;
		}
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
//			string trip = CommonClass.calData [indexPath.Row].TripNumber;
//			string ss = CommonClass.selectedTrip;
//			if (CommonClass.calData [indexPath.Row].TripNumber != null) {
//				NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopShow", new NSString (trip));
//				CommonClass.selectedTrip = trip;
//				CommonClass.isLastTrip = CommonClass.calData [indexPath.Row].IsLastTrip;
//			} else {
//				NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopHide", null);
//				CommonClass.selectedTrip = null;
//				CommonClass.isLastTrip = false;
//			}
//			collectionView.ReloadData ();
		}
		public void handleReloadCalendar (NSNotification n){
//			CollectionView.ReloadData ();
		}
	
	}
}

