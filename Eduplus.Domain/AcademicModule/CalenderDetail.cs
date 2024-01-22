using System;

namespace Eduplos.Domain.AcademicModule
{
    public class CalenderDetail
    {
        public int DetailsId { get; set; }
        public string Activity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate{get;set;}
        public string Semester { get; set; }
        public DateTime LastUpDate { get;set;}
        public int CalenderId { get; set; }
        public virtual Calender Calender { get; set; }


    }
}
