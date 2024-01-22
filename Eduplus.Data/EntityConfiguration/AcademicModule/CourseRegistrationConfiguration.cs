using Eduplos.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class CourseRegistrationConfiguration:EntityTypeConfiguration<CourseRegistration>
    {
        internal CourseRegistrationConfiguration()
        {
            HasKey(a => a.RegistrationId);
            Property(a => a.RegistrationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.StudentId).HasMaxLength(20).IsRequired();
            Property(a => a.SemesterId);
           
            Property(a => a.CourseId).HasMaxLength(40);
            Property(a => a.SessionId);
            Property(a => a.Lvl);
            Property(a => a.Grade).IsOptional().HasMaxLength(5);
            HasRequired(a => a.Student).WithMany().HasForeignKey(a => a.StudentId).WillCascadeOnDelete(false);
            HasRequired(a => a.Semester).WithMany().HasForeignKey(c => c.SemesterId).WillCascadeOnDelete(false);
            HasRequired(a => a.Session).WithMany().HasForeignKey(c => c.SessionId).WillCascadeOnDelete(false);
            Ignore(p => p.TScore);

        }
    }
}
