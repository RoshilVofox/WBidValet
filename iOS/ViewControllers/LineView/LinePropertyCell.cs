
using System;

using Foundation;
using UIKit;
using Bidvalet.Model;

namespace Bidvalet.iOS
{
    public partial class LinePropertyCell : UICollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("LinePropertyCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("LinePropertyCell");

        public LinePropertyCell(IntPtr handle) : base(handle)
        {
        }
        public void BindData(ColumnDefinition columDefinition, int row)
        {
            lblHeader.Text = columDefinition.DisplayName;
            string displayName = columDefinition.DisplayName;

            Line line = GlobalSettings.Lines[row];
            Console.WriteLine("line" + row);
            lblValue.Text = GetLineProperty(displayName, line);



        }
        public static LinePropertyCell Create()
        {
            return (LinePropertyCell)Nib.Instantiate(null, null)[0];
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static string GetLineProperty(string displayName, Line line)
        {
            if (displayName == "Line")
            {
                return line.LineDisplay;
            }
            else if (displayName == "$/Day")
            {
                return string.Format("{0:0.00}", line.TfpPerDay);
            }
            else if (displayName == "$/DHr")
            {
                return string.Format("{0:0.00}", line.TfpPerDhr);
            }
            else if (displayName == "$/Hr")
            {
                return string.Format("{0:0.00}", line.TfpPerFltHr);
            }
            else if (displayName == "$/TAFB")
            {
                return line.TfpPerTafb.ToString();
            }
            else if (displayName == "+Grd")
            {
                return line.LongestGrndTime.ToString();
            }
            else if (displayName == "+Legs")
            {
                return line.MostLegs.ToString();
            }
            else if (displayName == "+Off")
            {
                return line.LargestBlkOfDaysOff.ToString();
            }
            else if (displayName == "1Dy")
            {
                return line.Trips1Day.ToString();
            }
            else if (displayName == "2Dy")
            {
                return line.Trips2Day.ToString();
            }
            else if (displayName == "3Dy")
            {
                return line.Trips3Day.ToString();
            }
            else if (displayName == "4Dy")
            {
                return line.Trips4Day.ToString();
            }
            else if (displayName == "8753")
            {
                return line.Equip8753.ToString();
            }
            else if (displayName == "A/P")
            {
                return line.AMPM.ToString();
            }
            else if (displayName == "ACChg")
            {
                return line.AcftChanges.ToString();
            }
            else if (displayName == "ACDay")
            {
                return line.AcftChgDay.ToString();
            }
            else if (displayName == "CO")
            {
                return line.CarryOverTfp.ToString();
            }
            else if (displayName == "DP")
            {
                return line.TotDutyPds.ToString();
            }
            else if (displayName == "DPinBP")
            {
                return line.TotDutyPdsInBp.ToString();
            }
            else if (displayName == "EDomPush")
            {
                return (line.EDomPush != null) ? line.EDomPush : string.Empty;
            }
            else if (displayName == "EPush")
            {
                return (line.EPush != null) ? line.EPush : string.Empty;
            }
            else if (displayName == "FA Posn")
            {
                return string.Join("", line.FAPositions.ToArray());
            }
            else if (displayName == "Flt")
            {
                return (line.BlkHrsInBp != null) ? line.BlkHrsInBp : string.Empty;
            }
            else if (displayName == "LArr")
            {
                return line.LastArrTime.ToString(@"hh\:mm");
            }
            else if (displayName == "AEDP")
            {
                return line.AvgEarliestDomPush.ToString(@"hh\:mm");
            }
            else if (displayName == "ALDA")
            {
                return line.AvgLatestDomArrivalTime.ToString(@"hh\:mm");

            }
            else if (displayName == "LDomArr")
            {
                return line.LastDomArrTime.ToString(@"hh\:mm");
            }
            else if (displayName == "Legs")
            {
                return line.Legs.ToString();
            }
            else if (displayName == "LgDay")
            {
                return line.LegsPerDay.ToString();
            }
            else if (displayName == "LgPair")
            {
                return line.LegsPerPair.ToString();
            }
            else if (displayName == "ODrop")
            {
                return line.OverlapDrop.ToString();
            }
            else if (displayName == "Off")
            {
                return line.DaysOff.ToString();
            }
            else if (displayName == "Pairs")
            {
                return line.TotPairings.ToString();
            }
            else if (displayName == "Pay" || displayName == "TotPay")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.Tfp, 2));
            }
            else if (displayName == "PDiem")
            {
                return (line.TafbInBp != null) ? line.TafbInBp : string.Empty;
            }
            else if (displayName == "MyValue")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.Points, 2));
            }
            else if (displayName == "SIPs")
            {
                return line.Sips.ToString();
            }
            else if (displayName == "StartDOW")
            {
                return (line.StartDow != null) ? line.StartDow : string.Empty;
            }
            else if (displayName == "T234")
            {
                return (line.T234 != null) ? line.T234 : string.Empty;
            }
            else if (displayName == "VDrop")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacationDrop, 2));
            }
            else if (displayName == "WkEnd")
            {
                return (line.Weekend != null) ? line.Weekend.ToLower() : string.Empty;
            }
            else if (displayName == "FltRig")
            {
                return line.RigFltInBP.ToString();
            }
            else if (displayName == "MinPayRig")
            {
                return line.RigDailyMinInBp.ToString();
            }
            else if (displayName == "DhrRig")
            {
                return line.RigDhrInBp.ToString();
            }
            else if (displayName == "AdgRig")
            {
                return line.RigAdgInBp.ToString();
            }
            else if (displayName == "TafbRig")
            {
                return line.RigTafbInBp.ToString();
            }
            else if (displayName == "TotRig")
            {
                return line.RigTotalInBp.ToString();
            }
            else if (displayName == "VacPayCu")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayCuBp, 2));
            }
            else if (displayName == "VacPayNe")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayNeBp, 2));
            }
            else if (displayName == "VacPayBo")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayBothBp, 2));
            }
            else if (displayName == "Vofrnt")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacationOverlapFront, 2));
            }
            else if (displayName == "Vobk")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacationOverlapBack, 2));
            }
            else if (displayName == "800legs")
            {
                return line.LegsIn800.ToString();
            }
            else if (displayName == "700legs")
            {
                return line.LegsIn700.ToString();
            }
            else if (displayName == "500legs")
            {
                return line.LegsIn500.ToString();
            }
            else if (displayName == "300legs")
            {
                return line.LegsIn300.ToString();
            }
            else if (displayName == "Maxlegs")
            {
                return line.LegsIn600.ToString();
            }
            else if (displayName == "DhrInBp")
            {
                return (line.DutyHrsInBp != null) ? line.DutyHrsInBp : string.Empty;
            }
            else if (displayName == "DhrInLine")
            {
                return (line.DutyHrsInLine != null) ? line.DutyHrsInLine : string.Empty;
            }
            else if (displayName == "Wts")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.TotWeight, 2));
            }
            else if (displayName == "LineRig")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.LineRig, 2));
            }
            else if (displayName == "FlyPay")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.FlyPay, 2));
            }
            else if (displayName == "Tag")
            {
                return (line.Tag != null) ? line.Tag : string.Empty;

            }
            else if (displayName == "HolRig")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.HolRig, 2));
            }
            else if (displayName == "Grp")
            {
                return (line.BAGroup != null) ? line.BAGroup : string.Empty;
            }
            else if (displayName == "VAne")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VAne, 2));
            }
            else if (displayName == "VAbo")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VAbo, 2));
            }
            else if (displayName == "VAbp")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VAbp, 2));
            }
            else if (displayName == "VAPne")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VAPne, 2));
            }
            else if (displayName == "VAPbo")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VAPbo, 2));
            }
            else if (displayName == "VAPbp")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VAPbp, 2));
            }
            else if (displayName == "Etrips")
            {
                return line.ETOPSTripsCount.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}

