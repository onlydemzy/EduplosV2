using System.Collections.Generic;

namespace Eduplus.DTO.BursaryModule
{
    public class FeeScheduleDTO
    {
        public int ScheduleId { get; set; }
        public string ProgrammeType { get; set; }
        public string FacultyCode { get; set; }
        public string Faculty { get; set; }
        public int SessionId { get; set; }
        public string Session { get; set; }
        public string Status { get; set; }
        public double Total { get; set; }
        public List<FeeScheduleDetailsDTO> Details { get; set; }
    }
}
