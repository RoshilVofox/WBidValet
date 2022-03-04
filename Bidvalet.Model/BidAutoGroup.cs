using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
    public class BidAutoGroup
    {
        public BidAutoGroup()
        {

        }


        [XmlAttribute("GrpName")]
        public string GroupName { get; set; }

        [XmlElement("Lines")]
        public List<int> Lines { get; set; }
    }
}
