using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
    public class FtCommutableLine
    {

        [XmlAttribute("Connect")]
        public int ConnectTime { get; set; }

        [XmlAttribute("Commute")]
        public int CommuteCity { get; set; }

        [XmlAttribute("City")]
        public string City { get; set; }

        [XmlAttribute("CheckIn")]
        public int CheckInTime { get; set; }

        [XmlAttribute("Base")]
        public int BaseTime { get; set; }

        [XmlAttribute("NoNights")]
        public bool NoNights { get; set; }

        [XmlAttribute("ToWork")]
        public bool ToWork { get; set; }

        [XmlAttribute("ToHome")]
        public bool ToHome { get; set; }

        [XmlAttribute("isNonStop")]
        public bool isNonStop { get; set; }
        
    }
}
