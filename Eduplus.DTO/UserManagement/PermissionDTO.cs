using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.UserManagement
{
    public class PermissionDTO
    {
        public string PermissionId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Activity { get; set; }
        public bool Remove { get; set; }
        public int RoleId { get; set; }
    }
}
