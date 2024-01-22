using System.Threading;
using System.Linq;
using Eduplos.Domain.CoreModule;
using System.Collections.Generic;

namespace Eduplos.Domain.AcademicModule
{
    public class ProgrammeTypes
    {
        public ProgrammeTypes()
        {
            PaymentGateWays = new HashSet<PaymentGateways>();
        }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public int MaxCreditUnit { get; set; }
        public bool AutoGenerateMatricNo { get; set; }
        public byte AdmissionPause { get; set; }
        public bool ApplyGatewayCharge { get; set; }
        public bool ApplyMajorCharge { get; set; }
        public bool ApplyMinorCharge { get; set; }
        public int MaxCA1 { get; set; }
        public int MaxCA2 { get; set; }
        public int MaxExam { get; set; }
        public virtual ICollection<PaymentGateways> PaymentGateWays { get; set; }
        public bool CollectAcceptanceFee { get; set; }
        public bool AcceptAdmissionFee { get; set; }
        
        // public byte AdmissionPause { get; set; }
        //public float geta { get; set; }

    }
}
