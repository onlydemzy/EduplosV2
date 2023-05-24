using KS.Core.UserManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace KS.Data.EntityConfiguration.UserManagement
{
    internal class MenuItemConfiguration:EntityTypeConfiguration<MenuItem>
    {
        internal MenuItemConfiguration()
        {
            HasKey(m => m.MenuItemId);
            Property(r => r.MenuItemId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(r => r.MenuItemId).HasMaxLength(5);
            Property(m => m.MenuItemName).HasMaxLength(50).IsRequired();
            Property(m => m.Description).HasMaxLength(50).IsOptional();
            Property(m => m.Controller).HasMaxLength(100);
            Property(m => m.Action).HasMaxLength(100);
            Property(m => m.Collapse).HasMaxLength(100);
            Property(m => m.Heading).HasMaxLength(100);
            Property(m => m.fawsome).HasMaxLength(100);
            Property(m => m.ParentMenuItemId).HasMaxLength(5);
            Property(m => m.AlwaysEnable).IsRequired();
            HasOptional(m => m.Permission).WithMany(p=>p.Menu).HasForeignKey(ra => ra.PermissionId);
            HasMany(m => m.ChildrenMenus).WithOptional(cm => cm.ParentMenuItem).HasForeignKey(m => m.ParentMenuItemId);
            
        }
    }
}
