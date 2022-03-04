﻿using System;
using System.Collections.Generic;

namespace Bidvalet.Model
{
    public class AwardDetails
    {
        public AwardDetails()
        {
        }
        public int EmployeeNumber { get; set; }
        public string Position { get; set; }

        public bool IsPaperbid { get; set; }
        public bool ReserveLine { get; set; }
        public bool BlankLine { get; set; }
        public int AwardedLine { get; set; }
        public List<BuddyAward> BuddyAwards { get; set; }
    }
    public class BuddyAward
    {
        public int BuddyEmpNum { get; set; }
        public string BuddyName { get; set; }
        public string BuddyPosition { get; set; }

    }
}