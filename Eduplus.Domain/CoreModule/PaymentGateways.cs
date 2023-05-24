using Eduplus.Domain.AcademicModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Domain.CoreModule
{
    public class PaymentGateways
    {
        public int GatewayId { get; set; }
        public string Name { get; set; }
        public string MerchantCode { get; set; }
        public string ServiceId { get; set; }
        public string MerchantKey { get; set; }
        public string TransConfirmationUrl { get; set; }
        public string TransRefGenUrl { get; set; }
        public string ResponseUrl { get; set; }
        public string TransPostUrl { get; set; }
        public double GatewayCharge { get; set; }
        public string ProviderAccountNumber { get; set; }
        public string ProviderAccountName { get; set; }
        public string ProviderBankCode { get; set; }
        public double ProviderMajorFee { get; set; }
        public double ProviderMinorFee { get; set; }
        public string ClientAccountNumber { get; set; }
        public string ClientAccountName { get; set; }
        public string ClientBankCode { get; set; }
        public bool IsDefault { get; set; }
        public string ProgrammeTypeCode { get; set; }
        public virtual ProgrammeTypes ProgrammeType { get; set; }

    }
}
