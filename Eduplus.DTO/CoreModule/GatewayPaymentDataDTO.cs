using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.CoreModule
{
    public class GatewayPaymentDataBaseDTO
    {
        public string orderId { get; set; }
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string payerPhone { get; set; }
        public string description { get; set; }
        public string amount { get; set; }
        public string serviceTypeId { get; set; }
    }
    public class CustomFields
    {
        public string name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
}
