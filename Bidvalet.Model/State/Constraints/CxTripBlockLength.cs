using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
    public class CxTripBlockLength
    {
        [XmlAttribute("T")]
        public bool Turns { get; set; }
        [XmlAttribute("twod")]
        public bool Twoday { get; set; }
        [XmlAttribute("threed")]
        public bool ThreeDay { get; set; }
        [XmlAttribute("fourd")]
        public bool FourDay { get; set; }
        [XmlAttribute("IsBlock")]
        public bool IsBlock { get; set; }
    }
}
