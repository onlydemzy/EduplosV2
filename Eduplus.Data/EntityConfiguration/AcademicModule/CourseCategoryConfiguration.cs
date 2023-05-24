using Eduplus.Domain.AcademicModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class CourseCategoryConfiguration:EntityTypeConfiguration<CourseCategory>
    {
        internal CourseCategoryConfiguration()
        {
            HasKey(a => a.CategoryId);
            Property(a => a.CategoryId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Category).HasMaxLength(70);
            Property(a => a.ProgrammeType).HasMaxLength(30);
        }
    }
}
