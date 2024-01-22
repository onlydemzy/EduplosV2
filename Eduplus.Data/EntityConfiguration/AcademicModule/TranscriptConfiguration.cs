using Eduplos.Domain.AcademicModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.AcademicModule
{
    internal class TranscriptConfiguration:EntityTypeConfiguration<TranscriptApplication>
    {
        public TranscriptConfiguration()
        {
            Property(a => a.ApplicationId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            HasKey(a => a.ApplicationId);
            Property(a => a.DeliveryAddress).HasMaxLength(300).IsRequired();
            Property(a => a.DeliveryEmail).HasMaxLength(100);
            Property(a => a.Recipient).HasMaxLength(200).IsRequired();
            Property(a => a.DeliveryMode).HasMaxLength(100).IsRequired();
            Property(a => a.Student).HasMaxLength(200).IsRequired();
            Property(a => a.City).HasMaxLength(100).IsRequired();
            Property(a => a.State).HasMaxLength(100).IsRequired();
            Property(a => a.Country).HasMaxLength(70).IsRequired();
            Property(a => a.StudentId).HasMaxLength(100).IsRequired();
            Property(a => a.TranscriptNo).HasMaxLength(300);
            Ignore(a => a.Phone);
        }
    }
}
