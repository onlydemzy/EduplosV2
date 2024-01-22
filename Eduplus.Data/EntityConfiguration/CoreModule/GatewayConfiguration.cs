using Eduplos.Domain.CoreModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    internal class GatewayConfiguration:EntityTypeConfiguration<PaymentGateways>
    {
        internal GatewayConfiguration()
        {
            HasKey(a => a.GatewayId);
            Property(a => a.GatewayId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.ClientAccountName).HasMaxLength(100);
            Property(a => a.ClientAccountNumber).HasMaxLength(15);
            Property(a => a.ClientBankCode).HasMaxLength(5);
            Property(a => a.MerchantCode).HasMaxLength(50);
            Property(a => a.MerchantKey).HasMaxLength(50);
            Property(a => a.Name).HasMaxLength(100);
            Property(a => a.ProviderAccountName).HasMaxLength(100);
            Property(a => a.ProviderAccountNumber).HasMaxLength(15);
            Property(a => a.ProviderBankCode).HasMaxLength(5);
            Property(a => a.TransConfirmationUrl).HasMaxLength(200);
            Property(a => a.TransPostUrl).HasMaxLength(200); 
            Property(a => a.ResponseUrl).HasMaxLength(200);
            Property(a => a.TransRefGenUrl).HasMaxLength(200);
            Property(a => a.ProgrammeTypeCode).HasMaxLength(70);
        }
    }
}
