using System;
using System.Collections.Generic;
using UIKit;

namespace Bidvalet.iOS.ViewControllers.HistoryBidData
{
    public class HistoryMonthSource : UICollectionViewSource
    {
        string[] _data;
        HistoryBidDataRetrieve _parentVC;
        Foundation.NSIndexPath currentIndexPath;
        static int currentMonth;
        static int currentYear;
        static int selectedYear;
       
        public HistoryMonthSource(string[] data)
        {
            currentMonth = DateTime.Now.Month;
            currentYear = DateTime.Now.Year;
            _data = data;
        }
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _data.Length;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, Foundation.NSIndexPath indexPath)
        {
            HistoryCell cell = (HistoryCell)collectionView.DequeueReusableCell(HistoryCell.Key, indexPath);

            cell.BackgroundColor = UIColor.SystemGray2Color;

            if (currentIndexPath == indexPath  )
            
                cell.BackgroundColor = UIColor.LinkColor;
             
            //else if (currentMonth == (indexPath.Row + 1) )
            
            //    cell.BackgroundColor = UIColor.LinkColor;
               
            
            else
            {
                if (selectedYear != 0 && selectedYear != currentYear)
                    cell.BackgroundColor = UIColor.SystemGray2Color;

                else if (indexPath.Row > (currentMonth - 1))
                    cell.BackgroundColor = UIColor.Black;
            }

            cell.FillData(_data[indexPath.Row]);
            return cell;
        }
        public override void ItemSelected(UICollectionView collectionView, Foundation.NSIndexPath indexPath)
        {
            currentIndexPath = new Foundation.NSIndexPath();

            if (indexPath.Row <= (currentMonth - 1) || (selectedYear != 0 && selectedYear != currentYear))
            {
                currentIndexPath = indexPath;
                collectionView.ReloadData();
                if (_parentVC == null)
                    _parentVC = new HistoryBidDataRetrieve();
               // _parentVC.ItemSelectedforMonth((indexPath.Row));
            }

        }

        public void reloadCollectionView(UICollectionView collectionView, int selectedYearFromUI)
        {
            selectedYear = selectedYearFromUI;
            collectionView.ReloadData();
        }
    }
}
