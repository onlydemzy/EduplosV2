using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class CalenderItemConfiguration:EntityTypeConfiguration<CalenderItems>
    {  
        internal CalenderItemConfiguration()
        {
            HasKey(c => c.ItemId);
            Property(c => c.ItemId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.Item).HasMaxLength(250);
           
        }
    }
}
