using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.BursaryModule
{
    public class PaymentInvoiceDTO
    {
        public string TransactionId { get; set; }
        public string Name { get; set; }
        public string PaymentType { get; set; }
        public string ProgrammeType { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Session { get; set; }
         public int? LevelToPay { get; set; }
        public string Semeseter { get; set; }
        public string Regno { get; set; }
        public string StudentId { get; set; }
        public string Particulars { get; set; }
        public DateTime GeneratedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string GeneratedBy { get; set; }
        public string Installment { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public string TransRef { get; set; }
        public byte[] Photo { get; set; }
        public string ApprovalChannel { get; set; }
        public int? PayOptionId { get; set; }
        public double ServiceCharge { get; set; }
        
        
        public string Phone { get; set; }


        public string Email { get; set; }
        public List<InvoiceDetailsDTO> Details { get; set; }

    }

    public class InvoiceDetailsDTO
    {
        public int DetailId { get; set; }
        public string Item { get; set; }
        public string ItemCode { get; set; }
        public double Amount { get; set; }
        public string InvoiceNo { get; set; }
    }
}
