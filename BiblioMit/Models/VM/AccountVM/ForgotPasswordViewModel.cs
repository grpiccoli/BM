using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }
}
