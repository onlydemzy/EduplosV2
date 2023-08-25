namespace Eduplus.DTO.AcademicModule
{
    public class ScoresEntryDTO
    {
        public string CourseCode { get; set; }
        public int CreditHour { get; set; }
        public int CourseLevel { get; set; }
        public int StudentLevel { get; set; }
        public string Grade { get; set; }
        public string CourseId { get; set; }
        public string StudentId { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        public string RegNo { get; set; }
        public int CA1 { get; set; }
        public int CA2 { get; set; }
        public int Exam { get; set; }
        public long RegistrationId { get; set; }
        public string Title { get; set; }
        public bool IsIR { get; set; }
        public string Programme { get; set; }
    }
}
