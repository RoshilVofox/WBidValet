using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Collections.ObjectModel;
using Bidvalet.iOS.Utility;
using Bidvalet.Business;
using CoreGraphics;


namespace Bidvalet.iOS
{
	public class LineViewMiniCalenderSource : UICollectionViewSource
	{
		int lineRow;
		public LineViewMiniCalenderSource (int Row)
		{
			lineRow = Row;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

	
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			return GlobalSettings.Lines[lineRow].BidLineIconList.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("MiniCalenderCell", NSBundle.MainBundle),new NSString ("MiniCalenderCell"));
			var cell = collectionView.DequeueReusableCell (MiniCalenderCell.Key, indexPath) as MiniCalenderCell;

			//MiniCalenderCell cell = collectionView.DequeueReusableCell (MiniCalenderCell.Key, indexPath) as MiniCalenderCell;
//
//			// TODO: populate the cell with the appropriate data based on the indexPath
//			cell.BackgroundColor = UIColor.White;
//			cell.ContentView.Alpha=(nfloat)0.0;

//			UIView.Animate(0.1,0.1, UIViewAnimationOptions.CurveEaseIn , () =>
//				{  

		
			cell.bindData (GlobalSettings.Lines[lineRow].BidLineIconList[indexPath.Row]);
//				} , null);
//			cell.ContentView.Alpha = (nfloat)1.0;
//
//			UIView.CommitAnimations();
			return cell;
		}
//		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
//		{
////			string trip = CommonClass.calData [indexPath.Row].TripNumber;
////			string ss = CommonClass.selectedTrip;
////			if (CommonClass.calData [indexPath.Row].TripNumber != null) {
////				NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopShow", new NSString (trip));
////				CommonClass.selectedTrip = trip;
////				CommonClass.isLastTrip = CommonClass.calData [indexPath.Row].IsLastTrip;
////			} else {
////				NSNotificationCenter.DefaultCenter.PostNotificationName ("TripPopHide", null);
////				CommonClass.selectedTrip = null;
////				CommonClass.isLastTrip = false;
////			}
////			collectionView.ReloadData ();
//		}
		public void handleReloadCalendar (NSNotification n){
//			CollectionView.ReloadData ();
		}

	
	}
}

