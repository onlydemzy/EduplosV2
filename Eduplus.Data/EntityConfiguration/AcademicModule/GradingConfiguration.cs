using Eduplus.Domain.AcademicModule;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class GradingConfiguration:EntityTypeConfiguration<Grading>
    {
        internal GradingConfiguration()
        {
            HasKey(a => a.GradeId);
            Property(g => g.GradeId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Grade).HasMaxLength(5).IsRequired();
            Property(a => a.ProgrammeType).HasMaxLength(50);
            Property(a => a.Remark).HasMaxLength(100);
        }
    }
}
