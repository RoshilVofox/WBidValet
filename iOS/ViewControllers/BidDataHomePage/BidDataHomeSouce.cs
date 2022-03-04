using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Bidvalet.iOS
{
   
    public class BidDataHomeSouce : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("BidDataHomeCell");
        UILabel titlelabel;
        UILabel subTitleLabel;
        UIButton deleteButton;
        [Export("initWithFrame:")]
        public BidDataHomeSouce(CGRect frame) : base(frame)
        {
        }
        public NSString title
        {
            set
            {
                titlelabel.Text = value;
            }
        }
        public NSString subTitle
        {
            set
            {
                subTitleLabel.Text = value;
            }
        }

       
    }

}


