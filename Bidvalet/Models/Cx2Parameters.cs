using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Bidvalet.Model;

namespace Bidvalet
{
	public class Cx2Parameters
	{
		[XmlAttribute("Type")]
		public int Type { get; set; }

		[XmlAttribute("Value")]
		public int Value { get; set; }

		public List<Cx2Parameter> lstParameters { get; set; }

	}
}

