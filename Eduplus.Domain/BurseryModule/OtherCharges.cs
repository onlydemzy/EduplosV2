using KS.Domain.AccountsModule;

namespace Eduplos.Domain.BurseryModule
{
    public class OtherCharges
    {
        public int ChargeId { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string ProgrammeType { get; set; }
         
    }
}
