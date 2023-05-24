using System;
using System.Collections.Generic;

namespace Eduplus.Domain.AcademicModule
{
    public class ResultComplain
    {
        public ResultComplain()
        {
            Details = new HashSet<ResultComplainDetail>();
        }
        public int ComplainId { get; set; }
        public string DepartmentCode { get; set; }
        public string ProgrammeCode { get; set; }
        public string Programme { get; set; }
        public string Complain { get; set; }
        public bool HODFlag { get; set; }
        public string HODComment { get; set; }
        public bool CourseLecturerFlag { get; set; }
        public string CourseLecturerComment { get; set; }
        public bool VCFlag { get; set; }
        public string VCComment { get; set; }
        public int SemesterId { get; set; }
        public int SessionId { get; set; }
        public int OldCA1 { get; set; }
        public int OldCA2 { get; set; }
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public int OldExam { get; set; }
        public int NewExam { get; set; }
        public int NewCA1 { get; set; }
        public int NewCA2 { get; set; }
        public string StudentId { get; set; }
        public string MatricNumber { get; set; }
        public int RegistrationId { get; set; }
         
        public string Semester { get; set; }
        public string Session { get; set; }
        public bool Treated { get; set; }
        public string ExamsOfficer { get; set; }
        public string HOD { get; set; }
        public string LecturerId { get; set; }
        public string HODId { get; set; }
        public string VCId { get; set; }
        public string VC { get; set; }
        public DateTime RaisedDate { get; set; }
        public DateTime InputtedDate { get; set; }
        public DateTime? HODFlagDate { get; set; }
        public DateTime? VCFlagDate { get; set; }
        public DateTime? CourseLecturerFlagDate { get; set; }
        public virtual ICollection<ResultComplainDetail> Details { get; set; }
    }
}
