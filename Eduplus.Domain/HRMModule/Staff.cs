using Eduplus.Domain.CoreModule;
using System;

namespace KS.Domain.HRModule
{
    public class Staff:Person
    {
        
        
        public string StaffCode { get; set; }
        public string Category { get; set; }
        public string Designation { get; set; }

        public string Unit { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public DateTime? DateEmployed { get; set; }
      
        
    }
}
