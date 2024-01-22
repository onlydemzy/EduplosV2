using Eduplos.Domain.AcademicModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class OLevelResultConfiguration:EntityTypeConfiguration<OLevelResult>
    {
        
        internal OLevelResultConfiguration()
        {
            HasKey(k => k.ResultId);
            Property(k=>k.ResultId).HasMaxLength(150);
            Property(k => k.ExamNumber).HasMaxLength(50);
            Property(k => k.ExamType).HasMaxLength(100);
            Property(k => k.Venue).HasMaxLength(200);
            Property(k => k.StudentId).HasMaxLength(30);
            HasRequired(k => k.Student).WithMany().HasForeignKey(k => k.StudentId);
            
        }
    }

    internal class OlevelResultDetailConfiguration : EntityTypeConfiguration<OlevelResultDetail>
    {

        internal OlevelResultDetailConfiguration()
        {
            HasKey(k => k.DetailId);
            Property(k => k.DetailId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(k => k.Subject).HasMaxLength(100);
            Property(k => k.Grade).HasMaxLength(10);
            HasRequired(a => a.OlevelResult).WithMany(a=>a.OlevelResultDetail).HasForeignKey(a => a.ResultId)
                .WillCascadeOnDelete(true);
        }
    }
}
