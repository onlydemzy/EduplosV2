using Eduplus.DTO.AcademicModule;
using Eduplus.Services.Contracts;
using Rotativa;
using Rotativa.Options;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Eduplus.Web.Controllers
{

    public class AdmissionsController : BaseController
    {
        // GET: Addmissions
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IStudentService _studentService;
        private readonly IArticleService _articleService;
        private readonly ICommunicationService _commService;
        private readonly IBursaryService _bursaryService;
        private readonly IAcademicAffairsService _academicAffairs;
        public AdmissionsController(IGeneralDutiesService generalDuties, IStudentService studentService,
            IArticleService articleService, ICommunicationService commService,IBursaryService bursaryService, IAcademicAffairsService academicService)
        {
            _generalDuties = generalDuties;
            _studentService = studentService;
            _articleService = articleService;
            _commService = commService;
            _bursaryService = bursaryService;
            _academicAffairs = academicService;

        }

        public ActionResult Instructions()
        {
            return View();
        }
        public ActionResult DegreeProgramme()
        {
            var progs = _generalDuties.AllProgrammesByFaculty();
            return View(progs);
        }
        
        public ActionResult Departments()
        {
            var depts = _generalDuties.AllAcademicDepartments();
            return View(depts);
        }
        public ActionResult View_Detail(string item)
        {
            var detail = _articleService.FetchFacultyArticle(item);
            return View(detail);
        }

        [AllowAnonymous]
        public JsonResult PopulateFaculty()
        {

            var faculties = _generalDuties.FetchFaculties().ToList();

            return this.Json(faculties, JsonRequestBehavior.AllowGet);
        } 
        [AllowAnonymous]
        public JsonResult PopulateDepartment(string _facultyCode)
        {
           
                var depts = _generalDuties.FetchDepartments(_facultyCode)
                            .OrderBy(a => a.Title).ToList();
                return this.Json(depts, JsonRequestBehavior.AllowGet);
           
        }
        [AllowAnonymous]
        public JsonResult PopulateCountry()
        {

            var depts =_generalDuties.FetchCountries();
                            //.OrderBy(a => a.Title).ToList();
                return this.Json(depts, JsonRequestBehavior.AllowGet);
          
        }
        [AllowAnonymous]
        public JsonResult PopulateState(string _country)
        {
            
                var depts = _generalDuties.FetchStates(_country);
                return this.Json(depts, JsonRequestBehavior.AllowGet);
           
        }

       
        [AllowAnonymous]
        public JsonResult PopulateLga(string _state)
        {
            
                var depts = _generalDuties.FetchLgs(_state);
                return this.Json(depts, JsonRequestBehavior.AllowGet);
           
        }
        #region ACADEMIC CALENDER
        [AllowAnonymous]
        public ActionResult AcademicCalender()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult CurrentAcademicCalender()
        {
            var calender = _academicAffairs.FetchCurrentCalender();

            return new ViewAsPdf(calender)
            {
                PageSize = Size.A4,
                PageOrientation=Orientation.Portrait,
                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
            };
        }
        #endregion



        [HttpGet]
        public ActionResult CurrentFeeSchedule()
        {
            
            return View();
        }
        [HttpGet]
        public ActionResult CurrentFeeSchedules(string faculty,string type)
        {
            var schedule = _bursaryService.FetchCurrentFeeSchedule(faculty,type);

            return new ViewAsPdf(schedule)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
            };
        }

        #region FRESHMEN PROFILE CREATION
        [HttpGet]
        public ActionResult NewStudentProfile()
        {
            return View();
        }

        public int SubmitNewStudent(ProspectiveStudentDTO data)
        {
            int stat = 0;//some has taken your email
            
            string studentId;
            string mail = data.Email;
            string pasword = data.Password;
            string mailresponse;
            string fullname = data.Title + " " + data.Surname + ", " + data.Firstname + " " + data.MIddlename;
            stat = _studentService.CreateNewStudentProfile(data, out studentId);
            if (stat == 1)//send mail
            {
                string msgBody = "Your personal data profile was successfully created." + "\n" +
                "Login to the registration portal to complete your registration." + "\n" +
                "Your login details is as follows: " + "\n" +
                "Registration Number= " + studentId + "\n" + "Username= " + mail + "Password= " + pasword + "\n";
                mailresponse = _commService.SendMail(mail, msgBody,"Profile Creation");
                if (mailresponse == "Ok")
                {
                    stat=1;
                }
                return stat;

            }
            return stat;
        }

        public int SubmitNewStudent1(ProspectiveStudentDTO data)
        {
            int stat = 0;//some has taken your email

            string studentId;
            string mail = data.Email;
            string pasword = data.Password;
            //string mailresponse;
            string fullname = data.Title + " " + data.Surname + ", " + data.Firstname + " " + data.MIddlename;
            stat = _studentService.CreateNewStudentProfile(data, out studentId);
            if (stat == 1)//send mail
            {
                
                return stat;

            }
            return stat;
        }
        #endregion
        void PopulateFaculties()
        {

            var _faculties = _generalDuties.FetchFaculties().ToList();

            ViewBag.faculties=new SelectList(_faculties,"FacultyCode","Title");
        }
        private void ProgrameTypes()
        {
            List<string> st = new List<string> {
                "Degree",
                "Predegree"
            };


            ViewBag.ProgrammeType = new SelectList(st);
           
        }

        public JsonResult AllProgrammes()
        {
            return Json(_generalDuties.FetchProgrammes(), JsonRequestBehavior.AllowGet);
        }

    }
}