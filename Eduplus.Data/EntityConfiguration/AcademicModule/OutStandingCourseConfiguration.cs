using Eduplos.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    public class OutStandingCourseConfiguration:EntityTypeConfiguration<OutStandingCourse>
    {
        public OutStandingCourseConfiguration()
        {
            HasKey(c => c.OutStandingCourseId);
            Property(c => c.OutStandingCourseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.CourseId).HasMaxLength(40).IsOptional();
            Property(c => c.OwingType).HasMaxLength(20).IsOptional();
            HasRequired(c => c.Student).WithMany().HasForeignKey(c => c.StudentId);
            HasRequired(c => c.Semester).WithMany().HasForeignKey(a => a.SemesterId).WillCascadeOnDelete(false);
            HasRequired(c => c.Session).WithMany().HasForeignKey(a => a.SessionId).WillCascadeOnDelete(false);
            HasRequired(c => c.Course).WithMany().HasForeignKey(a => a.CourseId).WillCascadeOnDelete(false);

        }
    }
}
