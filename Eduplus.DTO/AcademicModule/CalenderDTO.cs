using System.Collections.Generic;

namespace Eduplos.DTO.AcademicModule
{
    public class CalenderDTO
    {
        public int CalenderId { get; set; }
        public string Session { get; set; }
        public int SessionId { get; set; }
        public string Title { get; set; }
        public string Semester { get; set; }
        public bool IsCurrent { get; set; }
        public List<CalenderDetailsDTO> Details { get; set; }
    }
}
