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
    public class LoginWithRecoveryCodeModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginWithRecoveryCodeModel> _logger;
        private readonly IStringLocalizer<LoginWithRecoveryCodeModel> _localizer;
        public LoginWithRecoveryCodeModel(
            SignInManager<AppUser> signInManager, 
            ILogger<LoginWithRecoveryCodeModel> logger,
            IStringLocalizer<LoginWithRecoveryCodeModel> localizer)
        {
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
        }

        [BindProperty]
        public LoginWithRecoveryCodeInputModel Input { get; set; }

        public Uri ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(Uri returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException(_localizer[$"Unable to load two-factor authentication user."]);
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Uri returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException(_localizer[$"Unable to load two-factor authentication user."]);
            }

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode).ConfigureAwait(false);

            if (result.Succeeded)
            {
                _logger.LogInformation(_localizer["User with ID '{UserId}' logged in with a recovery code."], user.Id);
                return LocalRedirect(returnUrl?.AbsoluteUri ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(_localizer["User with ID '{UserId}' account locked out."], user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning(_localizer["Invalid recovery code entered for user with ID '{UserId}' "], user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return Page();
            }
        }
    }
    public class LoginWithRecoveryCodeInputModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}
