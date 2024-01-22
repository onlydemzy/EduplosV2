using System;
using System.Collections.Generic;

namespace Eduplos.DTO.CoreModule
{
    public class SessionDTO
    {
        public int SessionId { get; set; }
        public string Title { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<SemesterDTO> Semesters { get; set; }
        
    }

   
}
