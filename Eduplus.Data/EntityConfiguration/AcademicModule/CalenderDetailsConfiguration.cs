using Eduplos.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class CalenderDetailConfiguration:EntityTypeConfiguration<CalenderDetail>
    {  
        internal CalenderDetailConfiguration()
        {
            HasKey(c => c.DetailsId);
            Property(c => c.DetailsId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasRequired(c => c.Calender).WithMany(c=>c.Details).HasForeignKey(a => a.CalenderId);
            Property(c => c.Semester).HasMaxLength(50).IsRequired();
            Property(c => c.Activity).HasMaxLength(200).IsRequired();
            Property(c => c.StartDate).HasColumnType("Date");
            Property(c => c.EndDate).HasColumnType("Date");
            
        }
    }
}
