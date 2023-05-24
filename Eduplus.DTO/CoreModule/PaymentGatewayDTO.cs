using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.CoreModule
{
    public class PaymentGatewayDTO
    {
        public string ReturnUrl { get; set; }
        public string Checksum { get; set; }
        public string TerminalId { get; set; }
        public string Key { get; set; }
        public string ResponseUrl { get; set; }
        public string LogoUrl { get; set; }
        public string GatewayConfirmationUrl { get; set; }
        public string MerchantCode { get; set; }
        public string ServiceTypeId { get; set; }
        public string GatewayPostUrl { get; set; }
        public string ProviderAccountNumber { get; set; }
        public string ProviderAccountName { get; set; }
        public string ProviderBankCode { get; set; }
        public string ProviderMajorFee { get; set; }
        public string ProviderMinorFee { get; set; }
        public string ClientAccountNumber { get; set; }
        public string ClientAccountName { get; set; }
        public string ClientBankCode { get; set; }
        public string GatewayCharge { get; set; }

    }
}
