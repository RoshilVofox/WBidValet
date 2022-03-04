using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
    public class HistoricalBidDetails
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int Round { get; set; }

        public string Domicile { get; set; }

        public string Position { get; set; }

        public string FileName { get; set; }
    }

    public class HistoricalFileInfo
    {

        public string Title { get; set; }


        public byte[] Data { get; set; }


        public string DataString { get; set; }
    }
}
