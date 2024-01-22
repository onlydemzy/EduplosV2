using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.AcademicModule
{
    public class ProspectiveStudentDTO
    {
        public string Password { get; set; }
        public string StudentId { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string FullName { get; set; }
        public string MIddlename { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string ProgramType { get; set; }
        public string ProgrammeCode { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        
    }
}
