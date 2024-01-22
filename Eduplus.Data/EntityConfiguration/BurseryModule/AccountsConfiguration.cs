using Eduplos.Domain.BurseryModule;
using KS.Domain.AccountsModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.BurseryModule
{
    internal class AccountsConfiguration:EntityTypeConfiguration<Accounts>
    {
        internal AccountsConfiguration()
        {
            HasKey(a => a.AccountCode);
            Property(a => a.AccountCode).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)
                .HasMaxLength(20);
            Property(a => a.Title).HasMaxLength(100).IsRequired();
            Property(a => a.Description).HasMaxLength(200);
            Property(a => a.AccountType).HasMaxLength(100);
            Property(a => a.CollectionType).HasMaxLength(50);

            
        }
    }
}
