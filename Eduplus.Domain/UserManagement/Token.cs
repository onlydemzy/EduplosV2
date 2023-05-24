using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KS.Core.UserManagement
{
    public class Token
    {
        public int TokenId { get; set; }
        public int? UserId { get; set; }
        public string AuthToken { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Company { get; set; }
        public string ClientId { get; set; }//web, mobile
        public string ClientSecret { get; set; }
        public string Url { get; set; }
        
    }
}
