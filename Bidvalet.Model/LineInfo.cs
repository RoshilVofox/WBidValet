using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
namespace Bidvalet.Model
{
	[ProtoContract]
	public class LineInfo
	{
		[ProtoMember(1)]
		public string LineVersion { get; set; }
		[ProtoMember(2)]
		public Dictionary<string, Line> Lines { get; set; }
	}
}

