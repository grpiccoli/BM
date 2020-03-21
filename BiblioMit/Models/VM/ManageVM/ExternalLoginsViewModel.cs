using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace BiblioMit.Models.ManageViewModels
{
    public class ExternalLoginsViewModel
    {
        public List<UserLoginInfo> CurrentLogins { get; } = new List<UserLoginInfo>();

        public List<AuthenticationScheme> OtherLogins { get; } = new List<AuthenticationScheme>();

        public bool ShowRemoveButton { get; set; }

        public string StatusMessage { get; set; }
    }
}
