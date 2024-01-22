using Eduplos.Domain.BurseryModule;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.Data.EntityConfiguration.BurseryModule
{
    internal class InvoiceConfiguration:EntityTypeConfiguration<PaymentInvoice>
    {
        internal InvoiceConfiguration()
        {
            HasKey(a => a.TransactionId);
            Property(a => a.TransactionId).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            
            Property(a => a.GeneratedBy).HasMaxLength(200);
            Property(a => a.Name).HasMaxLength(100);
            Property(a => a.Particulars).HasMaxLength(300); 
            Property(a => a.PaymentType).HasMaxLength(300);
            Property(a => a.ProgrammeType).HasMaxLength(50);
            Property(a => a.Department).HasMaxLength(100);
            Property(a => a.Programme).HasMaxLength(100);
            Property(a => a.Regno).HasMaxLength(50);
            Property(a => a.Session).HasMaxLength(20);
            Property(a => a.Semester).HasMaxLength(50);
            Property(a => a.StudentId).HasMaxLength(20);
            Property(a => a.Status).HasMaxLength(300);
            Property(a => a.TransRef).HasMaxLength(100);
            Property(a => a.Installment).HasMaxLength(20);
            HasMany(a => a.Details).WithRequired(a=>a.PaymentInvoice).HasForeignKey(a => a.InvoiceNo).WillCascadeOnDelete(true);
            HasOptional(a => a.FeeOptions).WithMany().HasForeignKey(a => a.PayOptionId);
            HasRequired(a => a.Student).WithMany().HasForeignKey(a => a.StudentId);
            Ignore(a => a.Email);
            Ignore(a => a.Phone);
            Ignore(a => a.Photo);
            
        }
    }
}
