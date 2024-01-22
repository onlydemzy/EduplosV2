using System;

namespace Eduplos.Domain.AcademicModule
{
    public class ResultComplainDetail
    {
        public int DetailId { get; set; }
        public int OldCA1 { get; set; }
        public int OldCA2 { get; set; }
        public string CourseId { get; set;  }
        public string CourseCode { get; set; }
        public int OldExam { get; set; }
        public int NewExam { get; set; }
        public int NewCA1 { get; set; }
        public int NewCA2 { get; set; }
        public string StudentId { get; set; }
        public string MatricNumber { get; set; }
        public int RegistrationId { get; set; }
        public int ComplainId { get; set; }
        public virtual ResultComplain Complain { get; set; }
    }
}
