using Eduplos.Domain.CoreModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    internal class UserDataConfiguration:EntityTypeConfiguration<UserData>
    {
        internal UserDataConfiguration()
        {
            HasKey(a => a.UserDataId);
            Property(a => a.UserDataId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Address).HasMaxLength(300);
            Property(a => a.ContactPerson).HasMaxLength(200);
            Property(a => a.Email1).HasMaxLength(200);
            Property(a => a.EmailDomain).HasMaxLength(200);
            Property(a => a.InstitutionCode).HasMaxLength(10);
            Property(a => a.InstitutionName).HasMaxLength(300);
            Property(a => a.InstitutionType).HasMaxLength(100);
            Property(a => a.AffiliateInfo).HasMaxLength(300);
            Property(a => a.Phone).HasMaxLength(20);
            Property(a => a.Phone2).HasMaxLength(20);
            Property(a => a.Url).HasMaxLength(200);
            
        }
    }
}
