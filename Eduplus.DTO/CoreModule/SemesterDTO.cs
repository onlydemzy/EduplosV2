using System;

namespace Eduplus.DTO.CoreModule
{
    public class SemesterDTO
    {
        public int SemesterId { get; set; }
        public string Title { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LateRegistrationStartDate { get; set; }
        public DateTime LateRegistrationEndtDate { get; set; }
        public bool ApplyLate { get; set; }
         
        public int SessionId { get; set; }
        
    }
}
