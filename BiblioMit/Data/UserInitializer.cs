using BiblioMit.Models;
using BiblioMit.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Data
{
    public class UserInitializer
    {
        public static Task Initialize(ApplicationDbContext context)
        {
            var roleStore = new RoleStore<AppRole>(context);
            var userStore = new UserStore<AppUser>(context);

            if (!context.AppUserRole.Any())
            {
                if (!context.Users.Any())
                {
                    if (!context.AppRole.Any())
                    {
                        var applicationRoles = new List<AppRole> { };
                        foreach (var item in RoleData.AppRoles)
                        {
                            applicationRoles.Add(
                                new AppRole
                                {
                                    CreatedDate = DateTime.Now,
                                    Name = item,
                                    Description = "",
                                    NormalizedName = item.ToLower()
                                });
                        };

                        foreach (var role in applicationRoles)
                        {
                            context.AppRole.Add(role);
                        }
                        context.SaveChanges();
                    }

                    var users = new UserInitializerVM[]
                    {
                        new UserInitializerVM
                        {
                            Name = "PER",
                            Email = "javier.aros@mejillondechile.cl",
                            Roles = new string[] { RoleData.AppRoles.ElementAt(0) },
                            Key = "per2018",
                            Image = "/images/ico/mejillondechile.svg",
                            Plataforma = new Plataforma[]{ Plataforma.mytilidb },
                            Claims = new string[] { "per" }
                        },
                        new UserInitializerVM
                        {
                            Name = "MytiliDB",
                            Email = "mytilidb@bibliomit.clññ|",
                            Roles = new string[] { RoleData.AppRoles.ElementAt(0) },
                            Key = "sivisam2016",
                            Image = "/images/ico/bibliomit.svg",
                            Plataforma = new Plataforma[]{ Plataforma.mytilidb },
                            Claims = new string[] { "mitilidb" }
                        },
                        new UserInitializerVM
                        {
                            Name = "WebMaster",
                            Email = "adminmit@bibliomit.cl",
                            Roles = RoleData.AppRoles.ToArray(),
                            Key = "34#$erERdfDFcvCV",
                            Image = "/images/ico/bibliomit.svg",
                            Plataforma = new Plataforma[]{ Plataforma.bibliomit, Plataforma.boletin, Plataforma.mytilidb, Plataforma.psmb },
                            Rating = 10,
                            Claims = ClaimData.UserClaims.ToArray()
                        },
                        new UserInitializerVM
                        {
                            Name = "Sernapesca",
                            Email = "sernapesca@bibliomit.cl",
                            Roles = new string[] { RoleData.AppRoles.ElementAt(0) },
                            Key = "sernapesca2018",
                            Image = "/images/ico/bibliomit.svg",
                            Plataforma = new Plataforma[]{ Plataforma.boletin },
                            Claims = new string[]{"sernapesca"}
                        },
                        new UserInitializerVM
                        {
                            Name = "Intemit",
                            Email = "intemit@bibliomit.cl",
                            Roles = new string[] { RoleData.AppRoles.ElementAt(0) },
                            Key = "intemit2018",
                            Image = "/images/ico/bibliomit.svg",
                            Plataforma = new Plataforma[]{ Plataforma.psmb },
                            Claims = new string[]{"intemit"}
                        }
                    };

                    foreach (var item in users)
                    {
                        var user = new AppUser
                        {
                            UserName = item.Name,
                            NormalizedUserName = item.Name.ToLower(),
                            Email = item.Email,
                            NormalizedEmail = item.Email.ToLower(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            ProfileImageUrl = item.Image
                        };

                        var hasher = new PasswordHasher<AppUser>();
                        var hashedPassword = hasher.HashPassword(user, item.Key);
                        user.PasswordHash = hashedPassword;

                        foreach (var claim in item.Claims)
                        {
                            user.Claims.Add(new IdentityUserClaim<string>
                            {
                                ClaimType = claim,
                                ClaimValue = claim
                            });
                        }

                        foreach (var role in item.Roles)
                        {
                            var roller = context.Roles.SingleOrDefault(r => r.Name == role);
                            user.Roles.Add(new IdentityUserRole<string> {
                                UserId = user.Id,
                                RoleId = roller.Id
                            });
                        }
                        context.Users.Add(user);
                    }
                    context.SaveChanges();
                }
            }
            return Task.CompletedTask;
        }
    }
}
