using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.BursaryModule
{
    public class StudentFeeScheduleDTO
    {
        public string ProgrammeType { get; set; }
        public string Faculty { get; set; }
        public string Session { get; set; }
        public string FirstStallment { get; set; }
        public string Installment { get; set; }
        public double Total { get; set; }
        public string MatricNumber { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Programme { get; set; }
        public string Balance { get; set; }
        
        public DateTime PayDate { get; set; }

        public List<StudentFeeScheduleDetailDTO> Details { get; set; }
    }

    public class StudentFeeScheduleDetailDTO
    {
        public string Account { get; set; }
        public double Amount { get; set; }
    }
}
