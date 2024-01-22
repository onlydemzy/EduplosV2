using Eduplos.Domain.CoreModule;
using KS.Common;
using System;

namespace Eduplos.Domain.AcademicModule
{
    public class Course:EntityBase
    {
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public int CreditHours { get; set; }
        public string Semester { get; set; }
        public string DepartmentCode { get; set; }
        public int Level { get; set; }

        public string ProgrammeCode { get; set; }
        public bool IsActive { get; set; }

        public string CourseType { get; set; }
        public string Category { get; set; }
        public virtual Department Department { get; set; }
        public virtual Programme Programme { get; set; }
        
    }
}
