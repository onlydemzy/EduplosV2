using KS.Common;
using System.Collections.Generic;

namespace KS.Core.UserManagement
{
    public class Permission : EntityBase
    {
        public Permission()
        {
            UserRoles = new HashSet<Role>();
        }
        public string PermissionId { get; set; }
        public string Activity { get; set; }
        public string Module { get; set; }
        public ICollection<MenuItem> Menu { get; set; }
        public virtual ICollection<Role> UserRoles { get; set; }
        
    }
}
