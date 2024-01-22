using Eduplos.Domain.CoreModule;
using KS.Domain.HRModule;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Eduplos.Data.EntityConfiguration.CoreModule
{
    internal class PersonConfiguration:EntityTypeConfiguration<Person>
    {
        internal PersonConfiguration()
        {
            HasKey(p => p.PersonId);
            Property(p => p.PersonId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Surname).HasMaxLength(50).IsRequired();
            Property(p => p.PersonId).HasMaxLength(128);
            Property(p => p.Firstname).HasMaxLength(100).IsRequired();
            Property(p => p.MIddlename).HasMaxLength(100).IsOptional();
            Property(p => p.Title).HasMaxLength(20).IsOptional();
            Property(p => p.ResidentialAddress).HasMaxLength(100).IsOptional();
            Property(p => p.Phone).HasMaxLength(15).IsOptional();
            Property(p => p.Email).HasMaxLength(50).IsOptional();
            Ignore(p => p.Name);
            Property(p => p.MaritalStatus).HasMaxLength(20);
            Property(p => p.IDNumber).HasMaxLength(30);
            Property(p => p.IDType).HasMaxLength(20);
            Property(p => p.MailingAddress).IsOptional().HasMaxLength(100);
            
            Property(p => p.Sex).HasMaxLength(10).IsOptional();
            
            Property(p => p.Relationship).HasMaxLength(50);
            Property(p => p.kinAddress).HasMaxLength(100);
            Property(p => p.Country).HasMaxLength(100);
            Property(p => p.State).HasMaxLength(100);
            Property(p => p.HighestQualification).HasMaxLength(100);
            Property(p => p.Lg).HasMaxLength(100);
            Property(p => p.KinMail).HasMaxLength(50);
            Property(p => p.KinPhone).HasMaxLength(20).IsOptional();
            Property(p => p.PhotoId).HasMaxLength(300).IsOptional();
            HasOptional(a => a.Department).WithMany().HasForeignKey(p => p.DepartmentCode);
            HasOptional(s => s.Programme).WithMany().HasForeignKey(s => s.ProgrammeCode);
            Property(s => s.ProgrammeCode).HasMaxLength(30);
            HasOptional(a => a.Photo).WithMany().HasForeignKey(a => a.PhotoId);
            Property(p => p.Status).HasMaxLength(30);
            Map<Student>(m => m.ToTable("Student"));
            Map<Staff>(m => m.ToTable("Staff"));

            Property(a => a.HomeTown).HasMaxLength(100);
            Property(a => a.SpouseName).HasMaxLength(100);
            Property(a => a.SpouseAddress).HasMaxLength(150);
            Property(a => a.PermanentHomeAdd).HasMaxLength(150);


        }
    
    }
}
