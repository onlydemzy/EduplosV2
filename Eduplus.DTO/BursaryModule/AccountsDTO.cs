using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.BursaryModule
{
    public class AccountsDTO
    {
        public string AccountCode { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public string Session { get; set; }
        public DateTime DatePaid { get; set; }
        public string Particulars { get; set; }
    }
}
