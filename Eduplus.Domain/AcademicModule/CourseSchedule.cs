using KS.Domain.HRModule;

namespace Eduplus.Domain.AcademicModule
{
    public class CourseSchedule
    {
        public long ScheduleId { get; set; }

        public string CourseId { get; set; }
        public int SemesterId { get; set; }

        public string ProgrammeCode { get; set; }
        public virtual Course Course { get; set; }
        public string LecturerId { get; set; }
        public virtual Programme Programme { get; set; }
        public virtual Staff Lecturer { get; set; }
        public virtual CoreModule.Semester Semester {get;set;}
         
     
    }
    
}
