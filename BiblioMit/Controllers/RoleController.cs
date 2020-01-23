using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BiblioMit.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace BiblioMit.Controllers
{
    [Authorize(Policy = "Usuarios")]
    public class AppRoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        
        public AppRoleController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<AppRoleListViewModel> model = new List<AppRoleListViewModel>();
            model = _roleManager.Roles.Select(r => new AppRoleListViewModel
            {
                Id = r.Id,
                RoleName = r.Name,
                Description = r.Description,
                NumberOfUsers = r.Users.Count
            }).ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles ="Editor,Administrator")]
        public async Task<PartialViewResult> AddEditAppRole(string id)
        {
            AppRoleViewModel model = new AppRoleViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                AppRole applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    model.Id = applicationRole.Id;
                    model.RoleName = applicationRole.Name;
                    model.Description = applicationRole.Description;
                }
            }
            return PartialView("_AddEditAppRole", model);
        }
        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public async Task<IActionResult> AddEditAppRole(string id, AppRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isExist = !String.IsNullOrEmpty(id);
                AppRole applicationRole = isExist ? await _roleManager.FindByIdAsync(id) :
               new AppRole
               {
                   CreatedDate = DateTime.UtcNow
               };
                applicationRole.Name = model.RoleName;
                applicationRole.Description = model.Description;
                applicationRole.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                IdentityResult roleRuslt = isExist ? await _roleManager.UpdateAsync(applicationRole)
                                                    : await _roleManager.CreateAsync(applicationRole);
                if (roleRuslt.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAppRole(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                AppRole applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    name = applicationRole.Name;
                }
            }
            return PartialView("_DeleteAppRole", name);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAppRole(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                AppRole applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    IdentityResult roleRuslt = _roleManager.DeleteAsync(applicationRole).Result;
                    if (roleRuslt.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
}