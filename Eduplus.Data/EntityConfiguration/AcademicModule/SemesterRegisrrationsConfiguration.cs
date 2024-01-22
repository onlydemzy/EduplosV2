using Eduplos.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class SemesterRegistrationsConfiguration:EntityTypeConfiguration<SemesterRegistrations>
    {
        internal SemesterRegistrationsConfiguration()
        {
            HasKey(a => a.RegistrationId);
            Property(a => a.RegistrationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.Semester).HasMaxLength(20);
            Property(a => a.Session).HasMaxLength(20);
            HasRequired(a => a.Student).WithMany().HasForeignKey(a => a.StudentId);
            Ignore(a=> a.InputtedBy);
           
        }
    }
}
