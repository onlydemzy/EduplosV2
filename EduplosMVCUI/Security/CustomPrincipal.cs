using KS.Core.UserManagement;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace KS.Web.Security
{
    public  class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {
            if (Roles.Any(r => role.Contains(r)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasPermission(string permission)
        {
            //bool pfound = true;
            if (Permissions.Any(p => permission.Equals(p)))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public CustomPrincipal(string Username)
        {
            this.Identity = new GenericIdentity(Username);
        }

        public string UserId { get; set; }
        public string FullName { get; set; }
        public string DepartmentCode { get; set; }
        public string ProgrammeCode { get; set; }
        public string ProgrammeType { get; set; }
        public string[] Roles { get; set; }
        public string[] Permissions { get; set; }
        public List<MenuItem> UserMenus { get; set; }
        public string UserCode { get; set; }
        public string Username { get; set; }
        public bool IsSysAdmin { get; set; }
        public byte[] Photo { get; set; }
        public int CurrentSemesterId { get; set; }
        public string Email { get; set; }
    }

    public class CustomPrincipalSerializeModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string UserCode { get; set; }

        
    }
}