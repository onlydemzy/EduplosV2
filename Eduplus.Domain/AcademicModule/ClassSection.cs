using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Domain.AcademicModule
{
    public class ClassSection
    {
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string CourseId { get; set; }
        public int Capacity { get; set; }
        
    }
    public class ClassProfile
    {
        public long ProfileId { get; set; }
        public string StudentId { get; set; }
        public string Student { get; set; }
        public string CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual ClassSection ClassSection { get; set; }
        public int SemesterId { get; set; }
        public int ClassSectionId { get; set; }
    }
}
