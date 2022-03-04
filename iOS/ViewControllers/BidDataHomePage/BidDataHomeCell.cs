using System;
using Bidvalet.Model;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
    public partial class BidDataHomeCell : UICollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("BidDataHomeCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("BidDataHomeCell");
        

        public BidDataHomeCell(IntPtr handle) : base(handle)
        {
           
        }
        public static BidDataHomeCell Create()
        {
           
            return (BidDataHomeCell)Nib.Instantiate(null, null)[0];
        }
        public void BindData(RecentFile aFile, NSIndexPath index, bool jiggle,bool isSelected)
        {
           // btnDelete.Layer.CornerRadius = 3f;
            lblTitle.Text = aFile.MonthDisplay + " " + aFile.Year;
            lblSubTitle.Text = aFile.Domcile + "-" + aFile.Position + "-" + aFile.Round;
            if (isSelected)
                btnDelete.ImageView.Image = UIImage.FromBundle("check-new");
            else
                btnDelete.ImageView.Image = UIImage.FromBundle("uncheck-new");

        }
        
        public int tag
        {
            set
            {
                this.Tag = value;
                btnDelete.Tag = value;
            }
        }
        partial void btnDeleteTapped(UIKit.UIButton sender)
        {
            
            NSNumber num = new NSNumber(sender.Tag);


            NSNotificationCenter.DefaultCenter.PostNotificationName("HandleMultipleDelete", num);
        }

       
    }
}
