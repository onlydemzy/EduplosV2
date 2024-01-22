using Eduplos.Domain.AcademicModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplos.Web.SMC.ViewModels
{
    public class CalenderViewModel
    {
        public int CalenderId { get; set; }

        public int SessionId { get; set; }
        public string Title { get; set; }
        public bool IsCurrent { get; set; }
        
        public List<CalenderDetail> Details { get; set; }
    }
}