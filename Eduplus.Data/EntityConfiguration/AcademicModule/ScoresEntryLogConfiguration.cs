using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class ScoresEntryLogConfiguration:EntityTypeConfiguration<ScoresEntryLog>
    {
        internal ScoresEntryLogConfiguration()
        {
            HasKey(c => c.EntryId);
            Property(c => c.EntryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.EnteredBy).HasMaxLength(100);
            Property(c => c.CourseId).HasMaxLength(40);
        }
    }
}
