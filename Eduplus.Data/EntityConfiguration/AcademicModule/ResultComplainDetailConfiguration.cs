using Eduplus.Domain.AcademicModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class ResultComplainDetailConfiguration : EntityTypeConfiguration<ResultComplainDetail>
    {
        internal ResultComplainDetailConfiguration()
        {
            HasKey(r => r.DetailId);
            Property(r => r.DetailId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(r => r.CourseCode).HasMaxLength(50);
            Property(r => r.CourseId).HasMaxLength(50);
            Property(r => r.MatricNumber).HasMaxLength(50);
            Property(r => r.StudentId).HasMaxLength(20);
            HasRequired(c => c.Complain).WithMany(c => c.Details).HasForeignKey(c => c.ComplainId);
        }
    }
}
