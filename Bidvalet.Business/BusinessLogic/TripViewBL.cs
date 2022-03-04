﻿using Bidvalet.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Bidvalet.Business.BusinessLogic
{
    public class TripViewBL
    {

        public static ObservableCollection<TripData> GenerateTripDetails(string tripName, CorrectionParams correctionParams, bool isLastTrip)
        {
            ObservableCollection<TripData> lstTripData = new ObservableCollection<TripData>();
            Trip trip = GetTrip(tripName);
            string strContent = string.Empty;


            if (trip != null)
            {
                if (GlobalSettings.MenuBarButtonStatus.IsMIL && GlobalSettings.MILData != null && GlobalSettings.MILDates != null)
                {

                    lstTripData = GenerateMILTripDetails(tripName, correctionParams, isLastTrip, trip);
                }
                else
                {
                    VacationTrip vacTrip = null;
                    //DateTime selectedTripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, Convert.ToInt32(tripName.Substring(4, 2)));
                    DateTime selectedTripStartDate = WBidCollection.SetDate(Convert.ToInt16(tripName.Substring(4, 2)), isLastTrip);
                    if (GlobalSettings.VacationData != null && (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM))
                    {
                        int tripEndTime = 0;
                        DateTime tripEndDate = selectedTripStartDate.AddDays(trip.DutyPeriods.Count - 1);


                        // added to capture trips that arrive back in domicile after 2400 domicile time
                        tripEndTime = (trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg) % 1440;
                        tripEndTime = WBidCollection.AfterMidnightLandingTime(tripEndTime);
                        tripEndTime = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripEndDate, tripEndTime);

                        if (GlobalSettings.VacationData.ContainsKey(tripName))
                        {
                            if (GlobalSettings.TempOrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.StartAbsenceDate <= selectedTripStartDate && x.EndAbsenceDate >= tripEndDate)))
                            {
                                vacTrip = GlobalSettings.VacationData[tripName].VaData;
                            }
                            else if (GlobalSettings.TempOrderedVacationDays.Any(x => x.AbsenceType == "VA" && ((x.StartAbsenceDate >= selectedTripStartDate) && (x.StartAbsenceDate <= tripEndDate)) || ((x.StartAbsenceDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440))))
                            {
                                vacTrip = GlobalSettings.VacationData[tripName].VofData;
                            }
                            else if (GlobalSettings.TempOrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.EndAbsenceDate >= selectedTripStartDate && x.EndAbsenceDate <= tripEndDate)))
                            {
                                vacTrip = GlobalSettings.VacationData[tripName].VobData;
                            }
                        }
                    }
                    else if (GlobalSettings.MenuBarButtonStatus.IsOverlap)
                    {
                        DateTime SelectedTripEndtDate = selectedTripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                        var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == correctionParams.selectedLineNum);
                        if (line != null)
                        {
                            List<LineSip> linesips = line.LineSips.Where(x => x.Dropped).Where(x => (x.SipStartDate == selectedTripStartDate || selectedTripStartDate <= x.SipStartDate.AddDays(x.Sip.SipDutyPeriods.Count - 1)) && x.SipStartDate <= SelectedTripEndtDate).ToList();
                            correctionParams.LineSips = linesips;
                        }
                    }
                    correctionParams.vacTrip = vacTrip;

                    lstTripData.Add(GetTripTopContent(trip));
                    //lstTripData.Add(new TripData() { BackColor = string.Empty, Content = string.Empty, IsStrike = false });
                    //lstTripData.Add(new TripData() { BackColor = string.Empty, Content = string.Empty, IsStrike = false });
                    lstTripData.Add(new TripData() { BackColor = string.Empty, Content = string.Empty, IsStrike = false });
                    strContent = " Flt\t\t\t   Out    In   Block   Pay   Remarks";
                    lstTripData.Add(new TripData() { BackColor = string.Empty, Content = strContent, IsStrike = false });

                    ObservableCollection<TripData> lstdata=GetFlightData(trip, correctionParams,selectedTripStartDate);
                   
                    lstTripData = new ObservableCollection<TripData>(lstTripData.Concat(lstdata));
                    lstTripData.Add(new TripData() { BackColor = string.Empty, Content = string.Empty, IsStrike = false });
                    if (trip.ReserveTrip)
                    {
                        GetReserveTripSummary(trip, correctionParams);

                    }
                    else
                    {
                        lstTripData.Add(GetTripSummary(trip, correctionParams));

                    }

                }
            }

            return lstTripData;


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

        private static TripData GetReserveTripSummary(Trip trip, CorrectionParams correctionParams)
        {
            string result = string.Empty;
            decimal totalTfp = 0;
            int totalBlockTimeMinute = 0;
            int totalDutyTime = 0;
            TripData tripData = null;


            bool isStrikeOut = false;

            int cntVD = 0;
            if (correctionParams.vacTrip != null)
            {
                cntVD = correctionParams.vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split").Count();
                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && cntVD > 0)
                {
                    isStrikeOut = true;
                }
            }

            foreach (var dp in trip.DutyPeriods)
            {
                totalBlockTimeMinute += dp.Block;
                totalTfp += dp.Tfp;
                totalDutyTime += (dp.ReserveIn - dp.ReserveOut);
            }


            result = String.Format("{0}           {1}  {2} {3}", " TAFB :" + Helper.TimeFromMinutes(trip.Tafb.ToString()).PadLeft(6, ' '),
                 Helper.TimeFromMinutes(totalDutyTime.ToString()).PadLeft(5, ' '),
                              Helper.CalcTimeFromMinutesFromMidnight(totalBlockTimeMinute.ToString()).PadLeft(5, ' '), totalTfp.ToString("0.00")).PadLeft(5, ' ');
            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeOut, Content = result };
            return tripData;



        }

        private static ObservableCollection<TripData> GenerateMILTripDetails(string tripName, CorrectionParams correctionParams, bool isLastTrip, Trip CurrentTrip)
        {
            MILTrip milTrip = null;
            DateTime tripStart;
            DateTime tripEnd;
            //TimeSpan time = new TimeSpan(23, 0, 0);
            //tripEnd = tripEnd.Date + time;
            tripStart = WBidCollection.SetDate(Convert.ToInt16(tripName.Substring(4, 2)), isLastTrip);

            tripEnd = tripStart.AddDays(CurrentTrip.DutyPeriods.Count - 1);

            ObservableCollection<TripData> tripDetails = new ObservableCollection<TripData>();

            int time = (CurrentTrip.ReserveTrip) ? CurrentTrip.DutyPeriods[CurrentTrip.PairLength - 1].ReserveIn : CurrentTrip.DutyPeriods[CurrentTrip.PairLength - 1].ReleaseTime;
            if (time > 1440)
            {
                time = time - (CurrentTrip.PairLength - 1) * 1440;
            }
            tripEnd = tripEnd.AddMinutes(time);

            //trip start time
            time = (CurrentTrip.ReserveTrip) ? CurrentTrip.DutyPeriods[0].ReserveOut : CurrentTrip.DutyPeriods[0].ShowTime;
            tripStart = tripStart.AddMinutes(time);


            //if (GlobalSettings.MILData != null && GlobalSettings.MILDates != null)
            //{
            if (GlobalSettings.MILData.ContainsKey(tripName))
            {
                if (GlobalSettings.MILDates.Any(x => (x.StartAbsenceDate <= tripStart && x.EndAbsenceDate >= tripEnd)))
                {
                    milTrip = GlobalSettings.MILData[tripName].MaData;
                }
                else if (GlobalSettings.MILDates.Any(x => ((x.StartAbsenceDate > tripStart) && (x.StartAbsenceDate < tripEnd) && x.EndAbsenceDate >= tripEnd)))
                {
                    milTrip = GlobalSettings.MILData[tripName].MofData;
                }
                else if (GlobalSettings.MILDates.Any(x => (x.EndAbsenceDate > tripStart && x.EndAbsenceDate < tripEnd && tripStart >= x.StartAbsenceDate)))
                {
                    milTrip = GlobalSettings.MILData[tripName].MobData;
                }
                else if (GlobalSettings.MILDates.Any(x => (x.StartAbsenceDate > tripStart && x.EndAbsenceDate < tripEnd)))
                {
                    milTrip = GlobalSettings.MILData[tripName].MofbData;
                }
            }

            GetTripHeaderDetailsMIL(CurrentTrip, tripName, tripDetails);

            GetFlightDataMIL(CurrentTrip, milTrip, tripDetails);


            if (CurrentTrip.ReserveTrip)
            {
                tripDetails.Add(GetReserveTripSummaryMIL(CurrentTrip, milTrip));

            }
            else
            {
                tripDetails.Add(GetTripSummaryMIL(CurrentTrip, milTrip));

            }
            //}
            return tripDetails;
        }

        private static TripData GetTripTopContent(Trip trip)
        {
            TripData tripData = null;
            string strContent = "Pairing " + trip.TripNum.Substring(0, 4) + " Show " + ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(trip.DutyPeriods[0].ReserveOut.ToString()).PadLeft(5, ' ') : Helper.ConvertMinuteToHHMM(trip.DutyPeriods[0].ShowTime)).PadLeft(5, ' ') + " TFP " +
                           (trip.Tfp).ToString("0.00").PadLeft(5, ' ') + " Blk " +
                Helper.CalcTimeFromMinutesFromMidnight(trip.Block.ToString()).PadLeft(6, ' ') + " DutPds " +
                           trip.DutyPeriods.Count + " Flts " + trip.DutyPeriods.SelectMany(x => x.Flights).Count();

            tripData = new TripData() { BackColor = string.Empty, IsStrike = false, Content = strContent };
            return tripData;


        }

        private static ObservableCollection<TripData> GetFlightData(Trip trip, CorrectionParams correctionParams,DateTime selectedTripStartDate)
        {
            ObservableCollection<TripData> lstTrips = new ObservableCollection<TripData>();

            int count = 0;
            int flightIndex = 0;
            string vacationType = string.Empty;
            foreach (var dp in trip.DutyPeriods)
            {
                DateTime dpdate = selectedTripStartDate.AddDays(count);
                if (trip.ReserveTrip)
                {

                    lstTrips.Add(GetReserveSingleLineData(trip, dp, correctionParams,dpdate));
                    lstTrips.Add(new TripData() { BackColor = string.Empty, IsStrike = false, Content = string.Empty });
                    lstTrips.Add(GetReserveSummary(trip, dp, correctionParams));


                }
                else
                {
                    flightIndex = 0;
                    foreach (var flt in dp.Flights)
                    {


                        if (correctionParams.vacTrip != null)
                        {
                            correctionParams.vacationType = correctionParams.vacTrip.DutyPeriodsDetails[count].FlightDetails[flightIndex].VacationType;
                        }
                        if (flt != null)
                            lstTrips.Add(GetSingleFlightData(flt, correctionParams,dpdate));


                        flightIndex++;
                    }
                    if (!trip.ReserveTrip)
                    {
                        if (count + 1 < trip.DutyPeriods.Count)
                        {
                            ///add Hotels data to the flight details
                            var hotels = GlobalSettings.WBidINIContent.Hotels.HotelList.Where(x => x.City == trip.DutyPeriods[count].ArrStaLastLeg).FirstOrDefault();
                            if (hotels != null)
                            {
                                lstTrips.Add(new TripData() { BackColor = string.Empty, IsStrike = false, Content = hotels.Hotels });

                            }

                        }
                    }

                    lstTrips.Add(GetDutyPdSummary(trip, dp, correctionParams));


                }
                count++;
                lstTrips.Add(new TripData() { BackColor = string.Empty, Content = string.Empty, IsStrike = false });
            }

            return lstTrips;

        }

        private static TripData GetTripSummary(Trip trip, CorrectionParams correctionParams)
        {



            string result = string.Empty;
            TripData tripData = null;
            bool isStrikeOut = false;

            int cntVD = 0;
            if (correctionParams.vacTrip != null)
            {
                cntVD = correctionParams.vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split").Count();
                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && cntVD > 0)
                {
                    isStrikeOut = true;
                }
            }

            result = String.Format("{0}            {1}  {2}  {3}", "TAFB :" + Helper.TimeFromMinutes(trip.Tafb.ToString()).PadLeft(6, ' '),
                Helper.ConvertMinutesToHhhmm(trip.DutyPeriods.Sum(x => x.DutyTime)).PadLeft(5, ' '),
                              Helper.ConvertMinutesToHhhmm(trip.Block).PadLeft(5, ' '), trip.Tfp.ToString("0.00")).PadLeft(5, ' ');
            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeOut, Content = result };
            return tripData;

        }

        private static TripData GetDutyPdSummaryFiltered(Trip trip, DutyPeriod dp, CorrectionParams correctionParams)
        {
            string result = string.Empty;
            bool isStrikeout = false;
            TripData tripData = null;

            if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
            {
                if (correctionParams.vacTrip != null && (correctionParams.vacationType == "VD" || correctionParams.vacationType == "Split"))
                {
                    isStrikeout = true;
                }
            }

            //result = String.Format("{0}                   {1}  {2}  {3}  {4}  {5}",
            result = String.Format("{0}                   {1}  ",
                trip.DutyPeriods.Count() == dp.DutPerSeqNum ? "     " : Helper.CalcTimeFromMinutesFromMidnight(dp.DutyBreak.ToString()).PadLeft(5, ' '),
                        string.Format(Helper.CalcTimeFromMinutesFromMidnight(dp.DutyTime.ToString()), "HH:mm:ss").PadLeft(5, ' '));

            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeout, Content = result };
            return tripData;


        }

        private static void GetTripHeaderDetailsMIL(Trip trip, string tripName, ObservableCollection<TripData> tripDetails)
        {
            string strLine = "Pairing " + tripName + "  Show " + ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(trip.DutyPeriods[0].ReserveOut.ToString()) : Helper.ConvertMinuteToHHMM(trip.DutyPeriods[0].ShowTime)) + "  TFP " +
                             (trip.Tfp).ToString("0.00") + "  Blk " +
                             Helper.CalcTimeFromMinutesFromMidnight(trip.Block.ToString()) + "  DutPds " +
                             trip.DutyPeriods.Count + "  Flts " + trip.DutyPeriods.SelectMany(x => x.Flights).Count();
            //CountFlights(trip);

            tripDetails.Add(new TripData { Content = strLine, BackColor = string.Empty, IsStrike = false });

            tripDetails.Add(new TripData() { BackColor = string.Empty, Content = string.Empty, IsStrike = false });
            strLine = " Flt\t\t\t\tOut    In     Block   Pay   Remarks";
            tripDetails.Add(new TripData() { BackColor = string.Empty, Content = strLine, IsStrike = false });

        }


        private static void GetFlightDataMIL(Trip trip, MILTrip milTrip, ObservableCollection<TripData> lstTrips)
        {
            //ObservableCollection<TripData> lstTrips = new ObservableCollection<TripData>();

            int count = 0;
            int flightIndex = 0;
            string milType = string.Empty;
            foreach (var dp in trip.DutyPeriods)
            {
                milType = string.Empty;

                if (milTrip != null)
                {
                    milType = milTrip.DutyPeriodsDetails[count].Type;
                }
                if (trip.ReserveTrip)
                {

                    lstTrips.Add(GetReserveSingleLineDataMIL(trip, dp, milType, milTrip));
                    lstTrips.Add(new TripData() { BackColor = string.Empty, IsStrike = false, Content = string.Empty });
                    lstTrips.Add(GetReserveSummaryMIL(trip, dp, milType, milTrip));


                }
                else
                {
                    flightIndex = 0;
                    string milflightType = string.Empty;
                    foreach (var flt in dp.Flights)
                    {
                        //
                        //
                        if (milTrip != null)
                        {
                            milflightType = milTrip.DutyPeriodsDetails[count].FlightDetails[flightIndex].Type;
                        }
                        lstTrips.Add(GetSingleFlightDataMIL(flt, milflightType));
                        //
                        //
                        flightIndex++;
                    }
                    if (!trip.ReserveTrip)
                    {
                        if (count + 1 < trip.DutyPeriods.Count)
                        {
                            ///add Hotels data to the flight details
                            var hotels = GlobalSettings.WBidINIContent.Hotels.HotelList.Where(x => x.City == trip.DutyPeriods[count].ArrStaLastLeg).FirstOrDefault();
                            if (hotels != null)
                            {
                                lstTrips.Add(new TripData() { BackColor = string.Empty, IsStrike = false, Content = hotels.Hotels });

                            }

                        }
                    }
                    //
                    lstTrips.Add(GetDutyPdSummaryMIL(trip, dp, milType, milTrip));


                }
                count++;
                lstTrips.Add(new TripData() { BackColor = string.Empty, Content = string.Empty, IsStrike = false });
            }

            //return lstTrips;

        }


        private static TripData GetSingleFlightDataMIL(Flight flt, string mILType)
        {
            bool isStrike = false;
            string backColor = string.Empty;
            TripData tripData = null;
            string flight = null;
            string changeEqp = string.Empty;
            string equipment = string.Empty;
            flight = flt.FltNum.ToString().PadLeft(5, ' ');

            if (mILType != string.Empty && (mILType == "MD" || mILType == "MA"))
            {

                isStrike = true;
                backColor = "MD";


            }


            if (flt.Equip != null)
            {
                if (flt.Equip.Length > 0)
                {
                    changeEqp = (flt.Equip.Substring(0, 1) == "*" ? "chg" : "");
                    equipment = WBidCollection.GetEquipmentName(flt.Equip);
                }
            }


            flight += "  " +
                flt.DepSta + "  " + flt.ArrSta + "  " +
                Helper.CalcTimeFromMinutesFromMidnight(flt.DepTime.ToString()).PadLeft(5, ' ') + "  " +
                Helper.CalcTimeFromMinutesFromMidnight(flt.ArrTime.ToString()).PadLeft(5, ' ') + "  " +
                Helper.CalcTimeFromMinutesFromMidnight((flt.ArrTime - flt.DepTime).ToString()).PadLeft(5, ' ') + "  " +
                flt.Tfp.ToString("0.0").PadLeft(4, ' ') + "   " +  // block time
                equipment + " " + (flt.DeadHead ? "dhd" : "") +
                changeEqp;


            tripData = new TripData() { BackColor = backColor, IsStrike = isStrike, Content = flight };

            return tripData;



        }

        private static TripData GetDutyPdSummaryMIL(Trip trip, DutyPeriod dp, string mILType, MILTrip milTrip)
        {
            string result = string.Empty;
            bool isStrikeout = false;
            TripData tripData = null;

            if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
            {
                if (milTrip != null && (mILType == "MD" || mILType == "Split"))
                {
                    isStrikeout = true;
                }
            }

            result = String.Format("{0}                   {1}  {2}  {3}  {4}  {5}",
                trip.DutyPeriods.Count() == dp.DutPerSeqNum ? "     " : Helper.CalcTimeFromMinutesFromMidnight(dp.DutyBreak.ToString()).PadLeft(5, ' '),
                string.Format(Helper.CalcTimeFromMinutesFromMidnight(dp.DutyTime.ToString()), "HH:mm:ss").PadLeft(5, ' '),
                Helper.CalcTimeFromMinutesFromMidnight(dp.Block.ToString()), dp.Tfp.ToString("#.00").PadLeft(5, ' '),
                string.Empty, string.Empty);

            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeout, Content = result };
            return tripData;


        }


        private static TripData GetReserveTripSummaryMIL(Trip trip, MILTrip milTrip)
        {
            TripData tripData = null;
            string result = string.Empty;
            decimal totalTfp = 0;
            int totalBlockTimeMinute = 0;
            int totalDutyTime = 0;

            int cntVD = 0;
            bool isStrikeout = false;

            if (milTrip != null)
            {


                isStrikeout = true;

            }
            foreach (var dp in trip.DutyPeriods)
            {
                totalBlockTimeMinute += dp.Block;
                totalTfp += dp.Tfp;
                totalDutyTime += (dp.ReserveIn - dp.ReserveOut);
            }


            result = String.Format("{0}\t\t\t{1}\t{2}\t{3}", "TAFB :\t" + Helper.TimeFromMinutes(trip.Tafb.ToString()),
                Helper.TimeFromMinutes(totalDutyTime.ToString()),
                Helper.CalcTimeFromMinutesFromMidnight(totalBlockTimeMinute.ToString()), totalTfp.ToString("0.00"));
            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeout, Content = result };
            return tripData;


        }

        private static TripData GetTripSummaryMIL(Trip trip, MILTrip milTrip)
        {
            TripData tripData = null;
            int cntDrop = 0;
            bool isStrikeout = false;
            string result = string.Empty;

            if (milTrip != null)
            {
                cntDrop = milTrip.DutyPeriodsDetails.Count(x => x.Type == "MD" || x.Type == "MA");
                if (cntDrop > 0)
                {
                    isStrikeout = true;
                }
            }

            result = String.Format("{0}\t\t\t{1}\t{2}\t{3}", "TAFB :\t" + Helper.TimeFromMinutes(trip.Tafb.ToString()),
                Helper.ConvertMinutesToHhhmm(trip.DutyPeriods.Sum(x => x.DutyTime)),
                Helper.ConvertMinutesToHhhmm(trip.Block), trip.Tfp.ToString("0.00"));
            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeout, Content = result };
            return tripData;

        }

        private static TripData GetReserveSingleLineDataMIL(Trip trip, DutyPeriod dutyPeriod, string mILType, MILTrip milTrip)
        {
            string result = string.Empty;
            TripData tripData = null;
            string backColor = string.Empty;
            result = " RSRV  " + trip.DepSta + "  " + trip.RetSta + "  " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveOut - (1440 * (dutyPeriod.DutPerSeqNum - 1)))).PadLeft(5, ' ') + "  " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveIn - (1440 * (dutyPeriod.DutPerSeqNum - 1)))).PadLeft(5, ' ') + "  ";
            result += Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.Block.ToString()).PadLeft(5, ' ') + "  " + dutyPeriod.Tfp.ToString("0.0").PadLeft(4, ' ') + "   ";
            bool isStrikeout = false;
            if (milTrip != null)
            {
                if (GlobalSettings.MenuBarButtonStatus.IsMIL && (mILType == "MD" || mILType == "MA"))
                {
                    backColor = "MD";
                    isStrikeout = true;
                }



            }

            tripData = new TripData() { BackColor = backColor, IsStrike = isStrikeout, Content = result };
            return tripData;


        }

        private static TripData GetReserveSummaryMIL(Trip trip, DutyPeriod dutyPeriod, string mILType, MILTrip milTrip)
        {
            string result = string.Empty;
            string dutybreak = string.Empty;
            bool isStrikeOut = false;
            string backColor = string.Empty;
            TripData tripData = null;


            if (milTrip != null)
            {

                isStrikeOut = true;

            }

            if (trip.DutyPeriods.Count() == dutyPeriod.DutPerSeqNum)
            {
                result = "     ";
            }
            else
            {
                // need to checked this
                //dutybreak = ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString((trip.DutyPeriods[dutyPeriod.DutPerSeqNum + 1].ReserveOut - dutyPeriod.ReserveIn))) : Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.DutyBreak.ToString()));
                dutybreak = ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString((trip.DutyPeriods[dutyPeriod.DutPerSeqNum].ReserveOut - dutyPeriod.ReserveIn))) : Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.DutyBreak.ToString()));
                result = dutybreak.PadLeft(5, ' ');
            }

            result += "                   " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveIn - dutyPeriod.ReserveOut)).PadLeft(5, ' ') + "  ";
            result += Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.Block.ToString()).PadLeft(5, ' ') + "  " + dutyPeriod.Tfp.ToString("0.0").PadLeft(4, ' ') + "   ";

            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeOut, Content = result };
            return tripData;


        }


        private static TripData GetReserveSingleLineFilteredData(Trip trip, DutyPeriod dutyPeriod)
        {
            string result = string.Empty;
            TripData tripData = null;
            string backColor = string.Empty;
            result = " RSRV  " + trip.DepSta + "  " + trip.RetSta + "  " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveOut - (1440 * (dutyPeriod.DutPerSeqNum - 1)))).PadLeft(5, ' ') + "  " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveIn - (1440 * (dutyPeriod.DutPerSeqNum - 1)))).PadLeft(5, ' ') + "  ";
            // result += Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.Block.ToString()).PadLeft(5, ' ') + "  " + dutyPeriod.Tfp.ToString("0.0").PadLeft(4, ' ') + "   ";
            bool isStrikeout = false;


            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeout, Content = result };
            return tripData;


        }

        private static TripData GetReserveSummaryFiltered(Trip trip, DutyPeriod dutyPeriod, CorrectionParams correctionParams)
        {
            string result = string.Empty;
            string dutybreak = string.Empty;
            bool isStrikeOut = false;
            string backColor = string.Empty;
            TripData tripData = null;


            if (correctionParams.vacTrip != null)
            {
                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && (correctionParams.vacationType == "VD" || correctionParams.vacationType == "Split"))
                {
                    isStrikeOut = true;
                }
            }

            if (trip.DutyPeriods.Count() == dutyPeriod.DutPerSeqNum)
            {
                result = "     ";
            }
            else
            {
                // need to checked this
                //dutybreak = ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString((trip.DutyPeriods[dutyPeriod.DutPerSeqNum + 1].ReserveOut - dutyPeriod.ReserveIn))) : Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.DutyBreak.ToString()));
                dutybreak = ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString((trip.DutyPeriods[dutyPeriod.DutPerSeqNum].ReserveOut - dutyPeriod.ReserveIn))) : Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.DutyBreak.ToString()));
                result = dutybreak.PadLeft(5, ' ');
            }

            result += "                   " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveIn - dutyPeriod.ReserveOut)).PadLeft(5, ' ') + "  ";
            // result += Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.Block.ToString()).PadLeft(5, ' ') + "  " + dutyPeriod.Tfp.ToString("0.0").PadLeft(4, ' ') + "   ";

            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeOut, Content = result };
            return tripData;


        }

        private static TripData GetReserveSingleLineData(Trip trip, DutyPeriod dutyPeriod, CorrectionParams correctionParams,DateTime selecteddpDate)
        {
            string result = string.Empty;
            TripData tripData = null;
            string backColor = string.Empty;
            result = " RSRV  " + trip.DepSta + "  " + trip.RetSta + "  " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveOut - (1440 * (dutyPeriod.DutPerSeqNum - 1)))).PadLeft(5, ' ') + "  " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveIn - (1440 * (dutyPeriod.DutPerSeqNum - 1)))).PadLeft(5, ' ') + "  ";
            result += Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.Block.ToString()).PadLeft(5, ' ') + "  " + dutyPeriod.Tfp.ToString("0.0").PadLeft(4, ' ') + "   ";
            bool isStrikeout = false;
            if (correctionParams.vacTrip != null)
            {
                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && (correctionParams.vacationType == "VD" || correctionParams.vacationType == "Split"))
                {
                    isStrikeout = true;
                }

                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection))
                {
                    backColor = SetBackColor(correctionParams.vacationType);
                }

            }
            if (GlobalSettings.IsFVVacation)
            {
                var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == correctionParams.selectedLineNum);
                if (line != null && line.FVvacationData != null)
                {
                    foreach (var item in line.FVvacationData)
                    {
                        if (item.FVStartDate <= selecteddpDate && item.FVEndDate >= selecteddpDate)
                        {
                            backColor = "FV";
                        }
                    }
                }
            }
            tripData = new TripData() { BackColor = backColor, IsStrike = isStrikeout, Content = result };
            return tripData;


        }

        private static TripData GetReserveSummary(Trip trip, DutyPeriod dutyPeriod, CorrectionParams correctionParams)
        {
            string result = string.Empty;
            string dutybreak = string.Empty;
            bool isStrikeOut = false;
            string backColor = string.Empty;
            TripData tripData = null;


            if (correctionParams.vacTrip != null)
            {
                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && (correctionParams.vacationType == "VD" || correctionParams.vacationType == "Split"))
                {
                    isStrikeOut = true;
                }
            }

            if (trip.DutyPeriods.Count() == dutyPeriod.DutPerSeqNum)
            {
                result = "     ";
            }
            else
            {
                // need to checked this
                //dutybreak = ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString((trip.DutyPeriods[dutyPeriod.DutPerSeqNum + 1].ReserveOut - dutyPeriod.ReserveIn))) : Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.DutyBreak.ToString()));
                dutybreak = ((trip.ReserveTrip) ? Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString((trip.DutyPeriods[dutyPeriod.DutPerSeqNum].ReserveOut - dutyPeriod.ReserveIn))) : Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.DutyBreak.ToString()));
                result = dutybreak.PadLeft(5, ' ');
            }

            result += "                   " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveIn - dutyPeriod.ReserveOut)).PadLeft(5, ' ') + "  ";
            result += Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.Block.ToString()).PadLeft(5, ' ') + "  " + dutyPeriod.Tfp.ToString("0.0").PadLeft(4, ' ') + "   ";

            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeOut, Content = result };
            return tripData;


        }
        private static string SetBackColor(int vacType, int drpCount, int flightsequenceNumber)
        {
            string backColor = string.Empty;
            switch (vacType)
            {
                case (int)BidLineType.VD:
                case (int)BidLineType.VDDrop:
                    backColor = "VD";
                    break;
                case (int)BidLineType.VA:
                    backColor = "VA";
                    break;
                case (int)BidLineType.VO:
                    backColor = "VO";
                    break;
                case (int)BidLineType.VOBSplit:
                case (int)BidLineType.VOBSplitDrop:
                    if (drpCount >= (flightsequenceNumber))
                    {
                        backColor = "VO";
                    }
                    else
                    {
                        backColor = "VD";
                    }
                    break;

                case (int)BidLineType.VOFSplit:
                case (int)BidLineType.VOFSplitDrop:
                    if (drpCount >= (flightsequenceNumber))
                    {
                        backColor = "VD";
                    }
                    else
                    {
                        backColor = "VO";
                    }
                    break;

            }

            return backColor;
        }

        private static string SetBackColor(string vacType)
        {
            string backColor = string.Empty;

            if (vacType == "VD")
            {
                backColor = "VD";
            }
            else if (vacType == "VA")
            {
                backColor = "VA";
            }
            else if (vacType == "VO")
            {
                backColor = "VO";
            }

            return backColor;
        }
        private static TripData GetDutyPdSummary(Trip trip, DutyPeriod dp, CorrectionParams correctionParams)
        {
            string result = string.Empty;
            bool isStrikeout = false;
            TripData tripData = null;

            if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
            {
                if (correctionParams.vacTrip != null && (correctionParams.vacationType == "VD" || correctionParams.vacationType == "Split"))
                {
                    isStrikeout = true;
                }
            }

            result = String.Format("{0}                   {1}  {2}  {3}  {4}  {5}",
                trip.DutyPeriods.Count() == dp.DutPerSeqNum ? "     " : Helper.CalcTimeFromMinutesFromMidnight(dp.DutyBreak.ToString()).PadLeft(5, ' '),
                        string.Format(Helper.CalcTimeFromMinutesFromMidnight(dp.DutyTime.ToString()), "HH:mm:ss").PadLeft(5, ' '),
                        Helper.CalcTimeFromMinutesFromMidnight(dp.Block.ToString()), dp.Tfp.ToString("#.00").PadLeft(5, ' '),
                       string.Empty, string.Empty);

            tripData = new TripData() { BackColor = string.Empty, IsStrike = isStrikeout, Content = result };
            return tripData;


        }

        private static TripData GetSingleFlightData(Flight flt, CorrectionParams correctionParams,DateTime selecteddpDate)
        {

            bool isStrike = false;
            string backColor = string.Empty;
            TripData tripData = null;
            string flight = null;
            string changeEqp = string.Empty;
            string equipment = string.Empty;
            flight = flt.FltNum.ToString().PadLeft(5, ' ');
           
          
            if (correctionParams.vacationType != string.Empty)
            {
                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && correctionParams.vacationType == "VD")
                {
                    isStrike = true;
                }

                if ((GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsVacationCorrection))
                {
                    
                    var bidlinetempate = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == correctionParams.selectedLineNum).BidLineTemplates;
                    var bidlinetype = bidlinetempate.FirstOrDefault(x => x.Date == selecteddpDate).BidLineType;
                    var drpcount = bidlinetempate.FirstOrDefault(x => x.Date == selecteddpDate).DrpCount;
                    backColor = SetBackColor(bidlinetype, drpcount, flt.FlightSeqNum);
                }

            }
            if (GlobalSettings.IsFVVacation)
            {
                var line=GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == correctionParams.selectedLineNum);
                if (line != null && line.FVvacationData != null)
                {
                    foreach (var item in line.FVvacationData)
                    {
                        if (item.FVStartDate <= selecteddpDate && item.FVEndDate >= selecteddpDate)
                        {
                            backColor = "FV";
                        }
                    }
                }
            }
            if (flt.Equip != null)
            {
                if (flt.Equip.Length > 0)
                {
                    changeEqp = (flt.Equip.Substring(0, 1) == "*" ? "chg" : "");
                    equipment = WBidCollection.GetEquipmentName(flt.Equip);
                }
            }


            flight += "  " +
                        flt.DepSta + "  " + flt.ArrSta + "  " +
                        Helper.CalcTimeFromMinutesFromMidnight(flt.DepTime.ToString()).PadLeft(5, ' ') + "  " +
                        Helper.CalcTimeFromMinutesFromMidnight(flt.ArrTime.ToString()).PadLeft(5, ' ') + "  " +
                        Helper.CalcTimeFromMinutesFromMidnight((flt.ArrTime - flt.DepTime).ToString()).PadLeft(5, ' ') + "  " +
                        flt.Tfp.ToString("0.0").PadLeft(4, ' ') + "   " +  // block time
                        equipment + " " + (flt.DeadHead ? "dhd" : "") +
                        changeEqp;

            if (GlobalSettings.MenuBarButtonStatus.IsOverlap && correctionParams != null)
            {
                List<Flight> flights = new List<Flight>();

                foreach (var sips in correctionParams.LineSips)
                {
                    foreach (var dutyperiods in sips.Sip.SipDutyPeriods)
                    {
                        foreach (var fl in dutyperiods.Flights)
                        {
                            flights.Add(fl);
                        }
                    }
                }

                //var flights = correctionParams.LineSips.SelectMany(x => x.Sip.SipDutyPeriods).SelectMany(y => y.Flights);

                if (flights != null && flights.Any(x => x.AcftChange == flt.AcftChange && x.ArrSta == flt.ArrSta && x.ArrTime == flt.ArrTime && x.Block == flt.Block && x.DeadHead == flt.DeadHead && x.DepSta == flt.DepSta && x.DepTime == flt.DepTime && x.DepType == flt.DepType && x.DutyBreak == flt.DutyBreak))
                {


                    tripData = new TripData() { BackColor = "Overlap", IsStrike = false, Content = flight };
                }
                else
                    tripData = new TripData() { BackColor = backColor, IsStrike = false, Content = flight };

            }
            else
            {
                tripData = new TripData() { BackColor = backColor, IsStrike = isStrike, Content = flight };
            }
            return tripData;



        }


    }
}
