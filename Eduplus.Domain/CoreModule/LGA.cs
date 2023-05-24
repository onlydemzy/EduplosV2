using KS.Common;

namespace Eduplus.Domain.CoreModule
{
    public class LGA : EntityBase
    {
       
        public int LgId { get; set; }
        public string LgTitle { get; set; }
        public string StateId { get; set; }
        public virtual State State { get; set; }
        
    }
}
