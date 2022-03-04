using System;

namespace Bidvalet.Droid
{
	public class BidEnum
	{
		public enum TypeOfSort
		{
			Standard,
			Block
		}
		public enum SortedOfKey
		{
			Single,
			Advance

		}
		public enum BidStartState{
			NEW_DOWNLOAD, VALID_SUB, EXPIRED_SUB
		}
        public enum FromApp
        {
            Wbidmax = 1,
            Crewbid,
            Wbidvalet,
            WbidmaxIpad,
            CrewbidApp,
            WbidmaxMACApp,
            WbidmaxWebApp

        }
		
	}
}

