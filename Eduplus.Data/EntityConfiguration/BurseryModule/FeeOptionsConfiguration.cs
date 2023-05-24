using Eduplus.Domain.BurseryModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.BurseryModule
{
    internal class FeeOptionsConfiguration:EntityTypeConfiguration<FeeOptions>
    {
        internal FeeOptionsConfiguration()
        {
            HasKey(a => a.OptionsId);
            Property(a => a.OptionsId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Installment).HasMaxLength(100);
            Property(a => a.ProgrammeType).HasMaxLength(100);
             
        }
    }
}
