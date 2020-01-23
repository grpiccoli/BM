using Microsoft.AspNetCore.Identity;

namespace BiblioMit.Models
{
    public class AppUserRole : IdentityUserRole<string>
    {
        public string RoleAssigner { get; set; }
    }
}
