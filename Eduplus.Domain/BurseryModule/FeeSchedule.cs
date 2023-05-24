using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eduplus.Domain.BurseryModule
{
    public class FeeSchedule
    {
        public FeeSchedule()
        {
            Details = new HashSet<FeeScheduleDetail>();
        }
        public int ScheduleId { get; set; }
        public int SessionId { get; set; }
        public string ProgrammeType { get; set; }
        public string FacultyCode { get; set; }
        public DateTime SetDate { get; set; }
        public double Total { get; set; }
        public virtual Faculty Faculty { get; set; }
        public string Status { get; set; }
        public virtual Session Session { get; set; }
        public virtual ICollection<FeeScheduleDetail> Details { get; set; }

    }
}
