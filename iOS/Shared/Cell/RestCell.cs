using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using Bidvalet.Model;
namespace Bidvalet.iOS
{
	public partial class RestCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("RestCell");
		public static readonly UINib Nib;
		ConstraintsChangeViewController _viewController;
		RestCx _cellObject;
		List<int> restValues = new List<int>();

		static RestCell ()
		{
			Nib = UINib.FromName ("RestCell", NSBundle.MainBundle);
		}

		public RestCell (IntPtr handle) : base (handle)
		{
		}
		public static RestCell Create()
		{
			return (RestCell)Nib.Instantiate(null, null)[0];
		}
		public void Filldata(ConstraintsChangeViewController parentVC, RestCx cellData)
		{
			_viewController = parentVC;
			_cellObject = cellData;
            //_cellObject.Dom = "All";
            //_cellObject.LessMore = "Less than";
            //_cellObject.Value = 0;
			UpdateUI ();
		}
		private void UpdateUI (){
			if(_cellObject!=null){
				btnRestDom.SetTitle(_cellObject.Dom, UIControlState.Normal);
				btnLessthan.SetTitle(_cellObject.LessMore, UIControlState.Normal);
				btnRestValue.SetTitle(String.Format("{0}",_cellObject.Value), UIControlState.Normal);
			}
		}

		partial void OnAwayDomEvent (Foundation.NSObject sender){
			if(_viewController!=null){
				UIActionSheet actionSheet = new UIActionSheet ("");
				actionSheet.AddButton ("All");
				actionSheet.AddButton ("inDom");
				actionSheet.AddButton ("AwayDom");
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					switch (b.ButtonIndex){
					case 0:
						_cellObject.Dom = "All";
						break;
					case 1:
						_cellObject.Dom = "inDom";
						break;
					case 2:
						_cellObject.Dom = "AwayDom";
						break;
					}
					UpdateUI();
				};
				actionSheet.ShowInView (_viewController.View);
			}
		}


		partial void OnDeleteEvent (Foundation.NSObject sender)
		{
			if(_viewController!=null){
				_viewController.DeleteObject(_cellObject);
			}
		}
			
		partial void OnLessThanEvent (Foundation.NSObject sender){
			UIActionSheet actionSheet = new UIActionSheet ("");
            actionSheet.AddButton("Less than");
			actionSheet.AddButton ("More than");
			
			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
				switch (b.ButtonIndex){
				case 0:
				
                    _cellObject.LessMore = "Less than";
					break;
				case 1:
                    _cellObject.LessMore = "More than";
					break;
				}
				UpdateUI();
			};
			actionSheet.ShowInView (_viewController.View);
		}

		partial void OnValueEvent (Foundation.NSObject sender){
			UIActionSheet actionSheet = new UIActionSheet ("");
			for (int i =0; i<=48;i++){
				actionSheet.AddButton (String.Format("{0}",i));	
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					_cellObject.Value =(int) b.ButtonIndex;
					UpdateUI();
				};
			}
			actionSheet.ShowInView (_viewController.View);
		}
	}
}
