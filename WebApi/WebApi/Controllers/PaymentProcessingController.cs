using Eduplus.Domain.BurseryModule;
using Eduplus.Services;
using Eduplus.Services.UtilityServices;
using KS.AES256Encryption;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controllers
{

    public class PaymentProcessingController : ApiController
    {

        #region Etranzact Payoutlet Operations
        [HttpPost]
        [HttpGet]
        //[AcceptVerbs("POST","HEAD")]
        //[AcceptVerbs()]
        public async Task<HttpResponseMessage> ETranzactBankOutletPayData(string PAYEE_ID, string PAYMENTTYPE, string KEY)
        {

            string record="";
            
            bool chk = TemporalStudentsAccountsService.ValidateToken(KEY);
            if(chk==false)
            {
                record="PayeeName=N/A~Department=N/A~ProgrammeType=N/A~Programme=N/A~Session=N/A~PayeeID=N/A~Amount=N/A"
                                    + "~FeeStatus=Invalid Token~MatricNumber=N/A~PaymentType=N/A";
            }
            else
            {
                string pay = PAYEE_ID.Substring(0, 2);
                if(pay=="02")//Degree Program return Degree Api
                {
                    record = await GetPaymentData("https://smc.obonguniversity.edu.ng/Payments/SendEtranzactPayoutletPayInfo", PAYEE_ID, PAYMENTTYPE);
                    /*string url = "http://localhost:13302/Payments/SendEtranzactPayoutletPayInfo";
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("PayeeId",PAYEE_ID);
                    parameters.Add("PaymentType", PAYMENTTYPE);
                    

                    using (var httpClient = new HttpClient())
                    {
                        using (var content = new FormUrlEncodedContent(parameters))
                        {
                            content.Headers.Clear();
                            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                            var reply = await httpClient.PostAsync(url, content);

                            record = await reply.Content.ReadAsStringAsync();
                            
                        }
                    }*/


                }

                if(pay=="01")
                {
                    record = await GetPaymentData("https://smc.obonguniversity.edu.ng/Payments/SendEtranzactPayoutletPayInfo", PAYEE_ID, PAYMENTTYPE);
                }
                
            }
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(record, System.Text.Encoding.UTF8, "text/plain");
            return response;

        }

        async Task<string> GetPaymentData(string url,string payeeId,string paymentType)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("PayeeId", payeeId);
            parameters.Add("PaymentType", paymentType);


            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(parameters))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    var reply = await httpClient.PostAsync(url, content);

                    return await reply.Content.ReadAsStringAsync();

                }
            }
        }
        //[IPFiltering]
        [HttpPost]
        [HttpGet]
        public async Task<HttpResponseMessage> EtranzactWebConnectPayConfirmation(string RECEIPT_NO,string PAYMENT_CODE,string MERCHANT_CODE,
            string TRANS_AMOUNT,string TRANS_DATE, string TRANS_DESCR, string CUSTOMER_ID,string BANK_CODE,string BRANCH_CODE,
            string SERVICE_ID,string CUSTOMER_NAME,string CUSTOMER_ADDRESS, string TELLER_ID, string USERNAME,string PASSWORD,
            string BANK_NAME,string BRANCH_NAME,string CHANNEL_NAME,string PAYMENT_METHOD_NAME,string PAYMENT_CURRENCY,
            string TRANS_TYPE,string TRANS_FEE, string TYPE_NAME,string LEAD_BANK_CODE,string LEAD_BANK_NAME,string COL1,
            string COL2,string COL3,string COL4,string COL5)
        {
            string msg;
            string resp="";
            // try
            // {
            GateWaylogs gl = new GateWaylogs();
                gl.ReceiptNo = RECEIPT_NO;
                gl.Address = CUSTOMER_ADDRESS;
                gl.Amount = Convert.ToDouble(TRANS_AMOUNT);
                gl.BankCode = BANK_CODE;
                gl.BankName = BANK_NAME;
                gl.BranchCode = BRANCH_CODE;
                gl.BranchName = BRANCH_NAME;
                gl.Channelname = CHANNEL_NAME;
                gl.Description = TRANS_DESCR;
                gl.MerchantCode = MERCHANT_CODE;
                gl.Password = PASSWORD;
                gl.PayeeId = CUSTOMER_ID;
                gl.PayeeName = CUSTOMER_NAME;
                gl.PaymentCode = PAYMENT_CODE;
                gl.PaymentMethod = PAYMENT_METHOD_NAME;
                gl.ReceiptNo = RECEIPT_NO;
                gl.ServiceId = SERVICE_ID;
                gl.TellerId = TELLER_ID;
                gl.TransDate = Convert.ToDateTime(TRANS_DATE);
                gl.TransFee = TRANS_FEE;
                gl.TransType = TRANS_TYPE;
                gl.TypeName = TYPE_NAME;
            gl.LogDate = DateTime.UtcNow;
            gl.COL5 = COL5;
                msg = TemporalStudentsAccountsService.EtranzactPayoutLetPaymentSubmission(gl);
            if(msg=="True")
            {
                if (gl.COL5 == "Degree")
                {

                    string url = "https://smc.obonguniversity.edu.ng/Payments/SubmitPayoutletConfirmation";
                    //string url = "http://localhost:13302/Payments/SubmitPayoutletConfirmation";
                    resp = await SendWebPayoutletRecordsToInternals(url,
                        gl.Amount.ToString(), gl.Description, gl.TypeName, gl.PayeeId, gl.ReceiptNo);

                    
                }

                if (gl.COL5 == "JUPEB")
                {
                    string url = "https://smc.obonguniversity.edu.ng/Payments/SubmitPayoutletConfirmation";
                    //string url = "https://localhost:13302/Payments/SubmitPayoutletConfirmationn";
                    resp = await SendWebPayoutletRecordsToInternals(url,
                        gl.Amount.ToString(), gl.Description, gl.TypeName, gl.PayeeId, gl.ReceiptNo);
                }
            }
            if(resp=="SUCCESS")
            {
                HttpResponseMessage rs = new HttpResponseMessage();
                rs.Content = new StringContent("True", System.Text.Encoding.UTF8, "text/plain");
                return rs;
            }
            else
            {
                HttpResponseMessage rs = new HttpResponseMessage();
                rs.Content = new StringContent("false 3", System.Text.Encoding.UTF8, "text/plain");
                return rs;
            }
            
        }

        async Task<string> SendWebPayoutletRecordsToInternals(string url, string amount,string description,string paymentType,
            string transactionId,string transactionRef)
        {
            
            var parameters = new Dictionary<string, string>();
            parameters.Add("Amount", amount);
            parameters.Add("Description", description);
            parameters.Add("PaymentType", paymentType);
            parameters.Add("TransactionId", transactionId);
            parameters.Add("TransactionRef", transactionRef);

            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(parameters))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    
                    return await response.Content.ReadAsStringAsync();
                    
                }
            }
        }

        #endregion

        #region ETranzact BankIT API Operations

        
        [HttpPost]//Cancellation Man
        public async Task<IHttpActionResult> BankITPaymentConfirmation(string SUCCESS, string TERMINAL_ID, double AMOUNT, string TRANSACTION_ID)
        {


            string response = "";
            
            string urlParam = "";
            //string returnUrl="http://localhost:13302/Payments/PaymentNotification?q="+urlParam;
            string returnUrl = "https://smc.obonguniversity.edu.ng/Payments/PaymentNotification?q=" + urlParam;


            //Get way information
            
            GateWaylogs logs = new GateWaylogs();
            logs.LogDate = DateTime.UtcNow;
            logs.PayeeId = TRANSACTION_ID;
            //logs.PaymentCode = TRANSACTION_REF;
            //logs.COL5 = COL5;
            logs.Amount = AMOUNT;
            logs.TransDate = DateTime.UtcNow;
            
                response = "Transaction Canceled";
                logs.ServerResponse = response + " BankIT";

                TemporalStudentsAccountsService.SubmitWebPayLog(logs);
                System.Uri uri1 = new System.Uri(returnUrl + response);
                return Redirect(uri1);
            
           

        }

        
        [HttpPost]
        public async Task<IHttpActionResult> BankITPaymentConfirmation(string SUCCESS,string TERMINAL_ID,double AMOUNT,string TRANSACTION_ID,string TRANSACTION_REF,string COL5,string KEY, string FINAL_CHECKSUM,
            string DESCRIPTION)
        {

            
            string response="";
            string myKey;
            string urlParam = "";
            //string returnUrl="http://localhost:13302/Payments/PaymentNotification?q="+urlParam;
            string returnUrl = "https://smc.obonguniversity.edu.ng/Payments/PaymentNotification?q=" + urlParam;


            //Get way information
            string responseurl = "";
            string terminalId = "";
            string key = "";
            string logourl = "";
            string noturl = "";
            GateWaylogs logs = new GateWaylogs();
            logs.LogDate = DateTime.UtcNow;
            logs.PayeeId = TRANSACTION_ID;
            //logs.PaymentCode = TRANSACTION_REF;
            //logs.COL5 = COL5;
            logs.Amount = AMOUNT;
            logs.TransDate = DateTime.UtcNow;

            
            //Validate Token
            var ck = TemporalStudentsAccountsService.ValidateToken(KEY);
            if (ck == false)
            {
                response= "Invalid token returned by service. Contact ICT Directorate, Obong University";
                logs.ServerResponse = response+" BankIT";
                
                TemporalStudentsAccountsService.SubmitWebPayLog(logs);
                System.Uri uri1 = new System.Uri(returnUrl + response);
                return Redirect(uri1);
            }
            //Check for cancel transaction
            
            


            FetchGatewayInfo.GetGatewayInfomation(out responseurl, out logourl, out noturl, out terminalId, out key);
            //Compare checksum


            if (SUCCESS == "0")//Succesfull transaction, Validate Final checksum
            {
                string text = "0" + AMOUNT.ToString() + terminalId + TRANSACTION_ID + responseurl + key;

                //Validate checksum
                bool chk = DataEncryption.ValidateFinalCheckSum(FINAL_CHECKSUM, text);
                if (chk == false)
                {
                    response = "Invalid checksum. Contact ICT Directorate, Obong University";
                    logs.ServerResponse = response + " BankIT";
                    
                    
                    TemporalStudentsAccountsService.SubmitWebPayLog(logs);
                    System.Uri uri1 = new System.Uri(returnUrl + response);
                    return Redirect(uri1);
                }
                
                if(COL5!="Degree")//sEND TO DEGREE APP
                {
                    //string url = "http://localhost:13302/Payments/PaymentNotification?q=";
                    string url = "https://smc.obonguniversity.edu.ng/Payments/PaymentConfirmation?q=";
                    response = await SendBankITPayRecordToInternals(url, AMOUNT.ToString(), DESCRIPTION,
                        COL5, TRANSACTION_ID, TRANSACTION_REF);
                    logs.ServerResponse = response + " BankIT";
                    
                    TemporalStudentsAccountsService.SubmitWebPayLog(logs);
                    
                    TemporalStudentsAccountsService.SubmitWebPayLog(logs);
                    System.Uri uri1 = new System.Uri(returnUrl + response);
                    return Redirect(uri1);
                }
                
            }

            logs.ServerResponse = ETransactionResponse(SUCCESS);
            TemporalStudentsAccountsService.SubmitWebPayLog(logs);
            System.Uri uri = new System.Uri(returnUrl+logs.ServerResponse);
                return Redirect(uri);
       
        }

        public async Task<string> SendBankITPayRecordToInternals(string url, string amount,string description,string col5,string transactionId,
            string transRef)
        {
            
            var parameters = new Dictionary<string, string>();
            parameters.Add("AMOUNT", amount);
            parameters.Add("DESCRIPTION", description);
            
            parameters.Add("COL5", col5);
            parameters.Add("TRANSACTION_ID", transactionId);
            parameters.Add("TRANSACTION_REF", transRef);
            //parameters.Add("Token",)

            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(parameters))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    return await response.Content.ReadAsStringAsync();

                }
            }
        }
        #region General Payment Processing
        
        #endregion

        string ETransactionResponse(string code)
        {
            string msg = "";
            switch (code)
            {
                case "CA":
                    msg = "Transaction canceled by user";
                    break;
                case "-1":
                    msg = "Transaction timeout or invalid parameters or unsuccessful transaction in the case of Query History";
                    break;
                case "Z":
                    msg = "Pending Transaction";
                    break;
                case "0":
                    msg = "Success";
                    break;
                case "1":
                    msg = "Destination Card Not Found";
                    break;
                case "2":
                    msg = "Card Number Not Found";
                    break;
                case "3":
                    msg = "Invalid Card PIN";
                    break;
                case "4":
                    msg = "Card Expiration Incorrect";
                    break;
                case "5":
                    msg = "Insufficient balance";
                    break;
                case "6":
                    msg = "Spending Limit Exceeded";
                    break;
                case "7":
                    msg = "Internal System Error Occurred, please contact the service provider";
                    break;
                case "8":
                    msg = "Financial Institution cannot authorize transaction, Please try later";
                    break;
                case "9":
                    msg = "PIN tries Exceeded";
                    break;
                case "10":
                    msg = "Card has been locked";
                    break;
                case "11":
                    msg = "Invalid Terminal Id";
                    break;
                case "12":
                    msg = "Payment Timeout";
                    break;
                case "13":
                    msg = "Destination card has been locked";
                    break;
                case "14":
                    msg = "Card has expired";
                    break;
                case "15":
                    msg = "PIN change required";
                    break;
                case "16":
                    msg = "Invalid Amount";
                    break;
                case "17":
                    msg = "Card has been disabled";
                    break;
                case "18":
                    msg = "Unable to credit this account immediately, credit will be done later";
                    break;
                case "19":
                    msg = "Transaction not permitted on terminal";
                    break;
                case "20":
                    msg = "Exceeds withdrawal frequency";
                    break;
                case "21":
                    msg = "Destination Card has expired";
                    break;
                case "22":
                    msg = "Destination Card Disabled";
                    break;
                case "23":
                    msg = "Source Card Disabled";
                    break;
                case "24":
                    msg = "Invalid Bank Account";
                    break;
                case "25":
                    msg = "Insufficient Balance";
                    break;
                case "1002":
                    msg = "CHECKSUM / FINAL_CHECKSUM error";
                    break;
                case "100":
                    msg = "Duplicate session id";
                    break;
                case "200":
                    msg = "Invalid client id";
                    break;
                case "300":
                    msg = "Invalid mac";
                    break;
                case "400":
                    msg = "Expired session";
                    break;
                case "500":
                    msg = "You have entered an account number that is not tied to your phone number with bank.Pls contact your bank for assistance";
                    break;
                case "600":
                    msg = "Expired session";
                    break;
                case "700":
                    msg = "Security Violation Please contact support@etranzact.com";
                    break;
                case "800":
                    msg = "Invalid esa code";
                    break;
                case "900":
                    msg = "Transaction limit exceeded";
                    break;

            }

            return msg;
        }
        #endregion


    }
}
