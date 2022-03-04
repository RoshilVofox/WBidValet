using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Bidvalet.Model.OldLine
{
    [ProtoContract]
    public class OldLineInfo
    {
        
            [ProtoMember(1)]
            public string LineVersion { get; set; }
            [ProtoMember(2)]
            public Dictionary<string, OldLine> Lines { get; set; }
        
    }
}
