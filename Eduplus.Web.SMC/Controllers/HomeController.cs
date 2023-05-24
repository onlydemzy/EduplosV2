using Eduplus.Services.Contracts;
using Eduplus.Web.SMC.ViewModels;
using KS.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Eduplus.Web.SMC.Controllers
{

    public class HomeController : BaseController
    {
        // GET: Home
        IGeneralDutiesService _generalDuties;
        IStudentService _studentService;
        IBursaryService _bursaryService;
        IAcademicProfileService _academicServices;
        IAcademicAffairsService _acadaAffairs;

        public HomeController(IGeneralDutiesService genduties,IStudentService studentService, IBursaryService bursaryService,
            IAcademicProfileService acadaService, IAcademicAffairsService acadAffairs)
        {
            _generalDuties = genduties;
            _studentService = studentService;
            _bursaryService = bursaryService;
            _academicServices = acadaService;
            _acadaAffairs = acadAffairs;

        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Accounts");
            }

            ViewBag.photo = user.Photo;
            return View();
        }
       
        public ActionResult _MenuPartial()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user == null)
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Accounts", new { returnUrl = Request.Url });
            }
            var menus = user.UserMenus;

            ViewBag.photo = user.Photo;

            return PartialView(menus);
        }
        public ActionResult _MenuPartialNew()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user == null)
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Accounts", new { returnUrl = Request.Url });
            }
            var menus = user.UserMenus;

            ViewBag.photo = user.Photo;

            return PartialView(menus);
        }

        public ActionResult _ProsMenuPartial()
        {
            var user = (CustomPrincipal)Session["loggedUser"];
            if (user!=null)
            {
                ViewBag.foto = user.Photo;
            }
            return PartialView();
        }
        public ActionResult _AlumniMenuPartial()
        {
            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult StudentHome()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Unauthorised()
        {
            return View();
        }

       public ActionResult TranscriptApplication()
        {
            return View();
        }
        
        [KSWebAuthorisation]
        public ActionResult Dashboard()
        {
            var currentSemester = _generalDuties.FetchCurrentSemester();
             
            var ses = currentSemester.Session.Title;
            var activeStudents = _studentService.TotalActiveStudentsByProgramme();
            var enrolProgs = _studentService.CurrentStudentEnrollmentsByProgType();
            var applicantsByProgType = _studentService.SessionAdmissionsSummaryProgType(ses);
            var feesCollectionByProgType = _bursaryService.SessionTotalCollectionsProgrammeType(ses);
            var totalRegs = _acadaAffairs.TotalSemesterRegistrationsByProgType(currentSemester.SemesterId);//Takes a lot of time
            

            StatDashboardViewModel dash = new StatDashboardViewModel();
            List<ActiveStudentProgType> enr = new List<ActiveStudentProgType>();
             
            List<FeesCollectedByProgType> fec = new List<FeesCollectedByProgType>();
            List<ActiveStudentProgType> applications = new List<ActiveStudentProgType>();
            dash.TotalStudents = 0;
            //Current Enrollment
            if (enrolProgs.Count > 0)
            {
                foreach(var e in enrolProgs)
                {
                    enr.Add(new ActiveStudentProgType
                    {
                        ProgType = e.ProgrammeType,
                        Total = e.TotalEnrollment
                    });
                }
                dash.TotalStudents = enr.Sum(e => e.Total);
            }
            List<SessionCollectionsSummary> sesSum = new List<SessionCollectionsSummary>();
            dash.ActiveStudentProgTypes = enr;
            dash.CurrentSemester=currentSemester.SemesterTitle+", "+ses;
            dash.TotalAdmissions = 0;
            
            dash.TotalCollections = _bursaryService.SessionTotalCollections(ses);
            //Current Applicants
            if(applicantsByProgType.Count>0)
            {
                foreach (var a in applicantsByProgType)
                {
                    applications.Add(new ActiveStudentProgType
                    {
                        ProgType = a.ProgrammeType,
                        Total = a.TotalEnrollment
                    });
                }
                dash.TotalAdmissions = applications.Sum(a => a.Total);
            }

            //Fees collection by progtype
            List<FeesCollectedByProgType> fc = new List<FeesCollectedByProgType>();
            if (feesCollectionByProgType.Count > 0)
            {
                foreach(var f in feesCollectionByProgType)
                {
                    fc.Add(new FeesCollectedByProgType
                    {
                        Title = f.Title,
                        Amount = f.Total
                    });
                }
                dash.FeesCollectedByProgTypes = fc;
            }
            else
            {
                dash.FeesCollectedByProgTypes = new List<FeesCollectedByProgType>();
            }
            
            dash.ApplicationsByProgTypes = applications;
            var currentCollections = _bursaryService.RevenueAccountsSummaryCollectionsBySession(ses).OrderBy(a=>a.Title).ToList();

            if (currentCollections.Count() > 0)
            {
                foreach (var c in currentCollections)
                {
                    sesSum.Add(new SessionCollectionsSummary
                    {
                        AccountCode = c.AccountCode,
                        Item = c.Title,
                        Amount = c.Amount

                    });
                }
                
                var items = (from cs in sesSum
                                 select cs.Item).ToList();
                var amount = (from cs in sesSum
                              select cs.Amount).ToList();
                ViewBag.items =items;
                ViewBag.amounts = string.Join(",", amount).Trim();
                dash.ChartData = ConvertToDataTable(currentCollections);
                dash.SessionCollectionSummary = sesSum;
            }

            //Summary Daily Collections
            var dailyCollectionByProgType = _bursaryService.DailyPaymentsByProgType(DateTime.UtcNow);
            var dailyCollectionsByPayType = _bursaryService.DailyPaymentsByPayType(DateTime.UtcNow);
            List<FeesCollectedByProgType> dCPT = new List<FeesCollectedByProgType>();
            List<SessionCollectionsSummary> dCPayT = new List<SessionCollectionsSummary>();
            if (dailyCollectionByProgType.Count>0)
            {
                
                foreach(var d in dailyCollectionByProgType)
                {
                    dCPT.Add(new FeesCollectedByProgType
                    {
                        Amount = d.Amount,
                        Title = d.ProgrammeType

                    });
                }
                
        
            }
            if (dailyCollectionsByPayType.Count > 0)
            {

                foreach (var d in dailyCollectionsByPayType)
                {
                    dCPayT.Add(new SessionCollectionsSummary
                    {
                        Amount = d.Amount,
                        Item = d.PaymentType

                    });
                }


            }

            List<ActiveStudentProgType> corsRegStudent = new List<ActiveStudentProgType>();
            if(totalRegs.Count>0)
            {
                foreach(var c in totalRegs)
                {
                    corsRegStudent.Add(new ActiveStudentProgType
                    {
                        ProgType = c.ProgramTpe,
                        Total = c.Total
                    });
                }
            }
            dash.SemesterRegistrationsByProgTypes = corsRegStudent;
            dash.DailyCollectionByProgTypes = dCPT;
            dash.DailyCollectionSummaryByPaymentType = dCPayT;
            return View(dash);
        }

    }
}