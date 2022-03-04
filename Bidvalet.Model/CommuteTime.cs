using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
    public class CommuteTime
    {
        public CommuteTime()
        {

        }

        [XmlAttribute("Day")]
        public DateTime BidDay { get; set; }

        [XmlAttribute("Arv")]
        public DateTime EarliestArrivel { get; set; }

        [XmlAttribute("Dpt")]
        public DateTime LatestDeparture { get; set; }
    }
}
