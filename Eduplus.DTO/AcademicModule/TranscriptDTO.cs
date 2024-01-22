using System;
using System.Collections.Generic;

namespace Eduplos.DTO.AcademicModule
{
    public class TranscriptDTO
    {
        public string RegNo { get; set; }
        public string StudentId { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Gender { get; set; }
        public string YearAdmitted { get; set; }
        public string Name { get; set; }
        public string CGPA { get; set; }
        public string DegreeClass { get; set; }
        public string EntryMode { get; set; }
        public byte? Duration { get; set; }
        public double BaseCGPA { get; set; }
        public string ProgrammeType { get; set; }
        public List<TranscriptSemesterHeading> SemesterSummaries { get; set; }
        public List<DegreeClassDTO> GradClass { get; set; }
        public byte[] Photo { get; set; }
    }
    public class TranscriptSemesterHeading
    {
        public int SemesterId { get; set; }
        public string Title { get; set; }
        public string GPA { get; set; }
        public string CGPA { get; set; }
        public int CreditUnit { get; set; }
        public List<TranscriptScoresDTO> SemesterResults { get; set; }
    }

    public class TranscriptScoresDTO
    {
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public int SemesterId { get; set; }
        public int CHr { get; set; }
        public int CA1 { get; set; }
        public int CA2 { get; set; }
        public int Exam { get; set; }
        public int Score { get; set; }
        public string Grade { get; set; }
        public double GradePoint { get; set; }
        public double QualityPoint { get; set; }
    }
    public class DegreeClassDTO
    {
        public double Low { get; set; }
        public double High { get; set; }
        public string Remark { get; set; }
    }


}
