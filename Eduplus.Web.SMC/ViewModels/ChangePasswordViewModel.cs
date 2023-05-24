namespace Eduplus.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPasword { get; set; }

    }
}