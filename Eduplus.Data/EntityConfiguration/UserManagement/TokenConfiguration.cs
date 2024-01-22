using KS.Core.UserManagement;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.UserManagement
{
    public class TokenConfiguration:EntityTypeConfiguration<Token>
    {
        public TokenConfiguration()
        {
            HasKey(a => a.TokenId);
            Property(a => a.TokenId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.AuthToken).HasMaxLength(1000);
            Property(a => a.Company).HasMaxLength(200);
            Property(a => a.ClientId).HasMaxLength(500);
            Property(a => a.ClientSecret).HasMaxLength(500);
        }
    }
}
