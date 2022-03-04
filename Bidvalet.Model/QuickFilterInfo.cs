using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
    [ProtoContract]
    public class QuickFilterInfo
    {
        [ProtoMember(1)]
        public string QuickFilterVersion { get; set; }
        [ProtoMember(2)]
        public List<BidAutoItem> QuickFilters { get; set; }
    }
}
