using Eduplos.DTO.AcademicModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplos.Web.SMC.ViewModels
{
    public class ResultSubmissionViewModel
    {
        public List<ScoresEntryVM> students { get; set; }
        public bool IsCarryOver { get; set; }
    }
}