using Eduplus.Domain.CoreModule;
using KS.Common;
using System.Collections.Generic;
using System.Linq;

namespace Eduplus.Domain.AcademicModule
{
    public class CourseRegRecover:EntityBase
    {
       
        public int RegistrationId { get; set; }
        public int SessionId { get; set; }
        public int SemesterId { get; set; }
        public string StudentId { get; set; }
        public string CourseId { get; set; }
        public int CourseCreditHour { get; set; }

        public int CA1{get;set;}
        public int CA2 { get; set; }
        public int Exam { get; set; }
        public string Grade { get; set; }
        public double GradePoint { get; set; }
        public bool IsApproved { get; set; }
        public int Lvl { get; set; }
         
        public int TScore { get { return this.CA1+this.CA2 + this.Exam; } }
        
        /*public string CreditUnitScore
        {

           get
            {
                return this.TScore.ToString() + "::" + Grade + "::" + this.QualityPointM.ToString();
            }
        }*/

        public string CreditHourHeading(string coursecode, int credithour)
        {


            return coursecode.Trim() + "\n(" + credithour.ToString().Trim() + ")";

        }
        public int CalculateTScore(int exam, int ca1,int ca2)
        {

            return (exam + ca1+ca2);

        }
         
        public Grading CalculateGrade(int score,List<Grading> grading)
        {
            return grading.Where(g => g.Low <= score && g.High >= score).FirstOrDefault();
        }

        public double QualityPointM(int chr, double gp)
        {
            return chr * gp;
        }

        public string RecordValueM(int score, string grade, double qpoint)
        {

            if(grade=="I")
            {
                return "::I::";
            }
            else
            {
                return score.ToString() + "::" + grade + "::" + qpoint.ToString();
            }
            

        }


    }
}
