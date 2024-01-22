
using Eduplos.Domain.AcademicModule;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class GraduatingClassConfiguration:EntityTypeConfiguration<GraduatingClass>
    {
        internal GraduatingClassConfiguration()
        {
            HasKey(a => a.ClassId);
            Property(g => g.ClassId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Remark).HasMaxLength(100).IsRequired();
            Property(a => a.ProgrammeType).HasMaxLength(50);
        }
    }
}
