using System.ComponentModel.DataAnnotations;

namespace Eduplus.Web.ViewModels
{
    public class LoginModel
    {
        [Required]
        
        public string Username { get; set; }
        [Required]
      
        public string Password { get; set; }
       
        //public string Department { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}