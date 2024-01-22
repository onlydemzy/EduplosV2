using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eduplos.Web.SMC.ViewModels
{
    public class ApplicantsVM
    {
        public string RegNo { get; set; }
public string Name { get; set; }
  public string Programme { get; set; }       
        
        
        public string Phone { get; set; }
        public string Email { get; set; }
        public string JambNo { get; set; }
        public int JambScore { get; set; }
        public string State { get;set; }
        public string Lga { get; set; }
        
        public string Status { get; set; }
        public byte AddmissionCompleteStage { get; set; }
    }
}