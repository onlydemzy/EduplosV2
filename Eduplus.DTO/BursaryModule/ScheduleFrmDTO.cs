using System.Collections.Generic;

namespace Eduplos.DTO.BursaryModule
{
    public class ScheduleFrmDTO
    {
        public FeeScheduleDTO Schedule { get; set; }
        public List<FeeScheduleDetailsDTO> Details { get; set; }
    }
}
