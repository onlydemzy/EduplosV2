using Eduplus.Domain.BurseryModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.BurseryModule
{
    internal class FeeScheduleConfiguration:EntityTypeConfiguration<FeeSchedule>
    {
        internal FeeScheduleConfiguration()
        {
            HasKey(f=>f.ScheduleId);
            Property(f => f.ScheduleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(f => f.ProgrammeType).HasMaxLength(50);
            HasMany(f => f.Details).WithRequired(f => f.FeeSchedule).HasForeignKey(f => f.ScheduleId);
            HasRequired(f => f.Faculty).WithMany().HasForeignKey(f => f.FacultyCode).WillCascadeOnDelete(false);
            Property(a => a.Status).IsRequired().HasMaxLength(20);
            
        }
    }
}
