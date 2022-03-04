using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
	public class EquipType
	{
		/// <summary>
		/// Id
		/// </summary>
		[XmlAttribute("Id")]
		public int Id { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		[XmlAttribute("Name")]
		public string Name { get; set; }
	}
}

