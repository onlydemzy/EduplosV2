using Eduplus.Domain.CoreModule;
using System.Collections.Generic;

namespace Eduplus.Domain.AcademicModule
{
    public class Calender
    {
       public Calender()
        {
            Details = new HashSet<CalenderDetail>();
        }
        public int CalenderId { get; set; }

        public int SessionId { get; set; }
        public string Title { get; set; }
        public bool IsCurrent { get; set; }
        public virtual Session Session{get;set;}
        
        public virtual ICollection<CalenderDetail> Details { get; set; }
    }
}
