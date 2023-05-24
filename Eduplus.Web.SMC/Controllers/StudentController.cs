using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.CoreModule;
using Eduplus.eb.SMC.ViewModels;
using Eduplus.Services.Contracts;
using Eduplus.Web.SMC.PDFGenerations;
using Eduplus.Web.SMC.ViewModels;
using KS.UI.ViewModel;
using KS.Web.Security;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Rotativa;
using Rotativa.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Eduplus.Web.SMC.Controllers
{
    [KS.Web.Security.KSWebAuthorisation]
    public class StudentController : BaseController
    {
        // GET: Student
        private readonly IAcademicProfileService _academicService;
        
        private readonly IStudentService _studentService;
        private readonly IBursaryService _bursaryService;
        private readonly IGeneralDutiesService _generalDutiesService;
        private readonly IStudentsAccountsService _studentsAccount;
        public StudentController(IAcademicProfileService academicService,IStudentService studentService,IBursaryService bursaryService,
            IGeneralDutiesService generalDuties, IStudentsAccountsService studentAccounts)
        {
            _academicService = academicService;
            _studentService = studentService;
            _bursaryService = bursaryService;
            _generalDutiesService = generalDuties;
            _studentsAccount = studentAccounts;
            
        }
       
        public ActionResult Index()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Accounts");
                
            }

            ViewBag.photo = user.Photo;
            var completion = _studentService.FetchStudent(user.UserId);
            if (completion.AddmissionCompleteStage == 2)
                return RedirectToAction("Addmissions_Step1", "Admission_Center");
            if(user.Photo==null)
            { return RedirectToAction("UploadPassport", "Admission_Center");}
            return View();
        }


        #region COURSE REGISTRATION PROCESSES

        //=================================================In house course registrations===========================
        public ViewResult StudentCourseRegistration()
        {
            return View();
        }

        
        //=================================================End in house course rEgsitrations=======================
        public ActionResult RegisterCourse()
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            if(user.Photo==null)
            {
                return RedirectToAction("UploadPassport", "Admission_Center");
            }
            return View();
        }

        public JsonResult CoursesToRegister(int level, int? semesterId, string studentId)
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            string programme;
            string stId;
            if (!string.IsNullOrEmpty(studentId))
            {

                 
                stId = studentId;
            }
            else
            {
                
                stId = user.UserId;
            }

            var expectedRegCourses = _academicService.FetchCoursesToRegister(stId, level, semesterId);

            return this.Json(expectedRegCourses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult  FetchStudentsForRegistration(string programmeCode)
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            string prog;
            if(!string.IsNullOrEmpty(programmeCode)||programmeCode!="undefined")
            {
                prog = programmeCode;
            }
            else
            {
                prog = user.ProgrammeCode;
            }
            return Json(_studentService.FetchStudents(prog), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult CheckIfQualifiedToRegister(int semesterId,string studentId)
        {
            int sem = 0;
            string _studentId;
            var currentSemester = _generalDutiesService.FetchCurrentSemester();
            if(semesterId==0&&string.IsNullOrEmpty(studentId))
            {
                sem = currentSemester.SemesterId;
                _studentId = User.UserId;
            }
            else
            {
                sem = semesterId;
                _studentId = studentId;
            }
            outPutMsg op = new outPutMsg();
            //check If cleared to register
            //if cleared 0, if not cleared 1, if already registered 2
           var cleared= _academicService.CheckIfStudentIsClearedToRegisterForCourse(_studentId,sem);
            switch(cleared)
            {
                case 0:
                    op.value = 0;
                    op.message = "good";
                    break;
                case 1:
                    op.value = 1;
                    op.message = "Please complete stipulated fees. Operation terminated";
                    break;
                case 2:
                    op.value = 2;
                    op.message = "Registration already done for current semester.OPeration terminated";
                    break;
                case 3:
                    var user = (CustomPrincipal)Session["LoggedUser"];
                    if (!user.IsInRole("Student"))
                    {
                        op.value = 0;
                        op.message = "good";
                    }
                    else
                    {
                        op.value = 3;
                        op.message = "Portal closed for current semester registrations";
                    }
                    break;
                case 4:
                    op.value = 4;
                    op.message = "You have entered Late Registration period." + "\r\n" +
                        "Please pay Late Registration Fees before registering for course";               
                    break;
                case 5:
                    op.value = 5;
                    op.message = "Fatal Error: Something went wrong while processing request. please try again later";
                    break;
            }
            
            return Json(op, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult AdditionalCourses(int lvl,string progCode,string studentId,int? semesterId)
        {
            string _progCode;
            string _studentId;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if(progCode=="undefined"||string.IsNullOrEmpty(progCode)||semesterId==0)
            {
                _progCode = user.ProgrammeCode;
                _studentId = user.UserId;
                return Json(_academicService.AdditionalCoursesToRegister(_progCode, lvl, _studentId, semesterId), JsonRequestBehavior.AllowGet);
            }
            else
            {
                _progCode = progCode;
                _studentId = studentId;
                return Json(_academicService.AdditionalCoursesToRegister(_progCode, lvl, _studentId, semesterId), JsonRequestBehavior.AllowGet);
            }
            
            
        }
        public string SubmitRegistration(CourseRegistrationVM data)
        {
            

            return  _academicService.SubmitCourseRegistration(data.RegCourses, data.RemovedCourses, User.UserId);
        }

        public ActionResult MySemesterRegistration(string studentId, int semesterId)
        {
            string stdId;
            if(string.IsNullOrEmpty(studentId) || studentId == "undefined")
            {
                stdId = User.UserId;
            }else
            {
                stdId = studentId;
            }
            var registration = _academicService.StudentSemesterCourseRegistration(semesterId, stdId);
            if (registration == null)
                return View();
            else
            {
                var userdata = _generalDutiesService.GetUserData();
                var pdf = StudentResultPdfGenerator.StudentCourseForm(registration, userdata);
                return File(pdf,"application/pdf");
            }
        }

        public ActionResult MyCourseRegistrations()
        {

            var test = User.UserId;
            var model = _academicService.FetchStudentRegistrations(User.UserId);
            return View(model);
        }
        public ActionResult MySemesterResult()
        {

            return View();
        }

        public ActionResult MyResult(int semesterId)
        {

            var broadSheet = _academicService.FetchSingleSemesterResultForStudent(User.UserId, semesterId,User.ProgrammeCode,2);

            return new ViewAsPdf(broadSheet)
            {
                //FileName = broadSheet.Session + "_" + broadSheet.Semester + "_" + "Broad_Sheet_for_" + broadSheet.Level + "_" + broadSheet.Department + ".pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
            };

        }
        #endregion
        #region STUDENTS ADMINISTRATION
        public ActionResult ViewStudents()
        {
            return View();
        }
        public JsonResult FetchStudents(string _programmeCode, string _sessionAdmitted)
        {

            string progCode;
            if(_programmeCode=="undefined"||string.IsNullOrEmpty(_programmeCode))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                
                progCode = user.ProgrammeCode;
                
            }
            else
            {
                
                progCode = _programmeCode;
                
            }
           return Json(_studentService.FetchStudents(progCode, _sessionAdmitted), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FindStudents(string search)
        {
             
            return Json(_studentService.SearchStudents(search), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintStudent(string deptCode, string progCode,string ses)
        {
            string _progCode;
            if (string.IsNullOrEmpty(deptCode) && string.IsNullOrEmpty(progCode))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                 
                _progCode = user.ProgrammeCode;
                
                
            }
            else
            {
               
                _progCode = progCode;
                
            }
            var students = _studentService.FetchStudentsInProgram(_progCode, ses);
            if(students==null)
            {
                return new ViewAsPdf(new StudentInProgDTO());
            }
            else
            {
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

        public ActionResult MatricRegisterGeneration()
        {
            return View();
        }
        public ActionResult GenerateMatricRegister(string yrAdmitted,string progType,string deptCode)
        {
            var register = _studentService.MatricnRegister(yrAdmitted, deptCode, progType);
            var userdata = _generalDutiesService.GetUserData();
            if(register!=null)
            {
                var doc = StudentPDFReports.GenerateMatricRegister(register, userdata);
                return File(doc, "Application/pdf");
            }
            return View();
        }

        public ActionResult GenerateMatricRegisterExcel(string yrAdmitted, string progType, string deptCode)
        {
            var register = _studentService.MatricnRegister(yrAdmitted, deptCode, progType);

            if (register != null)
            {
                ExcelPackage package = new ExcelPackage();
                var wrkSheet = package.Workbook.Worksheets.Add(register.Session+"_"+register.ProgrammeType+"_"+register.Department);
                var data = ConvertToDataTable(register.Headings);
                int tCol = data.Columns.Count - 1;
                int tRow = data.Rows.Count;
                int stRw = 1;
                
                foreach(var h in register.Headings)
                {

                    using (var cells = wrkSheet.Cells[stRw,1,stRw,13])
                    {
                        cells.Merge = true;
                        cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cells.Value = h.Heading;
                    }

                    
                    stRw++;
                    wrkSheet.Cells[stRw, 1].Value = "S/N";
                    wrkSheet.Cells[stRw, 2].Value = "StudentId";
                    wrkSheet.Cells[stRw, 3].Value = "Matric No";
                    wrkSheet.Cells[stRw, 4].Value = "Entry Mode";
                    wrkSheet.Cells[stRw, 5].Value = "Student";
                    wrkSheet.Cells[stRw, 6].Value = "Gender";
                    wrkSheet.Cells[stRw, 7].Value = "Birth Date";
                    
                    wrkSheet.Cells[stRw, 8].Value = "Jamb No";
                    wrkSheet.Cells[stRw, 9].Value = "Jamb Score";
                    wrkSheet.Cells[stRw, 10].Value = "Phone";
                    wrkSheet.Cells[stRw, 11].Value = "Email";
                    wrkSheet.Cells[stRw, 12].Value = "Address";
                    wrkSheet.Cells[stRw, 13].Value = "State";
                    wrkSheet.Cells[stRw, 14].Value = "LGA";
                    wrkSheet.Cells[stRw, 15].Value = "Signature";
                    stRw++;
                    int count = 1;
                    foreach (var d in h.Details)
                    {
                        
                        wrkSheet.Cells[stRw, 1].Value = count;
                        wrkSheet.Cells[stRw, 2].Value = d.StudentId;
                        wrkSheet.Cells[stRw, 3].Value = d.MatricNo;
                        wrkSheet.Cells[stRw, 4].Value = d.EntryMode;
                        wrkSheet.Cells[stRw, 5].Value = d.Name;
                        wrkSheet.Cells[stRw, 6].Value = d.Sex;
                        wrkSheet.Cells[stRw, 7].Value = d.BirthDate;
                        wrkSheet.Cells[stRw, 8].Value = d.JambRegNo;
                        wrkSheet.Cells[stRw, 9].Value = d.JambScore;
                        wrkSheet.Cells[stRw, 10].Value = d.Phone;
                        wrkSheet.Cells[stRw, 11].Value = d.Email;
                        wrkSheet.Cells[stRw, 12].Value = d.Address;
                        wrkSheet.Cells[stRw, 13].Value = d.State;
                        wrkSheet.Cells[stRw, 14].Value = d.Lg;
                        wrkSheet.Cells[stRw, 15].Value = "";
                        count++;
                        stRw++;
                    }
                }
                
                
                wrkSheet.View.FreezePanes(1,1);
                //wrkSheet.Cells[1, 1, 1, 12];
                wrkSheet.Cells[1, 1, 1, 15].Style.Font.Bold = true;

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    package.SaveAs(memoryStream);

                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return View();
        }
        public ActionResult StudentsAsExcel(string progCode, string ses)
        {
            string _progCode;
            if (string.IsNullOrEmpty(progCode))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                 
                _progCode = user.ProgrammeCode;


            }
            else
            {
                
                _progCode = progCode;

            }
            var students = _studentService.FetchStudentsInProgram(_progCode, ses);
            if (students != null)
            {
                List<ExcelStudentVM> excel = new List<ExcelStudentVM>();
                foreach (var s in students.Students)
                {
                    var ex = new ExcelStudentVM();

                    ex.StudentId = s.StudentId;
                    ex.FullName = s.FullName;
                    if (s.CurrentLevel != null) {
                        ex.CurrentLvel = s.CurrentLevel.Value;
                    }

                    ex.MatricnNumber = s.MatricNumber;
                    ex.Prgramme = s.Programme;
                    excel.Add(ex);
                }
                Export2Excel<ExcelStudentVM>(excel.OrderBy(s => s.MatricnNumber).ToList(), students.Set + "_" + students.Programme + "(" + students.ProgType + ")");
            }
            return View();
        }
        //================================Fetch student for editing process===================

        public JsonResult FetchStudentForEditing1(string studentId)
        {
            string _studentId="";


            if ((string.IsNullOrEmpty(studentId) || studentId == "undefined"))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                _studentId = user.UserId;

            }
            else
            {
                _studentId = studentId;
            }
            var student = _studentService.FetchStudent(_studentId);
            return Json(student, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditProfile(string studentId)
        {
            return View();
        }
        public string SubmitStudentEdit(StudentDTO viewModel)
        {
            string local = "";
            string flag;
            
            var st= _studentService.Step1Submission(viewModel, User.UserId,out flag);
            if (st == "00"&&string.IsNullOrEmpty(viewModel.StudentId) &&viewModel.Status=="Active")
            {
              return   _studentsAccount.DebitNewStudent(st, User.UserId);
            }
            switch(flag)
            {
                case "01":
                    local = "Email already in use by someone else";
                    break;
                case "02":
                    local= "Jamb Registration Number already in use by someone else";
                    break;
            }
            return local;
        }
        #endregion
        public ActionResult StudentBioProfile(string studentId)
        {
            string st;
            if(!string.IsNullOrEmpty(studentId))
            { st = studentId; }
            else { st = User.UserId; }
            var student = _studentService.FetchStudent(st);
            UserData data = _generalDutiesService.GetUserData();
             
            var pdf = StudentPDFReports.CreateStudentProfilePdf(student,data);  
            return File(pdf, "application/pdf");

        }

        public ActionResult ChangeStudentDept()
        {
            return View();
        }
        public string SubmitChangeOfDepartment(object[] data1)
        {
            string studentId = (string)data1[0];
            string progCode= (string)data1[1];
            string reson= (string)data1[2];
            return _studentService.UpdateStudentProgramme(studentId, progCode, reson, User.UserId);
        }
        #region STUDENTS DOCUMENT OPERATIONS


        public FileResult ViewDocument(string studentId=null)
        {
            if(string.IsNullOrEmpty(studentId))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                studentId = user.UserId;
            }
            var doc = _studentService.ViewDocument(studentId);
            
            string path = doc.Path;
            return File(path, "application/pdf");

        }
        #endregion


        public ActionResult MyOutstandings()
        {
            
            return View(_academicService.FetchStudentOutstandings(User.UserId));
        }
        


    }
}