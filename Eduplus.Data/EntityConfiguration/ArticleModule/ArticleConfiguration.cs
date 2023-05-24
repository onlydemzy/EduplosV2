using Eduplus.Domain.ArticleModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.ArticleModule
{
    internal class ArticleConfiguration:EntityTypeConfiguration<Article>
    {
        internal ArticleConfiguration()
        {
            HasKey(a => a.ArticleId);
            Property(a => a.ArticleId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.WrittenBy).HasMaxLength(200);
            Property(a => a.Title).HasMaxLength(300);
            Property(a => a.Content);
            Property(a => a.Type).HasMaxLength(50);
            Property(a => a.ImageId).HasMaxLength(300);
            HasOptional(a => a.Photo).WithMany().HasForeignKey(a => a.ImageId);

        }
    }
}
