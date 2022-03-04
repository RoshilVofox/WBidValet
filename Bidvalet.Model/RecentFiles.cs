﻿using System;
using System.Collections.Generic;

namespace Bidvalet.Model
{
    public class RecentFiles : List<RecentFile>
    {
    }
    public class RecentFile
    {
        public string Domcile { get; set; }
        public string Round { get; set; }
        public string Position { get; set; }
        public int Month { get; set; }
        public string MonthDisplay { get; set; }
        public string Year { get; set; }
    }
}
