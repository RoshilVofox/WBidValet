using System;

namespace Bidvalet
{
	public class CustomServiceResponse
	{
		public bool status{get; set;}

		public string Message{ get; set;}

		public DateTime? CBExpirationDate{ get; set;}

		public DateTime? WBExpirationDate{ get; set;}
	}
}

