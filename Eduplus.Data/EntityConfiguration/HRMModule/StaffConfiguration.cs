using KS.Domain.HRModule;
using System.Data.Entity.ModelConfiguration;

namespace Eduplus.Data.EntityConfiguration.HRMModule
{
    internal class StaffConfiguration : EntityTypeConfiguration<Staff>
    {
        internal StaffConfiguration()
        {
            Property(s => s.StaffCode).HasMaxLength(15);
            Property(s => s.Category).HasMaxLength(50);
            //HasRequired(s => s.Department).WithMany(d => d.People).HasForeignKey(s => s.DepartmentCode).WillCascadeOnDelete(false);
            Property(s => s.Designation).IsOptional().HasMaxLength(50);
            //HasRequired(s => s.User).WithOptional(u => u.);


        }
    }
}