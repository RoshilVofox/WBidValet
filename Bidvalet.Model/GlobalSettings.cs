using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Bidvalet.Model;


namespace Bidvalet
{
	public class GlobalSettings
	{
		
        public const string SupportEmail = "support@wbidvalet.com";

        public static bool IsNeedToDownloadSeniority = false;
        public static bool IsNeedToDownloadSeniorityUser = false;

		public static bool isAwardDownload = false;

		public const string IniFileVersion = "1.4";

		public const string LineVersion = "1.3";
		public const string TripVersion = "2.0";
		public const string DwcVersion = "2.2";
		public const string StateFileVersion = "2.3";
		public const string MILFileVersion = "1.0";
        public static Dictionary<string, string> Document = new Dictionary<string, string>() { { "How to use", "How to User WBidValet.pdf" }, { "Filters", "Filters.pdf" }, { "Logic of WBidValet", "Logic of WBidValet.pdf" }, { "Column Headings", "Column Headings.pdf" } };
        public static Dictionary<string, string> Videos = new Dictionary<string, string>() {{ "How to Use", "bf4ndDFOR28" }};
		public static List<ColumnDefinition> ColumnDefinition { get; set; }
		public const string QuickFilterVersion = "1.0";

		public static string ServerUrl="www.wbidmax.com";

		public static string ApplicationName="WBidValet";

        public static bool IsHistorical { get; set; }

		public static string Domicile;

		public static bool IsDifferentUser { get; set; }

		public static bool IsTestSouthWifiOn;

        public static List<int> SelectedLines { get; set; } 


        //public const string synchServiceUrl = "http://108.60.201.50:8006/SynchronizationService.svc/";
		public const string synchServiceUrl = "http://synch.wbidmax.com:8006/SynchronizationService.svc/";
		//public static string WBidAuthenticationServiceUrl= "http://192.168.10.100/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc";
        //public static string WBidAuthenticationServiceUrl = "http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc";
		public static string WBidAuthenticationServiceUrl = "http://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/";

        public static string VofoxAuthenticationServiceUrl = "http://122.166.23.155/WBidAuth/WBidDataDwonloadAuthService.svc";

       //public static string VPSAuthenticationServiceUrl = "http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc";
		public static string VPSAuthenticationServiceUrl = "http://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc/";

		//"http://www.auth.wbidmax.com/WBidDataDwonloadAuthService.svc";
		// "http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc";


		public const string DataDownloadAuthenticationUrl = "http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc/";
		// public static string WBidAuthenticationServiceUrl = "http://122.166.23.155/WBidAuth/WBidDataDwonloadAuthService.svc";

		//public const decimal FAReserveDayPay = 6.0m;

		public static bool QATest = false;

		public static BidDetails DownloadBidDetails { get; set; }

		public static BidDetails CurrentBidDetails { get; set; }


        public static SubmitBid SubmitBidDetails { get; set; }

		//public static WbidUser WbidUserContent { get; set; }

		public static UserInformation UserInfo { get; set; }

		public static string SessionCredentials{ get; set; }

		public static StringBuilder ExtraErrorInfo {get; set; }

		public static bool IsScrapStart { get; set; }

		public static Dictionary<string, Trip> parsedDict;

		public const int show1stDay = 60;
		public const int showAfter1stDay = 30;
		public const int debrief = 30;
        public const int ReserveBriefTime=0;
        public const int ReserveDeBriefTime = 0;

		public static string ModifiedEmployeeNumber { get; set; }

		public static WBidINI WBidINIContent { get; set; }

		/// <summary>
		/// second Sunday in March.  Used to calculate herb time for FA takeoff and land times from raw data
		/// </summary>
		public static DateTime FirstDayOfDST { get; set; }

		/// <summary>
		/// first Sunday in November.  Used to calculate herb time for FA takeoff and land times from raw data
		/// </summary>
		public static DateTime LastDayOfDST { get; set; }



		private static List<CityPair> _TtpCityPairs;
		public static List<CityPair> TtpCityPairs
		{
			get
			{
				return _TtpCityPairs ?? (_TtpCityPairs = new List<CityPair>());
			}
			set
			{
				_TtpCityPairs = value;
			}
		}

        public static List<BidAutoItem> QuickFilters { get; set; }

		public const decimal PltDpMinFactor = 5.0m;
		public const decimal PltDhrFactor = 0.74m;
		public const decimal PltAdgFactor = 6.5m;
		public const decimal PltTafbFactor = 0.333m;
		public const decimal FaDpMinFactor = 4.0m;
		public const decimal FaDhrFactor = 0.74m;
		public const decimal FaAdgFactor = 6.5m;
		public const decimal FaTafbFactor = 0.333m;


		public const int FAshow1stDutyPeriod = 60;
		public const int FAshow = 30;
		public const int FArelease = 30;
		public const int FAreleaseLastDutyPeriod = 30;
		public const decimal FAReserveDayPay = 6.0m;

		public static bool IsOverlapCorrection { get; set; }

		public const int requiredRest = 600;

		/// <summary>
		/// Store the last leg arrival time of the previous bid period to test rest  legality condition(atleat 9 hours between lead out days and lead in days )
		/// </summary>
		public static int LastLegArrivalTime { get; set; }

		private static ObservableCollection<Trip> _trip;
		public static ObservableCollection<Trip> Trip
		{
			get
			{
				return _trip ?? (_trip = new ObservableCollection<Trip>());
			}
			set
			{
				_trip = value;
			}
		}

		public const int ReserveAmPmClassification = 510;  // 510 in minutes is 08:30

		public static bool IsVacationCorrection { get; set; }

		public const decimal dutyHrRig = 60m / .74m;
		public const decimal adgRig = 6.5m;
		public const decimal tafbRig = 180m;
		public const decimal dailyMinRig = 5.0m;
		public const decimal fltMinRig = 1.0m;
		public const decimal lineGuar31day = 89.0m;
		public const decimal lineGuar30day = 87.0m;
		public const decimal lineGuar2829Day = 85.0m;


		/// <summary>
		/// Store Overlap correction dayas and flight time from the overlap corrrection Dialogue
		/// </summary>
		public static List<Day> LeadOutDays { get; set; }

		public const decimal ReserveDailyGuarantee = 6.0m;

		public static SeniorityListMember SeniorityListMember { get; set; }

		public static List<Absense> OrderedVacationDays { get; set; }


		public static List<Absense> TempOrderedVacationDays { get; set; }

		public static string CompanyVA { get; set; }

		public static decimal DailyVacPay = 3.75m;

		public static string WBidDownloadFileUrl = "http://www.wbidmax.com/downloads/swa/";

		public static List<FlightRouteDetails> FlightRouteDetails { get; set; }


		public static WBidStateCollection WBidStateCollection { get; set; }

		public static DateTime FAEOMStartDate { get; set; }

		public static MenuBarButtonStatus MenuBarButtonStatus { get; set; }

		public static Dictionary<string,TripMultiVacData> VacationData { get; set; }

		public const int connectTime = 40;

		public const decimal HotelCostCP = 0.4m;
		public const decimal HotelCostFO = 0.6m;
		public const decimal VDvsRCtfpFactor = 1.0m;
		//public const int LastLandingMinus1440 = 275;        // 4:35 am herb, earliest takeoff in database is 280 which is 4:40 am herb
		public const int LastLandingMinus1440 = 179;        // 2:59 is the end of SWA flight day

		public const int EarliestTakeOffMinutes = 180;
		public const int DutyDayMinutes = 720;
		public const int RcConnect = 30;


		public static SplitPointCities SplitPointCities { get; set; }

		public static List<City> OverNightCitiesInBid { get; set; }

		private static ObservableCollection<Line> _lines;
		public static ObservableCollection<Line> Lines
		{
			get
			{
				return _lines ?? (_lines = new ObservableCollection<Line>());
			}
			set
			{
				_lines = value;
			}
		}


		public static List<string> AllCitiesInBid { get; set; }

		public static List<Absense> MILDates { get; set; }
		public static Dictionary<string, TripMultiMILData> MILData { get; set; }


        public static bool IsDownloadProcess { get; set; }


        public static int InternetType { get; set; }



          public static string OperatingSystem = "iPhone OS";
          public static string Platform = "iPhone";

          public static bool IsCurrentMonthOn;

          public static bool IsVPSServiceOn;


          public static bool IsNeedToRedirectToBase;

          public static bool IsNeedToReload;

		public static string QAScrapPassword {
			get;
			set;
		}
        public static List<Absense> FVVacation{ get; set; }
        public static bool IsFVVacation { get; set; }
		public static bool iSNeedToShowMonthtoMonthAlert = false;


		public static bool IsNeedToEnableVacDiffButton = false;
	}
}

