using Eduplus.Domain.CoreModule;
using Eduplus.DTO.CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplus.Web.SMC.ViewModels
{
    public class AddStudentViewModel
    {
        public StudentDTO student { get; set; }
        public int sessionId { get; set; }
    }
}