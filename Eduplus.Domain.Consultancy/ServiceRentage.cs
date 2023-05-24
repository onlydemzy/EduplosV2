using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Domain.Consultancy
{
    public class ServiceRentage
    {
        public long RentId { get; set; }
        public string Tenure { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double AmountCharged { get; set; }
        public double AmountPaid { get; set; }
        public string AssetCode { get; set; }
        public string Title { get; set; }
        public string CustomerId { get; set; }
        public ServiceCustomer Customer { get; set; }
    }
}
