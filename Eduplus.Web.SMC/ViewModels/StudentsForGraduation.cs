using Eduplus.DTO.AcademicModule;
using System.Collections.Generic;

namespace Eduplus.Web.SMC.ViewModels
{
    public class StudentsForGraduationViewModel
    {
        public List<ProbationDetailsDTO> students { get; set; }
        public string sessionId { get; set; }
        public string batch { get; set; }
    }
}