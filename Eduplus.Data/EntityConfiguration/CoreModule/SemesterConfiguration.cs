using Eduplus.Domain.CoreModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.CoreModule
{
    internal class SemesterConfiguration: EntityTypeConfiguration<Semester>
    {
        internal SemesterConfiguration()
        {
            HasKey(s => s.SemesterId);
            Property(s => s.SemesterId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(s => s.SemesterTitle).HasMaxLength(30).IsRequired();
            
        }
    }
}
