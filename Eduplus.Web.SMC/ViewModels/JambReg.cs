﻿using Eduplos.Domain.AcademicModule;
using System.Collections.Generic;

namespace Eduplos.eb.SMC.ViewModels
{
    public class JambReg
    {
        public string JambNumber { get; set; }
        public string Year { get; set; }
        public List<JambResult> JambResults { get; set; }
    }
}
