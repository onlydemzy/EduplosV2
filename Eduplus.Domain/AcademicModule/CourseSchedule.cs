using Eduplus.Domain.CoreModule;
using System.Collections.Generic;

namespace Eduplus.Domain.AcademicModule
{
    public class CourseSchedule
    {
        public long ScheduleId { get; set; }
        
        public string CourseId { get; set; }
        public int SemesterId { get; set; }
        public string Session { get; set; }
        public string DepartmentCode { get; set; }
        public string ProgrammeCode { get; set; }
        public virtual Course Course { get; set; }    
        public virtual ICollection<CourseScheduleDetails> CourseScheduleDetails { get; set; }  
     
    }
    public class CourseScheduleDetails
    {
        public long DetailsId { get; set; }
        public string LecturerId { get; set; }
        public string LecturerName { get; set; }
        public long ScheduleId { get; set; }
        public virtual CourseSchedule CourseSchedule { get; set; }
        
    }
}
