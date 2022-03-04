﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
    public class CorrectionParams
    {
        public bool IsOverlap { get; set; }

        public bool IsVacation { get; set; }

        public bool IsDrop { get; set; }

        public bool IsEom { get; set; }

        public bool IsMIL { get; set; }

        public VacationTrip vacTrip { get; set; }

        public string vacationType { get; set; }

        public List<LineSip> LineSips { get; set; }

        public int selectedLineNum { get; set; }



    }
}
