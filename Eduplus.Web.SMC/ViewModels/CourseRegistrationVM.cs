using Eduplus.DTO.AcademicModule;
using System.Collections.Generic;

namespace Eduplus.Web.SMC.ViewModels
{
    public class CourseRegistrationVM
    {
        public List<CourseRegistrationDTO> RegCourses { get; set; }
        public List<CourseRegistrationDTO> RemovedCourses { get; set; }
    }
}