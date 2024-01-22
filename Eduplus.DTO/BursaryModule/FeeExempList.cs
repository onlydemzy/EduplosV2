using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.BursaryModule
{
    public class FeeExempListDTO
    {
        public string StudentId { get; set; }
        public string Matricnumber { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string TransType { get; set; }
        public bool Exempt { get; set; }

    }
}
