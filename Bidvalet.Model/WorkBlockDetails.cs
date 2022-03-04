using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
	[ProtoContract]
	public class WorkBlockDetails
	{
		[ProtoMember(1)]
		public int StartTime { get; set; }

		[ProtoMember(2)]
		public int EndTime { get; set; }
		[ProtoMember(3)]
		public int StartDay { get; set; }

		[ProtoMember(4)]
		public int EndDay { get; set; }
		[ProtoMember(5)]
		public int BackToBackCount { get; set; }
        [ProtoMember(6)]
        public DateTime StartDateTime { get; set; }
        [ProtoMember(7)]
        public DateTime EndDateTime { get; set; }
        [ProtoMember(8)]
        public DateTime EndDate { get; set; }
	}
}

