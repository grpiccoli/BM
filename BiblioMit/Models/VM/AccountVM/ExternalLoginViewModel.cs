using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Display(Name = "Nombre(s)")]
        [RegularExpression(@"^.*$")]
        public string Name { get; set; }

        [Display(Name = "Apellido(s)")]
        [RegularExpression(@"^.*$")]
        public string Last { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Url]
        [Display(Name = "Imagen")]
        public string ProfileImageUrl { get; set; }
    }
}
