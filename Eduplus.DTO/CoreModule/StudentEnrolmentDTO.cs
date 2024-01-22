using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.CoreModule
{
    public class StudentEnrolmentDTO
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        
        public string Phone { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public DateTime? DateAdmitted { get; set;}
        public string ProgrammeType { get; set; }
         
        public string YearAdmitted { get; set; }
        
        
    }

}
