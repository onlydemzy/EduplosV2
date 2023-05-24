using Eduplus.Domain.CoreModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace KS.Data.EntityConfiguration.UserManagement
{
    internal class ApiLogConfiguration:EntityTypeConfiguration<ApiLog>
    {
        internal ApiLogConfiguration()
        {
            HasKey(u => u.LogId);
            Property(u => u.LogId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(u => u.RequestContentType).HasMaxLength(50);
            Property(u => u.RequestUri).HasMaxLength(200);
            Property(u => u.RequestMethod).HasMaxLength(50);
            Property(u => u.ResponseContentType).HasMaxLength(50);
           // Property(u => u.RequestContent).HasMaxLength(1000);

            
            Property(u => u.ResponseStatusCode).HasMaxLength(50);
           // Property(u => u.ResponseContent).HasMaxLength(1000);
            
        }
    }
}
