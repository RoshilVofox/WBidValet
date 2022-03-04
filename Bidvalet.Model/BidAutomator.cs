using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
   
    public class BidAutomator
    {

        public BidAutomator()
        {

        }

        [XmlElement("BAFilter")]
        public List<BidAutoItem> BAFilter { get; set; }


        [XmlElement("BAWeight")]
        public List<BidAutoItem> BAWeight { get; set; }


        [XmlElement("BASort")]
        public SortDetails BASort { get; set; }

        //[XmlAttribute("IsRABBottom")]
        //public bool IsReserveAndBlankBottom { get; set; }

        [XmlAttribute("IsResBottom")]
        public bool IsReserveBottom { get; set; }

        [XmlAttribute("IsBlnBottom")]
        public bool IsBlankBottom { get; set; }

        [XmlAttribute("IsRFirst")]
        public bool IsReserveFirst { get; set; }


        [XmlElement("ComTimes")]
        public List<CommuteTime> DailyCommuteTimes { get; set; }


        [XmlElement("BAGroup")]
        public List<BidAutoGroup> BAGroup { get; set; }




    }
}
