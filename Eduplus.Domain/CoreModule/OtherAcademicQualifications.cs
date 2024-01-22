using KS.Common;

namespace Eduplos.Domain.CoreModule
{
    public class OtherAcademicQualifications:EntityBase
    {
        public long QualificationId { get; set; }
        public string StartMonth { get; set; }
        public string EndMonth { get; set; }
        public string Institution { get; set; }
        public string Qualification { get; set; }
        public string PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
