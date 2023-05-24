using Eduplus.Domain.CoreModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.CoreModule
{
    internal class ImagesConfiguration:EntityTypeConfiguration<AppImages>
    {
        internal ImagesConfiguration()
        {
            Property(a => a.ImageId).HasMaxLength(300);
            Property(a => a.Description).HasMaxLength(300);
            Property(a => a.FotoPath).HasMaxLength(300);
            HasKey(a => a.ImageId);
            
        }
    }
}
