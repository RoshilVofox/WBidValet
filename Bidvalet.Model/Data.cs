using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
	
	public class Data
	{
		[XmlAttribute("IsCompanyData")]
		public bool IsCompanyData { get; set; }

	}
}

