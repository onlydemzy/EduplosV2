

using System;

namespace Eduplos.DTO.AcademicModule
{
    public class CalenderDetailsDTO
    {
        public int DetailsId { get; set; }
        public string Activity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Semester { get; set; }
        public int CalenderId { get; set; }
        
    }
}
