using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.Services.Contracts;
using Eduplus.Web.SMC.PDFGenerations;
using Eduplus.Web.SMC.PDFGenerations.EventHandlers;
using Eduplus.Web.SMC.ViewModels;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using KS.Web.Security;
using Rotativa;
using Rotativa.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eduplus.Web.SMC.Controllers
{
    [KS.Web.Security.KSWebAuthorisation]
    public class ResultController : BaseController
    {
        private readonly IAcademicProfileService _academicService;
        private readonly IGeneralDutiesService _generalServices;
        private readonly IAcademicAffairsService _acadaAffairs;
       
        public ResultController(IAcademicProfileService academicService,IGeneralDutiesService generalServices,
            IAcademicAffairsService acadafairs)
        {
            _academicService = academicService;
            _generalServices = generalServices;
            _acadaAffairs = acadafairs;
            
        }
        public ActionResult ScoresEntry()
        {
            return View();
        }
        
        public JsonResult FetchStudentsForScoresEntry(string courseId, int semesterId,int flag)
        {
            byte result;
            //1=Scores already entered
            //2=Fresh course to enter
            //0=No student registered for that course
            var scores = _academicService.FetchCoursesForScoreEntry(semesterId, courseId,flag, out result);
            if (scores == null || scores.Count == 0)
                return Json(result, JsonRequestBehavior.AllowGet);
            else
                return Json(scores, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SupplementaryScoresEntry()
        {
            return View();
        }
       

        public ActionResult EditScores()
        {
            return View();
        }

        public ActionResult BacklogScoresEntry()
        {
            return View();
        }
        public JsonResult FetchStudentsForBacklogScoresEntry(string programmeCode,int sessionId,string courseId, int semesterId,
            string yearAdmitted, int level)
        {
            byte result;
            //1=Scores already entered
            //2=Fresh course to enter
            //0=No student registered for that course
            string progCode;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (programmeCode == "undefined" || string.IsNullOrEmpty(programmeCode))
            { progCode = user.ProgrammeCode; }
            else { progCode = programmeCode; }
            var scores = _academicService.FetchBackLogScoresEntry(sessionId, semesterId,courseId,yearAdmitted, progCode,  level, out result);
            if (result== 0)
            { return Json(result, JsonRequestBehavior.AllowGet); }
            else
            { return Json(scores, JsonRequestBehavior.AllowGet); }
        }

        
        public string SubmitScore(ScoresEntryDTO student)
        {

            return _academicService.SubmitScores(student, User.UserId);

        }


        public string SubmitEdit(ScoresEntryDTO data)
        {

            return _academicService.SubmitScoresEdit(data, User.UserId);
            
        }

        public string SubmitBacklogScores(ResultSubmissionViewModel data)
        {
            List<ScoresEntryDTO> dto = new List<ScoresEntryDTO>();
            foreach (var s in data.students)
            {
                if (s.CA1 > 0 || s.Exam > 0||s.CA2>0)
                {
                    dto.Add(s);
                }
            }

           string msg= _academicService.SubmitBacklogScores(dto, User.UserId,data.IsCarryOver);
            return msg;
        }
        public string SubmitBacklogScoresSingle(ScoresEntryDTO data)
        {
            

            string msg = _academicService.SubmitBacklogScores(data, User.UserId);
            return msg;
        }

        #region RESULT APPROVAL
        public ActionResult ApproveResult()
        {
            return View();
        }
        public string SubmitResultApproval(object[] data1)
        {
            string _prog;
            int semesterId = (int)data1[0];
            string prog = (string)data1[1];
            string appType = (string)data1[2];
            int lvl = (int)data1[3];
            if(prog=="undefined")
            {
                var user = (CustomPrincipal)Session["loggedUser"];
                _prog = user.ProgrammeCode;
            }
            else
            {
                _prog = prog;
            }
            return _academicService.ApproveResults(semesterId, lvl, prog, User.UserId);
        }
        #endregion

        #region RESULT REPORTS

        public ActionResult GenerateStudentResults()
        {
            //_acadaAffairs.EliminateIncompleteResult();
            return View();
        }
        public ActionResult StudentBroadSheet(int sessionId, int semesterId, int level, string progCode,string rptType)
        {
            string _progCode;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (progCode=="undefined" || string.IsNullOrEmpty(progCode))
            {
                _progCode = user.ProgrammeCode;
            }
            else { _progCode = progCode; }
            
            var broadSheet = _academicService.FetchBroadSheet(_progCode, sessionId, semesterId, level);
            if(broadSheet==null)
            {
                return View("GenerateStudentResults");
            }
            var data = _generalServices.GetUserData();
            if (rptType == "PDF")
            {
                var pdf = StudentResultPdfGenerator.GenerateBroadsheetPdf(broadSheet, data, "Students' Semester Result Broadsheet");
                return File(pdf, "application/pdf");
            }
            else
            {
                GenerateBroadsheetExcel(broadSheet, data, "Students' Semester Result Broadsheet");
                return View();
                    
            }
            

        }

        

        public ActionResult GenerateStudentResult()
        {

            return View();
        }

        public ActionResult StudentSemesterResult(int semesterId, string matricNumber)
        {
            var user = (CustomPrincipal)Session["LoggedUser"];
            string progCode = "";
            if (!user.IsSysAdmin)
                progCode = user.ProgrammeCode;
                
            var broadSheet = _academicService.FetchSingleSemesterResultForStudent(matricNumber,semesterId,progCode,1);
            
                return new ViewAsPdf(broadSheet)
                {
                    //FileName = broadSheet.Session + "_" + broadSheet.Semester + "_" + "Broad_Sheet_for_" + broadSheet.Level + "_" + broadSheet.Department + ".pdf",
                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,

                    CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                        "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                    PageMargins = { Left = 1, Right = 1, Top = 2, Bottom = 12 }
                };

        
        }
        
        public ActionResult SemesterResultForFile(int sessionId, int semesterId, int level, string progCode)
        {
            string _progCode = progCode;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (_progCode == "undefined" || string.IsNullOrEmpty(progCode))
            {
                _progCode = user.ProgrammeCode;
            }
            var results = _academicService.SemesterResultForDepartment(semesterId,level,_progCode);

            return new ViewAsPdf(results)
            {
                //FileName = broadSheet.Session + "_" + broadSheet.Semester + "_" + "Broad_Sheet_for_" + broadSheet.Level + "_" + broadSheet.Department + ".pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches ="--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 1, Right = 1, Top = 2, Bottom = 12 }
            };


        }
        public ActionResult SemesterCourseFormForFile(int sessionId, int semesterId, int level, string progCode)
        {
            string _progCode = progCode;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (_progCode == "undefined" || string.IsNullOrEmpty(progCode))
            {
                _progCode = user.ProgrammeCode;
            }
            var results = _academicService.SemesterResultForDepartment(semesterId, level, _progCode);

            return new ViewAsPdf(results)
            {
                //FileName = broadSheet.Session + "_" + broadSheet.Semester + "_" + "Broad_Sheet_for_" + broadSheet.Level + "_" + broadSheet.Department + ".pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,

                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 1, Right = 1, Top = 2, Bottom = 12 }
            };


        }
        public ActionResult GenerateTranscript()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GenerateTranscript(string regNo)
        {
            var transcript = _academicService.FetchStudentTranscript(regNo);
            if (transcript == null)
                return View();
            return new ViewAsPdf("StudentTranscript", transcript)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 2 }
            };
             
        }
         
        [HttpPost]
        public ActionResult Transcript(string transcriptNo)
        {
            var transcript = _academicService.FetchStudentAcademicProfile(transcriptNo,true);
            if (transcript == null)
                return View();
            UserData dat=_generalServices.GetUserData();

            var regHead = dat.Regbanner;
            var wata = dat.Logo;
            var regSign = dat.RegSign;
            var trans = StudentResultPdfGenerator.CreateTransacriptPdf(transcript,transcriptNo,dat);
            return File(trans, "application/pdf");

             
        }
       
        [HttpGet]
        public ActionResult ViewStudentProfile(string regNo)
        {
            var transcript = _academicService.FetchStudentAcademicProfile(regNo,false);
            
            return new ViewAsPdf(transcript)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 2 }
            };
        }
        
       
        #endregion
        #region Academic Profiles REPORTS
        public ActionResult ProbationList(int sessionId, string progCode)
        {
            string pr;
            if(progCode=="undefined" || string.IsNullOrEmpty(progCode))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                pr = user.ProgrammeCode;
            }
            else { pr = progCode; }
            var res = _academicService.GenerateStudentsOnProbation(sessionId, pr,2);
            
            return new ViewAsPdf(res)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
            };
            
        }

        public ActionResult GeneralStudentsPerformance(int sessionId, string progCode)
        {
            string pr;
            if (progCode == "undefined" || string.IsNullOrEmpty(progCode))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                pr = user.ProgrammeCode;
            }
            else { pr = progCode; }
            var res = _academicService.GenerateStudentsOnProbation(sessionId, pr, 1);

            return new ViewAsPdf(res)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
            };

        }

        public ActionResult GenerateStudentsDueForGraduation()
        {
            return View();
        }

        public JsonResult StudentsDueForGraduation(string sessionId,string programmeCode)
        {
            if(programmeCode=="undefined"||string.IsNullOrEmpty(programmeCode))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                programmeCode = user.ProgrammeCode;
            }
            var students = _academicService.StudentsDueForGraduation(programmeCode);
            return Json(students, JsonRequestBehavior.AllowGet);
        }

        public string GraduateStudent(StudentsForGraduationViewModel data)
        {
            List<ProbationDetailsDTO> details = new List<ProbationDetailsDTO>();
            foreach(var s in data.students)
            {
                var st = new ProbationDetailsDTO
                {
                    CGPA = s.CGPA,
                    StudentId = s.StudentId,
                    Graduate = s.Graduate,
                    MatricNumber = s.MatricNumber,
                    Qualification = s.Qualification
                    
                };
                details.Add(st);
            }
            return _academicService.GraduateAcademicStudent(details, data.sessionId,data.batch, User.UserId);
        }

        public ActionResult GenerateGraduants()
        {
            return View();
        }

        public ActionResult Graduants(string session,string programmeCode)
        {
            if(programmeCode=="undefined" || string.IsNullOrEmpty(programmeCode))
            {
                var user = (CustomPrincipal)Session["LoggedUser"];
                programmeCode = user.ProgrammeCode;
            }
            var model = _academicService.GraduatedStudent(session, programmeCode);
            return new ViewAsPdf(model)
            {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                CustomSwitches = "--footer-right \"Date: [date] [time]\" " +
                    "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 2, Right = 2, Top = 2, Bottom = 10 }
            };
        }

        public ActionResult GenerateGraduantBroadsheets()
        {
            return View();
        }

        public ActionResult GraduatedStudentBroadSheet(int sessionId, int semesterId, int level, string progCode, 
            string gradYr,string admitYr, string batch)
        {
            string _progCode = progCode;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (_progCode == "undefined" || string.IsNullOrEmpty(progCode))
            {
                _progCode = user.ProgrammeCode;
            }

            var broadSheet = _academicService.FetchGraduantsBroadSheet(_progCode, sessionId, semesterId, level,admitYr,gradYr,batch);

            if (broadSheet == null)
                return View();
            var data = _generalServices.GetUserData();
            var pdf = StudentResultPdfGenerator.GenerateBroadsheetPdf(broadSheet, data, "Students' Semester Broadsheet");

            return File(pdf, "Application/pdf");
            
        }
        public ActionResult GenerateGraduantSummary()
        {
            return View();
        }
        public ActionResult GraduantSummary(string sessionGrad,string programmeCode,string batch)
        {
            string _progCode = programmeCode;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (_progCode == "undefined" || string.IsNullOrEmpty(programmeCode))
            {
                _progCode = user.ProgrammeCode;
            }

            var broadSheet = _academicService.FetchGraduantsSummary(_progCode, sessionGrad, batch);
            
            var gradClass = _acadaAffairs.GraduatingClassByProgrammeType(broadSheet.ProgrammeType);
            if (broadSheet == null)
                return View();
            var data = _generalServices.GetUserData();
            var pdf = StudentResultPdfGenerator.GenerateFinalResultsheetPdf(broadSheet, data, "Final Result",gradClass);

            return File(pdf, "Application/pdf");
        }
        #endregion
    }
}