using Eduplos.Domain.CoreModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    internal class DocumentConfiguration : EntityTypeConfiguration<StudentDocuments>
    {
        public DocumentConfiguration()
        {
            HasKey(a => a.DocumentId);
            Property(a => a.DocumentId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Path).HasMaxLength(200);
            Property(a => a.PersonId).HasMaxLength(30);
        }
    }
}