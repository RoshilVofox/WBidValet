using System;

namespace Bidvalet
{
	public class ClientRequestModel
	{
		
		public string Platform{ get; set; }

		public string OperatingSystem { get; set; }

		public string Version { get; set; }

		public string Base { get; set; }

		public int BidRound { get; set; }

		public string Postion { get; set; }

		public string Month { get; set; }

		public int EmployeeNumber { get; set; }

		public Guid Token { get; set; }


	}
}

