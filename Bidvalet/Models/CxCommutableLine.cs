using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Bidvalet
{
	public class CxCommutableLine
	{
		[XmlElement("TimesList")]
		public List<Times> TimesList { get; set; }

		[XmlAttribute("CommuteToHome")]
		public bool CommuteToHome { get; set; }

		[XmlAttribute("CommuteToWork")]
		public bool CommuteToWork { get; set; }

		[XmlAttribute("AnyNight")]
		public bool AnyNight { get; set; }

		[XmlAttribute("RunBoth")]
		public bool RunBoth { get; set; }

		public Times MondayThu { get; set; }

		public Times Friday { get; set; }

		public Times Saturday { get; set; }

		public Times Sunday { get; set; }

		public Times MondayThuDefault { get; set; }

		public Times FridayDefault { get; set; }

		public Times SaturdayDefault { get; set; }

		public Times SundayDefault { get; set; }


	}
}

