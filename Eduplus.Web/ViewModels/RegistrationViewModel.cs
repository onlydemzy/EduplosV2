namespace Eduplus.Web.ViewModels
{
    public class RegistrationViewModel
    {
        public string PersonId { get; set; }
        public string DepartmentCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Pssword { get; set; }
        public string ConfirmPassword { get; set; }
        public string ReturnUrl { get; set; }
        public string Role { get; set; }
    }
}