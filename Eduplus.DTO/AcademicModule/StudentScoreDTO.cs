using System.Collections.Generic;

namespace Eduplos.DTO.AcademicModule
{

    public class StudentAcademicProfileDetailstDTO
    {

        public int RegistrationId { get; set; }
        public string ProgrammeCode { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public int CreditHour { get; set; }
        public int Score { get; set; }
        public string Grade { get; set; }
        public string CourseId { get; set; }
        public int CourseLevel { get; set; }
        public int SemesterId { get; set; }
        public int SessionId { get; set; }
        public string RegNo { get; set; }
        public string StudentId { get; set; }
        
        public int CA1 { get; set; }
        public int CA2 { get; set; }
        public int Exam { get; set; }
        public double GP { get; set; }
        public double QP { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public string CreditHourHeading { get; set; }
        public string RecordValue { get; set; }

    }
    public class StudentAcademicProfileDTO
    {
        public string Session { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Programme { get; set; }
        public string ProgrammeType { get; set; }
        public string Semester { get; set; }
        public int SemesterId { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string StudentId { get; set; }
        public string MatricNumber { get; set; }
        public string SessionAddmitted { get; set; }
        public string GPA { get; set; }
        public string CGPA { get; set; }
        public int TotalCreditUnit { get; set; }
        public byte[] Photo { get; set; }
        public List<StudentAcademicProfileDetailstDTO> Results { get; set; }
   
    }
}
