using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
    public class PaymentUpdateModel
    {
        public int EmpNum { get; set; }
        public int Month { get; set; }
        public string Message { get; set; }
        public string IpAddress { get; set; }
        public string TransactionNumber { get; set; }

        public int AppNum { get; set; }
    }
}
