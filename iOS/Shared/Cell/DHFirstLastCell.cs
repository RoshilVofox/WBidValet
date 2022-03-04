using System;

using Foundation;
using UIKit;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
    public partial class DHFirstLastCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("DHFirstLastCell");
        public static readonly UINib Nib;
        ConstraintsChangeViewController _viewController;
        DHFristLastConstraint _cellObject;

        static DHFirstLastCell()
        {
            Nib = UINib.FromName("DHFirstLastCell", NSBundle.MainBundle);
        }

        public DHFirstLastCell(IntPtr handle)
            : base(handle)
        {
        }
        public static DHFirstLastCell Create()
        {
            return (DHFirstLastCell)Nib.Instantiate(null, null)[0];
        }
        public void Filldata(ConstraintsChangeViewController parentVC, DHFristLastConstraint cellData)
        {
            _viewController = parentVC;
            _cellObject = cellData;
            //	_cellObject.DH = "Both";
            //_cellObject.LessMore = "Less than";
            //	_cellObject.Value = 1;
            UpdateUI();
        }
        private void UpdateUI()
        {
            btnDHValue.SetTitle(String.Format("{0}", _cellObject.Value), UIControlState.Normal);
            btnDHConstraint.SetTitle(_cellObject.DH, UIControlState.Normal);
            btnLessthan.SetTitle(_cellObject.LessMore, UIControlState.Normal);
        }


        partial void OnDeleteEvent(Foundation.NSObject sender)
        {
            if (_viewController != null)
            {
                _viewController.DeleteObject(_cellObject);
            }
        }

        partial void OnDHConstraintEvent(Foundation.NSObject sender)
        {
            UIActionSheet actionSheet = new UIActionSheet("");
            actionSheet.AddButton("first");
            actionSheet.AddButton("last");
            actionSheet.AddButton("both");
            actionSheet.Clicked += delegate(object a, UIButtonEventArgs b)
            {
                switch (b.ButtonIndex)
                {
                    case 0:
                        _cellObject.DH = "first";
                        break;
                    case 1:
                        _cellObject.DH = "last";
                        break;
                    case 2:
                        _cellObject.DH = "both";
                        break;
                }
                UpdateUI();
            };
            actionSheet.ShowInView(_viewController.View);
        }

        partial void OnDHValueEvent(Foundation.NSObject sender)
        {
            UIActionSheet actionSheet = new UIActionSheet("");
            for (int i = 0; i <= 6; i++)
            {
                actionSheet.AddButton(String.Format("{0}", i));
                actionSheet.Clicked += delegate(object a, UIButtonEventArgs b)
                {
                    _cellObject.Value = (int)b.ButtonIndex;
                    UpdateUI();
                };
            }
            actionSheet.ShowInView(_viewController.View);
        }


        partial void OnLessthanEvent(Foundation.NSObject sender)
        {
            UIActionSheet actionSheet = new UIActionSheet("");
            actionSheet.AddButton("Less than");
            actionSheet.AddButton("Equal");
            actionSheet.AddButton("More than");

            actionSheet.Clicked += delegate(object a, UIButtonEventArgs b)
            {
                switch (b.ButtonIndex)
                {
                    case 0:
                        _cellObject.LessMore = "Less than";
                        break;
                    case 1:
                        _cellObject.LessMore = "Equal";
                        break;
                    case 2:
                        _cellObject.LessMore = "More than";
                        break;
                }
                UpdateUI();
            };
            actionSheet.ShowInView(_viewController.View);
        }
    }
}
