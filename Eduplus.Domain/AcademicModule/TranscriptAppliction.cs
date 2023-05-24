using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Domain.AcademicModule
{
    public class TranscriptApplication
    {
        public int ApplicationId { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string DeliveryEmail { get; set; }
        public string Recipient { get; set; }
        public string DeliveryMode { get; set; }
        public string Student { get; set; }
        public string StudentId { get; set; }
        public string Matricnumber { get; set; }
        public string Status { get; set; }
        public DateTime DateApplied { get; set; }
        public DateTime? DateDelivered { get; set; }
        public string TranscriptNo { get; set; }
        public double AmountToPay { get; set; }
        public string Phone { get; set; }
        
    }
}
