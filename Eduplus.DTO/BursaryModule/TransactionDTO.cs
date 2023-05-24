using System;

namespace Eduplus.DTO.BursaryModule
{
    public class TransactionDTO
    {
        public string PaymentId { get; set; }
        public string StudentId { get; set; }
        public string AccountCode { get; set; }
        public string TransType { get; set; }
        public double Amount { get; set; }
        public string Particulars { get; set; }
        public string Semester { get; set; }
        public int SessionId { get; set; }
        public DateTime PayDate { get; set; }
        public string PaidBy { get; set; }
        public string PayMethod { get; set; }
        public string VoucherNumber { get; set; }
        public string BankAccountCode { get; set; }
        public string InputtedBy { get; set; }
        public string Installment { get; set; }
    }
}
