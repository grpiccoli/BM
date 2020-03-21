using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiblioMit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BiblioMit.Areas.Identity.Pages.Account.Manage
{
    public class GenerateRecoveryCodesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<GenerateRecoveryCodesModel> _logger;
        private readonly IStringLocalizer<GenerateRecoveryCodesModel> _localizer;
        public GenerateRecoveryCodesModel(
            UserManager<AppUser> userManager,
            ILogger<GenerateRecoveryCodesModel> logger,
            IStringLocalizer<GenerateRecoveryCodesModel> localizer)
        {
            _localizer = localizer;
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public List<string> RecoveryCodes { get; } = new List<string>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false);
            if (!isTwoFactorEnabled)
            {
                var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
                throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false);
            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAwait(false);
            RecoveryCodes.AddRange(recoveryCodes);

            _logger.LogInformation(_localizer["User with ID '{UserId}' has generated new 2FA recovery codes."], userId);
            StatusMessage = "You have generated new recovery codes.";
            return RedirectToPage("./ShowRecoveryCodes");
        }
    }
}