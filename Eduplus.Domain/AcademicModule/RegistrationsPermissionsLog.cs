using Eduplus.Domain.CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Domain.AcademicModule
{
    public class RegistrationsPermissionsLog
    {
        
        public long LogId { get; set; }
        public int SessionId { get; set; }
        public string StudentId { get; set; }
        public bool Register1 { get; set; }
        public bool Register2 { get; set; }
        public bool Write1 { get; set; }
        public bool Register3 { get; set; }
        public bool Write3 { get; set; }
        public bool Write2 { get; set; }
        public bool Late1Clear { get; set; }
        public bool Late2Clear { get; set; }
        public bool Late3Clear { get; set; }
        public bool LateRegistrationApplied { get; set; }
        public virtual Student Student { get; set; }
    }
}
