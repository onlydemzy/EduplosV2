using Eduplus.Domain.BurseryModule;
using KS.Domain.AccountsModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.BurseryModule
{
    internal class TransMasterConfiguration:EntityTypeConfiguration<TransMaster>
    {
        internal TransMasterConfiguration()
        {
            HasKey(t => t.TransId);
            Property(t => t.TransId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.TransRef).HasMaxLength(300);
            Property(t => t.PayMethod).HasMaxLength(30);
            Property(t => t.TransType).HasMaxLength(7).IsRequired();
            Property(t => t.TellerNo).HasMaxLength(20);
            Property(t => t.Bank).HasMaxLength(200);
            
            HasRequired(t => t.Accounts).WithMany().HasForeignKey(a => a.AccountCode);
            
            Property(t => t.Particulars).HasMaxLength(500);
           
        }
            
    }
}
