using Eduplus.Domain.AcademicModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    public class LategRegLogConfiguration:EntityTypeConfiguration<RegistrationsPermissionsLog>
    {
        public LategRegLogConfiguration()
        {
            HasKey(a => a.LogId);
            Property(a => a.LogId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.StudentId).HasMaxLength(50);
        }
    }
}
