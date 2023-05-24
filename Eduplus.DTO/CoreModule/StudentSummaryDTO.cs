using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.CoreModule
{
    public class StudentSummaryDTO
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string AdmissionStatus { get; set; }
        public string Phone { get; set; }
        public string Programme { get; set; }
        public string MatricNumber { get; set; }
        public byte[] Foto { get; set; }
        public string JambRegNumber { get; set; }
        public DateTime? DateAdmitted { get; set;}
        public byte Duration { get; set; }
        public string ProgrammeType { get; set; }
        public string Department { get; set; }
         public string Faculty { get; set; }
        public string YearAdmitted { get; set; }
        public string Award { get; set; }
        public string ProgrammeCode { get; set; }
         
    }

}
