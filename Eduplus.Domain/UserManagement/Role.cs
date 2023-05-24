using System;
using System.Collections.Generic;

namespace KS.Core.UserManagement
{
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
            Permissions = new HashSet<Permission>();
            
        }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public string RoleDescription { get; set; }
        public bool IsSystemAdmin { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
       
    }
}