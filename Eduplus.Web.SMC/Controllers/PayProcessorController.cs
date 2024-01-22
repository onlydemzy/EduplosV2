using Eduplos.Domain.BurseryModule;
using Eduplos.Services.Contracts;
using Eduplos.Web.SMC.ViewModels;
using KS.Services.Contract;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Eduplos.Web.SMC.Controllers
{
    [RoutePrefix("api/PayProcessor")]
    public class PayProcessorController : ApiController
    {

        private readonly IGeneralDutiesService _generalDuties;
        private readonly IStudentsAccountsService _studentAccounts;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly IAcademicAffairsService _acadaAffairs;
        public readonly Logger logger = LogManager.GetCurrentClassLogger();
        public PayProcessorController(IGeneralDutiesService generalDuties,IStudentsAccountsService st,IStudentService stud,
            IUserService users,IAcademicAffairsService acada)
        {
            _generalDuties = generalDuties;
            _studentAccounts = st;
            _studentService = stud;
            _userService = users;
            _acadaAffairs = acada;
        }
        public PayProcessorController() { }
        
        // GET: api/PayProcessor/5
        [HttpPost]
        [HttpGet]
        [Route("PaymentConfirmation")]
        public async Task<IHttpActionResult> PaymentConfirmation(object[] data1)
        {
            //Response information
            //try
           // {
                string clientResponse = "";

                remitaBankBranchResponseData response = JsonConvert.DeserializeObject<remitaBankBranchResponseData>(data1[0].ToString());
            if (response == null)
                return Ok("Invalid parameter found in API");
                var inv = _studentAccounts.GetStudentPaymentInvoice(response.orderRef, response.rrr);
                if (inv == null)
                     return Ok("not Ok");
            GateWaylogs gatelog = new GateWaylogs();
                gatelog.BankName = response.bank;

                gatelog.PayeeName = response.payerName;
                gatelog.Address = response.payerEmail;
                gatelog.TransDate = response.transactiondate;
                gatelog.Amount = response.amount;
                gatelog.LogDate = DateTime.UtcNow;
                gatelog.DebitDate = response.debitdate;
                gatelog.ReceiptNo = response.orderRef;
                gatelog.BranchName = response.branch;
                gatelog.Channelname = response.channel;
                gatelog.PaymentCode = response.rrr;
                gatelog.TellerId = response.rrr;

                var gateInfo = _generalDuties.GetDefaultPaymentGateway(inv.ProgrammeType);

                //Confirm payment request
                string hash = _generalDuties.Sha512Hasher(response.rrr + gateInfo.MerchantKey + gateInfo.MerchantCode);

                string url = gateInfo.TransConfirmationUrl + gateInfo.MerchantCode + "/" + gatelog.PaymentCode + "/" + hash + "/status.reg";
                string code = "";
                HttpClient client = new HttpClient();
                var respo = await client.GetAsync(url);
                if (respo.IsSuccessStatusCode)
                {
                    var data = await respo.Content.ReadAsStringAsync();
                    var di = JsonConvert.DeserializeObject<remitaPayConfirmationData>(data);
                    gatelog.ServerResponse = di.status + ":" + di.message;
                    code = di.status;
                    clientResponse = di.message;

                    if (di.status == "00")//Post transaction
                    {
                    
                        clientResponse = _studentAccounts.ProcessSuccessfulEPayments(di.orderId, di.paymentDate, "Bank Branch");
                        //UPdate invoice status
                        if (clientResponse == "PAID")
                            return Ok("not OK");
                        var session = _generalDuties.FetchSessions().Where(s => s.Title == inv.Session).FirstOrDefault();
                         
                        bool ko = false;
                        if (clientResponse=="Late")
                        {
                            ko = true;
                        }
                        _studentService.GrantStudentAccessToUtilitesBasedOnPayments(inv.PaymentType, inv.StudentId, inv.PayOptionId, session.SessionId, ko);
                        //UPdate prospective role to student role
                        if (inv.PaymentType == "Acceptance Fee")
                        {
                            _userService.ChangeUserRole(inv.StudentId, "Sysem", "Prospective", "Student");
                           _studentAccounts.DebitNewStudent(inv.StudentId, "Sys via PayGateway");
                    }
                    if (inv.PaymentType == "Transcript-Local" || inv.PaymentType == "Transcript-Abroad")
                    {
                        _acadaAffairs.UpdateTranscriptToPaidStatus(inv.Particulars);
                    }
                    gatelog.TransDate = di.transactiontime;
                        gatelog.DebitDate = di.paymentDate;

                        _generalDuties.SaveGatewayTransactionLogs(gatelog, clientResponse + "-" + di.orderId);

                        return Ok("OK");
                    }
                    else
                    {
                        gatelog.ServerResponse = di.status + ":" + di.message;
                        _generalDuties.SaveGatewayTransactionLogs(gatelog, "system on epayment " + di.orderId);
                        return Ok("not OK");
                    }
                }
                else
                {
                    return Ok("not OK");
                }
            //}
            /*catch(Exception ex)
            {
               logger.Error( ex.InnerException,ex.Message+"\r\n"+ex.StackTrace);
                                //logger.Info(ex.StackTrace);
                //logger.Debug(ex, "Debug Error");
                return Ok("not Ok");
            }*/
              
             
        }
        [HttpGet]
        [Route("TestingApi1")]
        public IHttpActionResult TestingApi1(string phone)
        {
            return Ok("good man is=" + phone);
        }


        string ResponseMessages(string code)
        {
            string msg = "";
            switch (code)
            {
                case "00":
                    msg= "Transaction Completed Successfully";
                    break;
                case "01":
                    msg= "Transaction Approved";
                    break;
                case "02":
                    msg= "Transaction Failed";
                    break;
                case "012":
                    msg= "User aborted Transaction";
                    break;
                case "020":
                    msg= "Invalid User Authentication";
                    break;
                case "021":
                    msg= "Transaction Pending";
                    break;
                case "022":
                    msg= "Invalid request";
                    break;
                case "023":
                    msg= "Service type or Merchant does not exist";
                    break;
                case "029":
                    msg= "Invalid bank code";
                    break;
                case "030":
                    msg= "Insufficient Balance";
                    break;
                case "031":
                    msg= "No funding Account";
                    break;
                case "032":
                    msg= "Invalid date format";
                    break;
                case "040":
                    msg= "Initial Request";
                    break;
                case "999":
                    msg= "Unknown error";
                    break;
            }
            return msg;
        }
    }
   
}
