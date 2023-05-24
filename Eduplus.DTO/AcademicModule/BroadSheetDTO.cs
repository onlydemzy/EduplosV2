using System.Data;

namespace Eduplus.DTO.AcademicModule
{
    public class BroadSheetDTO
    {
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Programme { get; set; }
        public int Level { get; set; }
        public string Session { get; set; }
        public string Semester { get; set; }
        public string Name { get; set; }
        public string RegNo { get; set; }
        public string StudentId { get; set; }
        public string ProgrammeType { get; set; }
        public DataTable Results { get; set; }
    }
}
