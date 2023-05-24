using Eduplus.Domain.CoreModule;
using System;

namespace Eduplus.Domain.AcademicModule
{
    public class SemesterRegistrations
    {
        public int RegistrationId { get;set;}
        public int Lvl { get; set; }
        public string Session { get; set; }
        public string Semester { get; set; }
        public int SemesterId { get; set; }
        public string StudentId { get;set;}
        public DateTime RegisteredDate { get; set; }
        public string InputtedBy { get; set; }
        public virtual Student Student { get; set; }
    }
}
