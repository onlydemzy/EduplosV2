using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Domain.Consultancy
{
    public class Asset
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public double RentPrice { get; set; }
        public string Status { get; set; }

    }
}
