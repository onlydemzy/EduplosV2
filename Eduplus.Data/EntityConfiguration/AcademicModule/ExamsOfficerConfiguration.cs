using Eduplus.Domain.AcademicModule;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class ExamsOfficerConfiguration:EntityTypeConfiguration<ExamsOfficer>
    {
        internal ExamsOfficerConfiguration()
        {
            HasKey(a => a.OfficerId);
            Property(a => a.OfficerId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.OfficerCode).HasMaxLength(50).IsRequired();
            Property(a => a.Fullname).HasMaxLength(150).IsRequired();
            Property(a => a.CourseCategory).HasMaxLength(70);
            Property(a => a.DepartmentCode).HasMaxLength(50);
            Property(a => a.ProgrammeCode).HasMaxLength(50);
            Property(a => a.ProgrammeType).HasMaxLength(50);
             
        }
    }
}
