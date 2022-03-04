using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Bidvalet.Model
{
	public class DaysOfMonthCx
	{
		[XmlElement("OFFDays")]
		public List<int> OFFDays { get; set; }

		[XmlElement("WorkDays")]
		public List<int> WorkDays { get; set; }
	}
}

