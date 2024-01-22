using Eduplos.Domain.BurseryModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.BurseryModule
{
    internal class StudentPaymentsConfiguration:EntityTypeConfiguration<StudentPayments>
    {
        internal StudentPaymentsConfiguration()
        {
            HasKey(t => t.PaymentId);
            Property(t => t.PaymentId).HasMaxLength(300);
            Property(t => t.PaymentId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(t => t.Particulars).HasMaxLength(300);
            Property(t => t.PayMethod).HasMaxLength(30);
            Property(t => t.TransType).HasMaxLength(20);
            Property(t => t.PaidBy).HasMaxLength(100);
            Property(t => t.RegNo).HasMaxLength(15);
            Property(t => t.PaymentType).HasMaxLength(100);
            Property(t => t.VoucherNo).HasMaxLength(50);
            Property(a => a.ReferenceCode).HasMaxLength(250);
            HasRequired(t => t.Session).WithMany().HasForeignKey(a => a.SessionId);
             
            HasRequired(t => t.Student).WithMany(t => t.Payments).HasForeignKey(s => s.RegNo);
        }
            
    }
}
