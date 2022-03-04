using System;
using Android.Views;
using Android.Widget;
using System.Linq;
using Android.App;
using Java.Lang;
using Android.Util;
using Java.Util;
using System.Collections.Generic;

namespace Bidvalet.Droid
{
	public class ConstraintsAdapter : BaseAdapter, IDraggableListAdapter
	{
		public int mMobileCellPosition { get; set; }

		public event EventHandler<CommutableLineCx> CmtLineTitleClick;
		public event EventHandler<DaysOfMonthCx> DaysOfMonthClick;
		public event EventHandler<OvernightCitiesCx> TitleOvernightCityClick;
		public event EventHandler<RestCx> LessMoreRestClick;
		public event EventHandler<RestCx> RestClick;
		public event EventHandler<RestCx> RestValueClick;
		public event EventHandler<EquirementConstraint> EquipmentValueClick;
		public event EventHandler<EquirementConstraint> EquipmentClick;
		public event EventHandler<EquirementConstraint> EquipmentLessMoreClick;
		public event EventHandler<DHFristLastConstraint> DHFirstLastValueClick;
		public event EventHandler<DHFristLastConstraint> DHLessMoreClick;
		public event EventHandler<DHFristLastConstraint> DHFirstLastClick;
		public event EventHandler<DaysOfWeekSome> DaysOfWeekValueClick;
		public event EventHandler<DaysOfWeekSome> DaysOfWeekLessMoreClick;
		public event EventHandler<DaysOfWeekSome> DaysOfWeekClick;

		Activity activity;

		public System.Collections.Generic.List<object> Items;

		public ConstraintsAdapter(Activity activity):base()
		{
			this.activity = activity;
			mMobileCellPosition = int.MinValue;
			Items = SharedObject.Instance.ListConstraint;
		}

		public override int Count
		{
			get { return SharedObject.Instance.ListConstraint.Count; }
		}

		public override Java.Lang.Object GetItem (int position)
		{
			var currentItem = Items [position];
			return (Java.Lang.Object)currentItem;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			var currentItem = Items[position];

			if (currentItem is AMPMConstriants) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellAmPm, parent, false);
				Button btnAM = view.FindViewById<Button> (Resource.Id.btnCellAm);
				Button btnPM = view.FindViewById<Button> (Resource.Id.btnCellPm);
				Button btnMix = view.FindViewById<Button> (Resource.Id.btnCellMix);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);
				AMPMConstriants ampm = currentItem as AMPMConstriants;
				ChangeColorButton (ampm.AM, btnAM);
				ChangeColorButton (ampm.PM, btnPM);
				ChangeColorButton (ampm.MIX, btnMix);
				btnAM.Click += (sender, e) => {
					ampm.AM = !ampm.AM;
					ChangeColorButton (ampm.AM, btnAM);
				};
				btnPM.Click += (sender, e) => {
					ampm.PM = !ampm.PM;
					ChangeColorButton (ampm.PM, btnPM);
				};
				btnMix.Click += (sender, e) => {
					ampm.MIX = !ampm.MIX;
					ChangeColorButton (ampm.MIX, btnMix);
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (ampm);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is CommutableLineCx) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellCommutableLines, parent, false);
				//cmtLinesViewHolder = new CellCommutableLinesViewHolder();
				TextView CmutLineTextView = view.FindViewById<TextView> (Resource.Id.tvBlockTitle);
				Button btnAny = view.FindViewById<Button> (Resource.Id.btnAny);
				Button btnBoth = view.FindViewById<Button> (Resource.Id.btnBoth);
				Button btnToHome = view.FindViewById<Button> (Resource.Id.btnToHome);
				Button btnToWork = view.FindViewById<Button> (Resource.Id.btnToWork);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				CommutableLineCx cm = currentItem as CommutableLineCx;
				ChangeColorButtonGreen (cm.IsAny, btnAny);
				ChangeColorButtonGreen (cm.IsRonBoth, btnBoth);
				ChangeColorButton (cm.IsHome, btnToHome);
				ChangeColorButton (cm.IsWork, btnToWork);
				CmutLineTextView.Text = string.Format ("Cmut Lines ({0})", cm.City);
				CmutLineTextView.Click += (sender, e) => {
					if (CmtLineTitleClick != null) {
						CmtLineTitleClick (this, cm);
					}
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (cm);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
				btnAny.Click += (sender, e) => {
					cm.IsAny = true;
					cm.IsRonBoth = false;
					ChangeColorButtonGreen (cm.IsAny, btnAny);
					ChangeColorButtonGreen (cm.IsRonBoth, btnBoth);
				};

				btnBoth.Click += (sender, e) => {
					cm.IsAny = false;
					cm.IsRonBoth = true;
					ChangeColorButtonGreen (cm.IsAny, btnAny);
					ChangeColorButtonGreen (cm.IsRonBoth, btnBoth);
				};
				btnToHome.Click += (sender, e) => {
					cm.IsHome = !cm.IsHome;
					ChangeColorButton (cm.IsHome, btnToHome);
				};
				btnToWork.Click += (sender, e) => {
					cm.IsWork = !cm.IsWork;
					ChangeColorButton (cm.IsWork, btnToWork);
				};
			} else if (currentItem is DaysOfMonthCx) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellDayOfMonth, parent, false);
				//domViewHolder = new CellDayOfMonthViewHolder ();
				TextView titleDOMTextView = view.FindViewById<TextView> (Resource.Id.tvDayOfMonthTitle);
				TextView daysOfMonthTextView = view.FindViewById<TextView> (Resource.Id.tvDayOfMonth);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				DaysOfMonthCx dom = currentItem as DaysOfMonthCx;
				StringBuilder builderOFF = new StringBuilder ();
				int[] arr = dom.OFFDays.ToArray ();
				foreach (int s in arr) {
					builderOFF.Append (s + ", ");
				}
				StringBuilder builderWORK = new StringBuilder ();
				int[] arrW = dom.WorkDays.ToArray ();
				foreach (int s in arrW) {
					builderWORK.Append (s + ", ");
				}
				daysOfMonthTextView.Text = string.Format ("off[{0}] work[{1}]", builderOFF.ToString (), builderWORK.ToString ());
				btnRemove.Click += (sd, e) => {
					Items.Remove (dom);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
				daysOfMonthTextView.Click += (sender, e) => {
					if (DaysOfMonthClick != null) {
						DaysOfMonthClick (this, dom);
					}
				};
				titleDOMTextView.Click += (sender, e) => {
					if (DaysOfMonthClick != null) {
						DaysOfMonthClick (this, dom);
					}
				};
			} else if (currentItem is DaysOfWeekAll) {
				//view = activity.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem1, parent, false);	
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellDayWeekAll, parent, false);

//				dowAllViewHolder = new CellDayWeekAllViewHolder ();
				Button btnSu = view.FindViewById<Button> (Resource.Id.btnSu);
				Button btnMo = view.FindViewById<Button> (Resource.Id.btnMo);
				Button btnTu = view.FindViewById<Button> (Resource.Id.btnTu);
				Button btnWe = view.FindViewById<Button> (Resource.Id.btnWe);
				Button btnTh = view.FindViewById<Button> (Resource.Id.btnTh);
				Button btnFr = view.FindViewById<Button> (Resource.Id.btnFr);
				Button btnSa = view.FindViewById<Button> (Resource.Id.btnSa);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				DaysOfWeekAll dowAll = currentItem as DaysOfWeekAll;
				//view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = string.Format("SUN:{0}",dowAll.Su);
				ChangeColorButton (dowAll.Su, btnSu);
				ChangeColorButton (dowAll.Mo, btnMo);
				ChangeColorButton (dowAll.Tu, btnTu);
				ChangeColorButton (dowAll.We, btnWe);
				ChangeColorButton (dowAll.Th, btnTh);
				ChangeColorButton (dowAll.Fr, btnFr);
				ChangeColorButton (dowAll.Sa, btnSa);

				btnSu.Click += (sender, e) => {
					dowAll.Su = !dowAll.Su;
					ChangeColorButton (dowAll.Su, btnSu);
				};
				btnMo.Click += (sender, e) => {
					dowAll.Mo = !dowAll.Mo;
					ChangeColorButton (dowAll.Mo, btnMo);
				};
				btnTu.Click += (sender, e) => {
					dowAll.Tu = !dowAll.Tu;
					ChangeColorButton (dowAll.Tu, btnTu);
				};
				btnWe.Click += (sender, e) => {
					dowAll.We = !dowAll.We;
					ChangeColorButton (dowAll.We, btnWe);
				};
				btnTh.Click += (sender, e) => {
					dowAll.Th = !dowAll.Th;
					ChangeColorButton (dowAll.Th, btnTh);
				};
				btnFr.Click += (sender, e) => {
					dowAll.Fr = !dowAll.Fr;
					ChangeColorButton (dowAll.Fr, btnFr);
				};
				btnSa.Click += (sender, e) => {
					dowAll.Sa = !dowAll.Sa;
					ChangeColorButton (dowAll.Sa, btnSa);
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (dowAll);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is DaysOfWeekSome) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellDayWeekSome, parent, false);
				//dowSomeViewHolder = new CellDayWeekSomeViewHolder ();
				TextView tvDay = view.FindViewById<TextView> (Resource.Id.tvDayOfWeek);
				TextView tvLessMore = view.FindViewById<TextView> (Resource.Id.tvMoreLess);
				TextView tvValue = view.FindViewById<TextView> (Resource.Id.tvDayValue);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				DaysOfWeekSome dowS = currentItem as DaysOfWeekSome;
				tvValue.Text = string.Format ("{0}", dowS.Value);
				tvLessMore.Text = string.Format ("{0}", dowS.LessOrMore);
				tvDay.Text = string.Format ("{0}", dowS.Date);
				tvDay.Click += (sender, e) => {
					//show dialog day of week
					if (DaysOfWeekClick != null) {
						DaysOfWeekClick (this, dowS);
					}
				};
				tvLessMore.Click += (sender, e) => {
					//show dialog less or more
					if (DaysOfWeekLessMoreClick != null) {
						DaysOfWeekLessMoreClick (this, dowS);
					}
				};
				tvValue.Click += (sender, e) => {
					//show dialog value
					if (DaysOfWeekValueClick != null) {
						DaysOfWeekValueClick (this, dowS);
					}
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (dowS);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is DHFristLastConstraint) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellDHFirstLast, parent, false);
				//dhViewHolder = new CellDHFirstLastViewHolder ();
				TextView tvDH = view.FindViewById<TextView> (Resource.Id.tvDHValue);
				TextView tvValue = view.FindViewById<TextView> (Resource.Id.tvValue);
				TextView tvLessMore = view.FindViewById<TextView> (Resource.Id.tvMoreLess);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				DHFristLastConstraint dh = currentItem as DHFristLastConstraint;
				tvDH.Text = string.Format ("{0}", dh.DH);
				tvLessMore.Text = string.Format ("{0}", dh.LessMore);
				tvValue.Text = string.Format ("{0}", dh.Value);
				tvDH.Click += (sender, e) => {
					//show dialog day of week
					if (DHFirstLastClick != null) {
						DHFirstLastClick (this, dh);
					}
				};
				tvLessMore.Click += (sender, e) => {
					//show dialog less or more
					if (DHLessMoreClick != null) {
						DHLessMoreClick (this, dh);
					}
				};
				tvValue.Click += (sender, e) => {
					//show dialog value
					if (DHFirstLastValueClick != null) {
						DHFirstLastValueClick (this, dh);
					}
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (dh);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is EquirementConstraint) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellEquipment, parent, false);
				//eqViewHolder = new CellEquipmentViewHolder ();
				TextView tvEquipment = view.FindViewById<TextView> (Resource.Id.tvEquipmentValue);
				TextView tvValue = view.FindViewById<TextView> (Resource.Id.tvValue);
				TextView tvLessMore = view.FindViewById<TextView> (Resource.Id.tvMoreLess);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				EquirementConstraint eq = currentItem as EquirementConstraint;
				tvEquipment.Text = string.Format ("{0}", eq.Equirement);
				tvLessMore.Text = string.Format ("{0}", eq.LessMore);
				tvValue.Text = string.Format ("{0}", eq.Value);
				tvEquipment.Click += (sender, e) => {
					//show dialog day of week
					if (EquipmentClick != null) {
						EquipmentClick (this, eq);
					}
				};
				tvLessMore.Click += (sender, e) => {
					//show dialog less or more
					if (EquipmentLessMoreClick != null) {
						EquipmentLessMoreClick (this, eq);
					}
				};
				tvValue.Click += (sender, e) => {
					//show dialog value
					if (EquipmentValueClick != null) {
						EquipmentValueClick (this, eq);
					}
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (eq);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is LineTypeConstraint) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellLineType, parent, false);
				//ltViewHolder = new CellLineTypeViewHolder ();
				Button btnHard = view.FindViewById<Button> (Resource.Id.btnHard);
				Button btnRes = view.FindViewById<Button> (Resource.Id.btnRes);
				Button btnBlank = view.FindViewById<Button> (Resource.Id.btnBlank);
				Button btnInt = view.FindViewById<Button> (Resource.Id.btnInt);
				Button btnNonCon = view.FindViewById<Button> (Resource.Id.btnNonCon);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				LineTypeConstraint lt = currentItem as LineTypeConstraint;
				ChangeColorButton (lt.Blank, btnBlank);
				ChangeColorButton (lt.Hard, btnHard);
				ChangeColorButton (lt.Res, btnRes);
				ChangeColorButton (lt.Int, btnInt);
				ChangeColorButton (lt.NonCon, btnNonCon);
				btnBlank.Click += (sender, e) => {
					lt.Blank = !lt.Blank;
					ChangeColorButton (lt.Blank, btnBlank);
				};
				btnHard.Click += (sender, e) => {
					lt.Hard = !lt.Hard;
					ChangeColorButton (lt.Hard, btnHard);
				};
				btnRes.Click += (sender, e) => {
					lt.Res = !lt.Res;
					ChangeColorButton (lt.Res, btnRes);
				};
				btnInt.Click += (sender, e) => {
					lt.Int = !lt.Int;
					ChangeColorButton (lt.Int, btnInt);
				};
				btnNonCon.Click += (sender, e) => {
					lt.NonCon = !lt.NonCon;
					ChangeColorButton (lt.NonCon, btnNonCon);
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (lt);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is OvernightCitiesCx) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellOvernightCity, parent, false);
				//ovViewHolder = new CellOvernightCitiesViewHolder ();
				TextView overnightTextView = view.FindViewById<TextView> (Resource.Id.tvDayOfMonthTitle);
				TextView cityTextView = view.FindViewById<TextView> (Resource.Id.tvDayOfMonth);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);
				OvernightCitiesCx ov = currentItem as OvernightCitiesCx;
				StringBuilder builderY = new StringBuilder ();
				string[] arrYes = ov.Yes.ToArray ();
				foreach (string s in arrYes) {
					builderY.Append (s + ", ");
				}
				StringBuilder builderN = new StringBuilder ();
				string[] arrNo = ov.No.ToArray ();
				foreach (string s in arrNo) {
					builderN.Append (s + ", ");
				}
				cityTextView.Text = string.Format ("yes[{0}] no[{1}]", builderY.ToString (), builderN.ToString ());
				overnightTextView.Click += (sender, e) => {
					if (TitleOvernightCityClick != null) {
						TitleOvernightCityClick (this, ov);
					}
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (ov);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is RestCx) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellRest, parent, false);
				//reViewHolder = new CellRestViewHolder ();
				TextView tvRest = view.FindViewById<TextView> (Resource.Id.tvRestValue);
				TextView tvLessMore = view.FindViewById<TextView> (Resource.Id.tvMoreLess);
				TextView tvValue = view.FindViewById<TextView> (Resource.Id.tvValue);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);

				RestCx re = currentItem as RestCx;
				tvRest.Text = string.Format ("{0}", re.Dom);
				tvLessMore.Text = string.Format ("{0}", re.LessMore);
				tvValue.Text = string.Format ("{0}", re.Value);
				tvRest.Click += (sender, e) => {
					//show dialog day of week
					if (RestClick != null) {
						RestClick (this, re);
					}
				};
				tvLessMore.Click += (sender, e) => {
					//show dialog less or more
					if (LessMoreRestClick != null) {
						LessMoreRestClick (this, re);
					}
				};
				tvValue.Click += (sender, e) => {
					//show dialog value
					if (RestValueClick != null) {
						RestValueClick (this, re);
					}
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (re);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is StartDayOfWeek) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellStartDayWeek, parent, false);
				Button btnSu = view.FindViewById<Button> (Resource.Id.btnSu);
				Button btnMo = view.FindViewById<Button> (Resource.Id.btnMo);
				Button btnTu = view.FindViewById<Button> (Resource.Id.btnTu);
				Button btnWe = view.FindViewById<Button> (Resource.Id.btnWe);
				Button btnTh = view.FindViewById<Button> (Resource.Id.btnTh);
				Button btnFr = view.FindViewById<Button> (Resource.Id.btnFr);
				Button btnSa = view.FindViewById<Button> (Resource.Id.btnSa);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);
				StartDayOfWeek sdow = currentItem as StartDayOfWeek;
				ChangeColorButton (sdow.Su, btnSu);
				ChangeColorButton (sdow.Mo, btnMo);
				ChangeColorButton (sdow.Tu, btnTu);
				ChangeColorButton (sdow.We, btnWe);
				ChangeColorButton (sdow.Th, btnTh);
				ChangeColorButton (sdow.Fr, btnFr);
				ChangeColorButton (sdow.Sa, btnSa);

				btnSu.Click += (sender, e) => {
					sdow.Su = !sdow.Su;
					ChangeColorButton (sdow.Su, btnSu);
				};
				btnMo.Click += (sender, e) => {
					sdow.Mo = !sdow.Mo;
					ChangeColorButton (sdow.Mo, btnMo);
				};
				btnTu.Click += (sender, e) => {
					sdow.Tu = !sdow.Tu;
					ChangeColorButton (sdow.Tu, btnTu);
				};
				btnWe.Click += (sender, e) => {
					sdow.We = !sdow.We;
					ChangeColorButton (sdow.We, btnWe);
				};
				btnTh.Click += (sender, e) => {
					sdow.Th = !sdow.Th;
					ChangeColorButton (sdow.Th, btnTh);
				};
				btnFr.Click += (sender, e) => {
					sdow.Fr = !sdow.Fr;
					ChangeColorButton (sdow.Fr, btnFr);
				};
				btnSa.Click += (sender, e) => {
					sdow.Sa = !sdow.Sa;
					ChangeColorButton (sdow.Sa, btnSa);
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (sdow);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else if (currentItem is TripBlockLenghtCx) {
				view = activity.LayoutInflater.Inflate (Resource.Layout.CellTripBlockLength, parent, false);

				Button btnTurn = view.FindViewById<Button> (Resource.Id.btnTurn);
				Button btnTwo = view.FindViewById<Button> (Resource.Id.btnTwoDay);
				Button btnThree = view.FindViewById<Button> (Resource.Id.btnThreeDay);
				Button btnFo = view.FindViewById<Button> (Resource.Id.btnFoDay);
				Button btnTrip = view.FindViewById<Button> (Resource.Id.btnTrip);
				Button btnBlock = view.FindViewById<Button> (Resource.Id.btnBlock);
				ImageView btnRemove = view.FindViewById<ImageView> (Resource.Id.btnRemove);
				TripBlockLenghtCx tr = currentItem as TripBlockLenghtCx;
				ChangeColorButton (tr.Turn, btnTurn);
				ChangeColorButton (tr.TwoDay, btnTwo);
				ChangeColorButton (tr.ThreeDay, btnThree);
				ChangeColorButton (tr.FoDay, btnFo);
				ChangeColorButtonGreen (tr.IsTrip, btnTrip);
				ChangeColorButtonGreen (tr.IsBlock, btnBlock);

				btnTurn.Click += (sender, e) => {
					tr.Turn = !tr.Turn;
					ChangeColorButton (tr.Turn, btnTurn);
				};
				btnTwo.Click += (sender, e) => {
					tr.TwoDay = !tr.TwoDay;
					ChangeColorButton (tr.TwoDay, btnTwo);
				};
				btnThree.Click += (sender, e) => {
					tr.ThreeDay = !tr.ThreeDay;
					ChangeColorButton (tr.ThreeDay, btnThree);
				};
				btnFo.Click += (sender, e) => {
					tr.FoDay = !tr.FoDay;
					ChangeColorButton (tr.FoDay, btnFo);
				};
				btnTrip.Click += (sender, e) => {
					tr.IsTrip = true;
					tr.IsBlock = false;
					ChangeColorButtonGreen (tr.IsTrip, btnTrip);
					ChangeColorButtonGreen (tr.IsBlock, btnBlock);
				};
				btnBlock.Click += (sender, e) => {
					tr.IsBlock = true;
					tr.IsTrip = false;
					ChangeColorButtonGreen (tr.IsBlock, btnBlock);
					ChangeColorButtonGreen (tr.IsTrip, btnTrip);
				};
				btnRemove.Click += (sd, e) => {
					Items.Remove (tr);
					ConstraintViewActivity act = (ConstraintViewActivity)activity;
					act.ReloadData ();
				};
			} else {	
				view = activity.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem1, parent, false);	
				view.SetMinimumHeight (150);
				view.SetBackgroundColor (Android.Graphics.Color.White);
				view.FindViewById<TextView> (Android.Resource.Id.Text1).SetText (Resource.String.app_name);
			}
			view.Visibility = mMobileCellPosition == position ? ViewStates.Invisible : ViewStates.Visible;
			Log.Debug ("postion", "" + mMobileCellPosition);
			view.TranslationY = 0;
			return view;
		}

		public void SwapItems (int indexOne, int indexTwo)
		{
			Console.WriteLine ("Swap: " + indexOne + " - " + indexTwo);
			var lstConstraint = SharedObject.Instance.ListConstraint;
			if ((indexOne == indexTwo) || (0 > indexOne) || (indexOne >= lstConstraint.Count) || (0 > indexTwo) ||
				(indexTwo >= lstConstraint.Count)) return;
//			var tmp = lstConstraint[indexOne];
//			lstConstraint[indexOne] = lstConstraint[indexTwo];
//			lstConstraint[indexTwo] = tmp; 

//			var i = 0;
			var tmp = lstConstraint[indexOne];
			lstConstraint [indexOne] = lstConstraint [indexTwo];
//			// move element down and shift other elements up
//			if (indexOne < indexTwo)
//			{
//				for (i = indexOne; i < indexTwo; i++)
//				{
//					lstConstraint[i] = lstConstraint[i + 1];
//				}
//			}
//			// move element up and shift other elements down
//			else
//			{
//				for (i = indexOne; i > indexTwo; i--)
//				{
//					lstConstraint[i] = lstConstraint[i - 1];
//				}
//			}
			// put element from position 1 to destination
			lstConstraint[indexTwo] = tmp;

			mMobileCellPosition = indexTwo;
			NotifyDataSetChanged ();
		}

//		public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
//		{
//			// exit if possitions are equal or outside array
//			if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= list.Count) || (0 > newIndex) ||
//				(newIndex >= list.Count)) return;
//			// local variables
//			var i = 0;
//			T tmp = list[oldIndex];
//			// move element down and shift other elements up
//			if (oldIndex < newIndex)
//			{
//				for (i = oldIndex; i < newIndex; i++)
//				{
//					list[i] = list[i + 1];
//				}
//			}
//			// move element up and shift other elements down
//			else
//			{
//				for (i = oldIndex; i > newIndex; i--)
//				{
//					list[i] = list[i - 1];
//				}
//			}
//			// put element from position 1 to destination
//			list[newIndex] = tmp;
//		}

		private void ChangeColorButton(bool clicked, Button btn){
			if (clicked) {
				btn.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#B6F3B7"));
			} else {
				btn.SetBackgroundColor (Android.Graphics.Color.ParseColor ("#d54e21"));
			}
		}
		private void ChangeColorButtonGreen(bool clicked, Button btn){
			if (clicked) {
				btn.SetBackgroundResource (Resource.Drawable.bidvalet_green_border_button);
				btn.SetTextColor (Android.Graphics.Color.ParseColor ("#ffffff"));
			} else {
				btn.SetBackgroundResource (Resource.Drawable.bidvalet_white_border_button);
				btn.SetTextColor (Android.Graphics.Color.ParseColor ("#000000"));
			}
		}
	}

	/*
	public class CellAmPmViewHolder : Java.Lang.Object
	{
		public Button btnAM{ get; set; }
		public Button btnPM{ get; set; }
		public Button btnMix{ get; set; }
		public ImageView btnRemove{ get; set;}

	}

	public class CellCommutableLinesViewHolder : Java.Lang.Object
	{
		public TextView CmutLineTextView { get; set; }
		public Button btnAny{ get; set; }
		public Button btnBoth{ get; set; }
		public Button btnToWork{ get; set; }
		public Button btnToHome{ get; set; }
		public ImageView btnRemove{ get; set;}
	}

	public class CellDayOfMonthViewHolder : Java.Lang.Object
	{
		public TextView daysOfMonthTextView { get; set; }
		public TextView titleDOMTextView { get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellDayWeekAllViewHolder : Java.Lang.Object
	{
		public Button btnSu{ get; set; }
		public Button btnMo{ get; set; }
		public Button btnTu{ get; set; }
		public Button btnWe{ get; set; }
		public Button btnTh{ get; set; }
		public Button btnFr{ get; set; }
		public Button btnSa{ get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellDayWeekSomeViewHolder : Java.Lang.Object
	{
		public TextView tvDay{ get; set; }
		public TextView tvLessMore{ get; set; }
		public TextView tvValue { get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellDHFirstLastViewHolder : Java.Lang.Object
	{
		public TextView tvDH{ get; set; }
		public TextView tvLessMore{ get; set; }
		public TextView tvValue { get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellEquipmentViewHolder : Java.Lang.Object
	{
		public TextView tvEquipment{ get; set; }
		public TextView tvLessMore{ get; set; }
		public TextView tvValue { get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellLineTypeViewHolder : Java.Lang.Object
	{
		public Button btnHard{ get; set; }
		public Button btnRes{ get; set; }
		public Button btnBlank { get; set; }
		public Button btnInt { get; set; }
		public Button btnNonCon { get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellOvernightCitiesViewHolder : Java.Lang.Object
	{
		public TextView cityTextView { get; set; }
		public TextView overnightTextView{ get; set;}
		public ImageView btnRemove{ get; set;}
	}
	public class CellRestViewHolder : Java.Lang.Object
	{
		public TextView tvRest{ get; set; }
		public TextView tvLessMore{ get; set; }
		public TextView tvValue { get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellStartDayOfWeekViewHolder : Java.Lang.Object
	{
		public Button btnSu{ get; set; }
		public Button btnMo{ get; set; }
		public Button btnTu{ get; set; }
		public Button btnWe{ get; set; }
		public Button btnTh{ get; set; }
		public Button btnFr{ get; set; }
		public Button btnSa{ get; set; }
		public ImageView btnRemove{ get; set;}
	}
	public class CellTripBlockLengthViewHolder : Java.Lang.Object
	{
		public Button btnTurn{ get; set; }
		public Button btnTwo{ get; set; }
		public Button btnThree{ get; set; }
		public Button btnFo{ get; set; }
		public Button btnTrip{ get; set; }
		public Button btnBlock{ get; set; }
		public ImageView btnRemove{ get; set;}
	}*/
}

