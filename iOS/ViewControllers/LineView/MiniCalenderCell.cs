
using System;

using Foundation;
using UIKit;
using CoreGraphics;
using System.Drawing;
using Bidvalet.Model;
using System.Collections.Generic;
using CoreAnimation;

namespace Bidvalet.iOS
{
	public partial class MiniCalenderCell : UICollectionViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("MiniCalenderCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("MiniCalenderCell");
		BidLineIcon  calenderIcon;
		public MiniCalenderCell (IntPtr handle) : base (handle)
		{
			
		}

		public void bindData(BidLineIcon  Icon)
		{
			calenderIcon = Icon;

//			BottomView.Frame = new CGRect (0, 2, this.Bounds.Size.Width, 2);
//			TopView.Frame = new CGRect (0, 0, this.Bounds.Size.Width, 2);
			BottomView.BackgroundColor = BottomColor ();
			TopView.BackgroundColor = TopColor ();
			baseView.BackgroundColor = UIColor.Clear;
			ViewLeftCornerBase.BackgroundColor = UIColor.Clear;
			if (!(calenderIcon.DutyPeriodIcon == 2 || calenderIcon.DutyPeriodIcon == 0)) {
				AddGraphics ();

			} 
//			else
//				baseView.BackgroundColor = TopColor ();

//			TopAddGraphics ();
//			BottomAddGraphics ();
		}



		void AddGraphics()
		{
			UIBezierPath Uppath = null;
			CGRect UppathRect;
			UIRectCorner UprectCorner= UIRectCorner.AllCorners;
			//UprectCorners = UIRectCorner.AllCorners;
			CGSize cornerRadii;

			UppathRect =new CGRect(0.0, 0.0, this.Bounds.Width-0.0, this.Bounds.Height);

//			if ((calenderIcon.DutyPeriodIcon == 2 || calenderIcon.DutyPeriodIcon == 0)) {
//				Uppath = UIBezierPath.FromRect (this.Bounds);
//				CAShapeLayer mask = new CAShapeLayer ();
//				mask.Frame = this.ContentView.Bounds;
//				mask.Path = Uppath.CGPath;
//				this.ContentView.Layer.Mask = mask;
//				this.BackgroundColor = TopColor ();
//				this.ClipsToBounds = true;
//				this.ContentView.ClipsToBounds = true;
//				return;
//			}

			switch (calenderIcon.DutyPeriodIcon) 
			{
			case 1:
				// Rounded on left.
				UprectCorner = UIRectCorner.TopLeft | UIRectCorner.BottomLeft;;
				UppathRect =new CGRect(0.0, 0.0, ViewLeftCornerBase.Bounds.Width-0.0, ViewLeftCornerBase.Bounds.Height);
				cornerRadii =new SizeF((float)(ViewLeftCornerBase.Bounds.Height / 2.0),(float) (ViewLeftCornerBase.Bounds.Height / 2.0));

				Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorner, cornerRadii);
				CAShapeLayer mask1 = new CAShapeLayer ();
				mask1.Frame = ViewLeftCornerBase.Bounds;
				mask1.Path = Uppath.CGPath;
				ViewLeftCornerBase.Layer.Mask = mask1;
				break;
			case 2:case 0:

				break;
			case 3:
				// Rounded on right.

				UprectCorner =  UIRectCorner.TopRight | UIRectCorner.BottomRight;
				UppathRect =new CGRect(0.0, 0.0, baseView.Bounds.Width-0.0, baseView.Bounds.Height);
				cornerRadii =new SizeF((float)(baseView.Bounds.Height / 2.0),(float) (baseView.Bounds.Height / 2.0));

				Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorner, cornerRadii);
				CAShapeLayer mask2 = new CAShapeLayer ();
				mask2.Frame = baseView.Bounds;
				mask2.Path = Uppath.CGPath;
				baseView.Layer.Mask = mask2;

				break;

			case 4:
				//baseView.Frame=	new CGRect(0.0, 0.0, this.Bounds.Width-0.0, this.Bounds.Height);
				UprectCorner = UIRectCorner.AllCorners;
				UppathRect =new CGRect(0.0, 0.0, this.ContentView.Bounds.Width-0.0, this.ContentView.Bounds.Height);
				cornerRadii =new SizeF((float)(this.ContentView.Bounds.Height / 2.0),(float) (this.ContentView.Bounds.Height / 2.0));

				Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorner, cornerRadii);
				CAShapeLayer mask3 = new CAShapeLayer ();
				mask3.Frame = this.ContentView.Bounds;
				mask3.Path = Uppath.CGPath;
				this.ContentView.Layer.Mask = mask3;
				break;
			}


			this.ClipsToBounds = true;
			this.ContentView.ClipsToBounds = true;

//			UppathRect =new CGRect(0.0, 0.0, this.Bounds.Width-0.0, this.Bounds.Height);
//			cornerRadii =new SizeF((float)(this.Bounds.Height / 2.0),(float) (this.Bounds.Height / 2.0));
//
//			Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorner, cornerRadii);
//			CAShapeLayer mask1 = new CAShapeLayer ();
//			mask1.Frame =this.ContentView.Bounds;
//			mask1.Path = Uppath.CGPath;
//			this.ContentView.Layer.Mask = mask1;
//			cornerRadii =new SizeF((float)(baseView.Bounds.Height / 2.0),(float) (baseView.Bounds.Height / 2.0));
//
//			Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorner, cornerRadii);
//			CAShapeLayer mask1 = new CAShapeLayer ();
//			mask1.Frame = baseView.Bounds;
//			mask1.Path = Uppath.CGPath;
//			baseView.Layer.Mask = mask1;

		}
		void TopAddGraphics()
		{
			UIBezierPath Uppath = null;
			CGRect UppathRect;
			UIRectCorner UprectCorner= UIRectCorner.AllCorners;
			//UprectCorners = UIRectCorner.AllCorners;
			CGSize cornerRadii;
			UppathRect =new CGRect(0.0, 0.0, TopView.Bounds.Width, TopView.Bounds.Height);

			if ((calenderIcon.DutyPeriodIcon == 2 || calenderIcon.DutyPeriodIcon == 0)) {
				Uppath = UIBezierPath.FromRect (TopView.Bounds);
				CAShapeLayer mask = new CAShapeLayer ();
				mask.Frame = TopView.Bounds;
				mask.Path = Uppath.CGPath;
				this.ContentView.Layer.Mask = mask;
				return;
			}

			switch (calenderIcon.DutyPeriodIcon) 
			{
			case 1:
				// Rounded on left.
				UprectCorner = UIRectCorner.TopLeft ;


				break;
			case 2:case 0:

				break;
			case 3:
				// Rounded on right.

				UprectCorner =  UIRectCorner.TopRight ;


				break;

			case 4:
				UprectCorner = UIRectCorner.TopLeft | UIRectCorner.TopRight ;
				break;
			}




			cornerRadii =new SizeF((float)(this.Bounds.Height / 2.0),(float) (this.Bounds.Height / 2.0));

			Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorner, cornerRadii);
			CAShapeLayer mask1 = new CAShapeLayer ();
			mask1.Frame = TopView.Bounds;
			mask1.Path = Uppath.CGPath;
			TopView.Layer.Mask = mask1;

		}

		void BottomAddGraphics()
		{
			UIBezierPath Uppath = null;
			CGRect UppathRect;
			UIRectCorner UprectCorner= UIRectCorner.AllCorners;
			//UprectCorners = UIRectCorner.AllCorners;
			CGSize cornerRadii;
			UppathRect =new CGRect(0.0, 0.0, BottomView.Bounds.Width, BottomView.Bounds.Height);

			if ((calenderIcon.DutyPeriodIcon == 2 || calenderIcon.DutyPeriodIcon == 0)) {
				Uppath = UIBezierPath.FromRect (BottomView.Bounds);
				CAShapeLayer mask = new CAShapeLayer ();
				mask.Frame = BottomView.Bounds;
				mask.Path = Uppath.CGPath;
				this.ContentView.Layer.Mask = mask;
				return;
			}

			switch (calenderIcon.DutyPeriodIcon) 
			{
			case 1:
				// Rounded on left.
				UprectCorner =  UIRectCorner.BottomLeft;;


				break;
			case 2:case 0:

				break;
			case 3:
				// Rounded on right.

				UprectCorner =   UIRectCorner.BottomRight;


				break;

			case 4:
				UprectCorner =  UIRectCorner.BottomLeft| UIRectCorner.BottomRight;
				break;
			}




			cornerRadii =new SizeF((float)(this.Bounds.Height / 2.0),(float) (this.Bounds.Height / 2.0));

			Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorner, cornerRadii);
			CAShapeLayer mask1 = new CAShapeLayer ();
			mask1.Frame = BottomView.Bounds;
			mask1.Path = Uppath.CGPath;
			BottomView.Layer.Mask = mask1;

		}
		public	UIColor BottomColor()
		{
			if (calenderIcon.ColorBottom != null) 
			{


				if (calenderIcon.ColorBottom == "Red")
					return ColorClass.OverlapColor;
				else if (calenderIcon.ColorBottom == "Green")
					return ColorClass.VacationOverlapTripColor;
                else if (calenderIcon.ColorBottom == CalenderColortype.FV.ToString())
                {
                    return ColorClass.FVVacationColor;
                }
				else if (calenderIcon.ColorBottom == CalenderColortype.VA.ToString ()) {
					return ColorClass.VacationTripColor;
				} else if (calenderIcon.ColorBottom == CalenderColortype.VO.ToString ()) {
					return ColorClass.VacationOverlapTripColor;
				} else if (calenderIcon.ColorBottom == CalenderColortype.VD.ToString ()) {
					return ColorClass.VacationDropTripColor;
				} else if (calenderIcon.ColorBottom == CalenderColortype.Transparaent.ToString ())
					return UIColor.Clear;
				else if (calenderIcon.ColorBottom == "AM")
					return ColorClass.AMTripColor;
				else if (calenderIcon.ColorBottom == "PM")
					return ColorClass.PMTripColor;
				else if (calenderIcon.ColorBottom == "MIX")
					return ColorClass.MixedTripColor;
				else if (calenderIcon.ColorBottom == "AMReserve")
					return ColorClass.AMReserveTripColor;
				else if (calenderIcon.ColorBottom == "PMReserve")
					return ColorClass.PMReserveTripColor;
				else if (calenderIcon.ColorBottom == "MIXReserve")
					return ColorClass.MixedTripColor;
                else if (calenderIcon.ColorBottom == "OtherReserve" || calenderIcon.ColorBottom == "Other")
                    return ColorClass.OtherTripColor;

				else
					return UIColor.White;

				//	Downpath.Fill ();
			}
			return UIColor.White;
		}

		public	UIColor TopColor()
		{
			if (calenderIcon.ColorTop != null) 
			{


				if (calenderIcon.ColorTop == "Red")
					return ColorClass.OverlapColor;
				else if (calenderIcon.ColorTop == "Green")
					return ColorClass.VacationOverlapTripColor;
                else if (calenderIcon.ColorTop == CalenderColortype.FV.ToString())
                {
                    return ColorClass.FVVacationColor;
                }
				else if (calenderIcon.ColorTop == CalenderColortype.VA.ToString ()) {
					return ColorClass.VacationTripColor;
				} else if (calenderIcon.ColorTop == CalenderColortype.VO.ToString ()) {
					return ColorClass.VacationOverlapTripColor;
				} else if (calenderIcon.ColorTop == CalenderColortype.VD.ToString ()) {
					return ColorClass.VacationDropTripColor;
				} else if (calenderIcon.ColorTop == CalenderColortype.Transparaent.ToString ())
					return UIColor.Clear;
				else if (calenderIcon.ColorTop == "AM")
					return ColorClass.AMTripColor;
				else if (calenderIcon.ColorTop == "PM")
					return ColorClass.PMTripColor;
				else if (calenderIcon.ColorTop == "MIX")
					return ColorClass.MixedTripColor;
				else if (calenderIcon.ColorTop == "AMReserve")
					return ColorClass.AMReserveTripColor;
				else if (calenderIcon.ColorTop == "PMReserve")
					return ColorClass.PMReserveTripColor;
				else if (calenderIcon.ColorTop == "MIXReserve")
					return ColorClass.MixedTripColor;
                else if (calenderIcon.ColorTop == "Other" || calenderIcon.ColorTop == "OtherReserve")
                    return ColorClass.OtherTripColor;

				else
					return UIColor.White;

				//	Downpath.Fill ();
			}

			return UIColor.White;
		}

//		public override void Draw (CGRect rect)
//		{
//			UIBezierPath Uppath = null;
////			UIBezierPath Downpath = null;
//
//
//			CGRect UppathRect;
//			UIRectCorner UprectCorners;
//
////			CGRect DownpathRect;
////			UIRectCorner DownrectCorners;
//
//			CGSize cornerRadii;
//
//
//			switch (calenderIcon.DutyPeriodIcon) {
//			case 1:
//				// Rounded on left.
//				UppathRect =new CGRect(0.0, 0.0, rect.Width- 0.0, rect.Height);
//				UprectCorners = UIRectCorner.TopLeft | UIRectCorner.BottomLeft;;
//				cornerRadii =new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
//				Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorners, cornerRadii);
//
////				DownpathRect =new CGRect(0.0,rect.Height/2, rect.Width - 0.0, rect.Height);
////				DownrectCorners =  UIRectCorner.BottomLeft;
////				cornerRadii =new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
////				Downpath = UIBezierPath.FromRoundedRect (DownpathRect, DownrectCorners, cornerRadii);
//
//				break;
//			case 2:case 0:
//				//Square corner
//				UppathRect =new CGRect(0.0, 0.0, rect.Width- 0.0, rect.Height);
//				Uppath = UIBezierPath.FromRect (rect);
////				DownpathRect =new CGRect(0.0,rect.Height/2, rect.Width - 0.0, rect.Height);
////				Downpath = UIBezierPath.FromRect (rect);
//				break;
//			case 3:
//				// Rounded on right.
//				UppathRect = new CGRect(0.0, 0.0,  rect.Width - 0.0, rect.Height);
//				UprectCorners =  UIRectCorner.TopRight ;
//				cornerRadii = new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
//				Uppath =  UIBezierPath.FromRoundedRect (UppathRect, UprectCorners, cornerRadii);
//
////				DownpathRect = new CGRect(0.0, 0.0,  rect.Width - 0.0, rect.Height/2);
////				DownrectCorners =  UIRectCorner.BottomRight;
////				cornerRadii = new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
////				Downpath =  UIBezierPath.FromRoundedRect (DownpathRect, DownrectCorners, cornerRadii);
//
//				break;
//
//			case 4:
//				break;
//			}
//			Uppath.Fill ();
//		}
//
//
//			if (calenderIcon.ColorBottom != null) 
//			{
//
//
//				if (calenderIcon.ColorBottom == "Red")
//					ColorClass.OverlapColor.SetColor ();
//				else if (calenderIcon.ColorBottom == "Green")
//					ColorClass.VacationOverlapTripColor.SetColor ();
//				else if (calenderIcon.ColorBottom == CalenderColortype.VA.ToString ()) {
//					ColorClass.VacationTripColor.SetColor ();
//				} else if (calenderIcon.ColorBottom == CalenderColortype.VO.ToString ()) {
//					ColorClass.VacationOverlapTripColor.SetColor ();
//				} else if (calenderIcon.ColorBottom == CalenderColortype.VD.ToString ()) {
//					ColorClass.VacationDropTripColor.SetColor ();
//				} else if (calenderIcon.ColorBottom == CalenderColortype.Transparaent.ToString ())
//					UIColor.Clear.SetColor ();
//				else if (calenderIcon.ColorBottom == "AM")
//					ColorClass.AMTripColor.SetColor ();
//				else if (calenderIcon.ColorBottom == "PM")
//					ColorClass.PMTripColor.SetColor ();
//				else if (calenderIcon.ColorBottom == "MIX")
//					ColorClass.MixedTripColor.SetColor ();
//				else if (calenderIcon.ColorBottom == "AMReserve")
//					ColorClass.AMReserveTripColor.SetColor ();
//				else if (calenderIcon.ColorBottom == "PMReserve")
//					ColorClass.PMReserveTripColor.SetColor ();
//				else if (calenderIcon.ColorBottom == "MIXReserve")
//					ColorClass.MixedTripColor.SetColor ();
//				
//				else
//					UIColor.White.SetColor ();
//
//			//	Downpath.Fill ();
//			}
//			if (calenderIcon.ColorTop != null) 
//			{
//				if (calenderIcon.ColorTop == CalenderColortype.VA.ToString ()) {
//					ColorClass.VacationTripColor.SetColor ();
//				} else if (calenderIcon.ColorTop == CalenderColortype.VO.ToString ()) {
//					ColorClass.VacationOverlapTripColor.SetColor ();
//				} else if (calenderIcon.ColorTop == CalenderColortype.VD.ToString ()) {
//					ColorClass.VacationDropTripColor.SetColor ();
//				}  else if (calenderIcon.ColorTop == CalenderColortype.Transparaent.ToString ()) {
//					UIColor.Clear.SetColor ();
//				} 
//				else if (calenderIcon.ColorTop == "AM")
//					ColorClass.AMTripColor.SetColor ();
//				else if (calenderIcon.ColorTop == "PM")
//					ColorClass.PMTripColor.SetColor ();
//				else if (calenderIcon.ColorTop == "MIX")
//					ColorClass.MixedTripColor.SetColor ();
//				else if (calenderIcon.ColorTop == "AMReserve")
//					ColorClass.AMReserveTripColor.SetColor ();
//				else if (calenderIcon.ColorTop == "PMReserve")
//					ColorClass.PMReserveTripColor.SetColor ();
//				else if (calenderIcon.ColorTop == "MIXReserve")
//					ColorClass.MixedTripColor.SetColor ();
//
//				else
//					UIColor.White.SetColor ();
//				Uppath.Fill ();
//			}
//
//
////
////
////				 if (calenderIcon.ColorTop == CalenderColortype.MILVO_No_Work.ToString())
////				{
////					UIColor.Orange.SetColor ();
////
////				}
////				else if (calenderIcon.ColorTop==CalenderColortype.MILBackSplitWork.ToString())
////				{
////					UIColor.Orange.SetColor ();
////
////				}
////				else if (calenderIcon.ColorTop == CalenderColortype.MILFrontSplitWork.ToString())
////				{
////					UIColor.Clear.SetColor ();
////
////				}
////				else if (calenderIcon.ColorTop==CalenderColortype.MILBackSplitWithoutStrike.ToString())
////				{
////					UIColor.Orange.SetColor ();
////
////				}
////				else if (calenderIcon.ColorTop==CalenderColortype.MILFrontSplitWithoutStrike.ToString())
////				{
////					UIColor.Clear.SetColor ();
////
////				}
////
////				if (calenderIcon.ColorBottom == CalenderColortype.MILVO_No_Work.ToString())
////				{
////
////					UIColor.Orange.SetColor ();
////				}
////				else if (calenderIcon.ColorBottom==CalenderColortype.MILBackSplitWork.ToString())
////				{
////
////					 UIColor.Clear.SetColor ();
////				}
////				else if (calenderIcon.ColorBottom == CalenderColortype.MILFrontSplitWork.ToString())
////				{
////
////					 UIColor.Orange.SetColor ();
////				}
////				else if (calenderIcon.ColorBottom==CalenderColortype.MILBackSplitWithoutStrike.ToString())
////				{
////
////					UIColor.Clear.SetColor ();
////				}
////				else if (calenderIcon.ColorBottom==CalenderColortype.MILFrontSplitWithoutStrike.ToString())
////				{
////
////					UIColor.Orange.SetColor ();
////				}
////				else
////			{
////				UIColor.White.SetColor ();
////
////			}
//
//			Uppath.Fill ();
//
//		}
//
//
////		public override void Draw (CGRect rect)
////		{
////			UIBezierPath Uppath = null;
////			UIBezierPath Downpath = null;
////
////
////			CGRect UppathRect;
////			UIRectCorner UprectCorners;
////
////			CGRect DownpathRect;
////			UIRectCorner DownrectCorners;
////
////			CGSize cornerRadii;
////
////		
////			switch (calenderIcon.DutyPeriodIcon) {
////			case 1:
////				// Rounded on left.
////				UppathRect =new CGRect(0.0, 0.0, rect.Width- 0.0, rect.Height/2);
////				UprectCorners = UIRectCorner.TopLeft ;
////				cornerRadii =new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
////				Uppath = UIBezierPath.FromRoundedRect (UppathRect, UprectCorners, cornerRadii);
////
////				DownpathRect =new CGRect(0.0,rect.Height/2, rect.Width - 0.0, rect.Height);
////				DownrectCorners =  UIRectCorner.BottomLeft;
////				cornerRadii =new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
////				Downpath = UIBezierPath.FromRoundedRect (DownpathRect, DownrectCorners, cornerRadii);
////
////				break;
////			case 2:case 0:
////				//Square corner
////				UppathRect =new CGRect(0.0, 0.0, rect.Width- 0.0, rect.Height/2);
////				Uppath = UIBezierPath.FromRect (rect);
////				DownpathRect =new CGRect(0.0,rect.Height/2, rect.Width - 0.0, rect.Height);
////				Downpath = UIBezierPath.FromRect (rect);
////				break;
////			case 3:
////				// Rounded on right.
////				UppathRect = new CGRect(0.0, 0.0,  rect.Width - 0.0, rect.Height/2);
////				UprectCorners =  UIRectCorner.TopRight ;
////				cornerRadii = new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
////				Uppath =  UIBezierPath.FromRoundedRect (UppathRect, UprectCorners, cornerRadii);
////
////				DownpathRect = new CGRect(0.0, 0.0,  rect.Width - 0.0, rect.Height/2);
////				DownrectCorners =  UIRectCorner.BottomRight;
////				cornerRadii = new SizeF((float)(rect.Height / 2.0),(float) (rect.Height / 2.0));
////				Downpath =  UIBezierPath.FromRoundedRect (DownpathRect, DownrectCorners, cornerRadii);
////
////				break;
////
////			case 4:
////				break;
////			}
////		
////
////			UIColor.Green.SetColor ();
////			Uppath.Fill ();
////
////			UIColor.Orange.SetColor ();
////			Downpath.Fill ();
////
////		}



		public static MiniCalenderCell Create ()
		{
			return (MiniCalenderCell)Nib.Instantiate (null, null) [0];
		}
	}
}

