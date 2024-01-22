using System;
using System.Collections.Generic;

namespace Eduplos.Domain.CoreModule
{
    public class Country
    {
        public Country()
        {
            States = new HashSet<State>();
        }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public virtual ICollection<State> States { get; set; }

    }
}
