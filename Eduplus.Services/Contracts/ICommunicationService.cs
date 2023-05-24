namespace Eduplus.Services.Contracts
{
    public interface ICommunicationService
    {
        string SendMail(string receiverMail, string msgBody, string subject);
        void SendMailViaApi(string receiverMail, string msgBody, string subject);
    }
}
