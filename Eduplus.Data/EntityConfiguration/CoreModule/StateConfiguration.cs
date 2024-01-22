using Eduplos.Domain.CoreModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    internal class StateConfiguration:EntityTypeConfiguration<State>
    {
        internal StateConfiguration()
        {
            HasKey(s => s.StateId);
            Property(m => m.StateId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(s => s.StateName).HasMaxLength(50);
            HasRequired(m => m.Country).WithMany(c => c.States).HasForeignKey(s => s.CountryId);
            HasMany(m => m.Lgs).WithRequired(c => c.State).HasForeignKey(s => s.StateId);

           
            
        }
    }
}
