using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Domain.AcademicModule
{
    public class ExamsOfficer
    {
        public int OfficerId { get; set; }
        public string OfficerCode { get; set; }
        public string Fullname { get; set; }
        public string ProgrammeType { get; set; }
        public string DepartmentCode { get; set;}
        public string ProgrammeCode { get; set;}
        public string CourseCategory { get; set; }
        public string Roles { get; set; }
        public int? Lvl { get; set; }
        public string SessionAppointed { get; set; }
        public bool Iscurrent { get; set; }
    }
}
