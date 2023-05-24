
using Eduplus.Domain.BurseryModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.BurseryModule
{
    internal class InvoiceDetailConfiguration:EntityTypeConfiguration<InvoiceDetails>
    {
        internal InvoiceDetailConfiguration()
        {
            HasKey(a => a.DetailId);
            Property(a => a.DetailId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Item).HasMaxLength(200);
            Property(a => a.ItemCode).HasMaxLength(50);
            Property(a => a.InvoiceNo).HasMaxLength(128);
            
        }
    }
}
