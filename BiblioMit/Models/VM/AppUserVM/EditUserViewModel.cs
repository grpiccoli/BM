using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [Display(Name = "Permisos de Usuario")]
        public List<SelectListItem> UserClaims { get; } = new List<SelectListItem>();

        public List<SelectListItem> AppRoles { get; } = new List<SelectListItem>();

        [Display(Name = "Rol")]
        public string AppRoleId { get; set; }
    }
}
