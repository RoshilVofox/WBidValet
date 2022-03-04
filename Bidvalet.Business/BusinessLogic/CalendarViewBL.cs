using Bidvalet.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Bidvalet.Business.BusinessLogic
{
    public class CalendarViewBL
    {

        public static ObservableCollection<CalendarData> GenerateCalendarDetails(Line line)
        {
			
            DateTime NextBidPeriodVacationStartDate = DateTime.MinValue;
            ObservableCollection<CalendarData> lstDayOfMonth = new ObservableCollection<CalendarData>();
            lstDayOfMonth = GenerateCalendar();
            DateTime tripStartDate = DateTime.MinValue;
            DateTime tripEndDate = DateTime.MinValue;
            int paringCount = 1;
            bool isLastTrip;
            Trip trip = null;
            CalendarData calendarData = null;
        
            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                NextBidPeriodVacationStartDate = WBidCollection.GetnextSunday();
            }
            else
            {
                NextBidPeriodVacationStartDate = GlobalSettings.FAEOMStartDate;

            }
            //we are not using overlap vacation correction
            ////hold the dropped days
            //var listDroppedDays = new List<DateTime>();

            //foreach (LineSip linesip in line.LineSips)
            //{
            //    for (int i = 0; i < linesip.Sip.SipDutyPeriods.Count; i++)
            //    {
            //        if (linesip.Dropped)
            //            listDroppedDays.Add(linesip.SipStartDate.AddDays(i));
            //    }
            //}

            // DateTime startDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            int id = (int)startDate.DayOfWeek;
            foreach (string pairing in line.Pairings)
            {

                trip = GetTrip(pairing);
                isLastTrip = (line.Pairings.Count == paringCount);
                paringCount++;
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
                tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);

                foreach (DutyPeriod dutyperiod in trip.DutyPeriods)
                {

                    calendarData = lstDayOfMonth.FirstOrDefault(x => x.Date == tripStartDate.AddDays(dutyperiod.DutPerSeqNum - 1));
                    calendarData.IsLastTrip = isLastTrip;
                    if (trip.ReserveTrip)
                    {
                        calendarData.ArrStaLastLeg = "R";
                    }
                    else
                    {
                        if (dutyperiod.DutPerSeqNum != 1)
                        {
                            calendarData.ArrStaLastLeg = dutyperiod.Flights.Any(x => x.DeadHead) ? dutyperiod.ArrStaLastLeg.ToLower() : dutyperiod.ArrStaLastLeg;
                        }
                        else
                        {
                            // lower case cities if the first leg of the trip is  deadhead.
                            calendarData.ArrStaLastLeg = (dutyperiod.Flights[0].DeadHead) ? dutyperiod.ArrStaLastLeg.ToLower() : dutyperiod.ArrStaLastLeg;
                        }
                    }


                    string depTime = ((trip.ReserveTrip) ? Helper.ConvertMinutesToFormattedHour(trip.DutyPeriods[0].ReserveOut) : Helper.ConvertMinutesToFormattedHour((dutyperiod.DepTimeFirstLeg - (1440 * (dutyperiod.DutPerSeqNum - 1))))).Replace(":", "");
                    if (depTime == "0000" && !trip.ReserveTrip)
                    {
                        calendarData.DepTimeFirstLeg = (dutyperiod.DepTimeFirstLeg != 0) ? depTime : "????";
                    }
                    else
                    {
                        calendarData.DepTimeFirstLeg = (depTime == "0000") ? "????" : depTime;
                    }



                    string landTime = ((trip.ReserveTrip) ? Helper.ConvertMinutesToFormattedHour(trip.DutyPeriods[0].ReserveIn % 1440) : Helper.ConvertMinutesToFormattedHour((dutyperiod.LandTimeLastLeg - (1440 * (dutyperiod.DutPerSeqNum - 1))))).Replace(":", "");
                    //CalenderData.LandTimeLastLeg = (landTime == "0000") ? "????" : landTime;

                    if (landTime == "0000" && !trip.ReserveTrip)
                    {
                        calendarData.LandTimeLastLeg = (dutyperiod.LandTimeLastLeg != 0) ? landTime : "????";
                    }
                    else
                    {
                        calendarData.LandTimeLastLeg = (landTime == "0000") ? "????" : landTime;
                    }
                    calendarData.TripNumber = ((dutyperiod.TripNum.Substring(1, 1) == "P") ? dutyperiod.TripNum.Substring(0, 4) : dutyperiod.TripNum) + tripStartDate.Day.ToString().PadLeft(2, '0');

                    if (dutyperiod.DutPerSeqNum == 1)
                    {
                        calendarData.IsTripStart = true;

                    }

                    if (dutyperiod.DutPerSeqNum == trip.DutyPeriods.Count)
                    {
                        calendarData.IsTripEnd = true;
                    }

                    //cuurently we are not using overlap vacation correction
                    ////if the day is dropped due to overlap correction,then we have to set  Isdropped=true and it executes when the user checked the overlap check box from the main window 
                    //if (GlobalSettings.MenuBarButtonStatus.IsOverlap)
                    //{

                    //    DateTime dutyperioddate = startDate.AddDays(calendarData.Id - id - 1);
                    //    if (listDroppedDays.Contains(dutyperioddate))
                    //    {
                    //        calendarData.ColorTop = "Red";
                    //        calendarData.ColorBottom = "Red";
                    //    }
                    //    else
                    //    {
                    //        calendarData.ColorTop = "Transparent";
                    //        calendarData.ColorBottom = "Transparent";
                    //    }
                    //}
                     if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        if (GlobalSettings.IsFVVacation && line.FVvacationData!=null)
                        {
                            bool isInFvPeriod = false;
                            foreach (var fvitem in line.FVvacationData)
                            {
                                if (calendarData.Date >= fvitem.FVStartDate && calendarData.Date <= fvitem.FVEndDate)
                                {
                                    isInFvPeriod = true;
                                }
                            }
                            if (isInFvPeriod)
                            {
                                calendarData.ColorTop = CalenderColortype.FV.ToString();
                                calendarData.ColorBottom = CalenderColortype.FV.ToString();
                            }

                        }
                        VacationStateTrip vacTrip = null;
                        VacationTrip vacationTrip = null;
                        if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && line.VacationStateLine!=null &&  line.VacationStateLine.VacationTrips != null)
                        {
                            //taking the vacation details from line  object
                            vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                            if (vacTrip != null)
                            {
                                SetBackColorOfVacationtrips(vacTrip.TripType, vacTrip.VacationDutyPeriods[dutyperiod.DutPerSeqNum - 1].DutyPeriodType, calendarData, GlobalSettings.MenuBarButtonStatus.IsVacationDrop);

                            }
                        }
                        //EOM Section

                        if (vacTrip == null && GlobalSettings.MenuBarButtonStatus.IsEOM && GlobalSettings.VacationData != null && tripEndDate >= NextBidPeriodVacationStartDate)
                        {

                            if (GlobalSettings.VacationData.ContainsKey(pairing))
                            {  //taking the vacation details from Vacation file
                                vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                                if (vacationTrip != null)
                                {
                                    SetBackColorOfVacationtrips(vacationTrip.VacationType, vacationTrip.DutyPeriodsDetails[dutyperiod.DutPerSeqNum - 1].VacationType, calendarData, GlobalSettings.MenuBarButtonStatus.IsVacationDrop);
                                }
                            }

                        }


                    }
                    if (GlobalSettings.MenuBarButtonStatus.IsMIL)
                    {
                        string dutyperiodworkStatus;
                        DateTime dutyperioddate = startDate.AddDays(calendarData.Id - id - 1);
                        var bidlinetemplate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Date == dutyperioddate.Date);
                        if (bidlinetemplate != null)
                        {
                            dutyperiodworkStatus = bidlinetemplate.workStatus;

                            if (dutyperiodworkStatus == "VO_No_Work")
                            {

                                calendarData.ColorTop = CalenderColortype.MILVO_No_Work.ToString();
                                calendarData.ColorBottom = CalenderColortype.MILVO_No_Work.ToString();
                                calendarData.DepTimeFirstLegDecoration = "Strikethrough";
                                calendarData.ArrStaLastLegDecoration = "Strikethrough";
                                calendarData.LandTimeLastLegDecoration = "Strikethrough";
                            }
                            else if (dutyperiodworkStatus == "BackSplitWork")
                            {
                                calendarData.ColorTop = CalenderColortype.MILVO_No_Work.ToString();
                                calendarData.ColorBottom = CalenderColortype.Transparaent.ToString();
                                calendarData.DepTimeFirstLegDecoration = "Strikethrough";

                            }
                            else if (dutyperiodworkStatus == "FrontSplitWork")
                            {
                                calendarData.ColorTop = CalenderColortype.Transparaent.ToString();
                                calendarData.ColorBottom = CalenderColortype.MILVO_No_Work.ToString();
                                calendarData.LandTimeLastLegDecoration = "Strikethrough";

                            }
                            else if (dutyperiodworkStatus == "BackSplitWithoutStrike")
                            {
                                calendarData.ColorTop = CalenderColortype.MILVO_No_Work.ToString();
                                calendarData.ColorBottom = CalenderColortype.Transparaent.ToString();

                            }
                            else if (dutyperiodworkStatus == "FrontSplitWithoutStrike")
                            {
                                calendarData.ColorTop = CalenderColortype.Transparaent.ToString();
                                calendarData.ColorBottom = CalenderColortype.MILVO_No_Work.ToString();
                            }

                        }
                    }
                }



            }
            return lstDayOfMonth;
        }

        private static ObservableCollection<CalendarData> GenerateCalendar()
        {

            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            ObservableCollection<CalendarData> lstDayOfMonth = new ObservableCollection<CalendarData>();
            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (GlobalSettings.CurrentBidDetails.Month == 3 || GlobalSettings.CurrentBidDetails.Month == 2))
            {
                startDate = startDate.AddDays(-1);
            }

            int iterationCount = (int)startDate.DayOfWeek;

            int id = 1;
            for (int count = 0; count < iterationCount; count++)
            {
                lstDayOfMonth.Add(new CalendarData() { Id = id, IsEnabled = false, Day = string.Empty, Date = DateTime.MinValue });
                id++;
            }

            iterationCount = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Subtract(startDate).Days + 1;
            DateTime calendarDate = startDate;
            for (int count = 1; count <= iterationCount; count++)
            {
                lstDayOfMonth.Add(new CalendarData() { Id = id, IsEnabled = true, Day = calendarDate.Day.ToString(), Date = calendarDate });
                calendarDate = calendarDate.AddDays(1);
                id++;
            }


            for (int count = 1; count <= 35 - iterationCount; count++)
            {
                lstDayOfMonth.Add(new CalendarData() { Id = id, IsEnabled = true, Day = calendarDate.Day.ToString(), Date = calendarDate });
                calendarDate = calendarDate.AddDays(1);
                id++;
            }

            for (int count = id; count <= 42; count++)
            {
                lstDayOfMonth.Add(new CalendarData() { Id = id, IsEnabled = false, Day = string.Empty, Date = DateTime.MinValue });
                id++;

            }

            return lstDayOfMonth;


        }

        private static Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
            if (trip == null)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
            }
			if (trip == null && pairing.Length>6)
            {
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0,6)).FirstOrDefault();
            }

            return trip;

        }

        private static void SetBackColorOfVacationtrips(string tripType, string vacationDutyType, CalendarData CalenderData, bool isVacDrop)
        {
            switch (vacationDutyType)
            {
                case "VA":
                    CalenderData.ColorTop = CalenderColortype.VA.ToString();
                    CalenderData.ColorBottom = CalenderColortype.VA.ToString();
                    break;
                case "VO":
                    CalenderData.ColorTop = CalenderColortype.VO.ToString();
                    CalenderData.ColorBottom = CalenderColortype.VO.ToString();
                    break;
                case "VD":
                    CalenderData.ColorTop = (!isVacDrop) ? CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                    CalenderData.ColorBottom = (!isVacDrop) ? CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                    break;
                case "Split":
                    if (tripType.ToUpper() == "VOB")
                    {
                        CalenderData.ColorTop = CalenderColortype.VO.ToString();
                        CalenderData.ColorBottom = (!isVacDrop) ? CalenderData.ColorTop = CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                    }
                    else if (tripType.ToUpper() == "VOF")
                    {
                        CalenderData.ColorTop = (!isVacDrop) ? CalenderColortype.VD.ToString() : CalenderColortype.Transparaent.ToString();
                        CalenderData.ColorBottom = CalenderColortype.VO.ToString();
                    }
                    break;

            }


            if (isVacDrop)
            {
                if (vacationDutyType == "VD")
                {
                    CalenderData.DepTimeFirstLegDecoration = "Strikethrough";
                    CalenderData.ArrStaLastLegDecoration = "Strikethrough";
                    CalenderData.LandTimeLastLegDecoration = "Strikethrough";
                }
                else if (vacationDutyType == "Split")
                {
                    if (tripType.ToUpper() == "VOB")
                    {
                        CalenderData.LandTimeLastLegDecoration = "Strikethrough";
                    }
                    else if (tripType.ToUpper() == "VOF")
                    {
                        CalenderData.DepTimeFirstLegDecoration = "Strikethrough";
                    }
                }


            }


        }
    }
}
