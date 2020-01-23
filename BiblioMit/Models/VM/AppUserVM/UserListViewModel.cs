using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblioMit.Models
{
    public class UserListViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Permisos de usuario")]
        public List<SelectListItem> UserClaims { get; set; }

        [Display(Name = "Rol de usuario")]
        public string RoleName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MMMM/yyyy}")]
        public DateTime MemberSince { get; set; }

        public int UserRating { get; set; }

        public string ProfileImageUrl { get; set; }
    }
}
