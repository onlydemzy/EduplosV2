using Eduplus.DTO.CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.CoreModule
{
    public class RemitaPaymentDataWithLineItems:GatewayPaymentDataBaseDTO
    {
        public List<lineItems> lineItems { get; set; }
        public List<CustomFields> customFields { get; set; }
    }
    public class RemitaPaymentDataWithNoLineItems : GatewayPaymentDataBaseDTO
    {
        public List<CustomFields> customFields { get; set; }
    }

    public class lineItems
    {
        public string lineItemsId { get; set; }
        public string beneficiaryName { get; set; }
        public string beneficiaryAccount { get; set; }
        public string bankCode { get; set; }
        public string beneficiaryAmount { get; set; }
        public string deductFeeFrom { get; set; }
    }
    public class RRRResponse
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }
    public class remitaRRRGenResponse
    {
        public string RRR { get; set; }
        public string status { get; set; }//Payment Reference generated
        public string statuscode { get; set; }
        public string statusMessage { get; set; }
    }
}
