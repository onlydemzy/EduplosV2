using Eduplos.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class CourseScheduleConfiguration:EntityTypeConfiguration<CourseSchedule>
    {
        internal CourseScheduleConfiguration()
        {
            HasKey(a => a.ScheduleId);
            Property(a => a.ScheduleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.CourseId).HasMaxLength(30);
            Property(a => a.ProgrammeCode).HasMaxLength(30);
            Property(a => a.LecturerId).HasMaxLength(128);
            HasRequired(a => a.Lecturer).WithMany().HasForeignKey(a => a.LecturerId);
            HasRequired(a => a.Course).WithMany().HasForeignKey(a => a.CourseId);
            HasRequired(a => a.Programme).WithMany().HasForeignKey(a => a.ProgrammeCode);
            HasRequired(a => a.Semester).WithMany().HasForeignKey(a => a.SemesterId);

        }
    }

    
}
