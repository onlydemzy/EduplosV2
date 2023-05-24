using Eduplus.Domain.BurseryModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.BurseryModule
{
    internal class FeesExceptionsConfiguration:EntityTypeConfiguration<FeesExceptions>
    {
        internal FeesExceptionsConfiguration()
        {
            HasKey(t => t.ExceptionId);
            Property(t => t.ExceptionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Ignore(t => t.InputtedBy);
            Property(t => t.AuthorizedBy).HasMaxLength(100);
            
            Property(t => t.StudentName).HasMaxLength(200);
            Property(t => t.RegNo).HasMaxLength(20);
            Property(t => t.Department).HasMaxLength(100);
            //HasRequired(t => t.Session).WithMany().HasForeignKey(a => a.SessionId).WillCascadeOnDelete(false);
            HasRequired(t => t.Semester).WithMany().HasForeignKey(a => a.SemesterId);
            
        }
            
    }
}
