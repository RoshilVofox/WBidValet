using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
    [ProtoContract]
    public class BidLineIcon
    {
        //RoundedLeft = 1,
        //SquareCorner=2,
        //RoundedRight=3,
        //RoundedOnBoth=4
        //Type of the Dutyperiod Icon
        [ProtoMember(1)]
        public int DutyPeriodIcon { get; set; }
        [ProtoMember(2)]
        public string ColorTop { get; set; }
        [ProtoMember(3)]
        public string ColorBottom { get; set; }
        [ProtoMember(4)]
        public DateTime Date { get; set; }
    }
}
