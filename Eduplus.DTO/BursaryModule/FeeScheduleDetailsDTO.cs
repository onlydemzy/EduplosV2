namespace Eduplus.DTO.BursaryModule
{
    public class FeeScheduleDetailsDTO
    {
        public int ScheduleDetailId { get; set; }
        public string AccountCode { get; set; }
        public double Amount { get; set; }
        public int ScheduleId { get; set; }
        public string AppliesTo { get; set; }
        public string Type { get; set; }
        public string Account { get; set; }
        
        public string ProgrammeType { get; set; }
        public string FacultyCode { get; set; }
        public int SessionId { get; set; }
        public string Status { get; set; }
        
    }
}
