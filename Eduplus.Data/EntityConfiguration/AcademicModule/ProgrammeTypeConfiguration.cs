using Eduplos.Domain.AcademicModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class ProgrammeTypeConfiguration:EntityTypeConfiguration<ProgrammeTypes>
    {
        internal ProgrammeTypeConfiguration()
        {
            HasKey(a => a.Type);
            Property(a => a.Type).HasMaxLength(70);
           
            HasMany(a => a.PaymentGateWays).WithRequired(a=>a.ProgrammeType).HasForeignKey(a => a.ProgrammeTypeCode);
        }
    }
}
