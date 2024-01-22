using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Domain.AcademicModule
{
    public class MatricNoFormat
    {
        public int FormatId { get; set; }
        public string ProgrammeCode { get; set; }
        public string BankKey { get; set; }
        public string BankValue { get; set; }
        public byte Position { get; set; }
        public virtual Programme Programme { get; set; }
    }
}
