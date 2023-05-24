using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.AcademicModule
{
    public class CurrentCalenderDTO
    {
        public string Title { get; set; }
        public List<SemesterCalender> Semesters { get; set; }
    }

    public class SemesterCalender
    {
        public string Semester { get; set; }
        public List<CalenderActivities> Activities { get; set; }
    }

    public class CalenderActivities
    {
        public string Activity { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Duration { get; set; }
    }
}
