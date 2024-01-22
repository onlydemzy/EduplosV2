using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.BursaryModule
{
    public class PaymentTypesCollectionSummaryDTO
    {
        public string PaymentType { get; set; }
        public double Amount { get; set; }
        public DateTime PayDate { get; set; }
        public string Particulars { get; set; }
        public string ProgrammeType { get; set; }
    }
}
