using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.ObjectModel;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
	public class TripPopListViewController : UITableViewController
	{
		public ObservableCollection<TripData> tripData;
		public TripPopListViewController () : base (UITableViewStyle.Plain)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register the TableView's data source
			TableView.Source = new TripPopListViewSource (tripData);
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			TableView.SeparatorColor = ColorClass.ListSeparatorColor;
			TableView.SeparatorInset = new UIEdgeInsets (0, 5, 0, 5);
			TableView.ContentInset = new UIEdgeInsets (0, 0, 5, 0);
			TableView.ScrollIndicatorInsets = new UIEdgeInsets (0, 0, 5, 0);
			TableView.AllowsSelection = false;
			TableView.Bounces = false;
		}
	}
}

