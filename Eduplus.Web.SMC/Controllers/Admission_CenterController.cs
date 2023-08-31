using Eduplus.Domain.AcademicModule;
using Eduplus.Domain.CoreModule;
using Eduplus.DTO.AcademicModule;
using Eduplus.DTO.CoreModule;
using Eduplus.eb.SMC.ViewModels;
using Eduplus.Services.Contracts;
using Eduplus.Web.SMC.PDFGenerations;
using Eduplus.Web.SMC.ViewModels;
using KS.AES256Encryption;
using KS.Services.Contract;
using KS.Web.Security;
using OfficeOpenXml;
using QRCoder;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Eduplus.Web.SMC.Controllers
{
   // [KSWebAuthorisation]
    public class Admission_CenterController : BaseController
    {
        // GET: Pre_Admission_Center
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IStudentService _studentService;
        private readonly ICommunicationService _commService;
        private readonly IBursaryService _bursaryService;
        private readonly IStudentsAccountsService _studentAccount;
        private readonly IUserService _userService;

        public Admission_CenterController(IStudentService studentService, IGeneralDutiesService generalDuties, ICommunicationService commService,
            IStudentsAccountsService aService, IBursaryService bursaryService, IUserService use)
        {
            _studentService = studentService;
            _generalDuties = generalDuties;
            _commService = commService;
            _bursaryService = bursaryService;
            _studentAccount = aService;
            _userService = use;
        }

        #region ALLOWED ADMISSION FOR PROSPECTIVES
       
        public ActionResult NewStudentProfile()
        {
            return View();
        }
        public int SubmitNewStudentProfile(ProspectiveStudentDTO data)
        {
            int stat = 0;//some has taken your email
            
            stat = _studentService.CreateNewStudentProfile(data);
            if (stat == 1)//send mail
            {
                
                return stat;

            }
            else
            return stat;
        }
        
        #endregion
        #region PROSPECTIVE STUDENTS FROM SCRATCH
        [KSWebAuthorisation]
        public ActionResult PayAdmissionFee()
        {
            var adminSesion = _generalDuties.FetchAdmissionSession().FirstOrDefault();
            
            var user = (CustomPrincipal)Session["LoggedUser"];

            //Check if Invoice Already exist
            var inv = _studentAccount.CheckAdmissionFeePayment(user.UserId);
            string flag;
            switch(inv)
            {
                case 0:
                    
                    var invoice = _studentAccount.GenerateStudentPaymentInvoice(user.UserId, "Screening Fee",adminSesion.SessionId, User.UserId, out flag);
                    if(flag!="Ok")
                    {
                        ViewBag.msg = flag;
                        return View("PaymentInvoice", "Payments", new { transId = invoice.TransactionId });
                    }
                    else
                    {
                        return RedirectToAction("PaymentInvoice", "Payments", new { transId = invoice.TransactionId });
                    }                  
                case 1:
                    return RedirectToAction("MyInvoices", "Payments");
                default:
                    return RedirectToAction("Prospectives");
            }
            
        }
        [KSWebAuthorisation]
        public ActionResult PayAcceptanceFee()
        {
            var currentSesion = _generalDuties.FetchCurrentSemester();
             
            var student = _studentService.FetchStudent(User.UserId);
            var session = _generalDuties.FetchSessions().Where(s => s.Title == student.YearAdmitted).FirstOrDefault();
            //Check if Invoice Already exist
            var inv = _studentAccount.CheckAcceptanceFeePayment(User.UserId);
            var _userData = _generalDuties.GetProgrammeTypes().Where(a=>a.Type==student.ProgrammeType).SingleOrDefault();
            string flag;
            switch (inv)
            {
                case 0:
                    if(_userData.CollectAcceptanceFee==true)
                    {
                        var invoice = _studentAccount.GenerateStudentPaymentInvoice(student.StudentId, "Acceptance Fee", session.SessionId, User.UserId, out flag);
                        if (flag != "Ok")
                        {
                            ViewBag.msg = flag;
                            return View();
                        }
                        else
                        {
                            return RedirectToAction("PaymentInvoice", "Payments", new { transId = invoice.TransactionId });
                        }
                    }
                    else //AdmitStudent
                    {
                        var ck=_studentService.AddmitWithoutAcceptanceFee(User.UserId);
                        _userService.ChangeUserRole(User.UserId, User.UserId, "Prospective", "Student");
                        //Send Student Matricnumber
                        string msg="Congratulations! Your admission has been accepted into this noble institution, please go ahead and pay stipulated fees."+"\n"+
                            "Your new RegNo is= "+ck+"\n"+"Note: If your new RegNo is contained in this mail, use it as your new username, else maintain your old username in the portal"+"\n"+ " Thank you";
                       // _commService.SendMail(student.Email, msg, "Admission Acceptance");
                        return RedirectToAction("Login", "Accounts"); ;
                    }
                    
                case 1:
                    return RedirectToAction("MyInvoices", "Payments");
                default:
                    return RedirectToAction("Prospectives");
            }

        }


        #endregion

        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult AddStudent()
        {

            return View();
        }
        
        [KSWebAuthorisation]
        public string SubmitNewStudent(AddStudentViewModel data)
        {
            Student st = new Student();
            
            st.Country = data.student.Country;
            st.BDay= data.student.BDay;
            st.BMonth = data.student.BMonth;
            st.BYear = data.student.BYear;
            st.DepartmentCode = data.student.DepartmentCode;
            st.Email = data.student.Email;
            st.EntryMode = data.student.EntryMode;
            st.Firstname = data.student.Firstname;
            st.Duration = data.student.Duration;
            st.MatricNumber = data.student.MatricNumber;
            st.kinAddress = data.student.KinAddress;
            st.KinMail = data.student.KinMail;
            st.Phone = data.student.Phone;
            st.ProgrammeCode = data.student.ProgrammeCode;
            st.ProgrammeType = data.student.ProgrammeType;
            st.Referee = data.student.Referee;
            st.RefereeAddress = data.student.RefereeAddress;
            st.RefereeMail = data.student.RefereeMail;
            st.RefereePhone = data.student.RefereePhone;
            st.Relationship = data.student.Relationship;
            st.ResidentialAddress = data.student.ResidentialAddress;
            st.Sex = data.student.Sex;
            st.State = data.student.State;
            st.Surname = data.student.Surname;
            st.Title = data.student.Title;
            st.Lg = data.student.Lg;
            st.MaritalStatus = data.student.MaritalStatus;
            st.Duration = data.student.Duration;
            st.CurrentLevel = data.student.CurrentLevel;
             
            var msg = _studentService.NewStudent(st, data.sessionId, User.UserId);
            
            return msg;

        }
        #region APPLICANTS OPERATIONS
        [KSWebAuthorisation]
        public ActionResult Prospectives()
        {

            //Check Current admission step
            var user = (CustomPrincipal)Session["LoggedUser"];
            var student = _studentService.FetchStudent(User.UserId);
            var userData = _generalDuties.GetProgrammeTypes().Where(a=>a.Type==student.ProgrammeType).FirstOrDefault();
             
            if(student.AddmissionCompleteStage==0)
            {
                ViewBag.stat = 0;
                return View();
            }
            if (student.AddmissionCompleteStage == 1)
            {
                return RedirectToAction("Addmissions_Step1");
                
            }
            if (student.AddmissionCompleteStage == 5 && string.IsNullOrEmpty(student.StudyMode))
            {
                return RedirectToAction("Addmissions_Step1");

            }
            if (student.AddmissionCompleteStage == 2 && userData.AdmissionPause>2)
            {
                return RedirectToAction("UploadPassport");

            }
            if (student.AddmissionCompleteStage == 3 && userData.AdmissionPause > 3)
            {
                return RedirectToAction("OlevelResults");

            }
            if (student.AddmissionCompleteStage == 4 && userData.AdmissionPause > 4)
            {
                return RedirectToAction("JambScore");

            }
            if (student.AddmissionCompleteStage == 5 && userData.AdmissionPause > 5)
            {
                return RedirectToAction("Addmissions_Step5");

            }
            if (student.AdmissionStatus == "Admitted")
            {
                ViewBag.stat = 3;
                return View();

            }
            else
            {
                ViewBag.stat = 4;
                return View();
            }
             
        }
        [KSWebAuthorisation]
        public JsonResult GetStudentStep1(string studentId)
        {
            string id;
            if (string.IsNullOrEmpty(studentId))
            {
                id = User.UserId;
            }
            else
            {
                id = studentId;
            }
                var student=_studentService.FetchStudent(id);
                 
                return Json(student,JsonRequestBehavior.AllowGet);
   
        }
        [KSWebAuthorisation]
        public ActionResult Addmissions_Step1()
        {
            var studentCode = (CustomPrincipal)Session["LoggedUser"];
            var cod = _studentService.FetchStudent(studentCode.UserId);

            if (cod.AddmissionCompleteStage == 0)
            {
                return RedirectToAction("Prospectives");
            }
            else if (cod.Status=="Active")
            {
                return View();
            }
            
            else
                return View();
        }

        [KSWebAuthorisation]
        public string Submit_Addmissions_Step1(StudentDTO step1VM)
        {
            //check if email 
            string flag;
            string local = "";
           _studentService.Step1Submission(step1VM, User.UserId,out flag);
            switch (flag)
            {
                case "01":
                    local = "Email already in use by someone else";
                    break;
                case "02":
                    local = "Jamb Registration Number already in use by someone else";
                    break;
                default:
                    local = "00";
                    break;
            }
            return local;
        }
        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult UploadPassport()
        {

            var user = (CustomPrincipal)Session["loggedUser"];
            var stu = _studentService.FetchStudent(user.UserId);

            if(stu.Foto!=null)
            {
                ViewBag.foto = stu.Foto;
            }
            return View();
        }
        [KSWebAuthorisation]
        [HttpPost]
        public ActionResult UploadPassport(HttpPostedFileBase image)
        {
            
            if (!ModelState.IsValid)
                return View();

            if (image == null)
            {
                ModelState.AddModelError("", "Invalid picture uploaded");
                return View();
            }

            int filelength = image.ContentLength;
            if (filelength > 50000 || filelength<5000)
            {
                ModelState.AddModelError("", "Error: Passport size must not exceed 50 kilobytes");
                return View();
            }
            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(image.InputStream))
            {
                imageData = binaryReader.ReadBytes(image.ContentLength);

            }
            var appImages = new AppImages
            {
                Foto = imageData
            };
            _studentService.SubmitPassport(appImages, User.UserId);
            var user = (CustomPrincipal)Session["loggedUser"];
            if(user.Roles.Contains("Alumnus"))
            {
                return RedirectToAction("AlumniDashboard", "Alumni");
            }
            
            else
            {
                return RedirectToAction("OlevelResults",new { studentId = User.UserId });
            }
            
        }
        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult OlevelResults()
        {
             
            var user = (CustomPrincipal)Session["loggedUser"];
            
            return View();
        }

        [KSWebAuthorisation]
        [HttpGet]
        public JsonResult FetchStudentOlevelResults(byte sitting)
        {


            return Json(_studentService.FetchOlevelResults(User.UserId,sitting),JsonRequestBehavior.AllowGet);

        }
         
        [HttpPost]
        public JsonResult AddOlevelResult(OlevelResultDetailDTO item)
        {

            string studentid;
            int flag;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (User.IsSysAdmin==true)
            {
                item.StudentId = (string)Session["studentid"];
            }
            else { item.StudentId = User.UserId; }
            var res=_studentService.AddOlevelResult(item, User.UserId,out flag);
           
                return Json(res, JsonRequestBehavior.AllowGet);
             
        }
        public string DeleteOlevelSubject(OlevelResultDetailDTO item)
        {
            return _studentService.DeleteOlevelResult(item,User.UserId);
        }
        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult AlevelResults(string studentId)
        {

            var user = (CustomPrincipal)Session["loggedUser"];
             
                var stu = _studentService.FetchStudent(studentId);
            if(stu.EntryMode=="Direct Entry")
            {
                return View();
            }
            else
            {
                if (stu.Status == "Active")
                    return RedirectToAction("Index", "Student");
                else
                    return RedirectToAction("Prospectives", "Admission_Center");
            }


        }
        

        [AllowAnonymous]
        public JsonResult GetAlevelResults(string studentId)
        {
            return Json(_studentService.GetStudentAlevel(studentId), JsonRequestBehavior.AllowGet);
        }
        [KSWebAuthorisation]
        public string SubmitAlevelResult(OtherQualificationDTO item)
        {

            return _studentService.SaveAlevel(item, User.UserId);

        }

        public string DeleteAlevelResult(OtherQualificationDTO item)
        {
            return _studentService.DeleteAlevel(item, User.UserId);
        }

        [KSWebAuthorisation]
        public ActionResult JambScore(string studentId)
        {
            var user = (CustomPrincipal)Session["loggedUser"];
            string id;
            if(string.IsNullOrEmpty(studentId))
            {
                id = user.UserId;
            }
            else { id = studentId; }
            var stu = _studentService.FetchStudent(id);
              
            
                    if (stu.EntryMode == "Direct Entry")
                    {
                        return RedirectToAction("AlevelResults", new { studentId = id });
                    }
                    else return View();
        
            
        }
        [AllowAnonymous]
        public JsonResult GetStudentJambParts(string studentId)
        {
            return Json(_studentService.GetStudentJambReg(User.UserId),JsonRequestBehavior.AllowGet);
        }
        [KSWebAuthorisation]
        public JsonResult SubmitJambResult(JambScoresDTO item)
        {
            int flag;

            return Json(_studentService.SaveJambScore(item, User.UserId,out flag),JsonRequestBehavior.AllowGet);
            
        }

        public string DeleteJambSubject(JambScoresDTO item)
        {
            return _studentService.DeleteJambScore(item, User.UserId);
        }
        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult Addmissions_Step5()
        {
            var user = (CustomPrincipal)Session["loggedUser"];
                 var stu = _studentService.FetchStudent(user.UserId);

                var userData = _generalDuties.GetProgrammeTypes().Where(a => a.Type == stu.ProgrammeType).FirstOrDefault();
                if (userData.AdmissionPause == 5)
                {
                    if(user.IsInRole("Prospective"))
                    {
                        return RedirectToAction("Student_Application_Summary",new { regNo = stu.StudentId });
                    }
                    else if (user.IsInRole("Student"))
                    {
                        return RedirectToAction("Index", "Student");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }


                }
                else
                {
                    return View();
                }

  
        }
        [KSWebAuthorisation]
        [HttpPost]
        public ActionResult Addmissions_Step5(HttpPostedFileBase file)
        {


            string studentid;
            var user = (CustomPrincipal)Session["LoggedUser"];
            if (user.IsSysAdmin == true)
            {
                studentid = (string)Session["studentid"];
            }
            else { studentid = User.UserId; }
            if (file.ContentLength > 0)
            {
                
                string filename = Path.GetFileName(file.FileName);
                string path = "~/Content/Documents/" + studentid+".pdf";
                string fPath = Server.MapPath("~/Content/Documents/" + studentid+".pdf");
                file.SaveAs(fPath);

                StudentDocuments doc = new StudentDocuments();
                
                doc.Path = path;
                doc.PersonId = studentid;

                _studentService.SaveDocument(doc, User.UserId);
                return RedirectToAction("Student_Application_Summary","Admission_Center");
            }
            else
            {
                ModelState.AddModelError("", "Error Uploading file");
                return View();
            }
            /*var data = new byte[file.ContentLength];
            file.InputStream.Read(data, 0, file.ContentLength);

            using (var sw = new FileStream(path, FileMode.Create))
            {
                sw.Write(data, 0, data.Length);
            }

            doc.PersonId = user.UserId;
            doc.Path = path;

            _studentService.SaveDocument(doc);
            return RedirectToAction("Index");*/
        }
        [KSWebAuthorisation]
        public ActionResult AdmissionLetter()
        {
                       
            var student = _studentService.FetchStudentSummary(User.UserId);
            if(student.AdmissionStatus==null||string.IsNullOrEmpty(student.MatricNumber))
            {
                
                return new ViewAsPdf(new StudentSummaryDTO())
                {
                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                };

            }
            else
            {

                string text = "AdmissionLetter_" + student.StudentId;
                
                ViewBag.qrCode = GenerateQRCode(text);
                ViewBag.dt = student.DateAdmitted==null?"":student.DateAdmitted.Value.ToString("dd-MMM-yyy");
                return new ViewAsPdf(student)
                {
                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                };
            }
            
             
        }
        string GenerateQRCode(string text)
        {
            string encrypt = DataEncryption.AESEncryptData(text);
            using (MemoryStream ms = new MemoryStream())
            {
                var req = System.Web.HttpContext.Current.Request;
                string final = req.Url.Scheme + "://" + req.Url.Authority + "/DocuVerify/Verify?q=" + encrypt;
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrData = qrGenerator.CreateQrCode(final, QRCodeGenerator.ECCLevel.Q);
                using (System.Drawing.Bitmap bitMap =new QRCode(qrData).GetGraphic(20))
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
             
        }
        [KSWebAuthorisation]
        public ActionResult Student_Application_Summary(string regNo)
        {

            
            string studentid;
            if(!string.IsNullOrEmpty(regNo))
            {
                studentid = regNo;
            }
            else
            {
                //var user = (CustomPrincipal)Session["LoggedUser"];
                studentid = User.UserId;
            }
            var stat = _studentService.CheckCompletedProfile(studentid);
            if(stat=="Ok")
            {
                var student = _studentService.StudentApplicationDetail(studentid);
                var userdata = _generalDuties.GetUserData();
                var pdf = StudentPDFReports.CreateStudentApplicationSlipPdf(student, userdata);
                if(string.IsNullOrEmpty(student.Sex)|| string.IsNullOrEmpty(student.MaritalStatus)||
                    string.IsNullOrEmpty(student.BirthDate))
                {
                    ViewBag.error ="You have not completed your form. Please completely update your profile to continue";
                    return View();
                }
                return File(pdf, "application/pdf");
            }
            else
            {
                ViewBag.error = stat;
                return View();
            }
            
             

        }

        [KSWebAuthorisation]
        public ActionResult PrintStudentProfile(string regNo)
        {


            string studentid;
            if (!string.IsNullOrEmpty(regNo))
            {
                studentid = regNo;
            }
            else
            {
                //var user = (CustomPrincipal)Session["LoggedUser"];
                studentid = User.UserId;
            }

            var student = _studentService.StudentApplicationDetail(studentid);
            return new ViewAsPdf(student) {
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
            };

        }



#endregion

#region ADMISSION DETAILS
        [KSWebAuthorisation]
        public JsonResult Applicants(string session,string prog,string rpt)
        {
            var apps = _studentService.FetchApplicants(session,prog,rpt);
            return this.Json(apps,JsonRequestBehavior.AllowGet);
        }
        [KSWebAuthorisation]
        public ActionResult PrintApplicants(string session,string prog,string rpt,string fil)
        {
            
                var apps = _studentService.FetchApplicants(session, prog, rpt);
                return new ViewAsPdf(apps)
                {
                    PageOrientation = Orientation.Portrait,
                    PageSize = Size.A4,
                    CustomSwitches = "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                    PageMargins = { Left = 1, Right = 1, Top = 2, Bottom = 12 }
                };
            
        }
        [KSWebAuthorisation]
        public ActionResult PrintApplicantsByDept(string session, string dept, string progType,string rpt)
        {
            var apps = _studentService.FetchApplicantsByDept(session, dept,progType, rpt);
            return new ViewAsPdf(apps)
            {
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                CustomSwitches = "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                PageMargins = { Left = 1, Right = 1, Top = 2, Bottom = 12 }
            };
        }
        [KSWebAuthorisation]
        public ActionResult ViewProspectives()
        {
            return View();
        }

        [KSWebAuthorisation]
        public ActionResult FindStudent()
        {
            return View();
        }
        [KSWebAuthorisation]
        public ActionResult StudentCredential(string studentId)
        {
            var st = _studentService.ViewDocument(studentId);
            return View();
        }
        [KSWebAuthorisation]
        public JsonResult AddmitStudent(string studentId)
        {
            
            var student = _studentService.AdmitStudent(studentId,User.UserId);
            var user = (CustomPrincipal)Session["LoggedUser"];
            //check if acceptance fee is 
            var prog = _generalDuties.GetStudentProgrameType(studentId);
            if (prog.AcceptAdmissionFee == false)
            {
                _userService.ChangeUserRole(studentId, user.UserId, "Prospective", "Student");
            }
            //_studentAccount.DebitNewStudent(studentId,User.UserId);
            return Json(_studentService.AdmitStudent(studentId, user.UserId),JsonRequestBehavior.AllowGet);
        }
        [KSWebAuthorisation]
        public ActionResult ViewApplicants()
        {
            return View();
        }

        [KSWebAuthorisation]
        public ActionResult ViewApplicantsRpt(string session,string progType)
        { 
            List<ApplicantsVM> vm = new List<ApplicantsVM>();
            var apps = _studentService.ApplicantsByProgrammeType(session, progType);
            if (apps.Count == 0)
                return null;
            else
            {
                var first = apps.First();
                foreach (var a in apps)
                {
                    vm.Add(new ApplicantsVM
                    {
                        AddmissionCompleteStage = a.AddmissionCompleteStage,
                        Email = a.Email,
                        JambNo = a.JambNo,
                        JambScore=a.JambScore,
                        Lga = a.Lga,
                        Name = a.Name,
                        Phone = a.Phone,
                        Programme = a.Programme,
                        RegNo = a.RegNo,
                        State = a.State,
                        Status = a.Status
                    });
                }
                Export2Excel<ApplicantsVM>(vm, first.Session + "_" + first.ProgrammeType + "_" + "Applicants");
                return View("ViewApplicants");
            }
            
        }

        [KSWebAuthorisation]
        public ActionResult StudentEnrolment()
        {
            return View();
        }

        public ActionResult StudentEnrolmentByProgrammeTypeRpt(string session, string progType)
        {
            var vm=_studentService.StudentEnrolmentByProgrammeType(session, progType);
            var first = vm.First();
            Export2Excel<StudentEnrolmentDTO>(vm, first.YearAdmitted + "_" + first.ProgrammeType + "_" + "Enrolment");
            return View("StudentEnrolment");

        }

        [KSWebAuthorisation]
        public ActionResult StudentsEnrolmentRpt(string session, string dept,string sex=null)
        {
            List<StudentEnrolmentDTO> vm;
             
            if (string.IsNullOrEmpty(sex) || sex == "undefined")
            {
                vm = _studentService.StudentEnrolment(session,dept);
            }
            else
            {
                vm = _studentService.StudentEnrolment(session, dept,sex);
            }

            if (vm != null)
            {

                var first = vm.First();
                Export2Excel<StudentEnrolmentDTO>(vm, first.YearAdmitted + "_" + first.Department + "_" + "Enrolment");
                return View("StudentEnrolment");
            }
            else return View("StudentEnrolment");
           

        }
        
        public JsonResult StudentsEnrolmentSummary(string session, string dept, string sex = null)
        {
            List<ProgrammeDTO> vm;

            if (string.IsNullOrEmpty(sex) || sex == "undefined")
            {
                vm = _studentService.StudentEnrolmentSummary(session, dept);
            }
            else
            {
                vm = _studentService.StudentEnrolmentSummary(session, dept, sex);
            }
            
            return Json(vm,JsonRequestBehavior.AllowGet);


        }

        public ActionResult UploadManualMatricNumbers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadManualMatricNumbers(HttpPostedFileBase file, string progType)
        {
            if (file.ContentLength > 0 && (file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                 || file.ContentType == "application/vnd.ms-excel")&& !string.IsNullOrEmpty(progType))
            {
                
                using (ExcelPackage package = new ExcelPackage(file.InputStream))
                {
                    List<MatricRegDetailsDTO> regs = new List<MatricRegDetailsDTO>();
                    var wrkSheet = package.Workbook.Worksheets[progType.Trim().ToUpper()];
                    int rows = wrkSheet.Dimension.Rows;
                    for (int row = 2; row <= rows; row++)
                    {
                        var mt = new MatricRegDetailsDTO();
                        mt.Programme = progType.Trim().ToUpper();
                        mt.StudentId = wrkSheet.Cells[row, 10].Value.ToString();
                        mt.MatricNo = wrkSheet.Cells[row, 2].Value.ToString();

                        regs.Add(mt);
                    }
                    _studentService.UpdateManualMatricNos(progType, regs, User.UserId);
                }
                ModelState.AddModelError("", "Matric Numbers successfully updated");
                return View();
            }
            else
            {
                ModelState.AddModelError("", "Required field is missing");
                return View();
            }
            
            return View();
        }
        #endregion



        #region PRIVATE HELPERS

        #endregion
    }
}