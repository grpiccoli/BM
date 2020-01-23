using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BiblioMit.Models
{
    public class AppRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IPAddress { get; set; }
        public virtual ICollection<IdentityUserRole<string>> Users { get; } = new List<IdentityUserRole<string>>();
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();
    }
}