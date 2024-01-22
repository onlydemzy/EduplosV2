using Eduplos.Domain.BurseryModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.BurseryModule
{
    internal class FeeScheduleDetailConfiguration:EntityTypeConfiguration<FeeScheduleDetail>
    {
        internal FeeScheduleDetailConfiguration()
        {
            HasKey(f=>f.ScheduleDetailId);
            Property(f => f.ScheduleDetailId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(f => f.AccountCode).HasMaxLength(30);
            Property(f => f.Type).HasMaxLength(50);
            Property(f => f.AppliesTo).HasMaxLength(50);
            HasRequired(a => a.Accounts).WithMany().HasForeignKey(f => f.AccountCode);
        }
    }
}
