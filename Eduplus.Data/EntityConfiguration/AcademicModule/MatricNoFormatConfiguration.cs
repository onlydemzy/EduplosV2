using Eduplos.Domain.AcademicModule;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class MatricNoFormatConfiguration:EntityTypeConfiguration<MatricNoFormat>
    {
        internal MatricNoFormatConfiguration()
        {
            HasKey(a => a.FormatId);
            Property(a => a.FormatId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.BankKey).HasMaxLength(50)
                .IsRequired();
            Property(a => a.BankValue).HasMaxLength(50)
                .IsRequired();
            Property(a => a.ProgrammeCode).HasMaxLength(30);
        }

    }
}
