#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization; 
#endregion

namespace Bidvalet.Model
{
	
	public  class Wt
	{

		[XmlAttribute("Key")]
		public int Key { get; set; }

		[XmlAttribute("Value")]
		public decimal Value { get; set; }

		public Wt()
		{

		}


		public Wt(Wt wt)
		{
			Key = wt.Key;
			Value = wt.Value;

		}
	}
}

