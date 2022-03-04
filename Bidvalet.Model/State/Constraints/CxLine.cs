using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bidvalet.Model
{
    public class CxLine
    {
        [XmlElement("Hard")]
        public bool Hard { get; set; }


        /// <summary>
        /// PURPOSE : Blank
        /// </summary>
        [XmlElement("Blank")]
        public bool Blank { get; set; }


        /// <summary>
        /// PURPOSE : Reserve
        /// </summary>
        [XmlElement("Reserve")]
        public bool Reserve { get; set; }

        /// <summary>
        /// PURPOSE : Ready
        /// </summary>
        [XmlElement("Ready")]
        public bool Ready { get; set; }

        /// <summary>
        /// PURPOSE : International
        /// </summary>
        [XmlElement("International")]
        public bool International { get; set; }

        /// <summary>
        /// PURPOSE : NonConus
        /// </summary>
        [XmlElement("NonConus")]
        public bool NonConus { get; set; }

    }
}
