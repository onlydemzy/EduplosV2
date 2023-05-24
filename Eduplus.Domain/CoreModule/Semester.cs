using KS.Common;
using System;

namespace Eduplus.Domain.CoreModule
{
    public class Semester:EntityBase
    {
        public int SemesterId { get; set; }
        public string SemesterTitle { get; set; }
        public int SessionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime LateRegistrationStartDate { get; set; }
        public DateTime LateRegistrationEndDate { get; set; }
         
        public bool ApplyLate { get; set; }
         

        public virtual Session Session { get; set; }
        
    }
}
