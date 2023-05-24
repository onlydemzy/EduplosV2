namespace Eduplus.DTO.AcademicModule
{
    public class CourseDTO
    {
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public int CreditHours { get; set; }
        public string Semester { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public string ProgrammeCode { get; set; }
        public string Category { get; set; }
    }
}
