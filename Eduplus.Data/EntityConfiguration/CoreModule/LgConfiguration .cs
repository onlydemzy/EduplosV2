using Eduplos.Domain.CoreModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;


namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    public class LgConfiguration:EntityTypeConfiguration<LGA>
    {
        public LgConfiguration()
        {
            HasKey(s => s.LgId);
            
            Property(m => m.LgId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           Property(s => s.LgTitle).HasMaxLength(50);
           
        }
    }
}
