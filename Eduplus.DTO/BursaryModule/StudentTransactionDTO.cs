using System;
using System.Collections.Generic;

namespace Eduplus.DTO.BursaryModule
{
    public class StudentTransactionDTO
    {
        public string StudentId { get; set; }
        public DateTime TransDate { get; set; }
        public string Type { get; set; }
        public string Bank { get; set; }
        public double Amount { get; set; }
        public string Particulars { get; set; }
        public string PaymentId { get; set; }
    }

    public class StudentAccountStatementDTO
    {
        public string Name { get; set; }
        public string MatricNo { get; set; }
        public string Department { get; set; }
        public string Programme { get; set; }
        public string Yearadmitted { get; set; }
        public double Balance { get; set; }
        public List<StudentTransactionDTO> Details { get; set; }
    }
}
