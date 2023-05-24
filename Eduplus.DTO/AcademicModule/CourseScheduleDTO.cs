using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.AcademicModule
{
    public class CourseScheduleDTO
    {
        public long ScheduleId { get; set; }
        public string CourseCode { get; set; }
        public string CourseId { get; set; }
        public string Title { get; set; }
        public string Lecturers { get; set; }
    }
}
