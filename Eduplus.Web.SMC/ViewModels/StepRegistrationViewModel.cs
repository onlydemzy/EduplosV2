using System.ComponentModel.DataAnnotations;

namespace Eduplos.Web.ViewModels
{
    public class Step1RegistrationViewModel
    {

        [Required]
        public string Surname { get; set; }
        [Required]
        public string Othernames { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        
        public string Phone { get; set; }
        public string ProgrammeType { get; set; }
              
        
    }
}