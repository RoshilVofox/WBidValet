
using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Collections.Generic;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using SimpleCollectionView;
namespace Bidvalet.iOS
{
	public partial class LineViewControllerCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("LineViewControllerCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("LineViewControllerCell");
		public int LineRow;
		UITapGestureRecognizer MiniCalenderClick;
		public LineViewControllerCell (IntPtr handle) : base (handle)
		{
			//MiniCalenderCollectionView.Source= LineViewMiniCalenderSource();
			//LinePropertyCollection.Source=new LineViewLinePropertySource();
		
		}
		public void BindData(List<ColumnDefinition> columndefinition ,int Row)
		{

			lblCellNumber.Text=(Row+1).ToString();
			//btnSelection.Tag = Row;
            Line line = GlobalSettings.Lines[Row];
            //lblLineNumber.Text = line.LineNum.ToString();  
            lblLineNumber.Text = line.LineDisplay.ToString();  
                //(GlobalSettings.Lines[Row].TopLock) ? "T" : "N";
            this.Tag = Row;

			MiniCalenderClick=new UITapGestureRecognizer (this, new  ObjCRuntime.Selector ("calenderTapped"));
			MiniCalenderClick.NumberOfTapsRequired=1;
			MiniCalenderCollectionView.AddGestureRecognizer (MiniCalenderClick);   
			LinePropertyCollection.BackgroundColor=UIColor.White;
			MiniCalenderCollectionView.BackgroundColor = UIColor.White;
			MiniCalenderCollectionView.Layer.BorderWidth = (nfloat)0.6;
			MiniCalenderCollectionView.ScrollEnabled = false;
			MiniCalenderCollectionView.Layer.BorderColor = UIColor.LightGray.CGColor;
			MiniCalenderCollectionView.Layer.CornerRadius =(nfloat) 2;

			LineViewLinePropertySource objSource =new LineViewLinePropertySource(  GlobalSettings.ColumnDefinition, LineRow);
			//MiniCalenderCollectionView.BackgroundColor = UIColor.LightGray;
			objSource.mainRow = Row;
			LinePropertyCollection.Source = objSource;

            LinePropertyCollection.AddGestureRecognizer(new UILongPressGestureRecognizer((longPress) =>
            {
                if (longPress.State == UIGestureRecognizerState.Began)
                {
                    Console.WriteLine("Long pressed");

                    NSNumber row = new NSNumber(line.LineNum);


                    NSNotificationCenter.DefaultCenter.PostNotificationName("showActionSheet", row);
                }
            }));
//			UICollectionViewFlowLayout layout = new UICollectionViewFlowLayout ();
//			layout.ItemSize= new CGSize (MiniCalenderCollectionView.Bounds.Size.Width/7, 4);
//			layout.MinimumLineSpacing = (nfloat)0.0;
//			layout.SectionInset = new UIEdgeInsets (0, 0, 0, 0);        
//			layout.MinimumInteritemSpacing = (nfloat)0.0;
//
//
//			MiniCalenderCollectionView.SetCollectionViewLayout (layout, true);
			DispatchQueue.MainQueue.DispatchAsync (() => {
					MiniCalenderCollectionView.Source=new LineViewMiniCalenderSource(Row);            
					
			});
			

            btnSelection.Tag = GlobalSettings.Lines[Row].LineNum;

			LinePropertyCollection.BackgroundColor = UIColor.Clear;
            setSelectButton(GlobalSettings.Lines[Row].LineNum);

            //this.BackgroundColor = UIColor.Red;

			
            if (line.BlankLine || line.LineDisplay.Contains("RR"))
                this.BackgroundColor = ColorClass.BlankLineColor;
            else if (line.ReserveLine)
                this.BackgroundColor = ColorClass.ReserveLineColor;
            else
            {
                if (Row % 2 == 0)
                    this.BackgroundColor = UIColor.FromRGB(255, 255, 255);
                else
                    this.BackgroundColor = UIColor.FromRGB(249, 249, 249);
            }

		}
		[Export("calenderTapped")]
		void calenderTapped()
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCalender",(NSString)this.Tag.ToString());
		}



        private void setSelectButton(int row)
        {
            if (GlobalSettings.SelectedLines.Contains(row))
            {
                this.btnSelection.Selected = true;
            }
            else
            {
                this.btnSelection.Selected = false;
            }

        }
		partial void btnSelectionClicked (NSObject sender)
		{
			btnSelection.Selected=!btnSelection.Selected;
            NSNumber row = new NSNumber(btnSelection.Tag);
            NSNotificationCenter.DefaultCenter.PostNotificationName("BidListRowSelected", row);
			//if(btnSelection.Selected) btnSelection.SetImage(new UIImage(),UIControlState.Selected
		}
		public static LineViewControllerCell Create ()
		{
			return (LineViewControllerCell)Nib.Instantiate (null, null) [0];
		}

		//public void reloadCalenderView(int Row)
  //      {
		//	MiniCalenderCollectionView.Source = new LineViewMiniCalenderSource(Row);
		//	MiniCalenderCollectionView.ReloadData();
		//}
	}
}


namespace SimpleCollectionView
{
	public class LineLayout : UICollectionViewFlowLayout
	{
		public const float ITEM_SIZE = 200.0f;
		public const int ACTIVE_DISTANCE = 0;
		public const float ZOOM_FACTOR = 0f;

		public LineLayout ()
		{
			ItemSize = new CGSize (7.0, 4.0);
			ScrollDirection = UICollectionViewScrollDirection.Horizontal;
			SectionInset = new UIEdgeInsets (0,0,0,0);
			MinimumLineSpacing = 0.0f;
		}

		public override bool ShouldInvalidateLayoutForBoundsChange (CGRect newBounds)
		{
			return true;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			var array = base.LayoutAttributesForElementsInRect (rect);
			var visibleRect = new CGRect (CollectionView.ContentOffset, CollectionView.Bounds.Size);

			foreach (var attributes in array) {
				if (attributes.Frame.IntersectsWith (rect)) {
					float distance = (float)(visibleRect.GetMidX () - attributes.Center.X);
					float normalizedDistance = distance / ACTIVE_DISTANCE;
					if (Math.Abs (distance) < ACTIVE_DISTANCE) {
						float zoom = 1 + ZOOM_FACTOR * (1 - Math.Abs (normalizedDistance));
						attributes.Transform3D = CATransform3D.MakeScale (zoom, zoom, 1.0f);
						attributes.ZIndex = 1;
					}
				}
			}
			return array;
		}

//		public override CGPoint TargetContentOffset (CGPoint proposedContentOffset, CGPoint scrollingVelocity)
//		{
//			float offSetAdjustment = float.MaxValue;
//			float horizontalCenter = (float)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
//			CGRect targetRect = new CGRect (proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
//			var array = base.LayoutAttributesForElementsInRect (targetRect);
//			foreach (var layoutAttributes in array) {
//				float itemHorizontalCenter = (float)layoutAttributes.Center.X;
//				if (Math.Abs (itemHorizontalCenter - horizontalCenter) < Math.Abs (offSetAdjustment)) {
//					offSetAdjustment = itemHorizontalCenter - horizontalCenter;
//				}
//			}
//			return new CGPoint (proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
//		}

	}
}