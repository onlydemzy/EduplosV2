namespace Eduplos.Services.Contracts
{
    public interface ICommunicationService
    {
        string SendMail(string receiverFullname, string receiverMail, string msgBody, string subject);
         
    }
}
