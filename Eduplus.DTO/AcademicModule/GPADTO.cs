namespace Eduplus.DTO.AcademicModule
{
    public class GPADTO
    {
        public string RegNo { get; set;  }
        public int TotCreditHour { get; set; }
        public double TotQualitPoints { get; set; }
        public string GPA{get;set;}
        public string CourseCategory { get; set; }

        public string SumRecord {
            get
            {
                return "TC="+ TotCreditHour.ToString() + "  " + "TQP="+ TotQualitPoints.ToString() + "  " + "GPA="+GPA.ToString();
            }
        }

    }
    public class FinalGPADTO
    {
        public string RegNo { get; set; }
        public int TotCreditHour { get; set; }
        public double TotQualitPoints { get; set; }
        public string CGPA { get; set; }
        public string Class { get; set; }
        
    }
}
