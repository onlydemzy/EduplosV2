using Eduplus.Services.Contracts;
using KS.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public string SendMail(string receiverFullname,string receiverMail,string msgBody,string subject)
        {

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("info@korrhsolutions.com");
            mailMessage.To.Add("demzy247@gmail.com");
            mailMessage.Subject = "Take mail";
            mailMessage.Body = "This is test email";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.zoho.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("info@korrhsolutions", "NETman123P^$$");
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
            return "Ok";

        }
        string HtmlMessageBody(string recieverFullname,string content)
        {
            string bd = @"<img src=cid:imageID /><h3>Dear "+recieverFullname+",</h3>"+
                content+"\r\n"+"<strong>-Registrar</strong>";
            return bd;
        }
        

    }


}
         

