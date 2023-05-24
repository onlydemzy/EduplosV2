using KS.Core.UserManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace UniversitySolution.Data.CoreBC
{
    internal class UserRoleConfiguration:EntityTypeConfiguration<Role>
    {
        internal UserRoleConfiguration()
        {
            HasKey(ur => ur.RoleId);
            Property(ur => ur.RoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(ur => ur.RoleName).HasMaxLength(50);
            Property(ur => ur.RoleDescription).HasMaxLength(300);
            HasMany(e => e.Users)
                .WithMany(e => e.UserRoles)
                .Map(m => m.ToTable("UserRole").MapLeftKey("RoleId").MapRightKey("UserId"));
        }
    }
}
