using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Domain.AcademicModule
{
    public class Grading
    {
        public int GradeId { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public string Grade { get; set; }
        public double GradePoint { get; set; }
        public string Remark { get; set; }
        public string ProgrammeType { get; set; }
    }

    public class GraduatingClass
    {
        public int ClassId { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public string Remark { get; set; }
        public string ProgrammeType { get; set; }
        public bool IsProbation { get; set; }
    }
}
