using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class CalenderConfiguration:EntityTypeConfiguration<Calender>
    {  
        internal CalenderConfiguration()
        {
            HasKey(c => c.CalenderId);
            Property(c => c.CalenderId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.Title).HasMaxLength(70);
            HasRequired(c => c.Session).WithMany().HasForeignKey(a => a.SessionId);
            
            //HasMany(c => c.Details).WithRequired(a => a.Calender).HasForeignKey(a => a.CalenderId);


        }
    }
}
