using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BiblioMit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace BiblioMit.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IStringLocalizer<LogoutModel> _localizer;
        public LogoutModel(
            SignInManager<AppUser> signInManager, 
            ILogger<LogoutModel> logger,
            IStringLocalizer<LogoutModel> localizer)
        {
            _localizer = localizer;
            _signInManager = signInManager;
            _logger = logger;
        }

        public static void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(Uri returnUrl = null)
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation(_localizer["User logged out."]);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl.AbsoluteUri);
            }
            else
            {
                return Page();
            }
        }
    }
}