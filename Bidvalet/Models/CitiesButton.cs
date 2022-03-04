using System;

namespace Bidvalet
{
	public class CitiesButton
	{
		public int CountClicked{ get; set;}
		public string CityName{ get; set;}
		public CitiesButton ()
		{
		}
		public CitiesButton(string CityName, int Count) {
			this.CountClicked = Count;
			this.CityName = CityName;
		}

	}
}

