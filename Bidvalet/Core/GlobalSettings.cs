using System;

namespace Bidvalet
{
	public class GlobalSettings
	{
		public static string ServerUrl="www.wbidmax.com";

		public static string ApplicationName="BidValet";

		public static string Domicile;

		public static bool isTestSouthWifiOn;

		public static string WBidAuthenticationServiceUrl= "http://192.168.10.100/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc";


		public static bool buddyBidTest = false;

		public static BidDetails DownloadBidDetails { get; set; }

		public static BidDetails CurrentBidDetails { get; set; }


		public static UserInformation UserInfo { get; set; }

		public static string SessionCredentials{ get; set; }

	}
}

