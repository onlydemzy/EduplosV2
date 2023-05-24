using Eduplus.Domain.AcademicModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class ApplicantsConfiguration:EntityTypeConfiguration<Applicants>
    {
        internal ApplicantsConfiguration()
        {
            HasKey(s => s.StudentId);
            Property(s => s.StudentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(s => s.FullName).HasMaxLength(200);
            Property(s => s.RegNo).HasMaxLength(50);
            Property(s => s.JambNo).HasMaxLength(50);
            Property(s => s.Gender).HasMaxLength(7);
            Property(s => s.Phone).HasMaxLength(50);
            Property(s => s.RegNo).HasMaxLength(15);
            Property(s => s.Email).HasMaxLength(100);
            Property(s => s.Address).HasMaxLength(150);
            Property(s => s.Department).HasMaxLength(30);
            Property(s => s.Qualy).HasMaxLength(30);
            
        }
    }
}
