using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Bidvalet.Model
{
	[ProtoContract]
	public class WorkDaysOfBidLine
	{
		[ProtoMember(1)]
		public DateTime DayOfBidline { get; set; }
		[ProtoMember(2)]
		public bool Working { get; set; }
	}
}

