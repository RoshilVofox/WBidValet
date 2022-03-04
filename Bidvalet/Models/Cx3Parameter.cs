using System;
using System.Xml.Serialization;

namespace Bidvalet
{
	
	public class Cx3Parameter
	{
		[XmlAttribute("ThirdCell")]

		public string ThirdcellValue { get; set; }

		[XmlAttribute("Type")]

		public int Type { get; set; }

		[XmlAttribute("Value")]

		public int Value { get; set; }

	}
}

