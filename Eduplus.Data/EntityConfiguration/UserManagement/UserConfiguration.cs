using KS.Core.UserManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace KS.Data.EntityConfiguration.UserManagement
{
    internal class UserConfiguration:EntityTypeConfiguration<User>
    {
        internal UserConfiguration()
        {
            HasKey(u => u.UserId);
            Property(u => u.UserId).HasMaxLength(300);
            Property(u => u.Password).HasMaxLength(800);
            Property(u => u.UserName).HasMaxLength(100);
            
            Property(u => u.FullName).HasMaxLength(100);
            Property(u => u.DepartmentCode).HasMaxLength(30);
            Property(u => u.ProgrammeCode).HasMaxLength(30);
            Property(u => u.CreatedBy).HasMaxLength(100);
            Property(u => u.UserCode).HasMaxLength(20);
           
        }
    }
}
