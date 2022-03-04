using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Collections.ObjectModel;
using Bidvalet.iOS.Utility;
using Bidvalet.Business;
using CoreGraphics;
using System.Collections.Generic;

namespace Bidvalet.iOS
{
	public class BidAwardSource : UICollectionViewSource
	{
		int lineRow;
		BidAwardView obj;
		List<BidAwardView.Data> dataValue;
		public BidAwardSource (List<BidAwardView.Data> dataSource,BidAwardView objectBidAward)

		{
			obj = objectBidAward;
			dataValue = dataSource;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

	
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO: return the actual number of items in the section
			return dataValue.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			collectionView.RegisterNibForCell (UINib.FromName ("BidAwardCell", NSBundle.MainBundle),new NSString ("BidAwardCell"));
			var cell = collectionView.DequeueReusableCell (BidAwardCell.Key, indexPath) as BidAwardCell;

			//MiniCalenderCell cell = collectionView.DequeueReusableCell (MiniCalenderCell.Key, indexPath) as MiniCalenderCell;
			//
			//			// TODO: populate the cell with the appropriate data based on the indexPath
			//			cell.BackgroundColor = UIColor.White;
			//			cell.ContentView.Alpha=(nfloat)0.0;

			//			UIView.Animate(0.1,0.1, UIViewAnimationOptions.CurveEaseIn , () =>
			//				{  


			cell.bindData (dataValue[indexPath.Row]);
//				} , null);
//			cell.ContentView.Alpha = (nfloat)1.0;
//
//			UIView.CommitAnimations();
			return cell;
		}
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			BidAwardView.Data objdata = dataValue[indexPath.Row];
			obj.ShowBidAward(objdata.filename);

		}
		public void handleReloadCalendar (NSNotification n){
//			CollectionView.ReloadData ();
		}

	
	}
}

