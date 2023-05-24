using Eduplus.Domain.BurseryModule;
using Eduplus.Services;
using KS.AES256Encryption;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Security;

namespace WebApi.Controllers
{
    
    public class PaymentProcessingController1 : ApiController
    {
        
        [HttpPost]
        [HttpGet]
        //[AcceptVerbs("POST","HEAD")]
        //[AcceptVerbs()]
        public HttpResponseMessage ETranzactBankOutletPayData(string PAYEE_ID, string PAYMENTTYPE, string KEY)
        {

            string record = TemporalStudentsAccountsService.FetchInvoiceForEtranzactPayOutlet(PAYEE_ID, PAYMENTTYPE, KEY);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(record, System.Text.Encoding.UTF8, "text/plain");
            return response;
            
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
            gl.COL5 = COL5;
                msg = TemporalStudentsAccountsService.EtranzactPayoutLetPaymentSubmission(gl);
            if(msg=="True")
            {
                if (gl.COL5 == "Degree")
                {
                    string url = "https://smc.obonguniversity.edu.ng/Payments/GetPayoutletConfirmation";
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("Amount", gl.Amount.ToString());
                    parameters.Add("Description", gl.Description);
                    parameters.Add("PaymentType", gl.TypeName);
                    parameters.Add("TransactionId", gl.PayeeId);
                    parameters.Add("TransactionRef", gl.ReceiptNo);


                    using (var httpClient = new HttpClient())
                    {
                        using (var content = new FormUrlEncodedContent(parameters))
                        {
                            content.Headers.Clear();
                            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                            HttpResponseMessage response1 = await httpClient.PostAsync(url, content);
                            var response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("True", System.Text.Encoding.UTF8, "text/plain");
                            return response;

                            //return await response.Content.ReadAsAsync<TResult>();
                        }
                    }
                }

                if (gl.COL5 == "JUPEB")
                {
                    string url = "https://sbs.obonguniversity.edu.ng/Payments/GetPayoutletConfirmation";
                    var parameters = new Dictionary<string, string>();
                    parameters.Add("Amount", gl.Amount.ToString());
                    parameters.Add("Description", gl.Description);
                    parameters.Add("PaymentType", gl.TypeName);
                    parameters.Add("TransactionId", gl.PayeeId);
                    parameters.Add("TransactionRef", gl.ReceiptNo);
                    //parameters.Add("Token",)

                    using (var httpClient = new HttpClient())
                    {
                        using (var content = new FormUrlEncodedContent(parameters))
                        {
                            content.Headers.Clear();
                            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                            HttpResponseMessage response1 = await httpClient.PostAsync(url, content);
                            var response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("True", System.Text.Encoding.UTF8, "text/plain");
                            return response;

                            //return await response.Content.ReadAsAsync<TResult>();
                        }
                    }
                }
                else
                {
                    
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("false 3", System.Text.Encoding.UTF8, "text/plain");
                    return response;
                }
            }
            
            //}
            /*catch(Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("false 3", System.Text.Encoding.UTF8, "text/plain");
                return response;
            }*/
            /*GateWaylogs gl = new GateWaylogs();
            gl.Address = form.Get("ADDRESS");
            gl.Amount = form.Get("TRANS_AMOUNT");
            gl.BankCode = form.Get("BANK_CODE");
            gl.BankName = form.Get("BANK_NAME");
            gl.BranchCode = form.Get("BRANCH_CODE");
            gl.BranchName = form.Get("RRANCH_NAME");
            gl.Channelname = form.Get("CHANNEL_NAME");
            gl.Description = form.Get("DESCR");
            gl.MerchantCode = form.Get("MERCHANT_CODE");
            gl.Password = form.Get("PASSWORD");
            gl.PayeeId = form.Get("CUSTOMER_ID");
            gl.PayeeName = form.Get("CUSTOMER_NAME");
            gl.PaymentCode = form.Get("PAYMENT_CODE");
            gl.PaymentMethod = form.Get("YAYMENT_METHOD");
            gl.ReceiptNo = form.Get("RECEIPT_NO");
            gl.ServiceId = form.Get("SERVICE_ID");
            gl.TellerId = form.Get("TELLER_ID");
            gl.TransDate = Convert.ToDateTime(form.Get("TRANS_DATE"));
            gl.TransFee = form.Get("TRANS_FEE");
            gl.TransType = form.Get("TRANS_TYPE");
            gl.TypeName = form.Get("TYPE_NAME");
            */
            
        }
        /*[HttpGet]
        
        [AcceptVerbs("GET", "HEAD")]
        public HttpResponseMessage ETranzactBankOutletPayData()
        {

            string record = TemporalStudentsAccountsService.FetchInvoiceForEtranzactPayOutlet("", "","");
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(record, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }*/

        /* [AcceptVerbs("POST","HEAD")]
         public async Task SendData()
         {
             string url = "http://demo.etranzact.com/bankIT/";
             var parameters = new Dictionary<string, string>();
             parameters.Add("code", "yes");
             parameters.Add("client_secret", "CLIENT_SECRET");
             parameters.Add("client_id", "CLIENT_APP_KEY");
             parameters.Add("grant_type", "authorization_code");

             using (var httpClient = new HttpClient())
             {
                 using (var content = new FormUrlEncodedContent(parameters))
                 {
                     content.Headers.Clear();
                     content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                     httpClient.SendAsync(.UploadString("http://www.website.com/page/", "POST", data);
                     HttpResponseMessage response = await httpClient.PostAsync(url, content);

                     //return await response.Content.ReadAsAsync<TResult>();
                 }
             }
         }*/


    }
}
