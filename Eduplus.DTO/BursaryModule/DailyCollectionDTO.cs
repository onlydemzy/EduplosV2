using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.BursaryModule
{
    public class DailyCollectionDTO
    {
        public string Title { get; set; }
        public double Total { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<DailyPayHeadersDTO> Headers { get; set; }
    }
    public class DailyCollectionDetailsDTO
    {
        public string Particulars { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string RegNo { get; set; }
        public string PayType { get; set; }
        public DateTime PayDate { get; set; }

        public string TransRef { get; set; }
    }

    public class DailyPayHeadersDTO
    {
        public DateTime Date { get; set; }
        public double Total { get; set; }
        public List<DailyCollectionDetailsDTO> Details { get; set; }
    }
}
