using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BiblioMit.Models;
using BiblioMit.Models.AccountViewModels;
using BiblioMit.Services;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace BiblioMit.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<AccountController> _localizer;
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IStringLocalizer<AccountController> localizer
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
            _localizer = localizer;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Uri returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);
            ViewData["ReturnUrl"] = returnUrl?.AbsoluteUri;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, Uri returnUrl = null)
        {
            if (model == null) return NotFound();
            ViewData["ReturnUrl"] = returnUrl?.ToString();
            if (ModelState.IsValid)
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false))
                    {
                        ModelState.AddModelError(string.Empty,
                                      "Debes confirmar tu correo electrónico para poder iniciar sesión.");
                        return View(model);
                    }
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager
                    .PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false)
                    .ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(_localizer["User logged in."]);
                    return RedirectToLocal(returnUrl.ToString());
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl = returnUrl.AbsoluteUri, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(_localizer["User account locked out."]);
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, Uri returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);

            if (user == null)
            {
                throw new ApplicationException(_localizer[$"Unable to load two-factor authentication user."]);
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl?.AbsoluteUri;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, Uri returnUrl = null)
        {
            if (model == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace("-", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine).ConfigureAwait(false);

            if (result.Succeeded)
            {
                _logger.LogInformation(_localizer["User with ID {UserId} logged in with 2fa."], user.Id);
                return RedirectToLocal(returnUrl?.AbsoluteUri);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning(_localizer["User with ID {UserId} account locked out."], user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning(_localizer["Invalid authenticator code entered for user with ID {UserId}."], user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(Uri returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException(_localizer[$"Unable to load two-factor authentication user."]);
            }

            ViewData["ReturnUrl"] = returnUrl?.AbsoluteUri;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, Uri returnUrl = null)
        {
            if (model == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException(_localizer[$"Unable to load two-factor authentication user."]);
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode).ConfigureAwait(false);

            if (result.Succeeded)
            {
                _logger.LogInformation(_localizer["User with ID {UserId} logged in with a recovery code."], user.Id);
                return RedirectToLocal(returnUrl?.AbsoluteUri);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(_localizer["User with ID {UserId} account locked out."], user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning(_localizer["Invalid recovery code entered for user with ID {UserId}"], user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(Uri returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl?.AbsoluteUri;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Validate()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, Uri returnUrl = null)
        {
            if (model == null) return NotFound();
            ViewData["ReturnUrl"] = returnUrl?.AbsoluteUri;
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    MemberSince = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(_localizer["User created a new account with password."]);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl).ConfigureAwait(false);

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(_localizer["User created a new account with password."]);
                    return RedirectToAction("Validate","Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation(_localizer["User logged out."]);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, Uri returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl = returnUrl?.AbsoluteUri });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(Uri returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true).ConfigureAwait(false);
            if (result.Succeeded)
            {
                _logger.LogInformation(_localizer["User logged in with {Name} provider."], info.LoginProvider);
                return RedirectToLocal(returnUrl?.AbsoluteUri);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var id = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var externalLogin = new ExternalLoginViewModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Name = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    Last = info.Principal.FindFirstValue(ClaimTypes.Surname)
                };
                if (info.LoginProvider == "Facebook")
                {
                    externalLogin.ProfileImageUrl = new Uri($"https://graph.facebook.com/{id}/picture?type=large&w‌​idth=320&height=320");
                }
                else if (info.LoginProvider == "Google")
                {
                    var url = new Uri($"https://picasaweb.google.com/data/entry/api/user/{id}?alt=json");
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage json = await hc.GetAsync(url).ConfigureAwait(false);
                    string final = Encoding.UTF8.GetString(await json.Content.ReadAsByteArrayAsync().ConfigureAwait(false)).Trim().Trim('\0');
                    var data = JObject.Parse(final);
                    externalLogin.ProfileImageUrl = new Uri((string)data["entry"]["gphoto$thumbnail"]["$t"]+"?sz=320");
                }
                else if (info.LoginProvider == "LinkedIn")
                {
                    var token = info.AuthenticationTokens.Single(x => x.Name == "access_token").Value;
                    var url = new Uri($"https://api.linkedin.com/v1/people/~?oauth2_access_token={token}&format=json");
                    HttpClient hc = new HttpClient();
                    HttpResponseMessage json = await hc.GetAsync(url).ConfigureAwait(false);
                    string final = Encoding.UTF8.GetString(await json.Content.ReadAsByteArrayAsync().ConfigureAwait(false)).Trim().Trim('\0');
                    var data = JObject.Parse(final);
                    externalLogin.Name = (string)data["firstName"];
                    externalLogin.Last = (string)data["lastName"];
                    url = new Uri($"https://api.linkedin.com/v1/people/~/picture-urls::(original)?oauth2_access_token={token}");
                    hc = new HttpClient();
                    HttpResponseMessage response = await hc.GetAsync(url).ConfigureAwait(false);
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    externalLogin.ProfileImageUrl = new Uri(doc.DocumentNode.SelectSingleNode("//picture-url[@key='original']").InnerHtml);
                }
                else if (info.LoginProvider == "Microsoft")
                {
                    var token = info.AuthenticationTokens.Single(x => x.Name == "access_token").Value;
                    var bytes = await new HttpClient().GetStreamWithAuthAsync(token, new Uri("https://graph.microsoft.com/v1.0/{id}/photo/$value")).ConfigureAwait(false);
                    externalLogin.ProfileImageUrl = new Uri("data&colon;image/png;base64," + Convert.ToBase64String(bytes));
                }
                else if (info.LoginProvider == "Twitter")
                {
                    var oAuthConsumerKey = _configuration.GetValue<string>("T_KEY");
                    var oAuthConsumerSecret = _configuration.GetValue<string>("T_SECRET");
                    var oAuthUrl = new Uri("https://api.twitter.com/oauth2/token");
                    var screenname = externalLogin.Name;
                    var authHeaderFormat = "Basic {0}";
                    var authHeader = string.Format(CultureInfo.InvariantCulture, authHeaderFormat,
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(oAuthConsumerKey) + ":" +
                        Uri.EscapeDataString((oAuthConsumerSecret)))
                    ));
                    var postBody = "grant_type=client_credentials";
                    HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
                    authRequest.Headers.Add("Authorization", authHeader);
                    authRequest.Method = "POST";
                    authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    using (Stream stream = authRequest.GetRequestStream())
                    {
                        byte[] content = Encoding.ASCII.GetBytes(postBody);
                        stream.Write(content, 0, content.Length);
                    }
                    authRequest.Headers.Add("Accept-Encoding", "gzip");
                    WebResponse authResponse = authRequest.GetResponse();
                    TwitAuthenticateResponse twitAuthResponse;
                    using (authResponse)
                    {
                        using var reader = new StreamReader(authResponse.GetResponseStream());
                        var objectText = reader.ReadToEnd();
                        twitAuthResponse = JsonConvert.DeserializeObject<TwitAuthenticateResponse>(objectText);
                    }

                    // Do the avatar
                    var avatarFormat = "https://api.twitter.com/1.1/users/show.json?screen_name={0}";
                    var avatarUrl = new Uri(string.Format(CultureInfo.InvariantCulture, avatarFormat, screenname));
                    HttpWebRequest avatarRequest = (HttpWebRequest)WebRequest.Create(avatarUrl);
                    var timelineHeaderFormat = "{0} {1}";
                    avatarRequest.Headers.Add(  "Authorization",
                                                string.Format(CultureInfo.InvariantCulture, timelineHeaderFormat, 
                                                                twitAuthResponse.TokenType,
                                                                twitAuthResponse.AccessToken));
                    avatarRequest.Method = "Get";
                    WebResponse timeLineResponse = avatarRequest.GetResponse();

                    var avatarJson = string.Empty;
                    using (authResponse)
                    {
                        using var reader = new StreamReader(timeLineResponse.GetResponseStream());
                        avatarJson = reader.ReadToEnd();
                    }
                    
                    externalLogin.ProfileImageUrl = new Uri(info.Principal.FindFirstValue(avatarJson));
                }
                return View("ExternalLogin", externalLogin);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, Uri returnUrl = null)
        {
            if (model == null) return NotFound();
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
                if (info == null)
                {
                    throw new ApplicationException(_localizer["Error loading external login information during confirmation."]);
                }
                var user = new AppUser { UserName = model.Email, Name = model.Name, Last = model.Last, Email = model.Email, ProfileImageUrl = model.ProfileImageUrl };
                var result = await _userManager.CreateAsync(user).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                        _logger.LogInformation(_localizer["User created an account using {Name} provider."], info.LoginProvider);
                        return RedirectToLocal(returnUrl?.AbsoluteUri);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (model == null) return NotFound();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>").ConfigureAwait(false);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException(_localizer["A code must be supplied for password reset."]);
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
    public static class AccountControllerExtensions
    {
        public static async Task<byte[]> GetStreamWithAuthAsync(this HttpClient client, string accessToken, Uri endpoint)
        {
            if (client == null) return null;
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using var response = await client.GetAsync(endpoint).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return bytes;
            }
            else
                return null;
        }
    }
    public class TwitAuthenticateResponse
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
    }
}
