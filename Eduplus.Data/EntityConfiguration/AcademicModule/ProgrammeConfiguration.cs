using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class ProgrammeConfiguration:EntityTypeConfiguration<Programme>
    {  
        internal ProgrammeConfiguration()
        {
            HasKey(c => c.ProgrammeCode);
            Property(c => c.ProgrammeCode).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasMaxLength(30);
            Property(c => c.Title).HasMaxLength(200);
            Property(c => c.ProgrammeType).HasMaxLength(200);
            Property(c => c.DepartmentCode).HasMaxLength(30);
            Property(c => c.MatricNoGeneratorType).HasMaxLength(50);
            Property(c => c.MatricNoSeparator).HasMaxLength(1);
            Property(c => c.Award).HasMaxLength(100);
            HasMany(a => a.MatricNoFormats).WithRequired(a=>a.Programme).HasForeignKey(a => a.ProgrammeCode);
            HasRequired(c => c.Department).WithMany().HasForeignKey(c => c.DepartmentCode).WillCascadeOnDelete(false);

            
        }
    }
}
