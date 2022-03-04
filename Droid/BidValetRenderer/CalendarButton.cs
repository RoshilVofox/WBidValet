using System;
using Android.Widget;
using Android.Content;
using Android.Util;

namespace Bidvalet.Droid
{
	public class CalendarButton:Button
	{
		public enum CalendarButtonState
		{
			WHITE, GRAY, GREEN,RED
		}
		public int Position;
		public int ID;
		public CalendarButtonState State;
		public CalendarButton (Context context, IAttributeSet attrs): base (context, attrs)
		{
		}
		public CalendarButton (Context context): base(context){
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			int size = 0;
			size = MeasuredWidth;
			SetMeasuredDimension (size, size);
		}
	}
}

