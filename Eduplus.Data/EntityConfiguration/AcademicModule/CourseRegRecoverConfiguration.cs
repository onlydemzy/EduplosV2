using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class CourseRegRecoverConfiguration:EntityTypeConfiguration<CourseRegRecover>
    {
        internal CourseRegRecoverConfiguration()
        {
            HasKey(a => a.RegistrationId);
            Property(a => a.RegistrationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(a => a.StudentId).HasMaxLength(20).IsRequired();
            Property(a => a.SemesterId);
           
            Property(a => a.CourseId).HasMaxLength(40);
            Property(a => a.SessionId);
            Property(a => a.Lvl);
            Property(a => a.Grade).IsOptional().HasMaxLength(5);
            
            Ignore(p => p.TScore);

        }
    }
}
