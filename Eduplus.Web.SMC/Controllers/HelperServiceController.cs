
using Eduplos.DTO.AcademicModule;
using Eduplos.DTO.CoreModule;
using Eduplos.Services.Contracts;
using KS.Services.Contract;
using KS.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Eduplos.Web.SMC.Controllers
{
    
    public class HelperServiceController : BaseController
    {
        // GET: HelperService
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IStaffService _staffService;
        private readonly IUserService _userService;
        private readonly IBursaryService _bursaryService;
        private readonly IAccountsService _accountsService;
        private readonly IAcademicAffairsService _academAffairs;
        private readonly IStudentService _studentService;
        public HelperServiceController(IGeneralDutiesService generalDuties,IStaffService staffService,IUserService userService,
            IBursaryService bursaryService,IAccountsService accountsService, IAcademicAffairsService acada,
            IStudentService studentService)
        {
            _generalDuties = generalDuties;
            _staffService = staffService;
            _userService = userService;
            _bursaryService = bursaryService;
            _accountsService = accountsService;
            _academAffairs = acada;
            _studentService = studentService;
        }
        public JsonResult PopulateDepartment()
        {

            var depts = _generalDuties.FetchDepartments();
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PopulateAcademicDepartment()
        {

            var depts = _generalDuties.AllAcademicDepartments();
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }

        public JsonResult PopulateDepartment1(string _facultyCode)
        {
            if (string.IsNullOrEmpty(_facultyCode))
            { _facultyCode = "default"; }
            var depts = _generalDuties.FetchDepartments(_facultyCode);
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PopulateCountry()
        {

            var depts = _generalDuties.FetchCountries();
            //.OrderBy(a => a.Title).ToList();
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PopulateState(string _country)
        {

            var depts = _generalDuties.FetchStates(_country);
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }



        public JsonResult PopulateLga(string _state)
        {

            var depts = _generalDuties.FetchLgs(_state);
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }
        
        
        public JsonResult PopulateFaculty()
        {
            var faculties = _generalDuties.FacultyList();
            return this.Json(faculties, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadFaculties()
        {
            var faculties = _generalDuties.FetchFaculties().ToList();
            return this.Json(faculties, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult PopulateSession()
        {
            var session = _generalDuties.FetchSessions();
            return this.Json(session, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PopulateAdmissionSession()
        {
            var session = _generalDuties.FetchAdmissionSession();
            return this.Json(session, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SemesterList(int _sessionId)
        {
            var session = _generalDuties.FetchSemester(_sessionId);
            return this.Json(session, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SemesterBySessionList(int sessionId)
        {
            var session = _generalDuties.FetchSessionSemester(sessionId);
            return this.Json(session, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PopulateStaffUser(string _departmentCode)
        {
            var ds = _staffService.FetchStaff(_departmentCode);
            if (ds == null)
                return this.Json(null, JsonRequestBehavior.AllowGet);

            List<StaffDTO> dto = new List<StaffDTO>();
            foreach(var d in ds)
            {
                var dt = new StaffDTO
                {
                    PersonId = d.PersonId + ":" + d.Name,
                    Name = d.Name
                };
                dto.Add(dt);
            }

            return this.Json(dto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PopulateRoles()
        {
            var roles = _userService.FetchRoles();
            return this.Json(roles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AllCourses()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user.IsInRole("Administrator"))
            {
                var courses = _generalDuties.PopulateCourse();
                return Json(courses, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var courses = _generalDuties.PopulateCourse(user.ProgrammeCode);
                return Json(courses, JsonRequestBehavior.AllowGet);
            }
        }

        #region ACCOUNTS OPERATIONS        
        public JsonResult FetchBankAccounts()
        {
            return Json(_accountsService.FetchBankAccounts(), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetActiveFeeOptions()
        {

            return Json(_bursaryService.GetActiveFeeOptions(), JsonRequestBehavior.AllowGet);
        }
         
        public JsonResult StudentFeeOptionsByProgType(string studentId=null)
        {
            string regNo;
            if(string.IsNullOrEmpty(studentId))
            {
                regNo = User.UserId;
            }
            else
            {
                regNo = studentId;
            }
            return Json(_bursaryService.StudentFeeOptionsByProgType(regNo), JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentOtherChargesByProgType()
        {
            return Json(_bursaryService.FetchStudentOtherChargesByProgramType(User.UserId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AccountsList()
        {
            var accts = _accountsService.AllAccounts();
            return this.Json(accts.OrderBy(a=>a.Title).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchOtherCharges()
        {
           return Json(_bursaryService.AllOtherCharges(),JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchTranscriptAmount(string country)
        {
            var res = _bursaryService.FetchTranscriptcharges(country, User.UserId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public int UserStat()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user == null)
                return 0;
            if(user.IsInRole("Student"))
            { return 2; }
            else if (user.IsSysAdmin || user.IsInRole("VC") || user.IsInRole("Bursar") || user.IsInRole("Records") || user.IsInRole("Academic Affairs")
                || user.IsInRole("Academic Affairs") || user.IsInRole("Students Accounts Officer") || user.IsInRole("IT Support"))
            { return 1; }//Enable controll
            else if(user.IsInRole("GST Exams Officer") || user.IsInRole("Educatioin Exams Officer") || user.IsInRole("Degree Directorate Exams Officer"))
            {
                return 3;
            }
            else { return 4; }
            
        }

       

        #region Programmes
        public JsonResult ActiveCoursesByDepartment()
        {

            var user = (CustomPrincipal)Session["LoggedUser"];
            return Json(_generalDuties.PopulateActiveCourses(user.ProgrammeCode), JsonRequestBehavior.AllowGet);

        }

        public JsonResult CoursesForScoresEntry(string programmeCode)
        {
            List<CourseDTO> courses = null;//You must be an exams officer to access this
            var user = (CustomPrincipal)Session["LoggedUser"];

            if (user.Roles.Contains("Exam Officer"))
            {
                var oficer = _generalDuties.FetchActiveExamOfficer(User.UserId);
                string[] courseCats=null;
                if(!string.IsNullOrEmpty(oficer.CourseCategory))
                {
                    courseCats = oficer.CourseCategory.Split(',');
                }
                if (courseCats==null)
                {
                    courses = _generalDuties.PopulateCourse(programmeCode);
                }
                else
                {
                    courses = _generalDuties.PopulateCourseByCategory(programmeCode, courseCats);
                }
                
            }
            else if (user.IsSysAdmin)
            {
                courses = _generalDuties.PopulateCourse(programmeCode);
            }

            return this.Json(courses, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CoursesByProgramme(string programmeCode,int? UserId)
        {
            string progCode;
            List<CourseDTO> courses;
            if (programmeCode == "undefined" || string.IsNullOrEmpty(programmeCode))
            {
                progCode = ((CustomPrincipal)Session["LoggedUser"]).ProgrammeCode;
            }
            else { progCode = programmeCode; }

            
            if(UserId==3)
            {
               string role= ((CustomPrincipal)Session["LoggedUser"]).Roles[0];
                courses = _generalDuties.PopulateCourseByCategory(progCode, role);
            }
            else
            {
                courses = _generalDuties.PopulateCourse(progCode);
            }
             
            return Json(courses, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ProgrammesByType(string programType)
        {

            List<ProgrammeDTO> progs;
            var user = (CustomPrincipal)Session["loggedUser"];
            if (user == null || user.IsSysAdmin)
            {
                progs = _generalDuties.ProgrammesByType(programType);
            }
            else
            {
                progs = _generalDuties.ProgrammesByTypeDept(programType, user.DepartmentCode);
            }
            return Json(progs, JsonRequestBehavior.AllowGet);
             
        }
         
        public JsonResult ProgrammeTypes()
        {
            return Json(_generalDuties.GetProgrammeTypes(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProgrammeTypeMax(string progType)
        {
            return Json(_generalDuties.GetProgrammeTypes().Where(a=>a.Type==progType).FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PopulateProgramme(string _departmentCode)
        {
            if (string.IsNullOrEmpty(_departmentCode))
            {
                _departmentCode = "default";
            }
            var depts = _generalDuties.FetchProgrammes(_departmentCode)
                        .OrderBy(a => a.Title).ToList();
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PopulateProgrammeByDeptType(string _departmentCode,string programmeType)
        {
            string pt;
            if(programmeType=="undefined"||string.IsNullOrEmpty(programmeType))
            {
                pt = "default";
            }
            else
            {
                pt = programmeType;
            }
            var depts = _generalDuties.ProgrammesByTypeDept(pt,_departmentCode)
                        .OrderBy(a => a.Title).ToList();
            return this.Json(depts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PopulateDeptProgrammes(string deptCode=null)
        {

            string de;
            if (deptCode == "undefined" || string.IsNullOrEmpty(deptCode))
            {
                var us = (CustomPrincipal)Session["LoggedUser"];
                de = us.DepartmentCode;
            }
            else
            {
                de = deptCode;
            }

            var depts = _generalDuties.FetchProgrammes(de)
                        .OrderBy(a => a.Title).ToList();
            return this.Json(depts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ScoresEntryProgrammes(string action)
        {

            List<ProgrammeDTO> progs = null;//You must be an exams officer to access this
            var user = (CustomPrincipal)Session["LoggedUser"];
                          
                if (user.Roles.Contains("Exam Officer"))
                {
                    var oficer = _generalDuties.FetchActiveExamOfficer(User.UserId);
                    if(oficer.Roles=="Read Only" && action=="Write")
                    {
                         return this.Json(progs, JsonRequestBehavior.AllowGet);
                    }
                    if (oficer.ProgrammeType == null && oficer.DepartmentCode != null)//Departmental exams officer
                    {
                        progs = _generalDuties.FetchProgrammes(oficer.DepartmentCode);
                    }
                    else if (oficer.ProgrammeType != null && oficer.DepartmentCode != null)
                    {
                        progs = _generalDuties.ProgrammesByTypeDept(oficer.ProgrammeType, oficer.DepartmentCode);
                    }
                    else if (oficer.ProgrammeType != null && oficer.DepartmentCode == null)
                    {
                        progs = _generalDuties.ProgrammesByType(oficer.ProgrammeType);
                    }
                }
                else if(user.IsSysAdmin || user.Roles.Contains("VC"))
                {                 
                    progs = _generalDuties.FetchProgrammes();
                }            
            return this.Json(progs, JsonRequestBehavior.AllowGet);

        }
        public JsonResult LoadProgrammes()
        {
            
            var progs = _generalDuties.FetchProgrammes();
            return this.Json(progs, JsonRequestBehavior.AllowGet);
        }
        #endregion

         
        public JsonResult GenerateYrs()
        {
            int cury = DateTime.Now.Year;
            DateTime curt = DateTime.Now;
            int startYear = DateTime.Now.AddYears(-11).Year;
            int baseYear = 1960;
            List<int> yrs = new List<int>();
            int len = startYear - baseYear;
            yrs.Add(baseYear);
            for(int i=1;i<=(startYear-baseYear);i++)
            {
                yrs.Add(baseYear+i);
               
            }
            return Json(yrs, JsonRequestBehavior.AllowGet);
        }

        #region GRADINGS
        public JsonResult GetGradesByProgrammeType(string progType)
        {
            return Json(_academAffairs.GradesByProgrammeType(progType), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region uSER DATA
        public byte MaxAddmissionstage()
        {
            var stu = _studentService.FetchStudent(User.UserId);

            var userData = _generalDuties.GetProgrammeTypes().Where(a => a.Type == stu.ProgrammeType).FirstOrDefault();
            return userData.AdmissionPause;
        }
        public int GetMaxCreditHours(string studentId)
        {
            if(string.IsNullOrEmpty(studentId)||studentId=="undefined")
            {
                return _generalDuties.GetStudentProgrameType(User.UserId).MaxCreditUnit;
            }
            else
            {
                return _generalDuties.GetStudentProgrameType(studentId).MaxCreditUnit;
            }
            
        }
        
        #endregion

        #region CourseCategory
        public JsonResult CourseCategories(string progType,string progCode)
        {
            return Json(_academAffairs.AllCourseCategories(progType,progCode), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OLEVEL SUBJECTS
        public JsonResult OlevelSubjects()
        {
            return Json(_generalDuties.AllSubjects(), JsonRequestBehavior.AllowGet);
        }
        [KSWebAuthorisation]
        public string SaveSubject(string title)
        {
            return _generalDuties.AddSubject(title);
        }
        #endregion
    }
}