using System;
using System.Collections.Generic;
namespace KS.Core.UserManagement
{
    public class User
    {
        public User()
        {
            UserRoles = new HashSet<Role>();
           
        }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate{get;set;}
        public DateTime LastActivityDate { get; set; }
        public bool IsActive { set; get; }
        public string FullName { get; set; }
        public string DepartmentCode { get; set; }
        public string ProgrammeCode { get; set; }
        //public string ProgrammeType { get; set; }
        public int LoginCounter { get; set; }
        public string UserCode { get; set; }
        public string CreatedBy { get; set; }
        public virtual ICollection<Role> UserRoles { get; set; }

       
                
    }
}