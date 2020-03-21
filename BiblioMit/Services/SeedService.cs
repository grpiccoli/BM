using BiblioMit.Extensions;
using BiblioMit.Data;
using BiblioMit.Models;
using BiblioMit.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using OfficeOpenXml;

namespace BiblioMit.Services
{
    public class SeedService : ISeed
    {
        private readonly IImport _import;
        private readonly ILogger _logger;
        private readonly IStringLocalizer _localizer;
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _environment;
        private readonly string _os;
        private readonly string _conn;
        private readonly ApplicationDbContext _context;
        private readonly ILookupNormalizer _normalizer;
        public SeedService(
            ILogger<SeedService> logger,
            IImport import,
            IStringLocalizer<SeedService> localizer,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ApplicationDbContext context,
            ILookupNormalizer normalizer
            )
        {
            _import = import;
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
                await Users().ConfigureAwait(false);
                await AddProcedure().ConfigureAwait(false);

                var tsvPath = Path
                    .Combine(_environment.ContentRootPath, "Data", "FORA");
                if (!_context.Forum.Any())
                    await Insert<Forum>(tsvPath).ConfigureAwait(false);
                if (!_context.Post.Any())
                    await Insert<Post>(tsvPath).ConfigureAwait(false);

                tsvPath = Path
                    .Combine(_environment.ContentRootPath, "Data", "CENTROS");
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
                if (!_context.Contact.Any())
                    await Insert<Contact>(tsvPath).ConfigureAwait(false);
                if (!_context.Polygon.Any())
                    await Insert<Polygon>(tsvPath).ConfigureAwait(false);
                if (!_context.Coordinate.Any())
                    await Insert<Coordinate>(tsvPath).ConfigureAwait(false);

                tsvPath = Path
                    .Combine(_environment.ContentRootPath, "Data", "HISTOPATHOLOGY");
                if (!_context.Sampling.Any())
                    await Insert<Sampling>(tsvPath).ConfigureAwait(false);
                if (!_context.Individual.Any())
                    await Insert<Individual>(tsvPath).ConfigureAwait(false);
                if (!_context.Soft.Any())
                    await Insert<Soft>(tsvPath).ConfigureAwait(false);
                if (!_context.Photo.Any())
                    await Insert<Photo>(tsvPath).ConfigureAwait(false);
                
                tsvPath = Path
                    .Combine(_environment.ContentRootPath, "Data", "DIGEST");
                if (!_context.ExcelFile.Any())
                    await Insert<ExcelFile>(tsvPath).ConfigureAwait(false);
                if (!_context.Columna.Any())
                    await Insert<Columna>(tsvPath).ConfigureAwait(false);
                if (!_context.Origen.Any())
                    await Insert<Origen>(tsvPath).ConfigureAwait(false);
                if (!_context.Planilla.Any())
                    await Insert<Planilla>(tsvPath).ConfigureAwait(false);
                if (!_context.ProdEntry.Any())
                    await Insert<ProdEntry>(tsvPath).ConfigureAwait(false);
                if (!_context.EnsayoFito.Any())
                    await Insert<EnsayoFito>(tsvPath).ConfigureAwait(false);
                if (!_context.Groups.Any())
                    await Insert<Groups>(tsvPath).ConfigureAwait(false);
                if (!_context.Phytoplankton.Any())
                    await Insert<Phytoplankton>(tsvPath).ConfigureAwait(false);
                
                tsvPath = Path
                    .Combine(_environment.ContentRootPath, "Data", "SEMAFORO");
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
                await AddBulkFiles().ConfigureAwait(false);

                var adminId = _context.AppUser
                    .Where(u => u.Email == "adminmit@bibliomit.cl")
                    .SingleOrDefault().Id;

                //Post
                await _context.Post.ForEachAsync(p => p.UserId = adminId).ConfigureAwait(false);
                //Contacts
                await _context.Contact.ForEachAsync(c => c.OwnerId = adminId).ConfigureAwait(false);
                //Products
                await _context.ProdEntry.ForEachAsync(p => p.AppUserId = adminId).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);
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
            using SqlConnection connection = new SqlConnection(_conn);
            using SqlCommand command = new SqlCommand
            {
                Connection = connection,
                CommandText = query
            };
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
                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    spExists = true;
                    break;
                }
            }
            connection.Close();
        }
        public async Task Insert<TSource>(string path)
        {
            var name = typeof(TSource).ToString().Split(".").Last();
            _context.Database.SetCommandTimeout(10000);
            var tsv = Path.Combine(path, $"{name}.tsv");
            var tmp = Path.Combine(Path.GetTempPath(), $"{name}.tsv");
            File.Copy(tsv, tmp, true);
            await _context.Database
                .ExecuteSqlInterpolatedAsync($"BulkInsert {"dbo."+name}, {tmp}")
                .ConfigureAwait(false);
            File.Delete(tmp);
            return;
        }
        public async Task Users()
        {
            using var roleStore = new RoleStore<AppRole>(_context);
            using var userStore = new UserStore<AppUser>(_context);
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
                                    NormalizedName = _normalizer.NormalizeName(item)
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
                    var users = new List<UserInitializerVM>();
                    var userPer = new UserInitializerVM
                    {
                        Name = "PER",
                        Email = "javier.aros@mejillondechile.cl",
                        Key = "per2018",
                        ImageUri = new Uri("~/images/ico/mejillondechile.svg", UriKind.Relative)
                    };
                    userPer.Roles.Add(RoleData.AppRoles.ElementAt(0));
                    userPer.Plataforma.Add(Plataforma.mytilidb);
                    userPer.Claims.Add("per");
                    users.Add(userPer);
                    var userMitilidb = new UserInitializerVM
                    {
                        Name = "MytiliDB",
                        Email = "mytilidb@bibliomit.cl",
                        Key = "sivisam2016",
                        ImageUri = new Uri("~/images/ico/bibliomit.svg", UriKind.Relative)
                    };
                    userMitilidb.Roles.Add(RoleData.AppRoles.ElementAt(0));
                    userMitilidb.Plataforma.Add(Plataforma.mytilidb);
                    userMitilidb.Claims.Add("mitilidb");
                    users.Add(userMitilidb);
                    var userWebmaster = new UserInitializerVM
                    {
                        Name = "WebMaster",
                        Email = "adminmit@bibliomit.cl",
                        Key = "34#$erERdfDFcvCV",
                        ImageUri = new Uri("~/images/ico/bibliomit.svg", UriKind.Relative),
                        Rating = 10
                    };
                    userWebmaster.Roles.AddRange(RoleData.AppRoles);
                    userWebmaster.Plataforma.AddRange(EnumUtils.Enum2List<Plataforma>());
                    userWebmaster.Claims.AddRange(ClaimData.UserClaims);
                    users.Add(userWebmaster);
                    var userSernapesca = new UserInitializerVM
                    {
                        Name = "Sernapesca",
                        Email = "sernapesca@bibliomit.cl",
                        Key = "sernapesca2018",
                        ImageUri = new Uri("~/images/ico/bibliomit.svg", UriKind.Relative)
                    };
                    userSernapesca.Roles.Add(RoleData.AppRoles.ElementAt(0));
                    userSernapesca.Plataforma.Add(Plataforma.boletin);
                    userSernapesca.Claims.Add("sernapesca");
                    users.Add(userSernapesca);
                    var userIntemit = new UserInitializerVM
                    {
                        Name = "Intemit",
                        Email = "intemit@bibliomit.cl",
                        Key = "intemit2018",
                        ImageUri = new Uri("~/images/ico/bibliomit.svg", UriKind.Relative)
                    };
                    userIntemit.Roles.Add(RoleData.AppRoles.ElementAt(0));
                    userIntemit.Plataforma.Add(Plataforma.psmb);
                    userIntemit.Claims.Add("intemit");
                    users.Add(userIntemit);

                    foreach (var item in users)
                    {
                        var user = new AppUser
                        {
                            UserName = item.Name,
                            NormalizedUserName = _normalizer.NormalizeName(item.Name),
                            Email = item.Email,
                            NormalizedEmail = _normalizer.NormalizeEmail(item.Email),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            ProfileImageUrl = item.ImageUri
                        };

                        user.NormalizedUserName = _normalizer
                            .NormalizeName(user.UserName);
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

        public async Task AddBulkFiles()
        {
            var path = _environment.ContentRootPath;
            var sep = "/";

            var boletin = string.Join(sep, path, "..", "UNSYNC/DB");

            var pwd = string.Join(sep, "..", "UNSYNC/DB", "plankton");
            //var pwd = string.Join(sep, boletin, "TEST");
            if (!Directory.Exists(pwd)) return;
            var error = string.Join(sep, boletin, "ERROR");
            var ok = string.Join(sep, boletin, "OK");

            var entries = new HashSet<EnsayoFito>();
            var centres = new HashSet<Centre>();
            var fitos = new HashSet<Phytoplankton>();
            var groups = new HashSet<Groups>();
            using (var log = new StreamWriter(pwd + "/BulkPlankton.log"))
            {
                foreach (var d in Directory.GetDirectories(pwd))
                {
                    foreach (var f in Directory.GetFiles(d))
                    {
                        var toskip = new List<string> { "Centre", "Fitoplanctons", "PSMB" };

                        var stream = new StreamReader(f, Encoding.GetEncoding("Windows-1252"), true);
                        var html = stream.ReadToEnd();
                        stream.Close();
                        using var temp = new TableToExcel();
                        temp.Process(html, out ExcelPackage xlsx);
                        (Centre centre, EnsayoFito item, List<Phytoplankton> fito, List<Groups> group, string err) =
                            await _import.AmbAsync(xlsx, toskip)
                            .ConfigureAwait(false);
                        if (item != null && !fito.Any())
                        {
                            err = "fito empty";
                        }
                        var dest = err == null || err == "Archivo ya ingresado" ? ok : error;
                        var dir = new DirectoryInfo(d);
                        var targetPath = string.Join(sep, dest, dir.Name);
                        if (!Directory.Exists(targetPath))
                        {
                            Directory.CreateDirectory(targetPath);
                        }
                        var file = string.Join(sep, path, f);
                        var newf = string.Join(sep, targetPath, Path.GetFileName(f));
                        try
                        {
                            File.Move(file, newf);
                        }
                        catch (IOException e)
                        {
                            log.WriteLine(e);
                        }

                        if (!string.IsNullOrEmpty(err))
                        {
                            log.WriteLine(err);
                            continue;
                        }

                        if (centre != null) centres.UnionWith(new List<Centre> { centre });
                        if (item != null) entries.UnionWith(new List<EnsayoFito> { item });
                        if (fito != null) fitos.UnionWith(fito);
                        if (group != null) groups.UnionWith(group);
                    }
                }
                log.Close();
            }
            foreach (var c in centres)
            {
                var centre = await _context.FindAsync<Centre>(c.Id)
                    .ConfigureAwait(false);
                if (centre == null)
                {
                    await _context.Centre.AddAsync(c).ConfigureAwait(false);
                }
                else
                {
                    _context.Centre.Update(c);
                }
            }
            //context.BulkInsert(centres);
            foreach (var e in entries)
            {
                var entrie = await _context.FindAsync<EnsayoFito>(e.Id)
                    .ConfigureAwait(false);
                if (entrie == null)
                {
                    await _context.EnsayoFito.AddAsync(e)
                        .ConfigureAwait(false);
                }
            }

            //context.BulkInsert(entries);
            foreach (var f in fitos)
            {
                var fito = _context.Phytoplankton
                    .SingleOrDefault(p => p.EnsayoFitoId == f.EnsayoFitoId && p.Species == f.Species);
                if (fito == null)
                {
                    await _context.Phytoplankton.AddAsync(f)
                        .ConfigureAwait(false);
                }
            }
            await _context.SaveChangesAsync()
                .ConfigureAwait(false);
            //context.BulkInsert(fitos);
            await _context.AddRangeAsync(groups)
                .ConfigureAwait(false);
        }
    }
}