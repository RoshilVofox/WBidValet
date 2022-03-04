﻿#region NameSpace
using Bidvalet.Model;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Bidvalet.Business
{
    public class BidAutoHelper
    {


        #region Public Methods
        public static void SetSelectedFiltersToStateObject(List<Object> listFilter)
        {
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBIdStateContent != null && wBIdStateContent.BidAuto == null)
            {
                wBIdStateContent.BidAuto = new BidAutomator();
            }


            if (wBIdStateContent != null)
            {
                wBIdStateContent.BidAuto.BAFilter = new List<BidAutoItem>();


                int priority = 0;
                foreach (var bidItem in listFilter)
                {

                    switch (bidItem.GetType().Name.ToLower())
                    {
                        case "ampmconstriants":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetAMPMObjectToState((AMPMConstriants)listFilter[priority], priority));
                            break;


                        case "ftcommutableline":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetCommutableLineObjectToState((FtCommutableLine)listFilter[priority], priority));
                            break;

                        case "daysofmonthcx":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetDaysOfMonthObjectToState((DaysOfMonthCx)listFilter[priority], priority));
                            break;

                        case "daysofweekall":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetDaysOfWeekAllObjectToState((DaysOfWeekAll)listFilter[priority], priority));
                            break;

                        case "daysofweeksome":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetDaysOfWeekSomeObjectToState((DaysOfWeekSome)listFilter[priority], priority));
                            break;


                        case "dhfristlastconstraint":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetDeadHeadFirstLastTypeObjectToState((DHFristLastConstraint)listFilter[priority], priority));
                            break;

                        case "equirementconstraint":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetEquipementTypeObjectToState((EquirementConstraint)listFilter[priority], priority));
                            break;
                        case "linetypeconstraint":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetLineTypeObjectToState((LineTypeConstraint)listFilter[priority], priority));
                            break;

                        case "overnightcitiescx":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetOvernightTypeObjectToState((OvernightCitiesCx)listFilter[priority], priority));
                            break;

                        case "restcx":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetRestTypeObjectToState((RestCx)listFilter[priority], priority));
                            break;


                        case "startdayofweek":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetStartDayOfWeekObjectToState((StartDayOfWeek)listFilter[priority], priority));
                            break;


                        case "cxtripblocklength":
                            wBIdStateContent.BidAuto.BAFilter.Add(SetTripBlockLengthObjectToState((CxTripBlockLength)listFilter[priority], priority));
                            break;




                    }

                    priority++;

                }
            }
        }

        public static void LoadFilters(List<BidAutoItem> listFilters)
        {
            SharedObject.Instance.ListConstraint.Clear();
            foreach (var filter in listFilters)
            {

                object state = null;



                switch (filter.Name)
                {
                    case "AP":
                    case "CL":
                    case "DOM":
                    case "TBL":

                        state = filter.BidAutoObject;
                        break;
                    case "DOWA":
                        var cxDays = (CxDays)filter.BidAutoObject;
                        var daysOfWeekAll = new DaysOfWeekAll
                        {
                            Su = cxDays.IsSun,
                            Mo = cxDays.IsMon,
                            Tu = cxDays.IsTue,
                            We = cxDays.IsWed,
                            Th = cxDays.IsThu,
                            Fr = cxDays.IsFri,
                            Sa = cxDays.IsSat
                        };
                        state = daysOfWeekAll;

                        break;
                    case "DOWS":
                        var cx3Parameter = (Cx3Parameter)filter.BidAutoObject;
                        var daysOfWeekSome = new DaysOfWeekSome
                        {
                            LessOrMore = SetType(cx3Parameter.Type),
                            Value = cx3Parameter.Value,
                            Date = SetDay(cx3Parameter.ThirdcellValue)
                        };
                        state = daysOfWeekSome;
                        break;

                    case "DHFL":
                        var cx3ParameterDH = (Cx3Parameter)filter.BidAutoObject;
                        var dHFristLastConstraint = new DHFristLastConstraint
                        {
                            LessMore = SetType(cx3ParameterDH.Type),
                            Value = cx3ParameterDH.Value,
                            DH = SetDeadHeadType(cx3ParameterDH.ThirdcellValue)
                        };
                        state = dHFristLastConstraint;
                        break;
                    case "ET":
                        var cx3ParameterEQ = (Cx3Parameter)filter.BidAutoObject;
                        var equirementConstraint = new EquirementConstraint
                        {
                            LessMore = SetType(cx3ParameterEQ.Type),
                            Value = cx3ParameterEQ.Value,
                            Equipment = int.Parse(cx3ParameterEQ.ThirdcellValue)
                        };
                        state = equirementConstraint;
                        break;

                    case "LT":
                        var cxLine = (CxLine)filter.BidAutoObject;
                        var lineTypeConstraint = new LineTypeConstraint
                        {
                            Hard = cxLine.Hard,
                            Res = cxLine.Reserve,
                            Int = cxLine.International,
                            NonCon = cxLine.NonConus,
                            Blank = GlobalSettings.CurrentBidDetails.Postion == "FA" ? cxLine.Ready : cxLine.Blank
                        };

                        state = lineTypeConstraint;
                        break;



                    case "OC":

                        var bulkOvernightCityCx = (BulkOvernightCityCx)filter.BidAutoObject;
                        List<string> yes = GetOvernightCityNameListFromId(bulkOvernightCityCx.OverNightYes);
                        List<string> no = GetOvernightCityNameListFromId(bulkOvernightCityCx.OverNightNo);
                        var overnightCitiesCx = new OvernightCitiesCx
                        {
                            Yes = yes,
                            No = no
                        };

                        state = overnightCitiesCx;
                        break;

                    case "RT":

                        var cx3ParameterRest = (Cx3Parameter)filter.BidAutoObject;
                        var restCx = new RestCx
                        {
                            LessMore = SetType(cx3ParameterRest.Type),
                            Value = cx3ParameterRest.Value,
                            Dom = SetRestType(cx3ParameterRest.ThirdcellValue)


                        };
                        state = restCx;
                        break;
                    case "SDOW":
                        var cxDaysSDOW = (CxDays)filter.BidAutoObject;
                        var StartDayOfWeek = new StartDayOfWeek()
                        {
                            Fr = cxDaysSDOW.IsFri,
                            Su = cxDaysSDOW.IsSun,
                            Mo = cxDaysSDOW.IsMon,
                            Tu = cxDaysSDOW.IsTue,
                            We = cxDaysSDOW.IsWed,
                            Th = cxDaysSDOW.IsThu,
                            Sa = cxDaysSDOW.IsSat
                        };
                        state = StartDayOfWeek;
                        break;

                }

                if (state != null)
                {
                    SharedObject.Instance.ListConstraint.Add(state);
                }

            }

        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Set AMPM filter object to  State
        /// </summary>
        /// <param name="amPmConstraints"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        private static BidAutoItem SetAMPMObjectToState(AMPMConstriants amPmConstraints, int priority)
        {

            BidAutoItem bidAutoItem = new BidAutoItem();
            bidAutoItem.Name = "AP";
            bidAutoItem.Priority = priority;
            bidAutoItem.IsApplied = true; ;
            bidAutoItem.BidAutoObject = new AMPMConstriants()
            {
                AM = amPmConstraints.AM,
                PM = amPmConstraints.PM,
                MIX = amPmConstraints.MIX
            };

            return bidAutoItem;

        }

        /// <summary>
        /// Set Commuteline filter object to  State
        /// </summary>
        /// <param name="ftCommutableLine"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        private static BidAutoItem SetCommutableLineObjectToState(FtCommutableLine ftCommutableLine, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "CL",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new FtCommutableLine
                {
                    BaseTime = ftCommutableLine.BaseTime,
                    CheckInTime = ftCommutableLine.CheckInTime,
                    City = ftCommutableLine.City,
                    CommuteCity = ftCommutableLine.CommuteCity,
                    ConnectTime = ftCommutableLine.ConnectTime,
                    NoNights = ftCommutableLine.NoNights,
                    ToHome = ftCommutableLine.ToHome,
                    ToWork = ftCommutableLine.ToWork


                }
            };

            return bidAutoItem;

        }

        /// <summary>
        /// Set DaysofMonth filter to state
        /// </summary>
        /// <param name="daysOfMonthCx"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        private static BidAutoItem SetDaysOfMonthObjectToState(DaysOfMonthCx daysOfMonthCx, int priority)
        {

            var bidAutoItem = new BidAutoItem
            {
                Name = "DOM",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new DaysOfMonthCx
                {
                    OFFDays = daysOfMonthCx.OFFDays,
                    WorkDays = daysOfMonthCx.WorkDays

                }
            };

            return bidAutoItem;

        }

        /// <summary>
        /// Set Daysofweekall  filter to state
        /// </summary>
        /// <param name="daysOfWeekAll"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        private static BidAutoItem SetDaysOfWeekAllObjectToState(DaysOfWeekAll daysOfWeekAll, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "DOWA",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new CxDays
                {
                    IsSun = daysOfWeekAll.Su,
                    IsMon = daysOfWeekAll.Mo,
                    IsTue = daysOfWeekAll.Tu,
                    IsWed = daysOfWeekAll.We,
                    IsThu = daysOfWeekAll.Th,
                    IsFri = daysOfWeekAll.Fr,
                    IsSat = daysOfWeekAll.Sa

                }
            };
            return bidAutoItem;
        }

        /// <summary>
        ///  Set Days of week some  filter to state
        /// </summary>
        /// <param name="daysOfWeekSome"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        private static BidAutoItem SetDaysOfWeekSomeObjectToState(DaysOfWeekSome daysOfWeekSome, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "DOWS",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new Cx3Parameter
                {
                    Type = GetType(daysOfWeekSome.LessOrMore.ToLower()),
                    Value = daysOfWeekSome.Value,
                    ThirdcellValue = GetDay(daysOfWeekSome.Date.ToLower())

                }
            };

            return bidAutoItem;
        }

        private static BidAutoItem SetEquipementTypeObjectToState(EquirementConstraint equirementFilter, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "ET",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new Cx3Parameter
                {
                    Type = GetType(equirementFilter.LessMore.ToLower()),
                    Value = equirementFilter.Value,
                    ThirdcellValue = equirementFilter.Equipment.ToString()

                }
            };

            return bidAutoItem;
        }

        private static BidAutoItem SetDeadHeadFirstLastTypeObjectToState(DHFristLastConstraint dHFristLastFilter, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "DHFL",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new Cx3Parameter
                {
                    Type = GetType(dHFristLastFilter.LessMore.ToLower()),
                    Value = dHFristLastFilter.Value,
                    ThirdcellValue = GetDeadHeadType(dHFristLastFilter.DH)

                }
            };
            return bidAutoItem;
        }

        private static BidAutoItem SetLineTypeObjectToState(LineTypeConstraint lineTypeItem, int priority)
        {
            var bidAutoItem = new BidAutoItem {Name = "LT", Priority = priority, IsApplied = true};
            var cxObject = new CxLine
            {
                Hard = lineTypeItem.Hard,
                Reserve = lineTypeItem.Res,
                International = lineTypeItem.Int,
                NonConus = lineTypeItem.NonCon
            };

            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                cxObject.Ready = lineTypeItem.Blank;
            }
            else
            {
                cxObject.Blank = lineTypeItem.Blank;
            }

            bidAutoItem.BidAutoObject = cxObject;
            return bidAutoItem;
        }

        private static BidAutoItem SetOvernightTypeObjectToState(OvernightCitiesCx overnightCitiesCx, int priority)
        {
            var bidAutoItem = new BidAutoItem {Name = "OC", Priority = priority, IsApplied = true};
            List<int> yes = GetOvernightCityIdListFromName(overnightCitiesCx.Yes);
            List<int> no = GetOvernightCityIdListFromName(overnightCitiesCx.No);

            bidAutoItem.BidAutoObject = new BulkOvernightCityCx
            {
                OverNightYes = yes,
                OverNightNo = no

            };
            return bidAutoItem;
        }

        private static List<int> GetOvernightCityIdListFromName(List<string> cities)
        {
            var lstCity = new List<int>();
            if (cities != null && cities.Count > 0)
            {
                foreach (string city in cities)
                {
                    var objCity = GlobalSettings.OverNightCitiesInBid.FirstOrDefault(x => x.Name == city);
                    if (objCity != null)
                    {
                        lstCity.Add(objCity.Id);

                    }
                }
            }

            return lstCity;
        }

        private static List<string> GetOvernightCityNameListFromId(List<int> cities)
        {
            var lstCity = new List<string>();
            if (cities != null && cities.Count > 0)
            {
                foreach (int cityId in cities)
                {
                    var objCity = GlobalSettings.OverNightCitiesInBid.FirstOrDefault(x => x.Id == cityId);
                    if (objCity != null)
                    {
                        lstCity.Add(objCity.Name);

                    }
                }
            }

            return lstCity;
        }

        private static BidAutoItem SetRestTypeObjectToState(RestCx restCx, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "RT",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new Cx3Parameter
                {
                    Type = GetType(restCx.LessMore.ToLower()),
                    Value = restCx.Value,
                    ThirdcellValue = GetRestType(restCx.Dom.ToLower())

                }
            };
            return bidAutoItem;
        }

        private static BidAutoItem SetStartDayOfWeekObjectToState(StartDayOfWeek startDayOfWeek, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "SDOW",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = new CxDays
                {
                    IsSun = startDayOfWeek.Su,
                    IsMon = startDayOfWeek.Mo,
                    IsTue = startDayOfWeek.Tu,
                    IsWed = startDayOfWeek.We,
                    IsThu = startDayOfWeek.Th,
                    IsFri = startDayOfWeek.Fr,
                    IsSat = startDayOfWeek.Sa

                }
            };
            return bidAutoItem;
        }

        private static BidAutoItem SetTripBlockLengthObjectToState(CxTripBlockLength tripblockLengthItem, int priority)
        {
            var bidAutoItem = new BidAutoItem
            {
                Name = "TBL",
                Priority = priority,
                IsApplied = true,
                BidAutoObject = tripblockLengthItem
            };

            return bidAutoItem;
        }

        private static string GetDay(string day)
        {
            int result = 0;
            switch (day)
            {
                case "mon":
                    result = (int)DayofTheWeek.Monday;
                    break;
                case "tue":
                    result = (int)DayofTheWeek.Tuesday;
                    break;
                case "wed":
                    result = (int)DayofTheWeek.Wednesday;
                    break;
                case "thu":
                    result = (int)DayofTheWeek.Thursday;
                    break;

                case "fri":
                    result = (int)DayofTheWeek.Friday;
                    break;
                case "sat":
                    result = (int)DayofTheWeek.Saturday;
                    break;
                case "sun":
                    result = (int)DayofTheWeek.Sunday;
                    break;
            }
            return result.ToString();

        }

        private static string SetDay(string day)
        {
            string result = "0";
            switch (day)
            {
                case "0":
                    result = "monday";
                    break;
                case "1":
                    result = "tuesday";
                    break;
                case "2":
                    result = "wednesday";
                    break;
                case "3":
                    result = "thursday";
                    break;

                case "4":
                    result = "friday";
                    break;
                case "5":
                    result = "saturday";
                    break;
                case "6":
                    result = "sunday";
                    break;
            }
            return result;

        }

        private static int GetType(string type)
        {
            int result = 0;
            switch (type)
            {
                case "less than":
                    result = (int)ConstraintType.LessThan;
                    break;
                case "more than":
                    result = (int)ConstraintType.MoreThan;
                    break;
                case "equal":
                    result = (int)ConstraintType.EqualTo;
                    break;
                case "not equal to":
                    result = (int)ConstraintType.NotEqualTo;
                    break;

                case "at after":
                    result = (int)ConstraintType.atafter;
                    break;
                case "at before":
                    result = (int)ConstraintType.atbefore;
                    break;

            }
            return result;

        }

        public static string SetType(int type)
        {
            string result = string.Empty;
            switch (type)
            {
                case 1:
                    result = "Less than";
                    break;
                case 3:
                    result = "More than";
                    break;
                case 2:
                    result = "Equal";
                    break;
                case 4:
                    result = "Not equal to";
                    break;

                //case "at after":
                //    result = (int)ConstraintType.atafter;
                //    break;
                //case "at before":
                //    result = (int)ConstraintType.atbefore;
                //    break;

            }
            return result;

        }

        private static string GetDeadHeadType(string deadHead)
        {
            int result = 0;
            switch (deadHead)
            {
                case "first":
                    result = (int)DeadheadType.First;
                    break;
                case "last":
                    result = (int)DeadheadType.Last;
                    break;
                case "both":
                    result = (int)DeadheadType.Both;
                    break;
            }
            return result.ToString();
        }

        private static string SetDeadHeadType(string deadHead)
        {
            string result = string.Empty;
            switch (deadHead)
            {
                case "1":
                    result = "first";
                    break;
                case "2":
                    result = "last";
                    break;
                case "3":
                    result = "both";
                    break;
            }
            return result;
        }

        private static string GetRestType(string restType)
        {
            int result = 0;
            switch (restType)
            {
                case "all":
                    result = (int)RestType.All;
                    break;
                case "indom":
                    result = (int)RestType.InDomicile;
                    break;
                case "awaydom":
                    result = (int)RestType.AwayDomicile;
                    break;
            }
            return result.ToString();
        }

        private static string SetRestType(string restType)
        {
            string result = string.Empty;
            switch (restType)
            {
                case "1":
                    result = "All";
                    break;
                case "2":
                    result = "inDom";
                    break;
                case "3":
                    result = "AwayDom";
                    break;
            }
            return result;
        }

        #endregion

        //private static string GetEquipment(string equipment)
        //{
        //    string result = string.Empty;
        //    switch (equipment)
        //    {
        //        case "300":
        //            result = "300";
        //            break;
        //        case "500":
        //            result = "500";
        //            break;
        //        case "300 & 500":
        //            result = "35";
        //            break;
        //        case "700":
        //            result = "700";
        //            break;
        //        case "800":
        //            result = "800";
        //            break;

        //    }

        //    return result;
        //}

    }



}
