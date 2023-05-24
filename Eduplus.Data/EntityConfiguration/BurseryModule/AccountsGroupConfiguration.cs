using Eduplus.Domain.BurseryModule;
using KS.Domain.AccountsModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.BurseryModule
{
    class AccountsGroupConfiguration:EntityTypeConfiguration<AccountsGroup>
    {
        public AccountsGroupConfiguration()
        {
            HasKey(a => a.AccountsGroupId);
            Property(a => a.AccountsGroupId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Title).HasMaxLength(200);

        }
    }
}
