﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Bidvalet.Model
{
	public class BaseUpdates
	{

		/// <summary>
		/// Trips
		/// </summary>
		[XmlAttribute("Trips")]
		public int Trips { get; set; }

		/// <summary>
		/// News
		/// </summary>
		[XmlAttribute("News")]
		public int News { get; set; }

		/// <summary>
		/// Cities
		/// </summary>
		[XmlAttribute("Cities")]
		public int Cities { get; set; }

		/// <summary>
		/// Hotels
		/// </summary>
		[XmlAttribute("Hotels")]
		public int Hotels { get; set; }

		/// <summary>
		/// Domiciles
		/// </summary>
		[XmlAttribute("Domiciles")]
		public int Domiciles { get; set; }

		/// <summary>
		/// Equipment
		/// </summary>
		[XmlAttribute("Equipment")]
		public int Equipment { get; set; }

		/// <summary>
		/// EquipTypes
		/// </summary>
		[XmlAttribute("EquipTypes")]
		public int EquipTypes { get; set; }



	}
}

