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
using OfficeOpenXml;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Windows.Documents;

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

        public ActionResult ScoresEntryTemplate(string courseId,int semesterId)
        {
            byte result;
             
            var scores = _academicService.FetchCoursesForScoreEntry(semesterId, courseId, 1, out result);
            if (scores.Count > 0)             
             
            {
                var tit = scores.First();
                List<ScoresEntryVM> vm = new List<ScoresEntryVM>();
                foreach(var c in scores)
                {
                    vm.Add(new ScoresEntryVM
                    {
                        CourseCode = c.CourseCode,
                        Programme = c.Programme,
                        RegistrationId = c.RegistrationId,
                        StudentId = c.StudentId,
                        CourseId = c.CourseId,
                        RegNo = c.RegNo,
                        
                    });
                }
                
                
                ExcelPackage package = new ExcelPackage();
                var wrkSheet = package.Workbook.Worksheets.Add(tit.SemesterId.ToString() + "_" + tit.SessionId.ToString() + tit.Programme);
                
                
                wrkSheet.Cells[1, 1].Value = "StudentId";
                wrkSheet.Cells[1, 2].Value = "RegistrationId";
                wrkSheet.Cells[1, 3].Value = "CourseId";
                wrkSheet.Cells[1, 4].Value = "S/N";
                wrkSheet.Cells[1, 5].Value = "CourseCode";
                wrkSheet.Cells[1, 6].Value = "RegNo";
                wrkSheet.Cells[1, 7].Value = "CA1";
                wrkSheet.Cells[1, 8].Value = "CA2";
                wrkSheet.Cells[1, 9].Value = "Exam";
                wrkSheet.Cells[1, 10].Value = "Programme";

                for (int i = 0; i < scores.Count; i++)
                {
                    wrkSheet.Cells[i + 2, 1].Value = scores[i].StudentId;
                    wrkSheet.Cells[i + 2, 2].Value = scores[i].RegistrationId.ToString();
                    wrkSheet.Cells[i + 2, 3].Value = scores[i].CourseId;
                    wrkSheet.Cells[i + 2, 4].Value = i + 1;
                    wrkSheet.Cells[i + 2, 5].Value = scores[i].CourseCode;
                    wrkSheet.Cells[i + 2, 6].Value = scores[i].RegNo;
                    wrkSheet.Cells[i + 2, 10].Value = scores[i].Programme;
                }
                wrkSheet.View.FreezePanes(2, 1);
                wrkSheet.Cells[1, 1, 1, 1 + 1].Style.Font.Bold = true;
                wrkSheet.Column(1).Hidden = true;
                wrkSheet.Column(2).Hidden = true;
                wrkSheet.Column(3).Hidden = true;
                wrkSheet.Column(4).Width = 3;
                wrkSheet.Column(5).Width = 15;
                wrkSheet.Column(6).Width = 25;
                wrkSheet.Protection.IsProtected = true;

                wrkSheet.Column(7).Style.Locked = false;
                wrkSheet.Column(8).Style.Locked = false;
                wrkSheet.Column(9).Style.Locked = false;
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    package.SaveAs(memoryStream);
                     
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                return View("ScoresEntry");
            }
            else return View("ScoresEntry");
        }

        [HttpGet]
        public ActionResult UploadResultFromTemplate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadResultFromTemplate(HttpPostedFileBase result)
        {
            if(result.ContentLength>0 &&( result.ContentType== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                ||result.ContentType== "application/vnd.ms-excel"))
            {
                List<ScoresEntryDTO> dto = new List<ScoresEntryDTO>();
                using(ExcelPackage package=new ExcelPackage(result.InputStream))
                {
                    
                    ExcelWorksheet wrkSheet = package.Workbook.Worksheets[0];
                    int rows = wrkSheet.Dimension.Rows;
                    int cols = wrkSheet.Dimension.Columns;
                    for (int row=2; row<=rows; row++)
                    {

                        var score = new ScoresEntryDTO();
                        score.StudentId = wrkSheet.Cells[row, 1].Value.ToString();
                        score.RegistrationId = Convert.ToInt64(wrkSheet.Cells[row, 2].Value.ToString());
                        score.CourseId = wrkSheet.Cells[row, 3].Value.ToString();
                        if(wrkSheet.Cells[row, 7].Value!=null)
                        {
                            score.CA1= Convert.ToInt32(wrkSheet.Cells[row, 7].Value.ToString());
                        }
                        if (wrkSheet.Cells[row, 8].Value != null)
                        {
                            score.CA2 = Convert.ToInt32(wrkSheet.Cells[row, 8].Value.ToString());
                        }
                        if (wrkSheet.Cells[row, 9].Value != null)
                        {
                            score.Exam = Convert.ToInt32(wrkSheet.Cells[row, 9].Value.ToString());
                        }
                        dto.Add(score);
                    }

                    _academicService.SubmitTemplateScores(dto, User.UserId);

                }
                
            }
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
            if (scores.Count== 0)
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