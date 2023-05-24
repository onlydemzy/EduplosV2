using Eduplus.Domain.CoreModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.CoreModule
{
    internal class SessionConfiguration: EntityTypeConfiguration<Session>
    {
        internal SessionConfiguration()
        {
            HasKey(s => s.SessionId);
            Property(s => s.SessionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(s => s.Title).HasMaxLength(30).IsRequired();
            HasMany(s => s.Semesters).WithRequired(s => s.Session).HasForeignKey(f => f.SessionId);
        }
    }
}
