using Eduplos.Domain.CoreModule;
using System;
using System.Collections.Generic;

namespace Eduplos.Domain.AcademicModule
{
    public class Programme
    {
        public Programme()
        {
            MatricNoFormats = new HashSet<MatricNoFormat>();
        }
        
        public string ProgrammeCode { get; set;}
        public string DepartmentCode { get; set;}
        public string Title { get; set;}
        public string ProgrammeType { get; set; }//masters degree others
        public bool IsActive { get; set;}
        public virtual Department Department { get; set;}
        public byte SerialLength{ get; set; }
        public string MatricNoSeparator { get; set; }
        public string Award { get; set; }
        public string MatricNoGeneratorType { get; set; }//Use banks, RandomNos,cumulative
        public int GradLevel { get; set; }
        public bool EnableSiwes { get; set; }
        public int? SiwesLvl { get; set; }
        public string SiwesSemester { get; set; }
        public virtual ICollection<MatricNoFormat> MatricNoFormats { get; set; }
        
    }
}
