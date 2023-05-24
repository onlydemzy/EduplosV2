using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduplosMVCUI.Models
{
    public class MenuItem
    {
        public MenuItem()
        {
            ChildrenMenus = new HashSet<MenuItem>();

        }
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }//Display name
        public string Description { get; set; }//what it does
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ParentMenuItemId { get; set; }
        public byte MenuOrder { get; set; }
        public string PermissionId { get; set; }
        public bool AlwaysEnable { get; set; }
        public virtual ICollection<MenuItem> ChildrenMenus { get; set; }
       public virtual MenuItem ParentMenuItem { get; set; }
        public string Colapse { get; set; }
        public string Heading { get; set; }
        public string fawsome { get; set; }

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> mens = new List<MenuItem>();
            mens.Add(new MenuItem
            {
                MenuItemId = "01",
                Action = "GetStudents",
                AlwaysEnable = true,
                Controller = "Student",
                MenuItemName = "Parent 1",
                MenuOrder = 1,
                Colapse="colapseOne",
                Heading="headingOne",
                fawsome= "fa fa-fw fa-users",
                ChildrenMenus = new List<MenuItem>{ new MenuItem {MenuItemId = "03",
                Action = "GetStudents",
                AlwaysEnable = true,
                Controller = "Student",
                MenuItemName = "Child 1",
                MenuOrder = 1,
                ParentMenuItemId="01" },
                new MenuItem {MenuItemId = "04",
                Action = "GetStudents",
                AlwaysEnable = true,
                Controller = "Student",
                MenuItemName = "Child 2",
                MenuOrder = 1,
                ParentMenuItemId="01" }
                }

            });

            mens.Add(new MenuItem
            {
                MenuItemId = "02",
                Action = "GetStudents",
                AlwaysEnable = true,
                Controller = "Student",
                MenuItemName = "Parent 2",
                MenuOrder = 1,
                Colapse="collapseTwo",
                Heading = "headingTwo",
                fawsome= "fa fa-fw fa-table",
                ChildrenMenus = new List<MenuItem>{ new MenuItem {MenuItemId = "05",
                Action = "GetStudents",
                AlwaysEnable = true,
                Controller = "Student",
                MenuItemName = "Child 3",
                MenuOrder = 1,
                ParentMenuItemId="02" },
                new MenuItem {MenuItemId = "06",
                Action = "GetStudents",
                AlwaysEnable = true,
                Controller = "Student",
                MenuItemName = "Child 4",
                MenuOrder = 1,
                ParentMenuItemId="02" }
                }

            });

            return mens;
        }
    }
}