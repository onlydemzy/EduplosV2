using Eduplos.Domain.AcademicModule;
using Eduplos.DTO.AcademicModule;
using Eduplos.Services.Contracts;
using KS.Web.Security;
using Rotativa;
using Rotativa.Options;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Eduplos.Web.SMC.Controllers
{
    [KS.Web.Security.KSWebAuthorisation]
    public class AcademicAffairsController : BaseController
    {
        // GET: AcademicAffairs
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IAcademicAffairsService _academicAffairs;
        private readonly IAcademicProfileService _academicProfile;
        
        public AcademicAffairsController(IGeneralDutiesService generalDuties,IAcademicAffairsService academicAffairs,IAcademicProfileService acada)
        {
            _generalDuties = generalDuties;
            _academicAffairs = academicAffairs;
            _academicProfile = acada;
        }
        public ActionResult Courses()
        {
            //_academicAffairs.UpdateCourseRecovery();
            return View();
        }


        public CourseDTO SaveCourse(CourseDTO viewModel)
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if(viewModel.ProgrammeCode=="undefined"|| string.IsNullOrEmpty(viewModel.ProgrammeCode))
            {
                viewModel.ProgrammeCode = user.ProgrammeCode;
            }
            var chk= _generalDuties.SaveCourse(viewModel, user.ProgrammeCode, User.UserId);
            return chk;
        }

        public ActionResult EditCourseRegistration()
        {
            return View();
        }
        public JsonResult FetchRegistedCoursesByStudent(int semesterId, string regNo)
        {
            return Json(_academicProfile.FetchStudentRegisteredCourses(semesterId, regNo), JsonRequestBehavior.AllowGet);
        }

        public string DeleteCourseFromRegistration(CourseRegistrationDTO course)
        {
            return _academicProfile.DeleteCourseFromCourseRegistration(course,User.UserId);
            
        }
        public string DeleteCourseRegistration(object[] data1)
        {
            int semesterId = (int)data1[0];
            string regNo = (string)data1[1];
            return _academicProfile.DeleteCourseRegistration(regNo, semesterId, User.UserId);
        }
        public string AddCourseToRegistration(CourseRegistrationDTO course)
        {
            return _academicProfile.AddCourseToCourseRegistration(course, User.UserId);
        }

        public ActionResult StudentOutstandings()
        {
            return View();
        }
        public JsonResult FetchStudentOutstandings(string matricNo)
        {
            string st;
            if (string.IsNullOrEmpty(matricNo))
            {
                st = User.UserId;
            }
            else
            {
                st = matricNo;
            }
            return Json(_academicProfile.FetchStudentOutstandings(matricNo), JsonRequestBehavior.AllowGet);
        }
        public string DeleteOutstanding(int outstandingId)
        {
            return _academicAffairs.DeleteOutstandingCourse(outstandingId, User.UserId);
        }
        #region COURSESCHEDULE OPERATIONS
        public ActionResult ScheduleCourses()
        {
            return View();
        }
        public string SaveSchedule(CourseScheduleDTO schedules)
        {
            return _academicAffairs.AddCourseSchedules(schedules,User.UserId);
        }

        
        public JsonResult DepartmentalProgrammes()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user.IsSysAdmin == true)
                return Json(_generalDuties.FetchProgrammes(null), JsonRequestBehavior.AllowGet);
            else
                return Json(_generalDuties.FetchProgrammes(user.DepartmentCode), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CoursesForSchedule(string semester,string progCode)
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            string _progCode;
            if (user.IsSysAdmin == true)
                _progCode = progCode;

            else
                _progCode = user.ProgrammeCode;

            return Json(_generalDuties.PopulateCourse(_progCode,  semester), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeptLecturers(string deptCode)
        {
            string dept = null;
            if(string.IsNullOrEmpty(deptCode) || deptCode == "undefined"){
                var user = (CustomPrincipal)Session["loggedUser"];
                dept = user.DepartmentCode;
            }
            else { dept = deptCode; }
            return Json(_academicAffairs.FetchLecturersForCourseAllocation(dept), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region POST EXAMS OPERATIONS

        public ActionResult GeneratePostExamReports()
        {
            return View();
        }
        public ActionResult InputtedResultSheet(int semesterId, string courseId)
        {

                var students = _academicAffairs.LecturerScoreSheet(semesterId, courseId);

                return new ViewAsPdf(students)
                {

                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                    PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
                };
     
        }

        public ActionResult LecturerScoreSheet()
        {
            return View();
        }

        public JsonResult LecturerCourses(int semesterId,string lecturerId)
        {
            if(lecturerId=="undefined" || string.IsNullOrEmpty(lecturerId))
            {
                 
                lecturerId = User.UserId;
            }
            var courses = _academicAffairs.LecturerCourses(lecturerId, semesterId);
            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LecturerScoreSheet(int semesterId, string progCode)
        {

            if (semesterId == 0 || string.IsNullOrEmpty(progCode))
                return RedirectToAction("PreExamReport");
            else
            {
                var students = _academicAffairs.ExamAttendance(semesterId, progCode);
                return new ViewAsPdf(students)
                {

                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                    PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
                };
            }
        }

        public ActionResult CourseRegistrations()
        {
            return View();
        }
        //public JsonResult FetchCourseRegistrations(string programCode, int semester)
        //{

        //}
        #endregion

        #region PRE EXAM OPERATIONS/REPORT
        public ActionResult PermitStudentRegistration()
        {
            return View();
        }
        public string SubmitRegPermission(int semesterId,string regNo)
        {
            
            return _academicAffairs.AllowStudentRegistration(semesterId, regNo, User.UserId);
        }
        public ActionResult PreExamReports()
        {
            return View();
        }

        public ActionResult ExamsAttendanceByProgramme(int semesterId, string progCode)
        {

            
                var students = _academicAffairs.ExamAttendance(semesterId, progCode);
                return new ViewAsPdf(students)
                {

                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                    PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
                };
           
        }
        public ActionResult ExamsAttendanceByCourse(int semesterId, string courseId)
        {

           
            
                var students = _academicAffairs.ExamAttendanceByCourse(semesterId, courseId);
                return new ViewAsPdf(students)
                {

                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                    PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
                };
           
        }
        public ActionResult ExamsScoreSheet(int semesterId, string progCode)
        {

            
                var students = _academicAffairs.ExamAttendance(semesterId, progCode);
                return new ViewAsPdf(students)
                {

                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                    PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
                };
           
        }

        public ActionResult GetSemesterRegistrations()
        {
            return View();
        }
        public ActionResult ViewSemesterRegistrations()
        {
            return View();
        }

        public ActionResult SemesterRegistrations(int semesterId,string progCode)
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            List<StudentSemesterProfileDTO> list = new List<StudentSemesterProfileDTO>();
            
            list = _academicAffairs.GetREgisteredStudents(semesterId, progCode);
           

            ViewBag.total = list.Count;
            if (list.Count == 0)
            { ViewBag.semester = ""; }
            else { ViewBag.semester = list.First().Semester; }
            return new ViewAsPdf(list.OrderBy(a=>a.Programme).ToList()){PageSize=Size.A4,
            PageOrientation=Orientation.Portrait,
                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
            };
            
        }
        #endregion

        #region EXAMS OFFICERS
        public ActionResult ExamsOfficers()
        {
            return View();
        }
        public JsonResult FetchExamsOfficers(string program)
        {
            return Json(_academicAffairs.GetCurrentExamsOfficers(program), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CALENDER OPERATIONS
        public ActionResult AcademicCalender()
        {
            return View();
        }

        public ActionResult CalenderDetails(int id)
        {
            return View();
        }
        public JsonResult AcademicCalenders()
        {
            var cals = _academicAffairs.FetchCalenders();
            return this.Json(cals, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AcademicCalenderDetails(int id)
        {
            var cals = _academicAffairs.FetchCalenderDetails(id);
            return this.Json(cals, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditCalender()
        {
            return View();
        }

        public void SaveCalender(CalenderDTO calender)
        {

            _academicAffairs.SaveCalender(calender, User.UserId);
        }
        public void SaveCalenderDetail(CalenderDetailsDTO calenderDetail)
        {

            _academicAffairs.SaveCalenderDetail(calenderDetail, User.UserId);
        }
        public void DeleteCalenderDetail(CalenderDetailsDTO viewModel)
        {

            _academicAffairs.RemoveCalenderDetail(viewModel, User.UserId);
        }
        #endregion

        #region GRADING SYSTEM
        public ActionResult GradingSystem()
        {
            return View();
        }
        public JsonResult AllGradings()
        {
            return Json(_academicAffairs.AllGrades(), JsonRequestBehavior.AllowGet);
        }
        public GraduatingClass SaveGrade(GraduatingClass viewModel)
        {
            return _academicAffairs.SaveGraduatingClass(viewModel, User.UserId);
        }
        public string DeleteGrade(Grading viewModel)
        {
             _academicAffairs.DeleteGrade(viewModel.GradeId, User.UserId);
            return "Operation was successfull";
        }
        public ActionResult GradClassSystem()
        {
            return View();
        }
        public JsonResult AllGradClasses()
        {
            return Json(_academicAffairs.AllGradClasses(), JsonRequestBehavior.AllowGet);
        }
        public GraduatingClass SaveGradClass(GraduatingClass viewModel)
        {
            return _academicAffairs.SaveGraduatingClass(viewModel, User.UserId);
        }
        public string DeleteGradClass(GraduatingClass viewModel)
        {
            _academicAffairs.DeleteGradClass(viewModel.ClassId, User.UserId);
            return "Operation was successfull";
        }

        public ActionResult OlevelSubjects()
        {
            return View();
        }
        #endregion

        #region COURSE CATEGORY
        public ActionResult CourseCategories()
        {
            return View();
        }
        #endregion
    }
}