using Eduplos.Domain.CoreModule;
using KS.Domain.AccountsModule;
using System;

namespace Eduplos.Domain.BurseryModule
{
    public class StudentPayments
    {
        public string PaymentId { get; set; }
        public string RegNo { get; set; }
        public string TransType { get; set; }
        public double Amount { get; set; }
        public string Particulars { get; set; }
        public int SessionId { get; set; }
        public DateTime PayDate { get; set; }
        public string PaidBy { get; set; }
        public string PayMethod { get; set; }
        public string PaymentType { get; set; }
        public string VoucherNo { get; set; }
        public virtual Student Student{get;set;}
        public virtual Session Session { get; set; }
        public string ReferenceCode { get; set; }

    }
}
