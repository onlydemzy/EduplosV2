using Eduplos.Domain.CoreModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    internal class OtherQualificationsConfiguration:EntityTypeConfiguration<OtherAcademicQualifications>
    {
        internal OtherQualificationsConfiguration()
        {
            HasKey(s => s.QualificationId);
            Property(s => s.QualificationId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(s => s.StartMonth).HasMaxLength(50).IsRequired();
            Property(s => s.EndMonth).HasMaxLength(50).IsRequired();
            Property(s => s.Institution).HasMaxLength(100).IsOptional();
            Property(s => s.Qualification).HasMaxLength(70).IsRequired();
            
            HasRequired(s => s.Person).WithMany(a=>a.OtherQualifications).HasForeignKey(s => s.PersonId);
            
        }
    }
}
