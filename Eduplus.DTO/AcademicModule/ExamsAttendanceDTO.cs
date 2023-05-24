using System.Collections.Generic;

namespace Eduplus.DTO.AcademicModule
{
    public class ExamsAttendanceDTO
    {
        public string Department{get;set;}
        public string Session{get;set;}
        public string CourseCode{get;set;}
        public string Course{get;set;}
        public string Semester{get;set;}
        public int CreditHour{get;set;}
        public string Programme { get; set; }

        public List<ExamsAttendanceDetailDTO> Students { get; set; }
    }

    public class ExamsAttendanceDetailDTO
    {
        public int Count { get; set; }
        public string RegNo { get; set; }
        public string Student { get; set; }
        public int CA1 { get; set; }
        public int CA2 { get; set; }
        public int Exam { get; set; }
        public int Total { get; set; }
        public string Grade { get; set; }
        public string Remark { get; set; }
    }
}
