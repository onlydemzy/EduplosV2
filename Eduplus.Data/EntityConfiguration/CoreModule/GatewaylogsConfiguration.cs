using Eduplus.Domain.BurseryModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.CoreModule
{
    internal class GatewaylogsConfiguration:EntityTypeConfiguration<GateWaylogs>
    {
        internal GatewaylogsConfiguration()
        {
            HasKey(a => a.LogId);
            Property(a => a.LogId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.MerchantCode).HasMaxLength(20);
            Property(a => a.ServerResponse).HasMaxLength(200);
            Property(a => a.PayeeName).HasMaxLength(300);
            Property(a => a.PaymentCode).HasMaxLength(200);
            Property(a => a.PayeeName).HasMaxLength(100);

            Ignore(a => a.Password);
        }
    }
}
