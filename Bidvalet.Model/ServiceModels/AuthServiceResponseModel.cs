using System;
using System.Runtime.Serialization;

namespace Bidvalet
{
	
	public class AuthServiceResponseModel
	{
		
		public bool IsAuthorized { get; set; }


		public int AuthRequestId { get; set; }


		public string Message { get; set; }


		public string Type { get; set; }


		public DateTime ExpirationDate { get; set; }


		public DateTime CBExpirationDate { get; set; }


		public DateTime WBExpirationDate { get; set; }


		public bool IsFree { get; set; }


		public bool IsYearlySubscribed { get; set; }


		public bool IsMonthlySubscribed { get; set; }


		public string TopSubscriptionLine { get; set; }

		
		public string SecondSubscriptionLine { get; set; }

		
		public string ThirdSubscriptionLine { get; set; }

        public bool IsNeedToDownloadSeniorityFromServer { get; set; }

		public string FlightDataVersion { get; set; }

	}
}

