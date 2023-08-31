using Eduplus.Services.Contracts;
using KS.Core;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Windows;

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

            var userData = _unitOfWork.UserDataRepository.GetAll().First();
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(userData.AppEmail,userData.Sender);
            mailMessage.To.Add(receiverMail);
            mailMessage.Subject = subject;
           // mailMessage.Body =HtmlMessageBody(receiverFullname,msgBody);
            mailMessage.IsBodyHtml = true;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(HtmlMessageBody(receiverFullname, msgBody), null, "text/html");

            //Add Image
            var strem = new MemoryStream(userData.Regbanner);
             
            LinkedResource theEmailImage = new LinkedResource(strem);
            theEmailImage.ContentId = "imageID";

            //Add the Image to the Alternate view
            htmlView.LinkedResources.Add(theEmailImage);

            //Add view to the Email Message
            mailMessage.AlternateViews.Add(htmlView);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(userData.AppEmail, userData.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = userData.EmailDomain;
            smtpClient.Port = userData.Port;
            smtpClient.EnableSsl = true;
             
             smtpClient.Send(mailMessage);
                return "Ok";
             
             
        }
        string HtmlMessageBody(string recieverFullname,string content)
        {
          
            string bd = $@"<html>
                            <body>
                                <table width=""70%"">
                                <tr>
                                    <td><img src = cid:imageID style=""max-width:700px;max-height:200px"";/></td>
                                </tr>
                                <tr>
                                    <td><p>
                                        <h2>Dear {recieverFullname},</h2>
                                        {content}.
                                        <h3>-Registrar</h3>
                                        </p>
                                    <td>
                                </tr>
                                <footer>
                                    <tr>
                                        <td><hr/><strong>Note:</strong> This mail was sent to you as a result of an action you took using Eduplos portal used by the above
                                        named institution. If you are sure you did not initiate anything from the portal, please ignore this message.</br>
                                        For further enquiries reach us on info@korrhsolutions.com or visit us on https://korrhsolutions.com. Thank you.</td>
                                    </tr>
                                </footer>
                                </table>
                             </body>
                           </html>";
;                            
            return bd;
        }
        

    }


}
         

