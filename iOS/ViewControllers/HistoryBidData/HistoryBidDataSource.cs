using System;
using UIKit;
using Bidvalet.Model;
using System.Collections.Generic;
using Bidvalet.iOS.ViewControllers.HistoryBidData;

namespace Bidvalet.iOS
{
	public class HistoryBidDataSource : UICollectionViewSource
	{
		List<string> _data;
		HistoryBidDataRetrieve _parentVC;
		Foundation.NSIndexPath currentIndexPath;
		
		public HistoryBidDataSource(List<string> data)
		{
			
			_data = data;
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}
		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return _data.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, Foundation.NSIndexPath indexPath)
		{
			HistoryCell cell = (HistoryCell)collectionView.DequeueReusableCell(HistoryCell.Key, indexPath);
			
			cell.BackgroundColor = UIColor.SystemGray2Color;
			
			
			if (currentIndexPath == indexPath)
				cell.BackgroundColor = UIColor.LinkColor;
			cell.FillData(_data[indexPath.Row]);
			return cell;
		}
		public override void ItemSelected(UICollectionView collectionView, Foundation.NSIndexPath indexPath)
		{
			currentIndexPath = new Foundation.NSIndexPath();
			currentIndexPath = indexPath;
			collectionView.ReloadData();
			if(_parentVC==null)
			_parentVC = new HistoryBidDataRetrieve();
            _parentVC.ItemSelected(_data[indexPath.Row]);
		}
	}
}
