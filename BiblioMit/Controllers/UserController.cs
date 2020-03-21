using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BiblioMit.Data;
using Microsoft.Extensions.Localization;

namespace BiblioMit.Controllers
{
    [Authorize(Policy = "Usuarios")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger _logger;
        private readonly IStringLocalizer<UserController> _localizer;

        public UserController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ILogger<AccountController> logger,
            IStringLocalizer<UserController> localizer
            )
        {
            _localizer = localizer;
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            model = _userManager.Users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                Email = u.Email,
                UserRating = u.Rating,
                MemberSince = u.MemberSince,
                ProfileImageUrl = u.ProfileImageUrl,
                RoleName = u.Roles.Any() ? _context.AppUser.Where(r => r.Id == u.Roles.FirstOrDefault().RoleId).SingleOrDefault().Name : "Estándar"
            }).ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult AddUser()
        {
            UserViewModel model = new UserViewModel();
            model.UserClaims.AddRange(ClaimData.UserClaims.Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            }).ToList());
            model.AppRoles.AddRange(_roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList());
            return PartialView("_AddUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                AppUser applicationUser = await _userManager.FindByEmailAsync(user.Email).ConfigureAwait(false);
                List<SelectListItem> userClaims = model.UserClaims.Where(c => c.Selected).ToList();
                foreach (var claim in userClaims)
                {
                    applicationUser.Claims.Add(new IdentityUserClaim<string>
                    {
                        ClaimType = claim.Value,
                        ClaimValue = claim.Value
                    });
                }
                if (result.Succeeded)
                {
                    AppRole applicationRole = await _roleManager.FindByIdAsync(model.AppRoleId).ConfigureAwait(false);
                    if(applicationRole != null)
                    {
                        IdentityResult identityResult = await _userManager.AddToRoleAsync(user, applicationRole.Name).ConfigureAwait(false);
                        if(identityResult.Succeeded)
                        {
                            _logger.LogInformation(_localizer["Se ha añadido un nuevo usuario exitosamente."]);
                            return RedirectToAction("Index");
                        }
                    }
                }
                AddErrors(result);
            }
            return RedirectToAction("_AddUser");
            //return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
//        [Authorize(Policy = "Usuarios")]
        [HttpGet]
        [Authorize(Roles = "Administrador,Editor")]
        public async Task<IActionResult> EditUser(string id)
        {
            EditUserViewModel model = new EditUserViewModel();
            model.AppRoles.AddRange(_roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList());
            if (!string.IsNullOrEmpty(id))
            {
                AppUser applicationUser = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                if (applicationUser != null)
                {
                    model.Email = applicationUser.Email;
                    var claims = await _userManager.GetClaimsAsync(applicationUser).ConfigureAwait(false);
                    model.UserClaims.AddRange(ClaimData.UserClaims.Select(c => new SelectListItem
                    {
                        Text = c,
                        Value = c,
                        Selected = claims.Any(x => x.Value == c)
                    }).ToList());
                    var roles = await _userManager.GetRolesAsync(applicationUser).ConfigureAwait(false);
                    var role = roles.Single();
                    if (!string.IsNullOrEmpty(role))
                    {
                        string roleId = _roleManager.Roles.SingleOrDefault(r => r.Name == role)?.Id;
                        if (!string.IsNullOrEmpty(roleId))
                        {
                            model.AppRoleId = roleId;
                        }
                    }
                }
                else
                {
                    model.UserClaims.AddRange(ClaimData.UserClaims.Select(c => new SelectListItem
                    {
                        Text = c,
                        Value = c
                    }).ToList());
                }
            }
            return PartialView("_EditUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Editor")]
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model)
        {
            if (model == null) return NotFound();
            if (ModelState.IsValid)
            {
                AppUser applicationUser = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                if (applicationUser != null && applicationUser.Email != "adminmit@bibliomit.cl")
                {
                    applicationUser.Email = model.Email;
                    List<SelectListItem> userClaims = new List<SelectListItem>();
                    //try
                    //{
                        IList<Claim> claims = await _userManager.GetClaimsAsync(applicationUser).ConfigureAwait(false);
                        userClaims = model.UserClaims
                                        .Where(c => c.Selected && !claims
                                            .Any(u => u.Value == c.Value))
                                            .ToList();
                        List<Claim> userRemoveClaims = claims
                            .Where(c => model
                                        .UserClaims
                                        .Any(u => u.Value == c.Value && !u.Selected))
                            .ToList();
                        foreach (Claim claim in userRemoveClaims)
                        {
                            var idResult = await _userManager.RemoveClaimAsync(applicationUser, claim)
                                .ConfigureAwait(false);
                        if (idResult.Succeeded) continue;
                        }
                    //}
                    //catch
                    //{
                    //    userClaims = model.UserClaims.Where(u => u.Selected).ToList();
                    //}
                    foreach (var claim in userClaims)
                    {
                        applicationUser.Claims.Add(new IdentityUserClaim<string>
                        {
                            ClaimType = claim.Value,
                            ClaimValue = claim.Value
                        });
                    }
                    IdentityResult result = await _userManager.UpdateAsync(applicationUser).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        //try
                        //{
                            string existingRole = _userManager.GetRolesAsync(applicationUser).Result.Single();
                            string existingRoleId = _roleManager.Roles.Single(r => r.Name == existingRole).Id;
                            if (existingRoleId != model.AppRoleId)
                            {
                                IdentityResult roleResult = await _userManager.RemoveFromRoleAsync(applicationUser, existingRole).ConfigureAwait(false);
                                if (roleResult.Succeeded)
                                {
                                    AppRole applicationRole = await _roleManager.FindByIdAsync(model.AppRoleId).ConfigureAwait(false);
                                    if (applicationRole != null)
                                    {
                                        IdentityResult newRoleResult = await _userManager.AddToRoleAsync(applicationUser, applicationRole.Name).ConfigureAwait(false);
                                        if (newRoleResult.Succeeded)
                                        {
                                            return RedirectToAction("Index");
                                        }
                                    }
                                }
                            }
                        //}
                        //catch
                        //{
                        //    AppRole applicationRole = await _roleManager.FindByIdAsync(model.AppRoleId).ConfigureAwait(false);
                        //    if (applicationRole != null)
                        //    {
                        //        IdentityResult newRoleResult = await _userManager.AddToRoleAsync(applicationUser, applicationRole.Name).ConfigureAwait(false);
                        //        if (newRoleResult.Succeeded)
                        //        {
                        //            return RedirectToAction("Index");
                        //        }
                        //    }
                        //}
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUser(string id)
        {            
            string name = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                AppUser applicationUser = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                if (applicationUser != null)
                {
                    name = applicationUser.Email;
                }
            }
            return PartialView("_DeleteUser", name);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUser(string id, IFormCollection form)
        {
            if (!string.IsNullOrEmpty(id) && form != null)
            {
                AppUser applicationUser = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                if (applicationUser != null && applicationUser.Email != "adminmit@bibliomit.cl")
                {
                    IdentityResult result = await _userManager.DeleteAsync(applicationUser).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
}