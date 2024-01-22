using Eduplos.Domain.AcademicModule;
using System.Collections.Generic;

namespace Eduplos.Domain.CoreModule
{
    public class Department
    {
        public string DepartmentCode { get; set; }
        public string FacultyCode { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsAcademic { get; set; }
        public string Location { get; set; }
        public virtual Faculty Faculty { get; set; }
        
    }
}
