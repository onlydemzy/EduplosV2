using Eduplos.Domain.CoreModule;
using KS.Common;
using System.Collections.Generic;
using System.Linq;

namespace Eduplos.Domain.AcademicModule
{
    public class JambResult:EntityBase
    {
        public JambResult()
        {
            Scores = new HashSet<JambScores>();
        }
        public string JambRegNumber { get; set; }
        public string StudentId { get; set; }
        public int JambYear { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<JambScores> Scores { get; set; }

        public int Score
        {
            get
            {
                return Scores.Sum(s => s.Score);
            }
        }
        
    }

    public class JambScores
    {
        public long ScoreId { get; set; }
        public string JambRegNumber { get; set; }
        public string Subject { get; set; }
        public virtual JambResult JambResult { get; set; }
        public int Score { get; set; }
    }
}
