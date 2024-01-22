using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Domain.AcademicModule
{
    public class ScoresEntryLog
    {
        public int EntryId { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        public DateTime EntryDate { get; set; }
        public string EnteredBy { get; set; }
        public string CourseId { get; set; }
        public string Status { get; set; }
        public string DeptCode {get;set;}
        public string ProgrammeCode { get; set; }
        public string ProgrammeType { get; set; }
       

    }
}
