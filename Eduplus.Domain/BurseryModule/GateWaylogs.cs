using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Domain.BurseryModule
{
    public class GateWaylogs
    {
        public int LogId { get; set; }
        public string ReceiptNo { get; set; }
        public string PaymentCode { get; set; }
        public string MerchantCode { get; set; }
        public double Amount { get; set; }
        public DateTime TransDate { get; set; }
        public string Description { get; set; }
        public string PayeeId { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string ServiceId { get; set; }
        public string PayeeName { get; set; }
        public string Address { get; set; }
        public string TellerId { get; set; }
        public string Password { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string Channelname { get; set; }
        public string PaymentMethod { get; set; }
        public string TransType { get; set; }
        public string TypeName { get; set; }
        public string ServerResponse { get; set; }
        public string TransFee { get; set; }
        public string COL5 { get; set; }
        public DateTime LogDate { get; set; }
        public DateTime? DebitDate { get; set; }

    }
}
