using System;
using Android.Widget;
using Android.Views;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics.Drawables;
using Android.Content;

namespace Bidvalet.Droid
{
	public class SelectBaseAdapter: RecyclerView.Adapter
	{
		public event EventHandler<int> ItemBaseClick;
		View viewRow;
		List<string> items;
		int selectedPosition =-1;
		string _selected = "";
		Context mContext;
		public SelectBaseAdapter (Context context, List<string> data)
		{
			mContext = context;
			items = data;
		}

		// Create new views (invoked by the layout manager)
		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			var layoutInflater = LayoutInflater.From(parent.Context);
			viewRow = layoutInflater.Inflate(Resource.Layout.row_select_base, parent, false);
			var vh = new SelectBaseViewHolder (viewRow, OnItemBaseClickListener);
			return vh;
		}

		// Replace the contents of a view (invoked by the layout manager)
		public override void OnBindViewHolder (RecyclerView.ViewHolder viewHolder, int position)
		{
			//var item = items [position];
			// Replace the contents of the view with that element
			var holder = viewHolder as SelectBaseViewHolder;
			holder.DisplayName.Text = items.ElementAt(position);
			if (position == selectedPosition) {
//				Drawable img = mContext.Resources.GetDrawable (Resource.Drawable.fb_liked);
				viewRow.SetBackgroundColor (Android.Graphics.Color.LightGreen);
			} else {
				viewRow.SetBackgroundColor (Android.Graphics.Color.White);
			}

			viewRow.Click +=(sender, e) => {
				selectedPosition = position;
				viewRow.SetBackgroundColor (Android.Graphics.Color.LightGreen);
			};
		}

		public override int ItemCount {
			get {
				return items.Count;
			}
		}

		public class SelectBaseViewHolder : RecyclerView.ViewHolder 
		{
			public TextView DisplayName { get; set; }
			public RelativeLayout rowView{ get; set;}
			public SelectBaseViewHolder (View v, Action<int> listener) : base (v)
			{
				DisplayName = v.FindViewById<TextView>(Resource.Id.lbTitleBase);
				rowView = v.FindViewById<RelativeLayout>(Resource.Id.rlRowBase);
				v.Click += (sender, e) => listener (LayoutPosition);
			}
		}

		void OnItemBaseClickListener(int position){
			if (ItemBaseClick != null) {
				_selected = items.ElementAt (position);
				ItemBaseClick (this, position);
				//Toast.MakeText (mContext, _selected, ToastLength.Short).Show ();
			}
			NotifyItemChanged (position);
		}
	}
}

