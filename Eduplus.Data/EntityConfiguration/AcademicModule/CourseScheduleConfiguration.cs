using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class CourseScheduleConfiguration:EntityTypeConfiguration<CourseSchedule>
    {
        internal CourseScheduleConfiguration()
        {
            HasKey(a => a.ScheduleId);
            Property(a => a.ScheduleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.CourseId).HasMaxLength(30);
            Property(a => a.DepartmentCode).HasMaxLength(30);
            
        }
    }

    internal class CourseScheduleDetailsConfiguration : EntityTypeConfiguration<CourseScheduleDetails>
    {
        internal CourseScheduleDetailsConfiguration()
        {
            HasKey(a => a.DetailsId);
            Property(a => a.DetailsId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.LecturerId).HasMaxLength(20);
            HasRequired(a => a.CourseSchedule).WithMany(a=>a.CourseScheduleDetails).HasForeignKey(a => a.ScheduleId);
        }
    }
}
