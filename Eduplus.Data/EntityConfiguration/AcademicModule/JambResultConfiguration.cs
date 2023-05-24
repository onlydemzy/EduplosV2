using Eduplus.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.AcademicModule
{
    internal class JambResultConfiguration:EntityTypeConfiguration<JambResult>
    {
        
        internal JambResultConfiguration()
        {
            HasKey(k => k.JambRegNumber);
            Property(a => a.JambRegNumber).HasMaxLength(20);
            Property(a => a.JambRegNumber).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(k => k.StudentId).HasMaxLength(130).IsRequired();
            HasRequired(k => k.Student).WithMany().HasForeignKey(k => k.StudentId);
            HasMany(a => a.Scores).WithRequired(a => a.JambResult).HasForeignKey(k => k.JambRegNumber);
        }
    }

    internal class JambScoresConfiguration : EntityTypeConfiguration<JambScores>
    {

        internal JambScoresConfiguration()
        {
            HasKey(k => k.ScoreId);
            Property(k => k.ScoreId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(k => k.Subject).HasMaxLength(130).IsRequired();
             
        }
    }
}
