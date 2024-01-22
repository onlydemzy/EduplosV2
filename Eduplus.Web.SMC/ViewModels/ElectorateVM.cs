using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplos.Web.SMC.ViewModels
{
    public class ElectorateVM
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }

        public string Phone { get; set; }

        public string MatricNumber { get; set; }

        public string ProgrammeCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}