using System.Collections.Generic;

namespace Eduplus.DTO.BursaryModule
{
    public class ScheduleFrmDTO
    {
        public FeeScheduleDTO Schedule { get; set; }
        public List<FeeScheduleDetailsDTO> Details { get; set; }
    }
}
