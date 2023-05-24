using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.AcademicModule
{
    public class ProgrammeDTO
    {
        public string ProgrammeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProgrammeType { get; set; }//masters degree others
        public string MatricNoFormat { get; set; }
        public bool IsActive { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public int TotalEnrollment { get; set; }
        public string Award { get; set; }
        public string Fullname
        {
            get { return this.Title + "-" + this.ProgrammeType; }
        }
        
    }
}
