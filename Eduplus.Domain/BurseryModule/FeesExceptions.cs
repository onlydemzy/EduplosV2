using Eduplus.Domain.CoreModule;
using System;

namespace Eduplus.Domain.BurseryModule
{
    public class FeesExceptions
    {
        public int ExceptionId { get;set;}
        public string StudentId { get;set;}
        public string RegNo { get; set; }
        public string StudentName { get; set; }
        public string Reason { get; set; }
        public string ExpectedByBackTime { get; set; }
        public bool IsApproved { get; set; }
        public int SemesterId { get; set; }
        public double AmountOwed { get; set; }
        public string AuthorizedBy { get; set; }
        
        public string InputtedBy { get; set; }
        public string Department { get; set; }
        public virtual Semester Semester { get; set; }
        public string Programme { get; set; }

    }
}
