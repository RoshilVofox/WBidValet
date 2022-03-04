using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
	[DataContract]

	public class Theme
	{
		public  Theme()
		{ 
		}
		public Theme(Theme theme)
		{
			FontType = theme.FontType;
			FontSize = theme.FontSize;
			//Background = theme.Background;
			//TextColor = theme.TextColor;
		}

		[DataMember]
		[XmlAttribute("FontType")]
		public string FontType { get; set; }

		[DataMember]
		[XmlAttribute("FontSize")]
		public string FontSize { get; set; }

//		[DataMember]
//		[XmlElement("Background")]
//		public BackgroundColor Background { get; set; }
//
//		[DataMember]
//		[XmlElement("Text")]
//		public TextColor TextColor { get; set; }



	}
}

