using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplos.DTO.AcademicModule
{
    public class ProgrammeTypesDTO
    {
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public int MaxCreditUnit { get; set; }
        public bool AutoGenerateMatricNo { get; set; }
        public byte AdmissionPause { get; set; }
        public bool ApplyGatewayCharge { get; set; }
        public bool ApplyMajorCharge { get; set; }
        public bool ApplyMinorCharge { get; set; }
        public bool CollectAcceptanceFee { get; set; }
        public int MaxCA1 { get; set; }
        public int MaxCA2 { get; set; }
        public int MaxExam { get; set; }

    }
}
