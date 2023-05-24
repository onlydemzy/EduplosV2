using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eduplus.DTO.AcademicModule
{
    public class JambDTO
    {
        
        public string StudentId { get; set; }
        public string JambRegNumber { get; set; }
        public int JambYear { get; set; }
        public List<JambScoresDTO> Scores { get; set; }
        public int Total { get; set; }
    }
    public class JambScoresDTO
    {
        public string Subject { get; set; }
        public string StudentId { get; set; }
        public string JambRegNumber { get; set; }
        public int JambYear { get; set; }
        public int Score { get; set; }
        public long ScoreId { get; set; }

    }

}