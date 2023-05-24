using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.UserManagement
{
    public class UserDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int[] RoleIDs { get; set; }
        public string[] Roles { get; set; }
        public int NRoleID { get; set; }
       
    }
}
