using Eduplos.Domain.CoreModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    internal class StudentConfiguration:EntityTypeConfiguration<Student>
    {
        internal StudentConfiguration()
        {
            Property(s => s.MatricNumber).HasMaxLength(50).IsOptional();
            Property(s => s.EntryMode).HasMaxLength(30).IsOptional();
            Property(s => s.CurrentLevel).IsOptional();
            Property(s => s.YearAddmitted).HasMaxLength(30).IsOptional();
            Property(s => s.StudyMode).HasMaxLength(70).IsOptional();
                       
            Property(s => s.ProgrammeType).HasMaxLength(50);
            Property(s => s.GradYear).HasMaxLength(50);
            Property(s => s.GradBatch).HasMaxLength(50);
            Property(s => s.IsHandicapped).HasMaxLength(10);
            Property(a => a.WhyUs).HasMaxLength(700);
            Property(a => a.AdmissionStatus).HasMaxLength(30);
            
            HasMany(s => s.OlevelResults).WithRequired(o => o.Student).HasForeignKey(s => s.StudentId);
            HasMany(s => s.JambResults).WithRequired(o => o.Student).HasForeignKey(s => s.StudentId);
        }
    }
}
