using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bidvalet.Model;
namespace Bidvalet.Business
{
	public class ConstraintCalculations
	{


		#region Public Methods

		public void ApplyAllConstraints()
		{
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			ClearAllConstraintCalclculation();


			if (wBidStateContent.Constraints.Hard)
			{
				HardConstraintCalclculation(true);
			}
			if (wBidStateContent.Constraints.Ready)
			{
				ReadyReserveConstraintCalculation(true);
			}
			if (wBidStateContent.Constraints.Reserve)
			{
				ReserveConstraintCalculation(true);
			}
			if (wBidStateContent.Constraints.Blank)
			{
				BlankConstraintCalculation(true);
			}
			if (wBidStateContent.Constraints.International)
			{
				InternationalConstraintCalculation(true);
			}
			if (wBidStateContent.Constraints.NonConus)
			{
				Non_ConusConstraintClaculation(true);
			}
            if (wBidStateContent.Constraints.ETOP)
            {
                ETOPSConstraintClaculation(true);
            }

			if (wBidStateContent.CxWtState.AMPMMIX.AM)
			{
				AMPMMixConstraint("AM", true);
			}

			if (wBidStateContent.CxWtState.AMPMMIX.PM)
			{
				AMPMMixConstraint(" PM", true);
			}
			if (wBidStateContent.CxWtState.AMPMMIX.MIX)
			{
				AMPMMixConstraint("Mix", true);
			}

			//            if (wBidStateContent.CxWtState.FaPosition.A)
			//            {
			//                ABCDPositionsConstraint("A", true);
			//            }
			//            if (wBidStateContent.CxWtState.FaPosition.B)
			//            {
			//                ABCDPositionsConstraint("B", true);
			//            }
			//            if (wBidStateContent.CxWtState.FaPosition.C)
			//            {
			//                ABCDPositionsConstraint("C", true);
			//            }
			//            if (wBidStateContent.CxWtState.FaPosition.D)
			//            {
			//                ABCDPositionsConstraint("D", true);
			//            }
			var faposition = wBidStateContent.CxWtState.FaPosition;
			ABCDPositionsConstraint(faposition.A, faposition.B, faposition.C, faposition.D);

			string tripLength = string.Empty;
			tripLength = (wBidStateContent.CxWtState.TripLength.Turns) ? "1," : "";
			tripLength += (wBidStateContent.CxWtState.TripLength.Twoday) ? "2," : "";
			tripLength += (wBidStateContent.CxWtState.TripLength.ThreeDay) ? "3," : "";
			tripLength += (wBidStateContent.CxWtState.TripLength.FourDay) ? "4," : "";

			if (tripLength != string.Empty)
			{
				tripLength = tripLength.Substring(0, tripLength.Length - 1);
				ApplyTripLengthConstraint(tripLength);
			}


			List<int> weekDays = new List<int>();
			if (wBidStateContent.CxWtState.DaysOfWeek.MON)
				weekDays.Add(0);
			if (wBidStateContent.CxWtState.DaysOfWeek.TUE)
				weekDays.Add(1);
			if (wBidStateContent.CxWtState.DaysOfWeek.WED)
				weekDays.Add(2);
			if (wBidStateContent.CxWtState.DaysOfWeek.THU)
				weekDays.Add(3);
			if (wBidStateContent.CxWtState.DaysOfWeek.FRI)
				weekDays.Add(4);
			if (wBidStateContent.CxWtState.DaysOfWeek.SAT)
				weekDays.Add(5);
			if (wBidStateContent.CxWtState.DaysOfWeek.SUN)
				weekDays.Add(6);

			if (weekDays.Count > 0)
			{
				ApplyWeekDayConstraint(weekDays);
			}

			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);

			//Days of Month
			//--------------------------------------------------------------------
			DaysOfMonthCx daysOfMonthCx = wBidStateContent.Constraints.DaysOfMonth;
			List<DateTime> workDays = null;
			List<DateTime> offDays = null;
			if (daysOfMonthCx != null)
			{
				List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();

				DayOfMonth result = null;
				//  if (daysOfMonthCx.Work)
				//  {
				workDays = new List<DateTime>();
				if (daysOfMonthCx.WorkDays != null) {
					foreach (int id in daysOfMonthCx.WorkDays) {
						result = lstDaysOfMonth.FirstOrDefault (x => x.Id == id);
						if (result != null)
							workDays.Add (result.Date);

					}
				}

				//}

				//  if (daysOfMonthCx.Off)
				// {
				offDays = new List<DateTime>();
				if (daysOfMonthCx.OFFDays != null) {
					foreach (int id in daysOfMonthCx.OFFDays) {
						result = lstDaysOfMonth.FirstOrDefault (x => x.Id == id);
						if (result != null)
							offDays.Add (result.Date);

					}
				}
			}
			else
			{
				workDays = new List<DateTime>();
				offDays = new List<DateTime>();
			}
			// }

			//-------------------------------------------------------------------------------



			//Partial Days off --Generate calendar
			//-------------------------------------------------------------------------------
			//List<DateHelper> lstDateHelper = new List<DateHelper>();
			//int dayCount = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
			//DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
			//for (int count = 1; count <= dayCount; count++)
			//{
			//    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
			//    startDate = startDate.AddDays(1);
			//}
			//for (int count = dayCount + 1, i = 1; count <= 34; count++, i++)
			//{
			//    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
			//    startDate = startDate.AddDays(1);
			//}

			List<DateHelper> lstDateHelper = ConstraintBL.GetPartialDayList();
			//-----------------------------------------------------------------------------------



			foreach (Line line in requiredLines)
			{
				//---------------------------------------------------------------------------------------------
				//Aircraft Changes
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.AcftChanges = (wBidStateContent.CxWtState.ACChg.Cx) ? AirCraftChangesConstraint(wBidStateContent.Constraints.AircraftChanges, line) : false;

                ////---------------------------------------------------------------------------------------------
                ////Aircraft Changes
                ////---------------------------------------------------------------------------------------------
                //line.ConstraintPoints.ETOPS = (wBidStateContent.CxWtState.ETOPS.Cx) ? ETOPSConstraintClaculation(wBidStateContent.Constraints.ETOP, line) : false;



				//---------------------------------------------------------------------------------------------
				//BlockOfDaysOffConstraint DaysOff
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.BlkDaysOff = (wBidStateContent.CxWtState.BDO.Cx) ? BlockOfDaysOffConstraint(wBidStateContent.Constraints.BlockOfDaysOff, line) : false;

				//---------------------------------------------------------------------------------------------
				//  Cmut DHs
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.CmtDhds = (wBidStateContent.CxWtState.DHD.Cx) ? CommutableDeadHeadConstraint(wBidStateContent.Constraints.DeadHeads.LstParameter, line) : false;

				//---------------------------------------------------------------------------------------------
				//  Commutable Lines
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.CmtLines = (wBidStateContent.CxWtState.CL.Cx) ? CommutableLineConstraint(wBidStateContent.Constraints.CL, line) : false;


				//---------------------------------------------------------------------------------------------
				//Daysof week
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.DaysOfWeekOff = (wBidStateContent.CxWtState.DOW.Cx) ? DaysofWeekConstraint(wBidStateContent.Constraints.DaysOfWeek.lstParameters, line) : false;


				//---------------------------------------------------------------------------------------------
				//  Days of the Month
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.DaysOfMonthOff = (wBidStateContent.CxWtState.SDO.Cx) ? DaysOfMonthConstraint(workDays, offDays, line) : false;

				//---------------------------------------------------------------------------------------------
				//  DH - first - last
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.Deadhead = (wBidStateContent.CxWtState.DHDFoL.Cx) ? DeadHeadConstraint(wBidStateContent.Constraints.DeadHeadFoL.lstParameters, line) : false;

				//---------------------------------------------------------------------------------------------
				//Dutyperiod  DaysOff
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.DutyPeriod = (wBidStateContent.CxWtState.DP.Cx) ? DutyPeriodConstraint(wBidStateContent.Constraints.DutyPeriod, line) : false;



				//---------------------------------------------------------------------------------------------
				//  Equipment
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.EquipType = (wBidStateContent.CxWtState.EQUIP.Cx) ? EquipmentTypeConstraints(wBidStateContent.Constraints.EQUIP.lstParameters, line) : false;


				//---------------------------------------------------------------------------------------------
				// Flight Time
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.BlockTime = (wBidStateContent.CxWtState.FLTMIN.Cx) ? FlightTimeConstraint(wBidStateContent.Constraints.FlightMin, line) : false;

				//---------------------------------------------------------------------------------------------
				// GroundTime
				//---------------------------------------------------------------------------------------------
				if (!line.ReserveLine)
				{
					line.ConstraintPoints.GrndTime = (wBidStateContent.CxWtState.GRD.Cx) ? GrounTimeConstraint(wBidStateContent.Constraints.GroundTime, line) : false;
				}

				//---------------------------------------------------------------------------------------------
				//International-Nonconus
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.InterNonConus = (wBidStateContent.CxWtState.InterConus.Cx) ? InternationalNonConusConstraint(wBidStateContent.Constraints.InterConus.lstParameters, line) : false;

				//---------------------------------------------------------------------------------------------
				// Legs per dutyperiod
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.LegsPerDutPd = (wBidStateContent.CxWtState.LEGS.Cx) ? LegsPerDutyPeriodConstraint(wBidStateContent.Constraints.LegsPerDutyPeriod, line) : false;



				//---------------------------------------------------------------------------------------------
				// Legs per pairing
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.LegsPerTrip = (wBidStateContent.CxWtState.LegsPerPairing.Cx) ? LegsPerDutyPeriodConstraint(wBidStateContent.Constraints.LegsPerPairing, line) : false;


				//---------------------------------------------------------------------------------------------
				// Number of days Off
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.NumDaysOff = (wBidStateContent.CxWtState.NODO.Cx) ? NumberofDaysOffConstraint(wBidStateContent.Constraints.NumberOfDaysOff, line) : false;


				//---------------------------------------------------------------------------------------------
				// Overnight Cities
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.OvernightCities = (wBidStateContent.CxWtState.RON.Cx) ? OverNightCitiesConstraints(wBidStateContent.Constraints.OverNightCities.lstParameters, line) : false;

				//---------------------------------------------------------------------------------------------
				// Partial days off
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.PartialDaysOff = (wBidStateContent.CxWtState.WtPDOFS.Cx) ? PartialDaysOffConstraint(wBidStateContent.Constraints.PDOFS.LstParameter, line, lstDateHelper) : false;

				//---------------------------------------------------------------------------------------------
				// Start day of week
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.StartDow = (wBidStateContent.CxWtState.SDOW.Cx) ? StartDayOfWeekConstraint(wBidStateContent.Constraints.StartDayOftheWeek.lstParameters, line) : false;
				//---------------------------------------------------------------------------------------------
				// Rest
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.Rest = (wBidStateContent.CxWtState.Rest.Cx) ? RestConstraint(wBidStateContent.Constraints.Rest.lstParameters, line) : false;

				//---------------------------------------------------------------------------------------------
				// Timeaway from base
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.TimeAwayFromBase = (wBidStateContent.CxWtState.PerDiem.Cx) ? TimeAwayFromBaseConstraint(wBidStateContent.Constraints.PerDiem, line) : false;


				//---------------------------------------------------------------------------------------------
				// Trip Length
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.TripLength = (wBidStateContent.CxWtState.TL.Cx) ? TripLengthConstraint(wBidStateContent.Constraints.TripLength.lstParameters, line) : false;


				//---------------------------------------------------------------------------------------------
				// Work Block length
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.WorkBlklength = (wBidStateContent.CxWtState.WB.Cx) ? WorkBlockLengthConstraint(wBidStateContent.Constraints.WorkBlockLength.lstParameters, line) : false;


				//---------------------------------------------------------------------------------------------
				// Work Days
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.WorkDay = (wBidStateContent.CxWtState.WorkDay.Cx) ? WorkDaysConstraint(wBidStateContent.Constraints.WorkDay, line) : false;



				//---------------------------------------------------------------------------------------------
				// Min Pay
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.MinimumPay = (wBidStateContent.CxWtState.MP.Cx) ? MinimumPayConstraint(wBidStateContent.Constraints.MinimumPay, line) : false;


				//---------------------------------------------------------------------------------------------
				// 3 on 3 off
				//---------------------------------------------------------------------------------------------
				line.ConstraintPoints.No3On3off = (wBidStateContent.CxWtState.No3on3off.Cx) ? ThreeOn3offConstraint(wBidStateContent.Constraints.No3On3Off, line) : false;

				//---------------------------------------------------------------------------------------------
				// Overnight City
				//---------------------------------------------------------------------------------------------

				line.ConstraintPoints.OvernightCityBulk = (wBidStateContent.CxWtState.BulkOC.Cx) ? OvernightCitiesBulkCalculation(wBidStateContent.Constraints.BulkOvernightCity, line) : false;

				line.Constrained = line.ConstraintPoints.IsConstraint();




			}

		}



		public void ClearAllConstraintCalclculation()
		{

			foreach (Line line in GlobalSettings.Lines)
			{
				// line.ConstraintPoints.HardConstraint = isEnable;
				line.ConstraintPoints.Reset();
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}



		#region Hard Constraints
		/// <summary>
		/// Hard constraint calculation::Hard: any line that is not a “Reserve” or “Blank” or “Ready” is a hard line.  If this button is selected then all “Hard” lines will be constrained.
		/// </summary>
		public void HardConstraintCalclculation(bool isEnable)
		{
			var hardlines = GlobalSettings.Lines.Where(x => !x.ReserveLine && !x.BlankLine && !x.Pairings.Any(y => y.Substring(1, 1) == "R"));
			foreach (Line line in hardlines)
			{
				line.ConstraintPoints.HardConstraint = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();
			}

		}

		/// <summary>
		/// all Blank lines will be constrained.
		/// </summary>
		public void BlankConstraintCalculation(bool isEnable)
		{
			var blanklines = GlobalSettings.Lines.Where(x => x.BlankLine);
			foreach (Line line in blanklines)
			{
				line.ConstraintPoints.BlankConstraint = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		/// <summary>
		/// all reserve lines will be constrained
		/// </summary>
		public void ReserveConstraintCalculation(bool isEnable)
		{
			var reservelines = GlobalSettings.Lines.Where(x => x.ReserveLine && !x.BlankLine && !x.Pairings.Any(y => y.Substring(1, 1) == "R"));
			foreach (Line line in reservelines)
			{
				line.ConstraintPoints.ReserveConstraint = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}


		/// all ready reserve lines will be constrained
		/// </summary>
		public void ReadyReserveConstraintCalculation(bool isEnable)
		{
			var readyReservelines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.Pairings.Any(y => y.Substring(1, 1) == "R"));
			foreach (Line line in readyReservelines)
			{
				line.ConstraintPoints.ReadyReserveConstraint = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		/// <summary>
		/// all the international lines will be constrained
		/// </summary>
		public void InternationalConstraintCalculation(bool isEnable)
		{
			var internationallines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.International);
			foreach (Line line in internationallines)
			{
				line.ConstraintPoints.InternationalConstraint = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		/// <summary>
		/// All the non-conus lines will be constrained
		/// </summary>
		public void Non_ConusConstraintClaculation(bool isEnable)
		{

			var nonConuslines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.NonConus);
			foreach (Line line in nonConuslines)
			{
				line.ConstraintPoints.NonConusConstraint = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
        /// <summary>
        /// All the non-conus lines will be constrained
        /// </summary>
        public void ETOPSConstraintClaculation(bool isEnable)
        {

            var etops = GlobalSettings.Lines.Where(x => !x.BlankLine && x.ETOPS);
            foreach (Line line in etops)
            {
                line.ConstraintPoints.ETOPS = isEnable;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }


		/// <summary>
		/// Am Pm Mix constraints
		/// </summary>
		/// <param name="type"></param>
		/// <param name="isEnable"></param>
		public void AMPMMixConstraint(string type, bool isEnable)
		{

			var aMPMMixLines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.AMPM == type);
			foreach (Line line in aMPMMixLines)
			{
				line.ConstraintPoints.AmPmNte = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}



		}
		/// <summary>
		/// AbCD position constraints
		/// </summary>
		/// <param name="type"></param>
		/// <param name="isEnable"></param>
		public void ABCDPositionsConstraint( bool posA, bool posB, bool posC, bool posD)
		{
			List<string> positions=new List<string>();
			List<string> Linepositions=new List<string>();
			if(posA)
				positions.Add("A");
			if(posB)
				positions.Add("B");
			if(posC)
				positions.Add("C");
			if(posD)
				positions.Add("D");
			var requiredlines=GlobalSettings.Lines.Where(x=>!x.BlankLine);
			foreach (Line line in requiredlines)
			{
				if (GlobalSettings.CurrentBidDetails.Round == "M")
					Linepositions = line.FAPositions;
				else
					//Linepositions =line.FASecondRoundPositions.Values.ToList().Select(y => y.Substring(y.Length - 1, 1)).ToList();
					Linepositions = line.FASecondRoundPositions.ToList().Select(y => y.Value).ToList();// need to check this. Commented above code bcz we changed FA position line object property

				bool isPositionExist=Linepositions.Any(x=>positions.Any(y=>y==x));
				if (isPositionExist)
				{
					line.ConstraintPoints.Positions = true;
					line.Constrained = line.ConstraintPoints.IsConstraint();
				}
				else
				{
					line.ConstraintPoints.Positions = false;
					line.Constrained = line.ConstraintPoints.IsConstraint();
				}

			}

		}
		/// <summary>
		/// AbCD position constraints
		/// </summary>
		/// <param name="type"></param>
		/// <param name="isEnable"></param>
		public void ABCDPositionsConstraint(string type, bool isEnable)
		{

			var aBCDLines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.FAPositions.Any(y => y == type));
			foreach (Line line in aBCDLines)
			{
				line.ConstraintPoints.Positions = isEnable;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}



		}

		/// <summary>
		/// Trip Length constraint
		/// </summary>
		/// <param name="tripDays"></param>
		public void ApplyTripLengthConstraint(string tripDays)
		{

			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{

				TripLengthConstraint(tripDays, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();
			}



		}

		/// <summary>
		/// Calculatae Sunday,Monday,Tuesday,..Saturday button constraint--Placed in the top 4 line of constraint section
		/// Monday 0,tuesday 1...Sunday 6
		/// </summary>
		/// <param name="days"></param>
		public void ApplyWeekDayConstraint(List<int> days)
		{


			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				WeekDayConstraint(days, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		#endregion

		#region AirCraft Changes

		/// <summary>
		/// Aircraft Changes constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyAirCraftChangesConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.AcftChanges = AirCraftChangesConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}



		}

		/// <summary>
		/// Remove Aircraft changes constraints
		/// </summary>
		public void RemoveAirCraftChangesConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.AcftChanges);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.AcftChanges = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}



		}
		/// <summary>
		/// Remove Commutable line constraints
		/// </summary>
		public void RemoveCommutableLinesConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.CmtLines);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.CmtLines = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}



		}
		#endregion

		#region Block Of Days Off

		/// <summary>
		/// Block of days off constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyBlockOfDaysOffConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.BlkDaysOff = BlockOfDaysOffConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
		}

		/// <summary>
		/// Remove Block Of DaysOff constraints
		/// </summary>
		public void RemoveBlockOfDaysOffConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.BlkDaysOff);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.BlkDaysOff = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Commutable deadHead

		/// <summary>
		/// Commuttable dead head
		/// </summary>
		/// <param name="lstCx4Parameter"></param>
		public void ApplyCommutableDeadHeadConstraint(List<Cx4Parameter> lstCx4Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.CmtDhds = CommutableDeadHeadConstraint(lstCx4Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		#endregion

		#region Commutable lines

		/// <summary>
		/// Commutable lines
		/// </summary>
		/// <param name="CxCommutableLine"></param>
		public void ApplyCommutableLinesConstraint(CxCommutableLine CxCommutableLine)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.CmtLines = CommutableLineConstraint(CxCommutableLine, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
		}

		#endregion

		#region Days of Week
		public void ApplyDaysofWeekConstraint(List<Cx3Parameter> lstcx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.DaysOfWeekOff = DaysofWeekConstraint(lstcx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();
			}
		}

		public void RemoveDaysofWeekConstraint()
		{

			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.DaysOfWeekOff);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.DaysOfWeekOff = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Days Of Month

		/// <summary>
		/// Days Of Month Constraint
		/// </summary>
		/// <param name="daysOfMonthCx"></param>
		public void ApplyDaysOfMonthConstraint(DaysOfMonthCx daysOfMonthCx)
		{

			List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();
			List<DateTime> workDays = null;
			List<DateTime> offDays = null;
			DayOfMonth result = null;
			//  if (daysOfMonthCx.Work)
			// {
			workDays = new List<DateTime>();
			foreach (int id in daysOfMonthCx.WorkDays)
			{
				result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
				if (result != null)
					workDays.Add(result.Date);

			}
			// }

			// if (daysOfMonthCx.Off)
			//{
			offDays = new List<DateTime>();
			foreach (int id in daysOfMonthCx.OFFDays)
			{
				result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
				if (result != null)
					offDays.Add(result.Date);

			}
			//}

			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.DaysOfMonthOff = DaysOfMonthConstraint(workDays, offDays, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveDaysOfMonthConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.DaysOfMonthOff);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.DaysOfMonthOff = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
		}
		#endregion

		#region DeadHead
		public void ApplyDeadHeadConstraint(List<Cx3Parameter> lstCx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.Deadhead = DeadHeadConstraint(lstCx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Duty Period
		/// <summary>
		/// Block of days off constraint
		/// </summary>
		/// <param name="constraintParams"></param>
		public void ApplyDutyPeriodConstraint(Cx2Parameter Cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.DutyPeriod = DutyPeriodConstraint(Cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveDutyPeriodConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.DutyPeriod);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.DutyPeriod = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
		}
		#endregion

		#region Equip Type
		public void ApplyEquipmentTypeConstraint(List<Cx3Parameter> lstCx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.EquipType = EquipmentTypeConstraints(lstCx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveEquipmentTypeConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.EquipType);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.EquipType = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
		}
		#endregion

		#region Flight Time

		/// <summary>
		/// Flight Time constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyFlightTimeConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.BlockTime = FlightTimeConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
		}

		public void RemoveFlightTimeConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.BlockTime);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.BlockTime = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		#endregion

		#region Ground Time
		public void ApplyGroundTimeConstraint(Cx3Parameter cx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine && !x.ReserveLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.GrndTime = GrounTimeConstraint(cx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
		}

		public void RemoveGroundTimeConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.GrndTime);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.GrndTime = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region International NonConus

		public void ApplyInternationalonConusConstraint(List<Cx2Parameter> lstCx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.InterNonConus = InternationalNonConusConstraint(lstCx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Legs Per DutyPeriod

		/// <summary>
		/// Legs per Duty period constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyLegsPerDutyPeriodConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.LegsPerDutPd = LegsPerDutyPeriodConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}


		}

		public void RemoveLegsPerDutyPeriodConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.LegsPerDutPd);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.LegsPerDutPd = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Legs Per Pairing

		/// <summary>
		/// Legs per Pairing constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyLegsPerPairingConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.LegsPerTrip = LegsPerPairingConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}


		}

		public void RemoveLegsPerPairingConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.LegsPerTrip);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.LegsPerTrip = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Number of Days Off
		/// <summary>
		/// Number of Days off Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyNumberofDaysOffConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.NumDaysOff = NumberofDaysOffConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveNumberofDaysOffConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.NumDaysOff);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.NumDaysOff = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		#endregion

		#region Overnight Cities

		public void ApplyOvernightCitiesConstraint(List<Cx3Parameter> lstCx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.OvernightCities = OverNightCitiesConstraints(lstCx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveOvernightCitiesConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.OvernightCities);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.OvernightCities = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		#endregion

		#region Partial days Off

		public void ApplyPartialdaysOffConstraint(List<Cx4Parameter> lstCx4Parameter)
		{

			//Generate calendar
			//----------------------------------------
			//List<DateHelper> lstDateHelper = new List<DateHelper>();
			//int dayCount = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
			//DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
			//for (int count = 1; count <= dayCount; count++)
			//{
			//    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
			//    startDate = startDate.AddDays(1);
			//}
			//for (int count = dayCount + 1, i = 1; count <= 34; count++, i++)
			//{
			//    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
			//    startDate = startDate.AddDays(1);
			//}

			List<DateHelper> lstDateHelper = ConstraintBL.GetPartialDayList();
			//----------------------------------------
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.PartialDaysOff = PartialDaysOffConstraint(lstCx4Parameter, line, lstDateHelper);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemovePartialdaysOffConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.PartialDaysOff);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.PartialDaysOff = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		#endregion

		#region Start day of week

		public void ApplyStartDayOfWeekConstraint(List<Cx3Parameter> lstCx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.StartDow = StartDayOfWeekConstraint(lstCx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveStartDayOfWeekConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.StartDow);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.StartDow = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Rest

		public void ApplyRestConstraint(List<Cx3Parameter> lstCx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.Rest = RestConstraint(lstCx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveRestConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.Rest);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.Rest = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region TimeAwayFromBaseConstraint

		/// <summary>
		/// Time Away From Base
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyTimeAwayFromBaseConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.TimeAwayFromBase = TimeAwayFromBaseConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveTimeAwayFromBaseConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.TimeAwayFromBase);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.TimeAwayFromBase = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region TripLength

		/// <summary>
		/// Trip Length
		/// </summary>
		/// <param name="lstCx3Parameter"></param>
		public void ApplyTripLengthConstraint(List<Cx3Parameter> lstCx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.TripLength = TripLengthConstraint(lstCx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveTripLengthConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.TripLength);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.TripLength = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Work Block Length
		/// <summary>
		/// Work Block length constraint
		/// </summary>
		/// <param name="lstCx3Parameter"></param>
		public void ApplyWorkBlockLengthConstraint(List<Cx3Parameter> lstCx3Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.WorkBlklength = WorkBlockLengthConstraint(lstCx3Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		public void RemoveWorkBlockLengthConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.WorkBlklength);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.WorkBlklength = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region WorkDay Constraint
		/// <summary>
		/// Work Days Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyWorkDaysConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.WorkDay = WorkDaysConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}



		}

		/// <summary>
		/// Remove Work Day constraint
		/// </summary>
		public void RemoveWorkDayConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.WorkDay);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.WorkDay = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region MinimumPay

		/// <summary>
		/// Miniimum pay  Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyMinimumPayConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.MinimumPay = MinimumPayConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		/// <summary>
		/// Miniimum pay  Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		public void ApplyOvernightBulkConstraint(BulkOvernightCityCx BulkOvernightCityCx)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			//if (BulkOvernightCityCx.OverNightYes.Count != 0 || BulkOvernightCityCx.OverNightNo.Count != 0)
			//{
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.OvernightCityBulk = OvernightCitiesBulkCalculation(BulkOvernightCityCx, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}
			// }




		}
		public void RemoveMinimumPayConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.MinimumPay);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.MinimumPay = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}
		#endregion

		#region Three On 3 off
		public void ApplyThreeOn3offConstraint(Cx2Parameter cx2Parameter)
		{
			var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.No3On3off = ThreeOn3offConstraint(cx2Parameter, line);
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}


		public void RemoveThreeOn3offConstraint()
		{
			var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.No3On3off);
			foreach (Line line in requiredLines)
			{
				line.ConstraintPoints.No3On3off = false;
				line.Constrained = line.ConstraintPoints.IsConstraint();

			}

		}

		#endregion

		#region Display line details

		public string LinesNotConstrained()
		{
			return GlobalSettings.Lines.Where(x => !x.Constrained).Count().ToString() + " of " + GlobalSettings.Lines.Count();
		}
		#endregion

		#region Clear Constraints
		/// <summary>
		/// PURPOSE Clear Constraints
		/// </summary>
		public void ClearConstraints()
		{
			List<int> lstOff = new List<int>() { };

			List<int> lstWork = new List<int>() { };
			//var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in GlobalSettings.Lines)
			{
				line.ConstraintPoints.Reset();
				line.Constrained = false;

			}

			WBidState currentState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			CxWtState states = currentState.CxWtState;

			currentState.Constraints.Hard = false;
			currentState.Constraints.Ready = false;
			currentState.Constraints.Reserve = false;
			currentState.Constraints.Blank = false;
			currentState.Constraints.International = false;
			currentState.Constraints.NonConus = false;

            currentState.Constraints.ETOP = false;

			currentState.CxWtState.AMPMMIX.AM = false;
			currentState.CxWtState.AMPMMIX.PM = false;
			currentState.CxWtState.AMPMMIX.MIX = false;

			currentState.CxWtState.FaPosition.A = false;
			currentState.CxWtState.FaPosition.B = false;
			currentState.CxWtState.FaPosition.C = false;
			currentState.CxWtState.FaPosition.D = false;

			currentState.CxWtState.TripLength.Turns = false;
			currentState.CxWtState.TripLength.Twoday = false;
			currentState.CxWtState.TripLength.ThreeDay = false;
			currentState.CxWtState.TripLength.FourDay = false;

			currentState.CxWtState.DaysOfWeek.MON = false;
			currentState.CxWtState.DaysOfWeek.TUE = false;
			currentState.CxWtState.DaysOfWeek.WED = false;
			currentState.CxWtState.DaysOfWeek.THU = false;
			currentState.CxWtState.DaysOfWeek.FRI = false;
			currentState.CxWtState.DaysOfWeek.SAT = false;
			currentState.CxWtState.DaysOfWeek.SUN = false;



			states.ACChg.Cx = false;
			//states.AMPM.Cx = false;
			states.BDO.Cx = false;
			states.CL.Cx = false;
			states.DHD.Cx = false;
			states.DHDFoL.Cx = false;
			states.DOW.Cx = false;
			states.DP.Cx = false;
			states.EQUIP.Cx = false;
			states.FLTMIN.Cx = false;
			states.GRD.Cx = false;
			states.InterConus.Cx = false;
			states.LEGS.Cx = false;
			states.LegsPerPairing.Cx = false;
			states.MP.Cx = false;
			states.No3on3off.Cx = false;
			states.NODO.Cx = false;
			states.NOL.Cx = false;
			states.PerDiem.Cx = false;
			states.Rest.Cx = false;
			states.RON.Cx = false;
			states.SDO.Cx = false;
			states.SDOW.Cx = false;
			states.TL.Cx = false;
			states.WB.Cx = false;
			states.WtPDOFS.Cx = false;
			states.LrgBlkDaysOff.Cx = false;
			states.Position.Cx = false;
			states.WorkDay.Cx = false;
			states.BulkOC.Cx = false;
			// states.InterCon.Cx = false;
			//Update the state object


			Constraints constraint = new Constraints()
			{
				Hard = false,
				Ready = false,
				Reserve = false,
				International = false,
				NonConus = false,
                ETOP=false,
				// AM_PM = new AMPMConstriants{AM=false,PM=false,MIX=false},
				LrgBlkDayOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 10 },
				AircraftChanges = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
				BlockOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },
				DeadHeads = new Cx4Parameters { SecondcellValue = "1", ThirdcellValue = ((int)DeadheadType.First).ToString(), Type = (int)ConstraintType.LessThan, Value = 1,LstParameter=new List<Cx4Parameter>() },
				CL = new CxCommutableLine()
				{
					AnyNight = true,
					RunBoth = false,
					CommuteToHome = true,
					CommuteToWork = true,
					MondayThu = new Times() { Checkin = 0, BackToBase = 0 },
					MondayThuDefault = new Times() { Checkin = 0, BackToBase = 0 },
					Friday = new Times() { Checkin = 0, BackToBase = 0 },
					FridayDefault = new Times() { Checkin = 0, BackToBase = 0 },
					Saturday = new Times() { Checkin = 0, BackToBase = 0 },
					SaturdayDefault = new Times() { Checkin = 0, BackToBase = 0 },
					Sunday = new Times() { Checkin = 0, BackToBase = 0 },
					SundayDefault = new Times() { Checkin = 0, BackToBase = 0 },
					TimesList = new List<Times>()

				},

				DaysOfMonth = new DaysOfMonthCx() { OFFDays = lstOff, WorkDays = lstWork },
				DaysOfWeek = new Cx3Parameters() { ThirdcellValue = ((int)Dow.Tue).ToString(), Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				DeadHeadFoL = new Cx3Parameters { ThirdcellValue = ((int)DeadheadType.First).ToString(), Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				DutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 600 },

				EQUIP = new Cx3Parameters { ThirdcellValue = "500", Type = (int)ConstraintType.MoreThan, Value = 0, lstParameters = new List<Cx3Parameter>() },
				FlightMin = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 7200 },
				GroundTime = new Cx3Parameter { Type = (int)ConstraintType.MoreThan, Value = 1, ThirdcellValue = "30" },
				InterConus = new Cx2Parameters() { Type = (int)CityType.International, Value = 1, lstParameters = new List<Cx2Parameter>() },
				LegsPerDutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
				LegsPerPairing = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18 },
				NumberOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },

				OverNightCities = new Cx3Parameters { ThirdcellValue = "6", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				BulkOvernightCity = new BulkOvernightCityCx { OverNightNo = new List<int>(), OverNightYes = new List<int>() },
				PDOFS = new Cx4Parameters { SecondcellValue = "1", ThirdcellValue = "1", Type = (int)ConstraintType.atafter, Value = 915, LstParameter = new List<Cx4Parameter>() },
				Position = new Cx3Parameters { Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				StartDayOftheWeek = new Cx3Parameters { ThirdcellValue = "6", Type = (int)ConstraintType.MoreThan, Value = 3 ,lstParameters=new List<Cx3Parameter>()},
				Rest = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 8, lstParameters = new List<Cx3Parameter>() },
				PerDiem = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18000 },
				TripLength = new Cx3Parameters { ThirdcellValue = "4", Type = (int)ConstraintType.MoreThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				WorkBlockLength = new Cx3Parameters { ThirdcellValue = "4", Type = (int)ConstraintType.LessThan, Value = 2, lstParameters = new List<Cx3Parameter>() },
				MinimumPay = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 90 },
				No3On3Off = new Cx2Parameter { Type = (int)ThreeOnThreeOff.ThreeOnThreeOff, Value = 10 },
				WorkDay = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 11 },





			};

			currentState.Constraints = constraint;

		}

		#endregion

		#endregion

		#region Private Methods

		#region Hard Constraints

		/// <summary>
		/// Trip length constraint
		/// </summary>
		/// <param name="tripDays"></param>
		/// <param name="line"></param>
		private void TripLengthConstraint(string tripDays, Line line)
		{
			bool status = false;


			foreach (string str in tripDays.Split(','))
			{
				switch (str)
				{
				case "1":
					if (line.Trips1Day > 0)
					{
						status = true;
					}
					break;

				case "2":
					if (line.Trips2Day > 0)
					{
						status = true;
					}
					break;

				case "3":
					if (line.Trips3Day > 0)
					{
						status = true;
					}
					break;
				case "4":
					if (line.Trips4Day > 0)
					{
						status = true;
					}
					break;



				}
				if (status)
					break;
			}

			line.ConstraintPoints.TripLengthHard = status;


		}

		/// <summary>
		/// Single Week day  constraint
		/// </summary>
		/// <param name="days"></param>
		/// <param name="line"></param>
		private void WeekDayConstraint(List<int> days, Line line)
		{
			bool status = false;


			foreach (int day in days)
			{
				if (line.DaysOfWeekWork[day] != 0)
				{
					status = true;
					break;
				}
			}

			line.ConstraintPoints.WeekDayConstraint = status;


		}

		#endregion


		/// <summary>
		/// Single line Aircraft changes
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool AirCraftChangesConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			bool status = false;
			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (line.AcftChanges < cx2Parameter.Value);

			}
			if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
			{
				status = (line.AcftChanges == cx2Parameter.Value);

			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (line.AcftChanges > cx2Parameter.Value);
			}



			return status;

		}

		/// <summary>
		/// Block Of Days Off Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool BlockOfDaysOffConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			//if (line.LineNum == 3)
			//{
			//}
			//Type--Less than,equal to,More Than
			//Value= Number of days off
			bool status = false;
			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{

				if ((line.BlkOfDaysOff.Take(cx2Parameter.Value).Where(x => x != 0).Count() > 0) && (line.BlkOfDaysOff.Skip(cx2Parameter.Value).Where(x => x != 0).Count() == 0))
				{
					status = true;
				}

			}
			else if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
			{

				//if (line.BlkOfDaysOff[cx2Parameter.Value] != 0 && line.BlkOfDaysOff.Where(x => x != 0).Count() == 0)
				if (line.BlkOfDaysOff[cx2Parameter.Value] != 0)
				{
					status = true;

				}
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				//if ((line.BlkOfDaysOff.Take(cx2Parameter.Value+1).Where(x => x != 0).Count() == 0) && (line.BlkOfDaysOff.Skip(cx2Parameter.Value).Where(x => x != 0).Count() > 0))
				//{
				//    status = true;
				//}
				for (int count = cx2Parameter.Value + 1; count < line.BlkOfDaysOff.Count(); count++)
				{
					if (line.BlkOfDaysOff[count] != 0)
					{
						status = true;
						break;

					}
				}
			}

			return status;

		}

		/// <summary>
		/// Commutable Deadhead constraint
		/// </summary>
		/// <param name="lstCx4parameter"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		private bool CommutableDeadHeadConstraint(List<Cx4Parameter> lstCx4parameter, Line line)
		{   //SecondcellValue- CityId
			//ThirdcellValue -- Both,Begin,End
			//value          --Passes 

			bool status = false;

			if (lstCx4parameter.Count == 0)
				return status;

			foreach (Cx4Parameter cx4Parameter in lstCx4parameter)
			{

				//Both Ends
				if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.Both))
				{
					if (cx4Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom < cx4Parameter.Value) && (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo < cx4Parameter.Value));
					}
					else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom > cx4Parameter.Value) && (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo > cx4Parameter.Value));

					}
				}
				//Either
				else if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.Either))
				{
					if (cx4Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom < cx4Parameter.Value) || (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo < cx4Parameter.Value));
					}
					else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom > cx4Parameter.Value) || (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo > cx4Parameter.Value));

					}
				}
				//Begining Only
				else if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.First))
				{

					if (cx4Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo < cx4Parameter.Value);
					}
					else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo > cx4Parameter.Value);
					}

				}
				//end Only
				else if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.Last))
				{
					if (cx4Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom < cx4Parameter.Value);
					}
					else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom > cx4Parameter.Value);
					}
				}

				if (!status)
					break;
			}


			return !status;


		}

		/// <summary>
		/// Commutable Line Constraint
		/// </summary>
		/// <param name="cxCommutableLine"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		public bool CommutableLineConstraint(CxCommutableLine cxCommutableLine, Line line)
		{
			bool status = false;
			// CxCommutableLine cxDeadHead = GlobalSettings.WBidStateContent.Constraints.CL;

			bool isCommuteFrontEnd = false;
			bool isCommuteBackEnd = false;





			foreach (WorkBlockDetails workBlock in line.WorkBlockList)
			{

				int checkIntime = 0;
				int backToBaseTime = 0;

				switch (workBlock.StartDay)
				{
				//Monday--Thurs
				case 1:
				case 2:
				case 3:
				case 4:
					checkIntime = cxCommutableLine.MondayThu.Checkin;
					backToBaseTime = cxCommutableLine.MondayThu.BackToBase;
					break;
					//Friday
				case 5:
					checkIntime = cxCommutableLine.Friday.Checkin;
					backToBaseTime = cxCommutableLine.Friday.BackToBase;
					break;
					// saturday
				case 6:
					checkIntime = cxCommutableLine.Saturday.Checkin;
					backToBaseTime = cxCommutableLine.Saturday.BackToBase;
					break;
					//sunday
				case 0:
					checkIntime = cxCommutableLine.Sunday.Checkin;
					backToBaseTime = cxCommutableLine.Sunday.BackToBase;
					break;

				}


				isCommuteFrontEnd = checkIntime > workBlock.StartTime;

				isCommuteBackEnd = backToBaseTime < workBlock.EndTime;

				if (cxCommutableLine.RunBoth)
				{

					if (isCommuteFrontEnd || isCommuteBackEnd)
					{
						status = true;
						break;
					}
				}
				else if (cxCommutableLine.AnyNight)
				{

					if (cxCommutableLine.CommuteToHome && cxCommutableLine.CommuteToWork)
					{
						if (isCommuteFrontEnd || isCommuteBackEnd || workBlock.BackToBackCount > 0)
						{
							status = true;
							break;
						}
						//else if (cxCommutableLine.CommuteToHome && (isCommuteBackEnd || workBlock.BackToBackCount > 0))
						//{
						//    status = true;
						//    break;
						//}

						//else if (cxCommutableLine.CommuteToWork && (isCommuteFrontEnd || workBlock.BackToBackCount > 0))
						//{
						//    status = true;
						//    break;
						//}

					}
					else if (cxCommutableLine.CommuteToHome && (isCommuteBackEnd || workBlock.BackToBackCount > 0))
					{
						status = true;
						break;
					}

					else if (cxCommutableLine.CommuteToWork && (isCommuteFrontEnd || workBlock.BackToBackCount > 0))
					{
						status = true;
						break;
					}


				}

			}


			return status;
			//line.ConstraintPoints.CmtLines = status;

		}

		/// <summary>
		/// days of week constraint calculation
		/// </summary>
		/// <param name="lstcx3Parameter"></param>
		/// <param name="line"></param>
		private bool DaysofWeekConstraint(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{
				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					if (line.DaysOfWeekWork[Convert.ToInt32(cx3Parameter.ThirdcellValue)] < cx3Parameter.Value)
						status = true;
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					if (line.DaysOfWeekWork[Convert.ToInt32(cx3Parameter.ThirdcellValue)] > cx3Parameter.Value)
						status = true;
				}
				if (status)
					break;
			}

			return status;

		}


		/// <summary>
		/// Days of month 
		/// </summary>
		/// <param name="workingDays"></param>
		/// <param name="offDays"></param>
		/// <param name="line"></param>
		private bool DaysOfMonthConstraint(List<DateTime> workingDays, List<DateTime> offDays, Line line)
		{

			bool status = false;

			if (workingDays.Count > 0)
			{
				foreach (DateTime dateTime in workingDays)
				{
					var dayObject = line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == dateTime && x.Working);
					status = (dayObject == null);

					if (status)
						break;
				}
			}

			if (!status && offDays.Count > 0)
			{
				foreach (DateTime dateTime in offDays)
				{
					var dayObject = line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == dateTime);
					status = (dayObject != null);

					if (status)
						break;
				}

			}

			return status;



		}


		/// <summary>
		/// PURPOSE:calculate deadhead constraint.
		/// </summary>
		/// <param name="line"></param>
		private bool DeadHeadConstraint(List<Cx3Parameter> lstcx3Parameter, Line line)
		{      //ThirdcellValue --Both,First,Last
			//Type= Less than,More than


			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{
				if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.First).ToString())
				{
					//if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					//{
					//    if (line.DhFirstTotal > cx3Parameter.Value)
					//        status = true;
					//}

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhFirstTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhFirstTotal > cx3Parameter.Value)
							status = true;
					}
				}
				else if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.Last).ToString())
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhLastTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhLastTotal > cx3Parameter.Value)
							status = true;
					}
				}
				else if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.Both).ToString())
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhLastTotal < cx3Parameter.Value || line.DhFirstTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhLastTotal > cx3Parameter.Value || line.DhFirstTotal > cx3Parameter.Value)
							status = true;
					}
				}

				if (status)
					break;
			}

			return status;

		}


		/// <summary>
		/// Dutyperiod Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool DutyPeriodConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			//retrive the current constrain values from global settings
			// Cx2Parameter cxDutyPeriod = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.DutyPeriod;
			bool status = false;

			//only want duty period longer than length
			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = line.DutyPeriodHours.Any(x => x < cx2Parameter.Value);
			}

			else if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
			{
				status = line.DutyPeriodHours.Any(x => x == cx2Parameter.Value);
			}
			//only want duty period shorter than length
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = line.DutyPeriodHours.Any(x => x > cx2Parameter.Value);
			}
			return status;
		}


		/// <summary>
		/// Equipment Type constraint Calcaultion
		/// </summary>
		/// <param name="lstcx3Parameter"></param>
		/// <param name="line"></param>
		private bool EquipmentTypeConstraints(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{

				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					//if (cx3Parameter.ThirdcellValue == "300")
					//{
					//	if (line.LegsIn300 < cx3Parameter.Value)
					//		status = true;
					//}
					//else if (cx3Parameter.ThirdcellValue == "500")
					//{
					//	if (line.LegsIn500 < cx3Parameter.Value)
					//		status = true;
					//}
					 if (cx3Parameter.ThirdcellValue == "700")
					{
						if (line.LegsIn700 < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "800")
					{
						if (line.LegsIn800 < cx3Parameter.Value)
							status = true;

					}
					else if (cx3Parameter.ThirdcellValue == "600")
					{
						if (line.LegsIn600 < cx3Parameter.Value)
							status = true;

					}
					if (status)
						break;
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					//if (cx3Parameter.ThirdcellValue == "300")
					//{
					//	if (line.LegsIn300 > cx3Parameter.Value)
					//		status = true;
					//}
					//else if (cx3Parameter.ThirdcellValue == "500")
					//{
					//	if (line.LegsIn500 > cx3Parameter.Value)
					//		status = true;
					//}
				    if (cx3Parameter.ThirdcellValue == "700")
					{
						if (line.LegsIn700 > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "800")
					{
						if (line.LegsIn800 > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "600")
					{
						if (line.LegsIn600 > cx3Parameter.Value)
							status = true;
					}
					if (status)
						break;
				}
			}
			return status;
		}

		/// <summary>
		/// Flight Time Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool FlightTimeConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			bool status = false;

			int blkHrsInBp = Helper.ConvertHHMMtoMinute(line.BlkHrsInBp);

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (blkHrsInBp < cx2Parameter.Value);

			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (blkHrsInBp > cx2Parameter.Value);
			}


			return status;

		}

		private bool GrounTimeConstraint(Cx3Parameter cx3Parameter, Line line)
		{

			bool status;

			int count = 0;

			if (cx3Parameter.Type == (int)ConstraintType.LessThan)
			{
				count = line.GroundTimes.Count(x => x < int.Parse(cx3Parameter.ThirdcellValue));
			}
			else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
			{
				count = line.GroundTimes.Count(x => x > int.Parse(cx3Parameter.ThirdcellValue));
			}
			status = (count >= cx3Parameter.Value);

			return status;


		}


		private bool InternationalNonConusConstraint(List<Cx2Parameter> lstCx2Parameter, Line line)
		{

			bool status = false;
			List<string> internationalCities = null;
			List<string> nonConusCities = null;
			Trip trip = null;
			List<string> arrivalCities = new List<string>();
			foreach (string pairing in line.Pairings)
			{
				trip = GetTrip(pairing);
				if (trip != null)
				{
					arrivalCities.AddRange(trip.DutyPeriods.SelectMany(x => x.Flights.Select(y => y.ArrSta)).ToList());

				}

			}

			foreach (Cx2Parameter cx2Parameter in lstCx2Parameter)
			{
				//International 
				if (cx2Parameter.Type == (int)CityType.International)
				{
					internationalCities = new List<string>();
					//All cities
					if (cx2Parameter.Value == 0)
					{
						internationalCities = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).Select(y => y.Name).ToList();

					}
					else
					{
						var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.International && x.Id == cx2Parameter.Value);
						if (city != null)
							internationalCities.Add(city.Name);


					}
					status = arrivalCities.Intersect(internationalCities).Any();

				}

				//Non-Conus
				else if (cx2Parameter.Type == (int)CityType.NonConus)
				{
					nonConusCities = new List<string>();
					if (cx2Parameter.Value == 0)
					{
						nonConusCities = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).Select(y => y.Name).ToList();

					}
					else
					{
						var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.NonConus && x.Id == cx2Parameter.Value);
						if (city != null)
							nonConusCities.Add(city.Name);


					}
					status = arrivalCities.Intersect(nonConusCities).Any();

				}

				if (status)
					break;

			}


			return status;
			// line.ConstraintPoints.InterNonConus = status;


		}

		/// <summary>
		/// Legs per duty period constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool LegsPerDutyPeriodConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				for (int count = 1; count < cx2Parameter.Value; count++)
				{
					status = (line.LegsPerDutyPeriod[count] != 0);
					if (status)
						break;

				}
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				for (int count = cx2Parameter.Value + 1; count < 10; count++)
				{
					status = (line.LegsPerDutyPeriod[count] != 0);
					if (status)
						break;

				}
			}
			return status;


		}

		/// <summary>
		/// Legs Per Pairing
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool LegsPerPairingConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			bool status = false;


			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (line.MostLegs < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (line.MostLegs > cx2Parameter.Value);
			}

			return status;
		}


		private bool NumberofDaysOffConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (line.DaysOff < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (line.DaysOff > cx2Parameter.Value);
			}


			return status;
		}

		/// <summary>
		///  Overnight Cities constraint calcualtion
		/// </summary>
		/// <param name="lstcx3Parameter"></param>
		/// <param name="line"></param>
		private bool OverNightCitiesConstraints(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{

				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					// if (line.OvernightCities.Where(x => x == cx3Parameter.ThirdcellValue).Count() > cx3Parameter.Value)

					if (line.OvernightCities.Where(y => y.ToString() == GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == int.Parse(cx3Parameter.ThirdcellValue)).Name).Count() < cx3Parameter.Value)
						status = true;
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					// if (line.OvernightCities.Where(x => x == cx3Parameter.ThirdcellValue).Count() <= cx3Parameter.Value)
					if (line.OvernightCities.Where(y => y.ToString() == GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == int.Parse(cx3Parameter.ThirdcellValue)).Name).Count() > cx3Parameter.Value)
						status = true;

				}
				if (status)
					break;
			}
			return status;
		}

		/// <summary>
		/// Partial DaysOff Constraint
		/// </summary>
		/// <param name="lstCx4Parameter"></param>
		/// <param name="line"></param>
		/// <param name="lstDateHelper"></param>
		/// <returns></returns>
		private bool PartialDaysOffConstraint(List<Cx4Parameter> lstCx4Parameter, Line line, List<DateHelper> lstDateHelper)
		{
			bool status = false;
			List<City> citylist = GlobalSettings.WBidINIContent.Cities;
			DateTime date = new DateTime();


			Day newDay = null;
			foreach (Cx4Parameter cx4Parameter in lstCx4Parameter)
			{
				City city = citylist.FirstOrDefault(x => x.Id == int.Parse(cx4Parameter.ThirdcellValue));
				if (city != null)
				{

					date = lstDateHelper.FirstOrDefault(x => x.DateId == int.Parse(cx4Parameter.SecondcellValue)).Date;

					if (cx4Parameter.Type == (int)ConstraintType.atbefore)
					{
						newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true && x.Date == date && x.DepartutreCity == city.Name) || (x.Date == date && x.DepartutreCity == city.Name && (x.DepartutreTime) >= cx4Parameter.Value));

					}
					else
					{
						newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true && x.Date == date && x.ArrivalCity == city.Name) || (x.Date == date && x.ArrivalCity == city.Name && (x.ArrivalTime) <= cx4Parameter.Value));

					}

					if (newDay == null)
					{
						status = true;
						break;
					}
				}

			}

			return status;

			//   line.ConstraintPoints.PartialDaysOff = status;


		}

		private bool RestConstraint(List<Cx3Parameter> lstCx3Parameter, Line line)
		{

			bool status = false;


			foreach (Cx3Parameter cx3Parameter in lstCx3Parameter)
			{
				var restinMinutes = cx3Parameter.Value * 60;
				if (cx3Parameter.Type == (int)RestType.All)
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Any(x => x.RestMinutes < restinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Any(x => x.RestMinutes > restinMinutes);
					}
				}

				else if (cx3Parameter.Type == (int)RestType.InDomicile)
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Where(x => !x.IsInTrip).Any(x => x.RestMinutes < restinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Where(x => !x.IsInTrip).Any(x => x.RestMinutes > restinMinutes);
					}
				}

				else if (cx3Parameter.Type == (int)RestType.AwayDomicile)
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Where(x => x.IsInTrip).Any(x => x.RestMinutes < restinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Where(x => x.IsInTrip).Any(x => x.RestMinutes > restinMinutes);
					}
				}


			}

			return status;

		}

		/// <summary>
		/// Star day of the Week Constraint  calculation (Mon =0)
		/// </summary>
		/// <param name="lstcx3Parameter"></param>
		/// <param name="line"></param>
		private bool StartDayOfWeekConstraint(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{
				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					if (line.StartDaysList[Convert.ToInt32(cx3Parameter.ThirdcellValue)] < cx3Parameter.Value)
						status = true;
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					if (line.StartDaysList[Convert.ToInt32(cx3Parameter.ThirdcellValue)] > cx3Parameter.Value)
						status = true;
				}
				if (status)
					break;
			}
			return status;
		}

		/// <summary>
		/// Time Away From Base Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool TimeAwayFromBaseConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp) < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp) > cx2Parameter.Value);
			}

			return status;

		}

		private bool TripLengthConstraint(List<Cx3Parameter> lstCx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstCx3Parameter)
			{
				//Type=1Day, 2Day, 3Day, 4Day
				switch (cx3Parameter.ThirdcellValue)
				{
				case "1":

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips1Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips1Day > cx3Parameter.Value);
					}

					break;

				case "2":
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips2Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips2Day > cx3Parameter.Value);
					}

					break;

				case "3":
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips3Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips3Day > cx3Parameter.Value);
					}
					break;
				case "4":
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips4Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips4Day > cx3Parameter.Value);
					}
					break;



				}


				if (status)
					break;

			}


			return status;


		}

		/// <summary>
		/// WorkBlock Length
		/// </summary>
		/// <param name="lstCx3Parameter"></param>
		/// <param name="line"></param>
		private bool WorkBlockLengthConstraint(List<Cx3Parameter> lstCx3Parameter, Line line)
		{


			bool status = false;

			foreach (Cx3Parameter cx3Parameter in lstCx3Parameter)
			{

				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					status = (line.WorkBlockLengths[int.Parse(cx3Parameter.ThirdcellValue)] < cx3Parameter.Value);
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					status = (line.WorkBlockLengths[int.Parse(cx3Parameter.ThirdcellValue)] > cx3Parameter.Value);
				}


				if (status)
					break;

			}
			return status;


		}

		/// <summary>
		/// Work days Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		private bool WorkDaysConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;
			int workDaysCount = line.DaysOfMonthWorks.Count(x => x.Working);

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (workDaysCount < cx2Parameter.Value);

			}
			if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
			{
				status = (workDaysCount == cx2Parameter.Value);

			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (workDaysCount > cx2Parameter.Value);
			}

			return status;

		}


		private bool MinimumPayConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (line.Tfp < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (line.Tfp > cx2Parameter.Value);
			}

			return status;

		}
		public bool OvernightCitiesBulkCalculation(BulkOvernightCityCx bulkOvernightCityCx, Line line)
		{
			bool status = false;


			if (bulkOvernightCityCx.OverNightYes !=null &&bulkOvernightCityCx.OverNightYes.Count > 0)
			{
				List<string> lstYesCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightYes.Contains(x.Id)).Select(y => y.Name).ToList();
				status = !line.OvernightCities.Intersect(lstYesCityNames).Any();
			}

			if (status == false)
			{
				if (bulkOvernightCityCx.OverNightNo!=null&& bulkOvernightCityCx.OverNightNo.Count > 0)
				{
					List<string> lstNoCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightNo.Contains(x.Id)).Select(y => y.Name).ToList();
					status = line.OvernightCities.Intersect(lstNoCityNames).Any();
				}
			}


			return status;

		}

		/// <summary>
		/// 3 On 3 off Constraint
		/// </summary>
		/// <param name="line"></param>
		private bool ThreeOn3offConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			//            return (cx2Parameter.Type == (int)ThreeOnThreeOff.ThreeOnThreeOff) ? line.Is3on3Off : !line.Is3on3Off;
			return (cx2Parameter.Type == (int)ThreeOnThreeOff.ThreeOnThreeOff) ? !line.Is3on3Off : line.Is3on3Off;
			//bool status = false;

			//if (cx2Parameter.Type == (int)ThreeOnThreeOff.ThreeOnThreeOff)
			//{
			//    status= line.Is3on3Off;

			//}
			//else  if   (cx2Parameter.Type == (int)ThreeOnThreeOff.NoThreeOnThreeOff)
			//{
			//    status= !line.Is3on3Off;

			//}

			//return status;


		}


		private static Trip GetTrip(string pairing)
		{
			Trip trip = null;
			trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
			if (trip == null)
			{
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
			}

			return trip;

		}



		#endregion





	}
}

