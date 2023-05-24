using Eduplus.Domain.AcademicModule;
using Eduplus.DTO.CoreModule;
using Eduplus.Services.Contracts;
using KS.Web.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Eduplus.Web.SMC.Controllers
{
    public class AlumniController : BaseController
    {
        private readonly IStudentsAccountsService _studentAccount;
        private readonly IBursaryService _bursaryService;
        private readonly IStudentService _studentService;
        private readonly ICommunicationService _commService;
        private readonly IGeneralDutiesService _generalDuties;
        private readonly IAcademicAffairsService _acadaFairs;
        public AlumniController(IStudentsAccountsService studentAccount,IBursaryService bursery,IStudentService sudent,
            ICommunicationService coms,IGeneralDutiesService general,IAcademicAffairsService acadefairs)
        {
            _studentAccount = studentAccount;
            _bursaryService = bursery;
            _studentService = sudent;
            _commService = coms;
            _generalDuties = general;
            _acadaFairs = acadefairs;
        }
        // GET: Alumni
        [KSWebAuthorisation]
        public ActionResult AlumniDashboard()
        {
            return View();
        }
        public ActionResult NewAlumnusRegistration()
        {
            return View();
        }
        public int SubmitNewAlumnus(StudentDTO viewModel)
        {
            int stat = 0;//some has taken your email

            string studentId;
            string mail = viewModel.Email;
            string pasword = viewModel.Password;
            string mailresponse;
            string fullname = viewModel.Title + " " + viewModel.Surname + ", " + viewModel.Firstname + " " + viewModel.Middlename;
            stat = _studentService.CreateNewAlumus(viewModel, out studentId);
            if (stat == 1)//send mail
            {
                /* msgBody = "Your personal data profile was successfully created." + "\n" +
                "Login to the portal to continue." + "\n" +
                "Your login details is as follows: " + "\n" +
                "Registration Number= " + studentId + "\n" + "Username= " + mail + "Password= " + pasword + "\n";
                mailresponse = _commService.SendMail(mail, msgBody,"New Profile Creation");
                if (mailresponse == "Ok")
                {
                    stat = 1;
                }*/
                return stat;

            }
            return stat;
        }
        [KSWebAuthorisation]
        public ActionResult PayConvocationFee()
        {
            var currentSesion = _generalDuties.FetchCurrentSemester();
            var user = (CustomPrincipal)Session["LoggedUser"];
            string invoiceId = "";

            var student = _studentService.FetchStudent(user.UserId);
            if(string.IsNullOrEmpty(student.GradYear))
            {
                return View();
            }
            //Check if Invoice Already exist
            //var inv = _studentAccount.CheckIfOtherFeePaymentExist(User.UserId,"Convocation Fee");
            string flag;
               
                    var othercharges = _bursaryService.FetchOtherChargesAmount("Convocation Fee", User.UserId);

                    var invoice = _studentAccount.GenerateStudentPaymentInvoice(User.UserId, othercharges.ChargeId,currentSesion.SessionId, User.UserId,out flag);
                    if (invoice == null)
                        invoiceId = "Invalid";
                    else
                        invoiceId = invoice.TransactionId;
                    return RedirectToAction("PaymentInvoice", "Payments", new { transId = invoiceId });
               
                    //return RedirectToAction("AlumniDashboard");
          
        }
        [KSWebAuthorisation]
        public ActionResult NewTranscriptApplication()
        {
            var st = _studentService.FetchStudent(User.UserId);
            if (st.AddmissionCompleteStage < 2)
            {
                return RedirectToAction("Addmissions_Step1", "Admission_Center");
            }
            if (st.PhotoId==null)
            {
                return RedirectToAction("UploadPassport", "Admission_Center");
            }
            /*
            Dictionary<string, int> test = new Dictionary<string, int>();*/

            return View();
        }
        [KSWebAuthorisation]
        public string SubmitTranscriptApplication(TranscriptApplication viewModel)
        {
            viewModel.StudentId = User.UserId;
            int chargeId = Convert.ToInt32(viewModel.TranscriptNo);
            var no = _acadaFairs.SubmitTranscriptApplication(viewModel);
            var curSes = _generalDuties.FetchCurrentSemester();
            string flag;
            var inv = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(no))
            {
                var invoiceNo = _studentAccount.GenerateTranscriptInvoice(viewModel.TranscriptNo, chargeId,
                    curSes.SessionId, User.UserId, out flag);
                
                inv.Add("flag", flag);
                inv.Add("value", invoiceNo);             
            }

            else
            {
                inv.Add("flag", "Error encountered while submitting request.");
                inv.Add("value", "not Ok");

            }
            return JsonConvert.SerializeObject(inv);
        }
        [KSWebAuthorisation]
        public ActionResult CurrentTranscriptApplications()
        {

            return View(_acadaFairs.CurrentlyPaidTranscriptRequest20DaysOld());
        }
        [KSWebAuthorisation]
        public ActionResult TranscriptDetail(string transNo)
        {
            return View(_acadaFairs.FetchTranscriptApplication(transNo));
        }

        [KSWebAuthorisation]
        public ActionResult MyTranscriptApplications()
        {
            var transcripts = _acadaFairs.FetchTranscriptApplications(User.UserId);
            return View(transcripts);
        }
    }
}