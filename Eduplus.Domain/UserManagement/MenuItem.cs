using KS.Common;
using System.Collections.Generic;

namespace KS.Core.UserManagement
{
    public class MenuItem:EntityBase
    {
        public MenuItem()
        {
            ChildrenMenus =new  HashSet<MenuItem>();
            
        }
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }//Display name
        public string Description { get; set; }//what it does
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Collapse { get; set; }
        public string Heading { get; set; }
        public string fawsome { get; set; }
        public string ParentMenuItemId { get; set; }
        public byte MenuOrder { get; set; }
        public string PermissionId { get; set; }
        public bool AlwaysEnable { get; set; }
        public virtual ICollection<MenuItem> ChildrenMenus { get; set; }
        public Permission Permission { get; set; }
        public virtual MenuItem ParentMenuItem { get; set; }
    }
}
