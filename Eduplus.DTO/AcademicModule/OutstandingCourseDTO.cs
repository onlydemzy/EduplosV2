using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.AcademicModule
{
    public class OutstandingCoursesDTO
    {
        public int OutStandingCourseId { get; set; }
        public string CourseCode { get;set;}
        public string Title { get; set; }
        public string Grade { get; set; }
        public int Score { get; set; }
        public string Semester { get; set; }
        public string OwingType { get; set; }
    }
}
