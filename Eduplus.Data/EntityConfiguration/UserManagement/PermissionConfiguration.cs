using KS.Core.UserManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace KS.Data.EntityConfiguration.UserManagement
{
    internal class PermissionConfiguration:EntityTypeConfiguration<Permission>
    {
        internal PermissionConfiguration()
        {
            HasKey(r => r.PermissionId);
            Property(r => r.PermissionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(r => r.PermissionId).HasMaxLength(5);
            Property(a => a.Activity).HasMaxLength(200);
            HasMany(e => e.UserRoles)
                .WithMany(e => e.Permissions)
                .Map(m => m.ToTable("RolePermission").MapLeftKey("PermissionId").MapRightKey("RoleId"));
            
        }
    }
}
