using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
    public class CxDays
    {

        [XmlAttribute("IsSun")]
        public bool IsSun { get; set; }

        [XmlAttribute("IsMon")]
        public bool IsMon { get; set; }

        [XmlAttribute("IsTue")]
        public bool IsTue { get; set; }

        [XmlAttribute("IsWed")]
        public bool IsWed { get; set; }

        [XmlAttribute("IsThu")]
        public bool IsThu { get; set; }

        [XmlAttribute("IsFri")]
        public bool IsFri { get; set; }

        [XmlAttribute("IsSat")]
        public bool IsSat { get; set; }
    }
}
