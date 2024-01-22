using KS.Domain.AccountsModule;

namespace Eduplos.Domain.BurseryModule
{
    public class FeeScheduleDetail
    {
        public int ScheduleDetailId { get; set; }
        public string AccountCode { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public string AppliesTo { get; set; }
        public virtual FeeSchedule FeeSchedule { get; set; }
        public virtual Accounts Accounts { get; set; }
        public int ScheduleId { get; set; }
    }
}
 