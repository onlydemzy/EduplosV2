using System;

namespace Eduplos.Domain.CoreModule
{
    public class AuditLog
    {
        public long Id { get; set; }

        public string Action { get; set; }

        public string TableName { get; set; }

        public string PrimaryKey { get; set; }

        public string ColumnName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public DateTime EventDate { get; set; }

        public string UserId { get; set; }

        
    }
    
}
