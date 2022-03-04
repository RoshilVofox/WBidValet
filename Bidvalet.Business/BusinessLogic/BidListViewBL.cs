#region NameSpace

using Bidvalet.Model;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Bidvalet.Business
{
    public class BidListViewBL
    {

        #region Public Methods
        public static void GenerateBidListIconCollection()
        {
            List<DateTime> dateList = GenerateDates();
            DateTime NextBidPeriodVacationStartDate;

            int paringCount = 1;
            NextBidPeriodVacationStartDate = GlobalSettings.CurrentBidDetails.Postion != "FA" ? WBidCollection.GetnextSunday() : GlobalSettings.FAEOMStartDate;
            foreach (var line in GlobalSettings.Lines)
            {

                line.BidLineIconList = new List<BidLineIcon>();
                foreach (var item in dateList)
                {
                    line.BidLineIconList.Add(new BidLineIcon { Date = item });

                }


                foreach (string pairing in line.Pairings)
                {

                    Trip trip = GetTrip(pairing);
                    bool isLastTrip = (line.Pairings.Count == paringCount);
                    paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);

                    foreach (DutyPeriod dutyperiod in trip.DutyPeriods)
                    {

                        BidLineIcon bidLineIcon = line.BidLineIconList.FirstOrDefault(x => x.Date == tripStartDate.AddDays(dutyperiod.DutPerSeqNum - 1));

                        if ((dutyperiod.DutPerSeqNum == 1) && (dutyperiod.DutPerSeqNum == trip.DutyPeriods.Count))
                        {
                            if (bidLineIcon != null) bidLineIcon.DutyPeriodIcon = (int)DutyPeriodIcon.RoundedOnBoth;
                        }
                        else if (dutyperiod.DutPerSeqNum == 1)
                        {
                            if (bidLineIcon != null) bidLineIcon.DutyPeriodIcon = (int)DutyPeriodIcon.RoundedLeft;
                        }
                        else if (dutyperiod.DutPerSeqNum == trip.DutyPeriods.Count)
                        {
                            bidLineIcon.DutyPeriodIcon = (int)DutyPeriodIcon.RoundedRight;
                        }

                        else
                        {
                            bidLineIcon.DutyPeriodIcon = (int)DutyPeriodIcon.SquareCorner;
                        }

                        if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                           
                            VacationStateTrip vacTrip = null;
                            if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && line.VacationStateLine != null && line.VacationStateLine.VacationTrips != null)
                            {
                                if (line.VacationStateLine.VacationTrips != null)
                                {
                                    //taking the vacation details from line  object
                                    vacTrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == pairing);
                                    if (vacTrip != null && GlobalSettings.MenuBarButtonStatus.IsEOM)
                                    {
                                        SetBackColorOfVacationtrips(vacTrip.TripType, vacTrip.VacationDutyPeriods[dutyperiod.DutPerSeqNum - 1].DutyPeriodType, bidLineIcon, GlobalSettings.MenuBarButtonStatus.IsVacationDrop);

                                    }
                                    else if (vacTrip != null && tripStartDate.Month == vacTrip.TripActualStartDate.Month)
                                    {
                                        SetBackColorOfVacationtrips(vacTrip.TripType, vacTrip.VacationDutyPeriods[dutyperiod.DutPerSeqNum - 1].DutyPeriodType, bidLineIcon, GlobalSettings.MenuBarButtonStatus.IsVacationDrop);

                                    }
                                    else
                                    {
                                        SetTripColor(line, trip.AmPm, bidLineIcon);
                                    }
                                }
                            }
                            //EOM Section

                            if (vacTrip == null && GlobalSettings.MenuBarButtonStatus.IsEOM && GlobalSettings.VacationData != null && tripEndDate >= NextBidPeriodVacationStartDate)
                            {

                                if (GlobalSettings.VacationData.ContainsKey(pairing))
                                {
                                    //taking the vacation details from Vacation file
                                    VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                                    if (vacationTrip != null)
                                    {
                                        SetBackColorOfVacationtrips(vacationTrip.VacationType, vacationTrip.DutyPeriodsDetails[dutyperiod.DutPerSeqNum - 1].VacationType, bidLineIcon, GlobalSettings.MenuBarButtonStatus.IsVacationDrop);
                                    }
                                    else
                                    {
                                        SetTripColor(line, trip.AmPm, bidLineIcon);
                                    }
                                }
                            }
                            if (GlobalSettings.IsFVVacation)
                            {
                                bool isInFvPeriod = false;
                                foreach (var fvitem in line.FVvacationData)
                                {
                                    if (bidLineIcon.Date >= fvitem.FVStartDate && bidLineIcon.Date <= fvitem.FVEndDate)
                                    {
                                        isInFvPeriod = true;
                                    }
                                }
                                if (isInFvPeriod)
                                {
                                    bidLineIcon.ColorTop = CalenderColortype.FV.ToString();
                                    bidLineIcon.ColorBottom = CalenderColortype.FV.ToString();
                                }

                            }
                            if (bidLineIcon.ColorTop == null)
                            {
                                SetTripColor(line, trip.AmPm, bidLineIcon);

                            }


                        }
                        else
                        {
                            SetTripColor(line, trip.AmPm, bidLineIcon);
                        }



                    }
                }
            }
        } 
        #endregion

        #region Private Methods
        private static void SetTripColor(Line line, string AMPMTypeStr, BidLineIcon bidLineIcon)
        {



            if (!line.ReserveLine)
            { // Not Reserve Line
                if (AMPMTypeStr == "1")
                { // AM
                    bidLineIcon.ColorTop = "AM";
                    bidLineIcon.ColorBottom = "AM";

                }
                else if (AMPMTypeStr == "2")
                { //PM
                    bidLineIcon.ColorTop = "PM";
                    bidLineIcon.ColorBottom = "PM";

                }
                else if (AMPMTypeStr == "3")
                { //Mix
                    bidLineIcon.ColorTop = "MIX";
                    bidLineIcon.ColorBottom = "MIX";

                }
                else
                {
                    bidLineIcon.ColorTop = "Other";
                    bidLineIcon.ColorBottom = "Other";

                }
            }
            else
            { // Reserve Line
                if (AMPMTypeStr == "1")
                { // AM
                    bidLineIcon.ColorTop = "AMReserve";
                    bidLineIcon.ColorBottom = "AMReserve";

                }
                else if (AMPMTypeStr == "2")
                { //PM
                    bidLineIcon.ColorTop = "PMReserve";
                    bidLineIcon.ColorBottom = "PMReserve";

                }
                else if (AMPMTypeStr == "3")
                { //Mix
                    bidLineIcon.ColorTop = "MIXReserve";
                    bidLineIcon.ColorBottom = "MIXMReserve";
                }
                else
                {
                    bidLineIcon.ColorTop = "OtherReserve";
                    bidLineIcon.ColorBottom = "OtherReserve";

                }
            }



        }

        private static void SetBackColorOfVacationtrips(string tripType, string vacationDutyType, BidLineIcon bidLineIcon, bool isVacDrop)
        {
            switch (vacationDutyType)
            {
                case "VA":
                    bidLineIcon.ColorTop = CalenderColortype.VA.ToString();
                    bidLineIcon.ColorBottom = CalenderColortype.VA.ToString();
                    break;
                case "VO":
                    bidLineIcon.ColorTop = CalenderColortype.VO.ToString();
                    bidLineIcon.ColorBottom = CalenderColortype.VO.ToString();
                    break;
                case "VD":
                    bidLineIcon.ColorTop = (!isVacDrop) ? CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                    bidLineIcon.ColorBottom = (!isVacDrop) ? CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                    break;
                case "Split":
                    if (tripType.ToUpper() == "VOB")
                    {
                        bidLineIcon.ColorTop = CalenderColortype.VO.ToString();
                        bidLineIcon.ColorBottom = (!isVacDrop) ? bidLineIcon.ColorTop = CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                    }
                    else if (tripType.ToUpper() == "VOF")
                    {
                        bidLineIcon.ColorTop = (!isVacDrop) ? CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                        bidLineIcon.ColorBottom = CalenderColortype.VO.ToString();
                    }
                    break;

            }





        }

        private static Trip GetTrip(string pairing)
        {
            Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4)) ??
                        GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);

            return trip;

        }

        private static List<DateTime> GenerateDates()
        {

            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            var dateList = new List<DateTime>();
            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (GlobalSettings.CurrentBidDetails.Month == 3 || GlobalSettings.CurrentBidDetails.Month == 2))
            {
                startDate = startDate.AddDays(-1);
            }

            var iterationCount = (int)startDate.DayOfWeek;

            int id = 1;
            for (int count = 0; count < iterationCount; count++)
            {
                dateList.Add(DateTime.MinValue);
                id++;
            }

            iterationCount = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Subtract(startDate).Days + 1;
            DateTime calendarDate = startDate;
            for (int count = 1; count <= iterationCount; count++)
            {
                dateList.Add(calendarDate);
                calendarDate = calendarDate.AddDays(1);
                id++;
            }


            for (int count = 1; count <= 35 - iterationCount; count++)
            {
                dateList.Add(calendarDate);
                calendarDate = calendarDate.AddDays(1);
                id++;
            }

            for (int count = id; count <= 42; count++)
            {
                dateList.Add(DateTime.MinValue);
                id++;

            }

            return dateList;


        } 
        #endregion
    }
}
