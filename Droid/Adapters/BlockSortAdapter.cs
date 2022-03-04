using System;
using Android.Widget;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using System.Linq;
using Android.Util;

namespace Bidvalet.Droid
{
	public class BlockSortAdapter: BaseAdapter, IDraggableListAdapter
	{
		public List<string> Items { get; set; }


		public event DeleteEventHandler DeletedItem;
		public int mMobileCellPosition { get; set; }

		Activity context;

		public BlockSortAdapter (Activity context, List<string> items) : base ()
		{
			Items = items;
			this.context = context;
			mMobileCellPosition = int.MinValue;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return Items.ElementAt(position);
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View cell = convertView;
			if (cell  == null) {
				cell = context.LayoutInflater.Inflate (Resource.Layout.Cell_Block_Sort, parent, false);	
				cell.SetMinimumHeight (150);
				cell.SetBackgroundColor (Android.Graphics.Color.White);

			}
			var btnDelete = cell.FindViewById<ImageView> (Resource.Id.btnRemove);
			btnDelete.Click -= BtnDelete_Click;
			btnDelete.Click += BtnDelete_Click;
			btnDelete.Tag = position;

		
			var text = cell.FindViewById<TextView> (Resource.Id.tvBlockTitle);
			if (text != null) {
				text.Text = Items.ElementAt(position);
			}

			cell.Visibility = mMobileCellPosition == position ? ViewStates.Invisible : ViewStates.Visible;
			Log.Debug ("postionBlock ", "" + mMobileCellPosition);
			cell.TranslationY = 0;

			return cell;
		}

		void BtnDelete_Click (object sender, EventArgs e)
		{
			ImageView sd = sender as ImageView;
			if (DeletedItem != null) {
				DeletedItem(Items.ElementAt((int)sd.Tag));
			}
		}
		public override int Count {
			get {
				return Items.Count;
			}
		}

		public void SwapItems (int indexOne, int indexTwo)
		{
			var oldValue = Items [indexOne];
			Items [indexOne] = Items [indexTwo];
			Items [indexTwo] = oldValue;
			mMobileCellPosition = indexTwo;
			NotifyDataSetChanged ();
		}

	}
}

