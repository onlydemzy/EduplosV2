using Eduplus.Domain.BurseryModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.BurseryModule
{
    internal class OtherChargesConfiguration:EntityTypeConfiguration<OtherCharges>
    {
        internal OtherChargesConfiguration()
        {
            HasKey(a => a.ChargeId);
            Property(a => a.ChargeId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.AccountCode).HasMaxLength(30);
             
            Property(a => a.Description).HasMaxLength(100);
            Property(a => a.ProgrammeType).HasMaxLength(30);

        }
    }
}
