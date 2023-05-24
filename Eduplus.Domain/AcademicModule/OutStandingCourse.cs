using Eduplus.Domain.CoreModule;

namespace Eduplus.Domain.AcademicModule
{
    public class OutStandingCourse
    {
        public int OutStandingCourseId { get; set; }
        //public string RegNo { get; set; }
        public string CourseId { get; set; }
        public string OwingType { get; set; }//OutStanding, Repeat
        public string StudentId { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        //public string CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
        public virtual Semester Semester { get; set; }
        public virtual Session Session { get;set;}
        public bool Owing { get; set; }
       
    }
}
