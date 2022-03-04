using System;
using System.Xml.Serialization;

namespace Bidvalet
{
	public class Cx2Parameter
	{
		[XmlAttribute("Type")]

		public int Type { get; set; }

		[XmlAttribute("Value")]

		public int Value { get; set; }

	}
}

