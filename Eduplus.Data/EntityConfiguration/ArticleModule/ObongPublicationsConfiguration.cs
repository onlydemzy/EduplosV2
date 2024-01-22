using Eduplos.Domain.ArticleModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.ArticleModule
{
    internal class ObongPublicationsConfiguration : EntityTypeConfiguration<ObongPublications>
    {
        internal ObongPublicationsConfiguration()
        {
            HasKey(a => a.PublicationId);
            Property(a => a.PublicationId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.Author).HasMaxLength(100);
            Property(a => a.Title).HasMaxLength(200);
            Property(a => a.Path).HasMaxLength(300);
            Property(a => a.PaperBackId).HasMaxLength(300);
            HasOptional(a => a.Photo).WithMany().HasForeignKey(a => a.PaperBackId);

        }
    }
}
