using System;

namespace KS.Domain.AccountsModule
{
    public class TransMaster
    {
        public long TransId { get; set; }
        public string TransRef { get; set; }
        public string Particulars { get; set; }
        public string PayMethod { get; set; }
        public string TransType { get; set; }//Debit, credit
        public double Amount { get; set; }
        public string AccountCode { get; set; }
        public string Bank { get;set;}
        public string TellerNo { get; set; }
        public DateTime TransDate { get;set;}
        public virtual Accounts Accounts { get; set; }
        

    }
}
