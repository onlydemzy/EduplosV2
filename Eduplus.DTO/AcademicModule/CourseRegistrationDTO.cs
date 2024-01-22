namespace Eduplos.DTO.AcademicModule
{
    public class CourseRegistrationDTO
    {
        public int RegistrationId { get; set; }
        public bool IsOutStanding { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public int CreditHour { get; set; }
        public int Level { get; set; }
        public string StudentId { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string ProgrammeCode { get;set;}
        public string Semester { get; set; }
    }
}
