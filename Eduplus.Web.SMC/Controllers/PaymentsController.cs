using Eduplos.Domain.BurseryModule;
using Eduplos.DTO.BursaryModule;
using Eduplos.Services.Contracts;
using Eduplos.Web.SMC.ViewModels;
using KS.Services.Contract;
using KS.UI.ViewModel;
using KS.Web.Security;
using Microsoft.Owin.Security.DataProtection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Eduplos.Web.SMC.Controllers
{

    public class PaymentsController : BaseController
    {

        private readonly IBursaryService _bursaryService;
        private readonly IStudentsAccountsService _studentAccounts;
        private readonly IUserService _userService;
        private readonly IGeneralDutiesService _generalDutiesService;
        private readonly IStudentService _studentService;
        private readonly IAcademicProfileService _academicProfile;
        private readonly IAcademicAffairsService _acadaAffairs;
        public PaymentsController(IBursaryService bursaryService, IStudentsAccountsService studentAccounts, IUserService userService,
            IGeneralDutiesService general, IStudentService studentservice, 
            IAcademicProfileService academicProfile, IAcademicAffairsService acadF)
        {
            _bursaryService = bursaryService;
            _studentAccounts = studentAccounts;
            _userService = userService;
            _generalDutiesService = general;
            _studentService = studentservice;
            _academicProfile = academicProfile;
            _acadaAffairs = acadF;
            //protector = prot.Create(GetType().FullName);

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> WebPayConfirmation(string RRR, string orderID)
        {
            //Check if transaction exist
            var inv = _studentAccounts.GetStudentPaymentInvoice(orderID, RRR);

            if (inv == null)
            {
                ViewBag.error = "Invalid transaction, please contact ICT Directorate";
                return View();
            }
            var session = _generalDutiesService.FetchSessions().Where(a => a.Title == inv.Session).FirstOrDefault();
            //Confirm transaction status
            var gatewayPaymentInfo = _generalDutiesService.GetDefaultPaymentGateway(inv.ProgrammeType);
            string hash = _generalDutiesService.Sha512Hasher(RRR + gatewayPaymentInfo.MerchantKey + gatewayPaymentInfo.MerchantCode);
            string url = gatewayPaymentInfo.TransConfirmationUrl + gatewayPaymentInfo.MerchantCode + "/" + RRR + "/" + hash + "/status.reg";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<remitaPayConfirmationData>(responseData);

                if (data.status == "00" && inv.Status != "PAID")//Successfull transaction Update invoice and log payment
                {
                    var lt=_studentAccounts.ProcessSuccessfulEPayments(data.orderId, data.paymentDate, "Electronic", User.UserId);
                    //UPdate student record for utilities
                    
                    bool ko = false;
                    if (lt== "Late")
                    {
                        ko = true;
                    }
                    _studentService.GrantStudentAccessToUtilitesBasedOnPayments(inv.PaymentType, inv.StudentId, inv.PayOptionId, session.SessionId, ko);
                    //UPdate prospective role to student role
                    if (inv.PaymentType == "Acceptance Fee")
                    {
                        _userService.ChangeUserRole(inv.StudentId, User.UserId, "Prospective", "Student");
                        //Debit New Student
                        _studentAccounts.DebitNewStudent(inv.StudentId, User.UserId);
                    }
                    if(inv.PaymentType=="Transcript-Local"||inv.PaymentType=="Transcript-Abroad")
                    {
                       _acadaAffairs.UpdateTranscriptToPaidStatus(inv.Particulars);
                    }
                }
                //log transaction
                GateWaylogs gatelog = new GateWaylogs();
                gatelog.BankName = "Electronic Card";

                gatelog.PayeeName = inv.Name;
                gatelog.Address = inv.Email;
                gatelog.TransDate = data.transactiontime;
                gatelog.Amount = inv.Amount;
                gatelog.LogDate = DateTime.UtcNow;
                gatelog.DebitDate = data.transactiontime;
                gatelog.ReceiptNo = data.orderId;
                gatelog.BranchName = "Electronic";
                gatelog.Channelname = "E-Channel";
                gatelog.PaymentCode = data.RRR;
                gatelog.TellerId = data.RRR;
                gatelog.ServerResponse = data.status + "-" + data.message;
                _generalDutiesService.SaveGatewayTransactionLogs(gatelog, "System on epayment " + data.orderId);
                return View(data);
            }
            else
            {
                ViewBag.error = response.Content.ReadAsStringAsync();
                remitaPayConfirmationData p = new remitaPayConfirmationData();
                p = null;
                return View(p);
            }

        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult WebPayConfirmation()
        {
            var user = (CustomPrincipal)Session["loggedUser"];
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            else if (user.Roles.Contains("Student"))
            {
                return RedirectToAction("Index", "Student");
            }
            else if (user.Roles.Contains("Prospective"))
            {
                return RedirectToAction("Prospectives", "Admission_Center");
            }
            else if (user.Roles.Contains("Alumnus"))
            {
                return RedirectToAction("AlumniDashboard", "Alumni");
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        #region STUDENT BURSARY OPERATIONS
        [KSWebAuthorisation]
        public ActionResult SchoolFeePayment(string studentId)
        {
            string stId;
            if(string.IsNullOrEmpty(studentId))
            {
                stId = User.UserId;
            }
            else { stId = studentId; }
            var completion = _studentService.FetchStudent(stId);
             
            if (completion.AddmissionCompleteStage == 2)
                return RedirectToAction("Addmissions_Step1", "Admission_Center");
            return View();
        }

        

        [KSWebAuthorisation]
        public string SubmitSchoolFeePayment(object[] data1)
        {
            int sessionId = (int)data1[0];
            int installment = (int)data1[1];
            int level = (int)data1[2];
            string matricNo =(string)data1[3];
            string flag;
            var ch = User.IsSysAdmin;
            var sem = _generalDutiesService.FetchCurrentSemester();
            var user = (CustomPrincipal)Session["loggedUser"];
            var st = _studentService.FetchStudent(user.UserId);
            if(st.Status=="Suspended"||st.Status=="Expelled")
            {
                return JsonConvert.SerializeObject(new outPutMsg
                {
                    message = "Oops! You are under suspension, hence cannot pay school fees.",
                    value = null
                });
            }
            if (DateTime.Now.Date > sem.LateRegistrationEndDate.Date && user.Roles.Contains("Student"))
            {
                return JsonConvert.SerializeObject(new outPutMsg
                {
                    message = "Oops! School fee payments for selected session has ended.",
                    value = null
                });
            }

            string amt = null;
            bool py = _studentAccounts.CheckPreviousSessionDebt(User.UserId, sessionId, out amt);
            if (py== true && !User.IsSysAdmin)
            {
                return JsonConvert.SerializeObject(new outPutMsg
                {
                    message = "Please complete previous session school fees before paying current session fees. You are owing the School "+amt,
                    value = null                    
                });
            }
            string studentId="";

            if (string.IsNullOrEmpty(matricNo))
            {
                studentId = User.UserId;
            }
            else
            {
                studentId = matricNo;
            }                   
            var schedule = _studentAccounts.GenerateSchoolFeesPaymentInvoice(studentId, sessionId, installment,level, User.UserId, out flag);
            outPutMsg result = new outPutMsg();
            result.message = flag;
            result.value = schedule == null ? "OO" : schedule.TransactionId;
            var chk = JsonConvert.SerializeObject(result);
            if (schedule != null)
            {
                _academicProfile.AddRegistrationPermissionsLog(studentId, sessionId);
            }

            return chk;
        }

        

        [KSWebAuthorisation]
        public ActionResult PayOtherCharges()
        {
            var completion = _studentService.FetchStudent(User.UserId);
            if (completion.AddmissionCompleteStage == 2)
                return RedirectToAction("Addmissions_Step1", "Admission_Center");
            return View();
        }
        [KSWebAuthorisation]
        public string SubmitOtherChargesPayment(object[] data1)
        {
            string chargeId = (string)data1[1];
            int sessionId = (int)data1[0];
            outPutMsg result = new outPutMsg();
            
            string msg;
            var invoice = _studentAccounts.GenerateStudentPaymentInvoice(User.UserId, chargeId, sessionId, User.UserId,out msg);
            result.message = msg;
            if(msg!="Ok")
            {
                result.message = msg;
                result.status = 0;
            }
            else
            {
                result.value = invoice.TransactionId;
                result.status = 1;
            }
            var chk = JsonConvert.SerializeObject(result);
            return chk;
        }

        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult PaymentInvoice(string transId)
        {

            if (string.IsNullOrEmpty(transId))
            {
                ViewBag.msg = "Invalid or no TransactionID supplied";
                return View();
            }
            ViewBag.logo = _generalDutiesService.GetUserData().Logo;
            var schedule = _studentAccounts.GetStudentPaymentInvoice(transId);

            if (schedule == null)
            {
                ViewBag.msg = "Invalid transaction";
                return View();
            }


            if (schedule.Status != "PAID")
            {
                var gatewayInfo = _generalDutiesService.GetStudentProgrameType(schedule.StudentId).PaymentGateWays.Where(a=>a.IsDefault==true).SingleOrDefault();
                ViewBag.merchantId = gatewayInfo.MerchantCode;
                ViewBag.rrr = schedule.TransRef;
                ViewBag.hash = _generalDutiesService.Sha512Hasher(gatewayInfo.MerchantCode + schedule.TransRef + gatewayInfo.MerchantKey);
                ViewBag.postUrl = gatewayInfo.TransPostUrl;
                ViewBag.response = gatewayInfo.ResponseUrl;

                return View(schedule);
            }

            else
            {
                return View(schedule);
            }
        }

        [KSWebAuthorisation]
        public ActionResult MyInvoices()
        {
            //var user = (CustomPrincipal)Session["LoggedUser"];
            var invoices = _studentAccounts.PaymentInvoices(User.UserId);
            return View(invoices);
        }

        [KSWebAuthorisation]
        public ActionResult MyPayments()
        {

            string balance;
            var user = (CustomPrincipal)Session["LoggedUser"];
            var payments = _bursaryService.StudentPayments(user.UserId, out balance);
            ViewBag.bal = balance;
            return View(payments);
        }

        #endregion
        #region TRANSACTION STATUS/REPORTING
        [KSWebAuthorisation]
        [HttpGet]
        public ActionResult CheckInvoiceStatus()
        {
            return View();
        }
        [KSWebAuthorisation]
        [HttpPost]
        public ActionResult CheckInvoiceStatus(string invoiceNo)
        {
            
            var inv = _studentAccounts.GetStudentPaymentInvoice(invoiceNo);
            if(inv==null)
            {
                ModelState.AddModelError("", "Invoice not found");
                return View();
            }
            var marchantInfo = _generalDutiesService.GetDefaultPaymentGateway(inv.ProgrammeType);
            string hash = _generalDutiesService.Sha512Hasher(inv.TransactionId + marchantInfo.MerchantKey + marchantInfo.MerchantCode);
            string url = marchantInfo.TransConfirmationUrl + marchantInfo.MerchantCode + "/" + inv.TransactionId + "/" + hash + "/orderstatus.reg";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpClient client = new HttpClient();
            var req = client.GetAsync(url).Result;

            if (req.IsSuccessStatusCode)
            {
                var data = req.Content.ReadAsStringAsync().Result;
                var finald = JsonConvert.DeserializeObject<remitaPayConfirmationData>(data);
                if(finald.status=="00"&&inv.Status=="Pending")
                {
                    var ses = _generalDutiesService.FetchSessions();
                   
                        var up = _studentAccounts.ProcessSuccessfulEPayments(inv.TransactionId, finald.paymentDate, "Electronic", "System");

                        var sessionId = ses.Where(s => s.Title == inv.Session).FirstOrDefault();

                        bool ko = false;
                        if (up == "Late")
                        {
                            ko = true;
                        }

                        _studentService.GrantStudentAccessToUtilitesBasedOnPayments(inv.PaymentType, inv.StudentId, inv.PayOptionId, sessionId.SessionId, ko);
                        if (inv.PaymentType == "Acceptance Fee")
                        {
                            _userService.ChangeUserRole(inv.StudentId, "System", "Prospective", "Student");
                            //debit New student
                            _studentAccounts.DebitNewStudent(inv.StudentId, User.UserId);
                        }
                        if (inv.PaymentType == "Transcript-Local" || inv.PaymentType == "Transcript-Abroad")
                        {
                            _acadaAffairs.UpdateTranscriptToPaidStatus(inv.Particulars);
                        }

                    
                }
                //finald.amount = inv.Amount;
                ViewBag.stat = "Ok";
                finald.amount = inv.Amount;
                return View(finald);

            }
            ViewBag.status = "Error confirming invoice";
            return View();
        }
        #endregion
        #region MANUAL FEE OPERATIONS
        [KSWebAuthorisation]
        public ActionResult ManualPayConfirmation()
        {
            return View();
        }
        [KSWebAuthorisation]
        public JsonResult FetchManualInvoice(string transId)
        {
            return Json(_studentAccounts.GetStudentPaymentInvoiceForManualConfirmation(transId), JsonRequestBehavior.AllowGet);
        }
        [KSWebAuthorisation]
        public JsonResult SubmitManualPayConfirmation(string transId, DateTime pDate)
        {

            var user = (CustomPrincipal)Session["LoggedUser"];

            var inv = _studentAccounts.GetStudentPaymentInvoice(transId);
            var session = _generalDutiesService.FetchSessions().Where(a => a.Title == inv.Session).FirstOrDefault();
            if (inv != null || inv.Status != "PAID")
            {
                var lt = _studentAccounts.ProcessSuccessfulEPayments(inv.TransactionId, pDate, "Manual", User.UserId);
                //UPdate student record for utilities

                bool ko = false;
                if (lt == "Late")
                {
                    ko = true;
                }
                _studentService.GrantStudentAccessToUtilitesBasedOnPayments(inv.PaymentType, inv.StudentId, inv.PayOptionId, session.SessionId, ko);
                //UPdate prospective role to student role
                if (inv.PaymentType == "Acceptance Fee")
                {
                    _userService.ChangeUserRole(inv.StudentId, User.UserId, "Prospective", "Student");
                    //Debit New Student
                    _studentAccounts.DebitNewStudent(inv.StudentId, User.UserId);
                }
                if (inv.PaymentType == "Transcript-Local" || inv.PaymentType == "Transcript-Abroad")
                {
                    _acadaAffairs.UpdateTranscriptToPaidStatus(inv.Particulars);
                }

                return Json("Invoice successfully confirmed", JsonRequestBehavior.AllowGet);
            }



            return Json("Invalid transactionID", JsonRequestBehavior.AllowGet);
        }
        #endregion
        [KSWebAuthorisation]
        public ActionResult FetchPayments()
        {
            return View();
        }
       
        [HttpGet]
        public async Task<ActionResult> ManualCompletedPaymentsStatusUPdate()
        {


            //Check if transaction exist
            var gateways = _generalDutiesService.GetAllPaymentGateway();
            var inv = _studentAccounts.FetchInvoiceforConfirmationProcess();
            List<TransIdsProcessMthdDTO> chk = new List<TransIdsProcessMthdDTO>();

            //Confirm transaction status
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpClient client = new HttpClient();
            foreach (var i in inv)
            {
                var gatewayPaymentInfo = gateways.Where(a => a.ProgrammeTypeCode == i.ProgrammeType&&a.IsDefault==true).FirstOrDefault();

                string hash = _generalDutiesService.Sha512Hasher(i.TransRef + gatewayPaymentInfo.MerchantKey + gatewayPaymentInfo.MerchantCode);
                string url = gatewayPaymentInfo.TransConfirmationUrl + gatewayPaymentInfo.MerchantCode + "/" + i.TransRef + "/" + hash + "/status.reg";
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<remitaPayConfirmationData>(responseData);
                    if (data.status == "00")
                    {
                        chk.Add(new TransIdsProcessMthdDTO
                        {
                            transactionID = data.orderId,
                            transDate = data.paymentDate,
                            TransMthd = "Electronic"
                        });
                    }
                    
                }
                 

            }
            if (chk.Count > 0)
            {

                _studentAccounts.UPdateManualInvoiceMethd(chk);


            }

            return View();

        }

        [HttpGet]
        public ActionResult AllCompletedRemitaPayments(DateTime startDate)
        {


            //Check if transaction exist
            var gateways = _generalDutiesService.GetAllPaymentGateway();
            var inv = _studentAccounts.FetchCompletePay4Confirmation(startDate);
             
            List<PaymentInvoiceDTO> verifiedInv = new List<PaymentInvoiceDTO>();
            //Confirm transaction status
             

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpClient client = new HttpClient();
           foreach (var i in inv)
            {
                var gatewayPaymentInfo = gateways.Where(a => a.ProgrammeTypeCode == i.ProgrammeType && a.IsDefault == true).FirstOrDefault();
                string hash = _generalDutiesService.Sha512Hasher(i.TransRef + gatewayPaymentInfo.MerchantKey + gatewayPaymentInfo.MerchantCode);
                string url = gatewayPaymentInfo.TransConfirmationUrl + gatewayPaymentInfo.MerchantCode + "/" + i.TransRef + "/" + hash + "/status.reg";
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseData = "";
                    responseData=response.Content.ReadAsStringAsync().Result;
                    remitaPayConfirmationData data=null;
                    data = JsonConvert.DeserializeObject<remitaPayConfirmationData>(responseData);
                    if (data.status == "00")
                    {
                        verifiedInv.Add(new PaymentInvoiceDTO
                        {
                            TransactionId = data.orderId,
                            CompletedDate = data.paymentDate,
                            Session = i.Session,
                            PaymentType=i.PaymentType,
                            StudentId=i.StudentId,
                            PayOptionId=i.PayOptionId
                        });
                    }
                }
                else
                {
                    i.Status = response.StatusCode.ToString();
                }

            }
            if (verifiedInv.Count > 0)
            {
                var ses = _generalDutiesService.FetchSessions();
                foreach (var c in verifiedInv)
                {
                    var up = _studentAccounts.ProcessSuccessfulEPayments(c.TransactionId, (DateTime)c.CompletedDate, "Electronic", "System");
                    
                    var sessionId = ses.Where(s => s.Title == c.Session).FirstOrDefault();
                    
                    bool ko = false;
                    if (up=="Late")
                    {
                        ko = true;
                    }

                    _studentService.GrantStudentAccessToUtilitesBasedOnPayments(c.PaymentType, c.StudentId, c.PayOptionId, sessionId.SessionId, ko);
                    if (c.PaymentType == "Acceptance Fee")
                    {
                        _userService.ChangeUserRole(c.StudentId, "System", "Prospective", "Student");
                        //debit New student
                        _studentAccounts.DebitNewStudent(c.StudentId, "System");
                    }
                    if (c.PaymentType == "Transcript-Local" || c.PaymentType == "Transcript-Abroad")
                    {
                        _acadaAffairs.UpdateTranscriptToPaidStatus(c.Particulars);
                    }

                }
            }

            return View();
        }
   
        
         
    }

    public class RRRResponse
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }
     
}

    