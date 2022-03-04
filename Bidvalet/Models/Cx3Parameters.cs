using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Bidvalet
{
	
	public class Cx3Parameters
	{
		[XmlAttribute("ThirdCell")]

		public string ThirdcellValue { get; set; }

		[XmlAttribute("Type")]

		public int Type { get; set; }

		[XmlAttribute("Value")]

		public int Value { get; set; }

		public List<Cx3Parameter> lstParameters { get; set; }

	}
}

