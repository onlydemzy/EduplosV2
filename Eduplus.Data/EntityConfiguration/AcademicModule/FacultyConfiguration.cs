using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    public class FacultyConfiguration:EntityTypeConfiguration<Faculty>
    {
        public FacultyConfiguration()
        {
            HasKey(c => c.FacultyCode);
            Property(c => c.FacultyCode).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(c => c.FacultyCode).HasMaxLength(10);
            Property(c => c.Title).HasMaxLength(100);
            Property(c => c.Location).HasMaxLength(200);

        }
    }
}
