using KS.Common;
using System.Collections.Generic;

namespace Eduplos.Domain.CoreModule
{ 
    public class State:EntityBase
    {
        public State()
        {
            Lgs=new HashSet<LGA>();
        }
        public string StateId { get; set; }
        
        public string StateName { get; set; }
        public string CountryId { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<LGA> Lgs { get; set; }
    }
}
