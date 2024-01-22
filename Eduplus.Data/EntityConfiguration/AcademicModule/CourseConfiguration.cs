using Eduplos.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class CourseConfiguration:EntityTypeConfiguration<Course>
    {  
        internal CourseConfiguration()
        {
            HasKey(c => c.CourseId);
            Property(c => c.CourseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasMaxLength(40);
            Property(c => c.Title).HasMaxLength(100);
            Property(c => c.CourseCode).HasMaxLength(10);
            Property(c => c.Semester).HasMaxLength(15);
            Property(c => c.CourseType).HasMaxLength(20);
            Property(c => c.DepartmentCode).HasMaxLength(30);
            Property(c => c.ProgrammeCode).HasMaxLength(30);
            HasRequired(c => c.Department).WithMany().HasForeignKey(c => c.DepartmentCode).WillCascadeOnDelete(false);
            HasRequired(c => c.Programme).WithMany().HasForeignKey(c => c.ProgrammeCode).WillCascadeOnDelete(false);
            Property(c => c.Category).HasMaxLength(70);
           
        }
    }
}
