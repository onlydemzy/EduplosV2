using System.Collections.Generic;
using System.Data;

namespace Eduplos.Web.SMC.ViewModels
{
    public class StatDashboardViewModel
    {
        public string CurrentSemester { get; set; }
        public double TotalCollections { get; set; }
        public int TotalStudents { get; set; }
        public int TotalAdmissions { get; set; }
        public List<ActiveStudentProgType> ApplicationsByProgTypes { get; set; }
        public List<ActiveStudentProgType> SemesterRegistrationsByProgTypes { get; set; }
        public List<ActiveStudentProgType> ActiveStudentProgTypes { get; set; }
        public List<FeesCollectedByProgType> FeesCollectedByProgTypes { get; set; }
        public List<SessionCollectionsSummary> SessionCollectionSummary { get; set; }

        public List<FeesCollectedByProgType> DailyCollectionByProgTypes { get; set; }
        public List<SessionCollectionsSummary> DailyCollectionSummaryByPaymentType { get; set; }
        public DataTable ChartData { get; set; }

    }

     
    public class ActiveStudentProgType
    {
        public string ProgType { get; set; }
        public int Total { get; set; }
    }
    public class FeesCollectedByProgType
    {
        public string Title { get; set; }
        public double Amount { get; set; }
    }
    public class SessionCollectionsSummary
    {
        public string AccountCode { get; set; }
        public string Item { get; set; }
        public double Amount { get; set; }
    }
}