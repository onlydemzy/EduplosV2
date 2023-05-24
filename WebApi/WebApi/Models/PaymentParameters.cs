using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class PaymentParameters
    {
        public string TRANSACTION_ID { get; set; }
        public double AMOUNT { get; set; }
        public string CHECKSUM { get; set; }
        public string DESCRIPTION { get; set; }
        public string RESPONSE_URL { get; set; }
        public string LOGO_URL { get; set; }
        public string TRANSACTION_REF { get; set; }
        public string FINAL_CHECKSUM { get; set; }
        public string SUCCESS { get; set; }
        public string KEY { get; set; }
        public string COL5 { get; set; }
    }
}