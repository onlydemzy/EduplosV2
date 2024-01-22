using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.CoreModule
{
    public class StudentInProgDTO
    {
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Programme { get; set; }
        public string ProgType { get; set; }
        public int? Level { get; set; }
        public string Set { get; set; }
        public List<StudentDTO> Students { get; set; } 
    }
}
