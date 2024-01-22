using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.BursaryModule
{
    public class StudentsBalancesDTO
    {
        public string StudentId { get; set; }
        public string MatricNumber { get; set; }
        public double Balance { get; set; }
        public double AmountPaid { get; set; }
        public double AmountOwed { get; set; }
        public string Programme { get; set; }
        public int CurrentLevel { get; set; }
        public DateTime PayDate { get; set; }
        public string Name { get; set; }
       public string Type { get; set; }

    }
}
