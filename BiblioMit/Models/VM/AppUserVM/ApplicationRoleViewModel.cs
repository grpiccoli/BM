using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class AppRoleViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
