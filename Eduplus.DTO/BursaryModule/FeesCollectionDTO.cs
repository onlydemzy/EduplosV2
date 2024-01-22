using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.BursaryModule
{
    public class FeesCollectionDTO
    {
        public string Title { get; set; }
        public double Total { get; set; }
        public string Session { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Department { get; set; }
        public string ProgrammeType { get; set; }
        public List<FeesCollectionDetailsDTO> Details { get; set; }

    }
    public class FeesCollectionDetailsDTO
    {
        public string Particulars { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string RegNo { get; set; }
        public string Programme { get; set; }
        public DateTime PayDate { get; set; }

        public string TransRef { get; set; }
        public int? Level { get; set; }
    }
}
