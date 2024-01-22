using KS.Common;
using System;
using System.Collections.Generic;

namespace Eduplos.Domain.CoreModule
{
    public class Session:EntityBase
    {
        public Session()
        {
            Semesters = new HashSet<Semester>();
            //FeeSchedules = new HashSet<FeeSchedule>();
        }
        public int SessionId { get; set; }
        public string Title { get; set; }
        public bool IsAdmissionSession { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual ICollection<Semester> Semesters { get; set; }
        public bool HideFreshmenMatricNo { get; set; }

    }
}
