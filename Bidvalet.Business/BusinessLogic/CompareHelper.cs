using Bidvalet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using WBid.WBidClient.Models;

namespace Bidvalet.Business
{
    public class CompareHelper
    {

        public static bool CompareStates(List<BidAutoItem> fileObject, List<BidAutoItem> stateObject)
        {
            bool isDifferent = false;

            if (fileObject.Count != stateObject.Count)
            {
                isDifferent = true;
                return isDifferent;
            }

            int index = 0;

            for (int cnt = 0; cnt < fileObject.Count; cnt++)
            {

                if (fileObject[cnt].Name != stateObject[cnt].Name)
                {
                    isDifferent = true;
                    break;
                }
                else
                {
                    switch (fileObject[cnt].Name)
                    {
                        case "AP":
                         isDifferent=   CompareAMPM((AMPMConstriants)fileObject[cnt].BidAutoObject, (AMPMConstriants)stateObject[cnt].BidAutoObject);
                         break;

                        case "CL":
                         isDifferent = CompareCL((FtCommutableLine)fileObject[cnt].BidAutoObject, (FtCommutableLine)stateObject[cnt].BidAutoObject);
                         break;

                        case "DOM":
                         isDifferent = CompareDOM((DaysOfMonthCx)fileObject[cnt].BidAutoObject, (DaysOfMonthCx)stateObject[cnt].BidAutoObject);
                         break;

                        case "DOWA":
                        case "SDOW":
                         isDifferent = CompareCxDays((CxDays)fileObject[cnt].BidAutoObject, (CxDays)stateObject[cnt].BidAutoObject);
                         break;


                        case "DOWS":
                        case "ET":
                        case "DHFL":
                        case "RT":
                         isDifferent = CompareCx3Parameter((Cx3Parameter)fileObject[cnt].BidAutoObject, (Cx3Parameter)stateObject[cnt].BidAutoObject);
                         break;

                        case "LT":
                         isDifferent = CompareCxLine((CxLine)fileObject[cnt].BidAutoObject, (CxLine)stateObject[cnt].BidAutoObject);
                         break;

                        case "OC":
                         isDifferent = CompareOvernight((BulkOvernightCityCx)fileObject[cnt].BidAutoObject, (BulkOvernightCityCx)stateObject[cnt].BidAutoObject);
                         break;
                            
                    }
                    if (isDifferent)
                        return isDifferent;
                }

                index++;
 
            }


                return isDifferent;


 
        }

        private static bool CompareOvernight(BulkOvernightCityCx fileBulkOvernightCityCx, BulkOvernightCityCx stateBulkOvernightCityCx)
        {
            bool status=false;
            if (fileBulkOvernightCityCx.OverNightNo != null && stateBulkOvernightCityCx.OverNightNo != null)
            {
                if (fileBulkOvernightCityCx.OverNightNo.Count != stateBulkOvernightCityCx.OverNightNo.Count)
                {
                    status = true;
                }
                else
                {
                    for (int cnt = 0; cnt < fileBulkOvernightCityCx.OverNightNo.Count; cnt++)
                    {
                        if (fileBulkOvernightCityCx.OverNightNo[cnt] != stateBulkOvernightCityCx.OverNightNo[cnt])
                        {
                            status = true;
                            if (status)
                                break;
                        }
                    }
                }
            }
            else if (fileBulkOvernightCityCx.OverNightNo == null && stateBulkOvernightCityCx.OverNightNo == null)
            {
                status = false;
 
            }
            else
            {
                status=true;
            }


            if (!status)
            {
                if (fileBulkOvernightCityCx.OverNightYes != null && stateBulkOvernightCityCx.OverNightYes != null)
                {
                    if (fileBulkOvernightCityCx.OverNightYes.Count != stateBulkOvernightCityCx.OverNightYes.Count)
                    {
                        status = true;
                    }
                    else
                    {
                        for (int cnt = 0; cnt < fileBulkOvernightCityCx.OverNightYes.Count; cnt++)
                        {
                            if (fileBulkOvernightCityCx.OverNightYes[cnt] != stateBulkOvernightCityCx.OverNightYes[cnt])
                            {
                                status = true;
                                if (status)
                                    break;
                            }
                        }
                    }
                }
                else if (fileBulkOvernightCityCx.OverNightYes == null && stateBulkOvernightCityCx.OverNightYes == null)
                {
                    status = false;

                }
                else
                {
                    status = true;
                }
 
            }

            return status;
        }

        private static bool CompareCxLine(CxLine fileCxLine, CxLine stateCxLine)
        {
            return fileCxLine.Blank != stateCxLine.Blank || fileCxLine.Hard != stateCxLine.Hard || fileCxLine.International != stateCxLine.International ||
                fileCxLine.NonConus != stateCxLine.NonConus || fileCxLine.Ready != stateCxLine.Ready || fileCxLine.Reserve != stateCxLine.Reserve;
        }

        private static bool CompareCx3Parameter(Cx3Parameter fileCx3Parameter, Cx3Parameter stateCx3Parameter)
        {
           return fileCx3Parameter.ThirdcellValue!=stateCx3Parameter.ThirdcellValue || fileCx3Parameter.Value!=stateCx3Parameter.Value ||fileCx3Parameter.Type!=stateCx3Parameter.Type;
        }

        private static bool CompareCxDays(CxDays fileCxDays, CxDays stateCxDays)
        {
            return fileCxDays.IsFri != stateCxDays.IsFri || fileCxDays.IsMon != stateCxDays.IsMon || fileCxDays.IsSat != stateCxDays.IsSat || fileCxDays.IsSun != stateCxDays.IsSun ||
                 fileCxDays.IsThu != stateCxDays.IsThu || fileCxDays.IsTue != stateCxDays.IsTue || fileCxDays.IsWed != stateCxDays.IsWed;
        }

        private static bool CompareDOM(DaysOfMonthCx fileDaysOfMonthCx, DaysOfMonthCx stateDaysOfMonthCx)
        {
            bool status = false;
            if (fileDaysOfMonthCx.OFFDays != null && stateDaysOfMonthCx.OFFDays != null)
            {
                if (fileDaysOfMonthCx.OFFDays.Count != stateDaysOfMonthCx.OFFDays.Count)
                {
                    status = true;
                }
                else
                {
                    for (int cnt = 0; cnt < fileDaysOfMonthCx.OFFDays.Count; cnt++)
                    {
                        if (fileDaysOfMonthCx.OFFDays[cnt] != stateDaysOfMonthCx.OFFDays[cnt])
                        {
                            status = true;
                            if (status)
                                break;
                        }
                    }
                }
            }
            else if (fileDaysOfMonthCx.OFFDays == null && stateDaysOfMonthCx.OFFDays == null)
            {
                status = false;

            }
            else
            {
                status = true;
            }


            if (!status)
            {
                if (fileDaysOfMonthCx.WorkDays != null && stateDaysOfMonthCx.WorkDays != null)
                {
                    if (fileDaysOfMonthCx.WorkDays.Count != stateDaysOfMonthCx.WorkDays.Count)
                    {
                        status = true;
                    }
                    else
                    {
                        for (int cnt = 0; cnt < fileDaysOfMonthCx.WorkDays.Count; cnt++)
                        {
                            if (fileDaysOfMonthCx.WorkDays[cnt] != stateDaysOfMonthCx.WorkDays[cnt])
                            {
                                status = true;
                                if (status)
                                    break;
                            }
                        }
                    }
                }
                else if (fileDaysOfMonthCx.WorkDays == null && stateDaysOfMonthCx.WorkDays == null)
                {
                    status = false;

                }
                else
                {
                    status = true;
                }

            }

            return status;
            
            //return fileDaysOfMonthCx.OFFDays != stateDaysOfMonthCx.OFFDays || fileDaysOfMonthCx.WorkDays != stateDaysOfMonthCx.WorkDays;
        }

        private static bool CompareCL(FtCommutableLine fileCL, FtCommutableLine stateCL)
        {
            return fileCL.City != stateCL.City || fileCL.BaseTime != stateCL.BaseTime || fileCL.CheckInTime != stateCL.CheckInTime
                || fileCL.CommuteCity != stateCL.CommuteCity || fileCL.ConnectTime != stateCL.ConnectTime || fileCL.NoNights != stateCL.NoNights ||
                 fileCL.ToHome != stateCL.ToHome ||  fileCL.ToWork != stateCL.ToWork || fileCL.IsNonStopOnly != stateCL.IsNonStopOnly;
            
        }

        private static bool CompareAMPM(AMPMConstriants fileAMPM, AMPMConstriants stateAMPM)
        {

            return fileAMPM.AM != stateAMPM.AM || fileAMPM.PM != stateAMPM.PM || fileAMPM.MIX != stateAMPM.MIX;
        }
    }
}
