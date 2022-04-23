using System;
using System.Collections.Generic;

namespace Bidvalet.Model
{
    public class VacationValueDifferenceOutputDTO
    {
        public VacationValueDifferenceOutputDTO()
        {
        }
        public string VacFileName { get; set; }
        public List<FlightDataChangeVacValues> lstFlightDataChangeVacValues { get; set; }
    }
}
