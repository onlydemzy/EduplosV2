namespace Eduplos.Domain.BurseryModule
{
    public class FeeOptions
    {
        public int OptionsId { get; set; }
        public string Installment { get; set; }
        public double PercentageTuition { get; set; }
        public string ProgrammeType { get; set; }
        public bool Register1 { get; set; }
        public bool Register2 { get; set; }
        public bool Write1 { get; set; }
        public bool Register3 { get; set; }
        public bool Write3 { get; set; }
        public bool Write2 { get; set; }
        public double PercentageSundry { get; set; }//Fees schedule should apply to: Sundry, tuition, Bill
        public bool Enabled { get; set; }
        public int Cycle { get; set; }

    }
   
}
