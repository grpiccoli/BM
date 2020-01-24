using BiblioMit.Data;
using BiblioMit.Models;
using BiblioMit.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class SeedService : ISeed
    {
        private readonly ILogger _logger;
        private readonly IStringLocalizer _localizer;
        public IConfiguration Configuration { get; }
        private readonly IHostingEnvironment _environment;
        private readonly string _os;
        private readonly string _conn;
        private readonly ApplicationDbContext _context;
        private readonly ILookupNormalizer _normalizer;
        public SeedService(
            ILogger<SeedService> logger,
            IStringLocalizer<SeedService> localizer,
            IConfiguration configuration,
            IHostingEnvironment environment,
            ApplicationDbContext context,
            ILookupNormalizer normalizer
            )
        {
            _logger = logger;
            _localizer = localizer;
            Configuration = configuration;
            _environment = environment;
            _os = Environment.OSVersion.Platform.ToString();
            _conn = Configuration.GetConnectionString($"{_os}Connection");
            _context = context;
            _normalizer = normalizer;
        }
        public async Task Seed()
        {
            try
            {
                await AddProcedure().ConfigureAwait(false);
                await Users().ConfigureAwait(false);
                var adminId = _context.AppUser.Where(u => u.Email == "adminmit@bibliomit.cl").SingleOrDefault().Id;
                var tsvPath = Path.Combine(_environment.ContentRootPath, "Data", "FORA");
                if (!_context.Forum.Any())
                    await Insert<Forum>(tsvPath).ConfigureAwait(false);

                await PostInitializer
                    .Initialize(_context, adminId)
                    .ConfigureAwait(false);

                tsvPath = Path.Combine(_environment.ContentRootPath, "Data", "CENTROS");
                if (!_context.Region.Any())
                    await Insert<Region>(tsvPath).ConfigureAwait(false);
                if (!_context.Provincia.Any())
                    await Insert<Provincia>(tsvPath).ConfigureAwait(false);
                if (!_context.AreaCode.Any())
                    await Insert<AreaCode>(tsvPath).ConfigureAwait(false);
                if (!_context.AreaCodeProvincia.Any())
                    await Insert<AreaCodeProvincia>(tsvPath).ConfigureAwait(false);
                if (!_context.Cuenca.Any())
                    await Insert<Cuenca>(tsvPath).ConfigureAwait(false);
                if (!_context.Comuna.Any())
                    await Insert<Comuna>(tsvPath).ConfigureAwait(false);
                if (!_context.PSMB.Any())
                    await Insert<PSMB>(tsvPath).ConfigureAwait(false);
                if (!_context.Company.Any())
                    await Insert<Company>(tsvPath).ConfigureAwait(false);
                if (!_context.Producto.Any())
                    await Insert<Producto>(tsvPath).ConfigureAwait(false);
                if (!_context.Centre.Any())
                    await Insert<Centre>(tsvPath).ConfigureAwait(false);
                if (!_context.CentreProducto.Any())
                    await Insert<CentreProducto>(tsvPath).ConfigureAwait(false);

                await ContactsInitializer
                    .Initialize(_context, adminId)
                    .ConfigureAwait(false);

                if (!_context.Polygon.Any())
                    await Insert<Polygon>(tsvPath).ConfigureAwait(false);
                if (!_context.Coordinate.Any())
                    await Insert<Coordinate>(tsvPath).ConfigureAwait(false);

                tsvPath = Path.Combine(_environment.ContentRootPath, "Data", "HISTOPATHOLOGY");
                if (!_context.Sampling.Any())
                    await Insert<Sampling>(tsvPath).ConfigureAwait(false);
                if (!_context.Individual.Any())
                    await Insert<Individual>(tsvPath).ConfigureAwait(false);
                if (!_context.Soft.Any())
                    await Insert<Soft>(tsvPath).ConfigureAwait(false);
                if (!_context.Photo.Any())
                    await Insert<Photo>(tsvPath).ConfigureAwait(false);
                
                tsvPath = Path.Combine(_environment.ContentRootPath, "Data", "DIGEST");
                if (!_context.Excel.Any())
                    await Insert<Excel>(tsvPath).ConfigureAwait(false);
                if (!_context.Columna.Any())
                    await Insert<Columna>(tsvPath).ConfigureAwait(false);
                if (!_context.Origen.Any())
                    await Insert<Origen>(tsvPath).ConfigureAwait(false);
                if (!_context.Planilla.Any())
                    await Insert<Planilla>(tsvPath).ConfigureAwait(false);
                
                await ProdInitializer
                    .Initialize(_context, adminId)
                    .ConfigureAwait(false);

                if (!_context.EnsayoFito.Any())
                    await Insert<EnsayoFito>(tsvPath).ConfigureAwait(false);
                if (!_context.Groups.Any())
                    await Insert<Groups>(tsvPath).ConfigureAwait(false);
                if (!_context.Phytoplankton.Any())
                    await Insert<Phytoplankton>(tsvPath).ConfigureAwait(false);
                
                tsvPath = Path.Combine(_environment.ContentRootPath, "Data", "SEMAFORO");
                if (!_context.Spawning.Any())
                    await Insert<Spawning>(tsvPath).ConfigureAwait(false);
                if (!_context.RepStage.Any())
                    await Insert<RepStage>(tsvPath).ConfigureAwait(false);
                if (!_context.Specie.Any())
                    await Insert<Specie>(tsvPath).ConfigureAwait(false);
                if (!_context.SpecieSeed.Any())
                    await Insert<SpecieSeed>(tsvPath).ConfigureAwait(false);
                if (!_context.Seed.Any())
                    await Insert<Seed>(tsvPath).ConfigureAwait(false);
                if (!_context.Talla.Any())
                    await Insert<Talla>(tsvPath).ConfigureAwait(false);
                if (!_context.Larvae.Any())
                    await Insert<Larvae>(tsvPath).ConfigureAwait(false);
                if (!_context.Larva.Any())
                    await Insert<Larva>(tsvPath).ConfigureAwait(false);
                await AddBulkFiles
                    .Add(_context, _environment.ContentRootPath)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer["There has been an error while seeding the database."]);
                throw;
            }
        }
        public async Task AddProcedure()
        {
            string query = "select * from sysobjects where type='P' and name='BulkInsert'";
            var sp = @"CREATE PROCEDURE BulkInsert(@TableName NVARCHAR(50), @Tsv NVARCHAR(100))
AS
BEGIN 
DECLARE @SQLSelectQuery NVARCHAR(MAX)=''
SET @SQLSelectQuery = 'BULK INSERT ' + @TableName + ' FROM ' + QUOTENAME(@Tsv) + ' WITH (DATAFILETYPE=''widechar'')'
  exec(@SQLSelectQuery)
END";
            bool spExists = false;
            using (SqlConnection connection = new SqlConnection(_conn))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;
                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            spExists = true;
                            break;
                        }
                    }
                    if (!spExists)
                    {
                        command.CommandText = sp;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                spExists = true;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
            }
        }
        public async Task Insert<TSource>(string path)
        {
            var name = typeof(TSource).ToString().Split(".").Last();
            _context.Database.SetCommandTimeout(10000);
            var tableName = $"dbo.{name}";
            var tsv = Path.Combine(path, $"{name}.tsv");
            var tmp = Path.Combine(Path.GetTempPath(), $"{name}.tsv");
            File.Copy(tsv, tmp, true);
            await _context.Database
                .ExecuteSqlCommandAsync($"BulkInsert @p0, @p1;", tableName, tmp)
                .ConfigureAwait(false);
            File.Delete(tmp);
            return;
        }
        public async Task Users()
        {
            using (var roleStore = new RoleStore<AppRole>(_context))
            using (var userStore = new UserStore<AppUser>(_context))
            {
                if (!_context.AppUserRole.Any())
                {
                    if (!_context.Users.Any())
                    {
                        if (!_context.AppRole.Any())
                        {
                            var applicationRoles = new List<AppRole>();
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
                                await _context.AppRole
                                    .AddAsync(role)
                                    .ConfigureAwait(false);
                            }
                            await _context.SaveChangesAsync()
                                .ConfigureAwait(false);
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

                            user.NormalizedUserName = _normalizer
                                .Normalize(user.UserName);
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
                                var roller = await _context.Roles
                                    .SingleOrDefaultAsync(r => r.Name == role)
                                    .ConfigureAwait(false);
                                user.Roles.Add(new IdentityUserRole<string>
                                {
                                    UserId = user.Id,
                                    RoleId = roller.Id
                                });
                            }
                            await _context.Users.AddAsync(user)
                                .ConfigureAwait(false);
                        }
                        await _context.SaveChangesAsync()
                            .ConfigureAwait(false);
                    }
                }
                return;
            }
        }
    }
}