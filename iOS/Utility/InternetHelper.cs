using System;
using SystemConfiguration;
using System.Net;
using CoreFoundation;
using Foundation;

namespace Bidvalet.iOS
{
    public class InternetHelper
    {



        public static int CheckInterNetConnection()
        {
            int type = 0;
           
            if (GlobalSettings.IsTestSouthWifiOn)
            {
                type = (int)InternetType.Air;
            }
            else if (Reachability.CheckVPSAvailable())
            {
                type = (int)InternetType.Ground;
            }
            else
            {
                if(IsSouthWestWifiOr2wire())
                {
                    type = (int)InternetType.Air;
                }
                else
                {
                    type = (int)InternetType.NoInternet;
                }
            }
            return type;   
                
        }
        //public static int CheckInterNetConnection()
        //{
        //    int type = 0;


        //    if (GlobalSettings.IsTestSouthWifiOn)
        //    {
        //        if (Reachability.IsHostReachable("https://www27.swalife.com"))
        //        {
        //            type = (int)InternetType.Air;
        //        }
        //        else
        //        {
        //            type = (int)InternetType.NoInternet;
        //        }

        //    }

        //    //Ground checking
        //    else if (Reachability.IsHostReachable(GlobalSettings.ServerUrl))
        //    {

        //        if (IsSouthWestWifiOr2wire())
        //        {
        //            type = (int)InternetType.Air;
        //        }
        //        else
        //        {
        //            type = (int)InternetType.Ground;
        //        }
        //    }
        //    else if (Reachability.IsHostReachable("https://www27.swalife.com"))
        //    {

        //        if (IsSouthWestWifiOr2wire())
        //        {
        //            type = (int)InternetType.Air;
        //        }
        //        else
        //        {
        //            type = (int)InternetType.NoInternet;
        //        }
        //    }
        //    //Air checking
        //    else
        //    {
        //        type = (int)InternetType.NoInternet;
        //    }
	
        //    return type;


        //}


        public static string CurrentSSID()
        {
            string result = string.Empty;
            try
            {
                NSDictionary dict;
                var status = CaptiveNetwork.TryCopyCurrentNetworkInfo("en0", out dict);
                if (dict != null)
                {
                    if (status == StatusCode.NoKey)
                    {
                        return string.Empty;
                    }

                    var bssid = dict[CaptiveNetwork.NetworkInfoKeyBSSID];
                    var ssid = dict[CaptiveNetwork.NetworkInfoKeySSID];
                    var ssiddata = dict[CaptiveNetwork.NetworkInfoKeySSIDData];

                    result = ssid.ToString();
                }
                else
                    result = string.Empty;
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public static bool IsSouthWestWifi()
        {
            if (CurrentSSID().ToLower() == "southwestwifi")
                return true;
            else
                return false;
        }
        public static bool IsSouthWestWifiOr2wire()
        {
            var currentwifi = CurrentSSID().ToLower();
            if (currentwifi == "southwestwifi" || currentwifi == "2wire")
                return true;
            else
                return false;
        }

    }





    public enum InternetType
    {
        NoInternet,
        Ground,
        Air,
        AirPaid

    }
}

