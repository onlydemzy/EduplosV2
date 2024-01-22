using Eduplos.Domain.CoreModule;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    public class AuditLogConfiguration:EntityTypeConfiguration<AuditLog>
    {
        public AuditLogConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(a => a.UserId).HasMaxLength(100);
            Property(a => a.Action).HasMaxLength(10);
            Property(a => a.ColumnName).HasMaxLength(200);
            Property(a => a.TableName).HasMaxLength(100);
            Property(a => a.PrimaryKey).HasMaxLength(500);
        }

    }
}
