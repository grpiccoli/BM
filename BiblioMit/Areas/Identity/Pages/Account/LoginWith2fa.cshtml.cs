using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginWith2faModel> _logger;
        private readonly IStringLocalizer<LoginWith2faModel> _localizer;
        public LoginWith2faModel(
            SignInManager<AppUser> signInManager, 
            ILogger<LoginWith2faModel> logger,
            IStringLocalizer<LoginWith2faModel> localizer
            )
        {
            _localizer = localizer;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public LoginWith2faInputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public Uri ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, Uri returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException(_localizer[$"Unable to load two-factor authentication user."]);
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, Uri returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? new Uri("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException(_localizer[$"Unable to load two-factor authentication user."]);
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace("-", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine).ConfigureAwait(false);

            if (result.Succeeded)
            {
                _logger.LogInformation(_localizer["User with ID '{UserId}' logged in with 2fa."], user.Id);
                return LocalRedirect(returnUrl.AbsoluteUri);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning(_localizer["User with ID '{UserId}' account locked out."], user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning(_localizer["Invalid authenticator code entered for user with ID '{UserId}'."], user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return Page();
            }
        }
    }
    public class LoginWith2faInputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }
}
