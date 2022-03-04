using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidvalet.Model
{
    public class MailInformation
    {

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string Subject { get; set; }

        public string MessageBody { get; set; }

        public string Alias { get; set; }

        public byte[] Attachment1 { get; set; }

        public string Attachment1Name { get; set; }

        public string BaseAttachment1 { get; set; }

        public int EmployeeNumber { get; set; }

        public string UserAppEmail { get; set; }

        public int AppNum { get; set; }

    }
}
