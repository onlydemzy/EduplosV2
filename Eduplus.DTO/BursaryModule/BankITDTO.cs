using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.BursaryModule
{
    public class BankITDTO
    {
        public string TransactionId { get; set; }
        public string Checksum { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string TerminalID { get; set; }
        public string ResponseUrl { get; set; }
        public string NotificationURL { get; set; }
        public string LogoUrl { get; set; }
        public string Col1 {get;set;}
        public string FinalChecksum { get; set; }
        public string TransactionRef { get; set; }
        public string ResponseMessage { get; set; }
        public string SecretKey { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; } 
        public string COL5 { get; set; }
    }
}
