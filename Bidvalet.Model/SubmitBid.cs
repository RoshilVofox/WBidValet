using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
    public class SubmitBid
    {
        public string Base { get; set; }
        public string Bid { get; set; }
        public string BidRound { get; set; }
        public string Bidder { get; set; }
        public string PacketId { get; set; }
        public string Seat { get; set; }
        public string Pilot1 { get; set; }
        public string Pilot2 { get; set; }
        public string Pilot3 { get; set; }
        public int SeniorityNumber { get; set; }
        public string Buddy1 { get; set; }
        public string Buddy2 { get; set; }
        public bool IsSubmitAllChoices { get; set; }
        public bool IsAddReserveToEnd { get; set; }
        public bool IsRepeatline { get; set; }

        public bool IsReserveOnly { get; set; }
       // public List<BuddyBidderBid> BuddyBidderBids { get; set; }
        public int TotalBidCount { get; set; }

        public  List<string> PositionChoices{ get; set; }
    }
}
