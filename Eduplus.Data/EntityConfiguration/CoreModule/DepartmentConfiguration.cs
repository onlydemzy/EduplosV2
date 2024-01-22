using Eduplos.Domain.CoreModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    public class DepartmentConfiguration:EntityTypeConfiguration<Department>
    {
        public DepartmentConfiguration()
        {
            HasKey(c => c.DepartmentCode);
            Property(c => c.DepartmentCode).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(c => c.DepartmentCode).HasMaxLength(30);
            Property(c => c.Title).HasMaxLength(200);
            Property(c => c.Location).HasMaxLength(200);
            HasOptional(c => c.Faculty).WithMany().HasForeignKey(d => d.FacultyCode);
 
        }
    }
}
