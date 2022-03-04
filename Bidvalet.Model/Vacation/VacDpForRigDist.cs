using System;
namespace Bidvalet.Model
{
    public class VacDpForRigDist
    {
        public VacDpForRigDist()
        {
        }
        public decimal Tfp { get; set; }
        public int DutPdSeqNum { get; set; }
        public string TripName { get; set; }
        public decimal THR { get; set; }
        public decimal ADG { get; set; }
        public decimal DPM { get; set; }
        public decimal DHR { get; set; }
        public string VacType { get; set; }
    }
}
