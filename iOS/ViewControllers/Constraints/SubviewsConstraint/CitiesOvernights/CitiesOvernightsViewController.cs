// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using Bidvalet.Model;
using System.Linq;
using System.Collections.Generic;
namespace Bidvalet.iOS
{
    public partial class CitiesOvernightsViewController : BaseViewController
    {
        public CitiesOvernightsViewController(IntPtr handle)
            : base(handle)
        {
        }

        public OvernightCitiesCx data;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupUI();
            collectionView.RegisterNibForCell(CityCell.Nib, CityCell.Key);
            collectionView.Source = new CitiesOvernightSource(data);
        }

        void SetupUI()
        {
            Title = "Overnight Cities";
            UIHelpers.StyleForButtons(new UIButton[] { btnClear, btnDone });
        }

        partial void OnClearEvent(Foundation.NSObject sender)
        {
            if (data != null)
            {
                if (data.No != null)
                {
                    data.No.Clear();
                }
                if (data.Yes != null)
                {
                    data.Yes.Clear();
                }
            }

            collectionView.Source = new CitiesOvernightSource(data);
            collectionView.ReloadData();
        }

        partial void OnDoneEvent(Foundation.NSObject sender)
        {
            if ((data.No == null || data.No.Count == 0) && (data.Yes == null || data.Yes.Count == 0))
            {
                SharedObject.Instance.ListConstraint.Remove(data);
            }

                PopViewController(null, true);
            
        }
    }
}