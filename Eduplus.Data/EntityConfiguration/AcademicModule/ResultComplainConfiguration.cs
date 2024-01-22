using Eduplos.Domain.AcademicModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class ResultComplainConfiguration:EntityTypeConfiguration<ResultComplain>
    {
        internal ResultComplainConfiguration()
        {
            HasKey(r => r.ComplainId);
            Property(r => r.ComplainId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(r => r.Complain).HasMaxLength(700);
            Property(r => r.ExamsOfficer).HasMaxLength(150);
            Property(r => r.HOD).HasMaxLength(150);
            Property(r => r.VC).HasMaxLength(150);
            Property(r => r.CourseLecturerComment).HasMaxLength(200);
            Property(r => r.VCComment).HasMaxLength(200);
            Property(r => r.CourseLecturerComment).HasMaxLength(200);
            Property(r => r.Programme).HasMaxLength(200);
            Property(r => r.ProgrammeCode).HasMaxLength(20);
            Property(r => r.Semester).HasMaxLength(20);
            Property(r => r.Session).HasMaxLength(20);
            
        }
    }
}
