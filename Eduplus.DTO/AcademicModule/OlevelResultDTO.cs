using System.Collections.Generic;

namespace Eduplos.DTO.AcademicModule
{
    public class OlevelResultDTO
    {
 
        public string ExamNumber { get; set; }
        public int ExamYear { get; set; }
        public string ExamType { get; set; }
        public string Venue { get; set; }
        public byte SitAttempt { get; set; }
        public string StudentId { get; set; }
        public string ResultId { get; set; }
        public virtual List<OlevelResultDetailDTO> Details { get; set; }
    }

    public class OlevelResultDetailDTO
    {
        public string Subject { get; set; }
        public string Grade { get; set; }
        public long DetailId { get; set; }
        public string ResultId { get; set; }
        public string ExamNumber { get; set; }
        public int ExamYear { get; set; }
        public string ExamType { get; set; }
        public string Venue { get; set; }
        public byte SitAttempt { get; set; }
        public string StudentId { get; set; }
    }
}