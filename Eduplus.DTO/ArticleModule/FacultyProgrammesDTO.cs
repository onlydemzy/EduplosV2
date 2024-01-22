using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.ArticleModule
{
    public class FacultyProgrammesDTO
    {
        public string FacultyCode { get; set; }
        public string Faculty { get; set; }
        
        public List<FacultyProgs> Programmes { get; set; }
    }
    public class FacultyProgs
    {
        public string ProgrammeCode { get; set; }
        public string Programme { get; set; }
        public string FacultyCode { get; set; }
    }

}
