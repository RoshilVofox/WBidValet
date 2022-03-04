using System;
using System.Xml.Serialization;

namespace Bidvalet
{
	public class AMPMConstriants
	{
		[XmlAttribute("AM")]
		public bool AM { get; set; }

		[XmlAttribute("PM")]
		public bool PM { get; set; }

		[XmlAttribute("MIX")]
		public bool MIX { get; set; }

	}
}

