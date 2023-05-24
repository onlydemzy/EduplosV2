using System.ComponentModel.DataAnnotations;

namespace Eduplus.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set;}
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Fullname { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}