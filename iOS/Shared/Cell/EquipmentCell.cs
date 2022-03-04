using System;

using Foundation;
using UIKit;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
    public partial class EquipmentCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("EquipmentCell");
        public static readonly UINib Nib;

        ConstraintsChangeViewController _viewController;
        EquirementConstraint _cellObject;

        static EquipmentCell()
        {
            Nib = UINib.FromName("EquipmentCell", NSBundle.MainBundle);
        }

        public EquipmentCell(IntPtr handle)
            : base(handle)
        {
        }
        public static EquipmentCell Create()
        {
            return (EquipmentCell)Nib.Instantiate(null, null)[0];
        }
        public void Filldata(ConstraintsChangeViewController parentVC, EquirementConstraint cellData)
        {
            _viewController = parentVC;
            _cellObject = cellData;
            //_cellObject.Equipment = 500;
            //_cellObject.LessMore = "Less than";
            //_cellObject.Value = 11;
            UpdateUI();
        }
        private void UpdateUI()
        {
            btnEquipment.SetTitle(String.Format("{0}", _cellObject.Value), UIControlState.Normal);
            btnEquipmentValue.SetTitle(String.Format("{0}",GetEquipmentName( _cellObject.Equipment)), UIControlState.Normal);
            btnLessthan.SetTitle(_cellObject.LessMore, UIControlState.Normal);
        }

        partial void OnEquipmentEvent(UIButton sender)
        {
            UIActionSheet actionSheet = new UIActionSheet("");
            for (int i = 0; i <= 20; i++)
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

        partial void OnEquipmentDeleteEvent(UIButton sender)
        {
            if (_viewController != null)
            {
                _viewController.DeleteObject(_cellObject);
            }
        }

        partial void OnEquipmentValueEvent(UIButton sender)
        {
            UIActionSheet actionSheet = new UIActionSheet("");
            //actionSheet.AddButton("300");
            //actionSheet.AddButton("500");
            //actionSheet.AddButton("300 & 500");
            actionSheet.AddButton("700");
            actionSheet.AddButton("800");
			actionSheet.AddButton("MAX");
            actionSheet.Clicked += delegate(object a, UIButtonEventArgs b)
            {
                switch (b.ButtonIndex)
                {
                    //case 0:
                    //    _cellObject.Equipment = 300;
                    //    break;
                    //case 1:
                    //    _cellObject.Equipment = 500;
                    //    break;
                    //case 2:
                    //    _cellObject.Equipment = 35;
                     //   break;
                    case 0:
                        _cellObject.Equipment = 700;
                        break;
                    case 1:
                        _cellObject.Equipment = 800;
                        break;
					case 2:
						_cellObject.Equipment = 600;
						break;
                }
                UpdateUI();
            };
            actionSheet.ShowInView(_viewController.View);
        }

        partial void OnLessthanEvent(UIButton sender)
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

        private string GetEquipmentName(int equipment)
        {
            string result = string.Empty;

            switch (equipment)
            {
                //case 300:
                //    result = "300";
                //    break;

                //case 500:
                //    result = "500";
                //    break;
                //case 35:
                //    result = "300 & 500";
                //    break;
                case 700:
                    result = "700";
                    break;
                case 800:
                    result = "800";
                    break;
				case 600:
					result = "MAX";
					break;


            }
            return result;


        }
    }
}
