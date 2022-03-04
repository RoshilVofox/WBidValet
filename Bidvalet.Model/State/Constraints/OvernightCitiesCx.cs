using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Bidvalet.Model
{
	public class OvernightCitiesCx
	{
		[XmlElement("Yes")]
		public List<string> Yes { get; set; }

		[XmlElement("No")]
		public List<string> No { get; set; }
	}
}

