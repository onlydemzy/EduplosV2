using Eduplus.Services.Contracts;
using KS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

namespace Eduplus.Services.Implementations
{
    public class CommunicationService:ICommunicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommunicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public string SendMail(string receiverMail,string msgBody,string subject)
        {

            var userData = _unitOfWork.UserDataRepository.GetAll().First();
            

                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(userData.AppEmail);
                message.To.Add(new MailAddress(receiverMail));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = msgBody;
                smtp.Port = 25;
                smtp.Host = userData.EmailDomain; //for gmail host  
                smtp.EnableSsl = userData.EnableSsl;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(userData.AppEmail, userData.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return "Ok";
           

        }

        public void SendMailViaApi(string receiverMail, string msgBody, string subject)
        {
            var userData = _unitOfWork.UserDataRepository.GetAll().First();
            HttpClient client = new HttpClient();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("fromAddress", userData.EmailDomain);
            parameters.Add("toAddress", receiverMail);
            parameters.Add("subject", subject);
            parameters.Add("content", msgBody);
            parameters.Add("mailFormat", "Html");
            HttpResponseMessage response = null;

            response=client.PutAsJsonAsync(userData.EmailDomain, parameters).Result;
            var chk = response.Content.ToString();
        }

    }


}
         

