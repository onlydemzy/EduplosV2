using System.Collections.Generic;

namespace Eduplos.DTO.AcademicModule
{
    public class ProbationDetailsDTO
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string MatricNumber { get; set; }
        public int Level { get; set; }
        public string  CGPA { get; set; }
        public int Count { get; set; }
        public bool Graduate { get; set; }
        public bool Qualified { get; set; }
        public string Status { get; set; }
        public string Qualification { get; set; }
        
    }

    public class ProbationDTO
    {
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string Programme { get; set; }
        public string Session { get; set; }
        public string ProgrammeType { get; set; }
        public List<ProbationDetailsDTO> Details { get; set; }
    }
}
