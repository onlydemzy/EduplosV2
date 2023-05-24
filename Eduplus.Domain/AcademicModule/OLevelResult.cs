using Eduplus.Domain.CoreModule;
using KS.Common;
using System.Collections.Generic;

namespace Eduplus.Domain.AcademicModule
{
    public class OLevelResult:EntityBase
    {

        public OLevelResult()
        {
            OlevelResultDetail = new HashSet<OlevelResultDetail>();
        }
        public string ExamNumber { get; set; }
        public int ExamYear { get; set; }
        public string ExamType { get; set; }
        public string Venue { get; set; }
        public byte SitAttempt { get; set; }
        public string StudentId { get; set; }
        public string ResultId { get; set; }
        public virtual ICollection<OlevelResultDetail> OlevelResultDetail { get; set; }
        public virtual Student Student{get;set;}


    }

    public class OlevelResultDetail
    {
        public long DetailId { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public string ResultId { get; set; }
        public virtual OLevelResult OlevelResult { get; set; }
    }

    
}
