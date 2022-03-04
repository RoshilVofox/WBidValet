using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
//using WBid.WBidClient.Models;

namespace Bidvalet.Model
{
    public class BidAutoItem
    {
        public BidAutoItem()
        {

        }


        [XmlAttribute("Priority")]
        public int Priority { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("BidAuto")]
        [XmlElementAttribute("AMPMConstriants", typeof(AMPMConstriants))]
        [XmlElementAttribute("CxDays", typeof(CxDays))]
        [XmlElementAttribute("Cx3Parameter", typeof(Cx3Parameter))]
        [XmlElementAttribute("CxLine", typeof(CxLine))]
        [XmlElementAttribute("FtCommutableLine", typeof(FtCommutableLine))]
        [XmlElementAttribute("DaysOfMonthCx", typeof(DaysOfMonthCx))]
        [XmlElementAttribute("CxTripBlockLength", typeof(CxTripBlockLength))]
        [XmlElementAttribute("BulkOvernightCityCx", typeof(BulkOvernightCityCx))]

        public object BidAutoObject { get; set; }

        [XmlAttribute("IsApplied")]
        public bool IsApplied { get; set; }

    }
}
