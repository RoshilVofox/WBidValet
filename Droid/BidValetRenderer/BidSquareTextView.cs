using System;
using Android.Widget;
using Android.Content;
using Android.Util;

namespace Bidvalet.Droid
{
	public class BidSquareTextView:TextView
	{
		public BidSquareTextView (Context context): base(context)
		{
		}
		public BidSquareTextView(Context context, IAttributeSet attrs):base(context, attrs){
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			int size = 0;
			int width = MeasuredWidth;
			int height = MeasuredHeight;
//			if (width > height) {
//				size = height;
//			} else {
				size = width;
//			}
			SetMeasuredDimension (size, size);
		}
	}
}

