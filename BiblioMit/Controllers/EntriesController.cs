using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Identity;
using BiblioMit.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using OfficeOpenXml;
using BiblioMit.Extensions;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public EntriesController(ApplicationDbContext context,
            IServiceProvider serviceProvider,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        // GET: Entries
        public async Task<IActionResult> Index(int? id, int? pg, int? rpp, string srt,
            bool? asc, string[] val)
        {
            if (!pg.HasValue) pg = 1;
            if (!rpp.HasValue) rpp = 20;
            if (string.IsNullOrWhiteSpace(srt)) srt = "Date";
            if (!asc.HasValue) asc = false;

            var pre = _context.ProdEntry.Pre();
            var sort = _context.ProdEntry.FilterSort(srt);
            ViewData = _context.ProdEntry.ViewData(pre, pg, rpp, srt, asc, val);
            var Filters = ViewData["Filters"] as IDictionary<string, List<string>>;

            var applicationDbContext = asc.Value ?
                pre
                .OrderBy(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(e => e.AppUser) :
                pre
                .OrderByDescending(x => sort.GetValue(x))
                .Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(e => e.AppUser);

            ViewData["Processing"] = id;

            //if(Filters["Tipo"] == null || Filters["Tipo"].Count() == 0)
            //{
                Filters["Tipo"] = new List<string> { "Semilla", "Cosecha", "Abastecimiento", "Producción" };
            //}

            ViewData[nameof(Tipo)] = EViewData.Enum2Select<Tipo>(Filters, "Name");

            ViewData["Date"] = string.Format(CultureInfo.CurrentCulture, "'{0}'",
                string.Join("','", _context.ProdEntry.Select(v => v.Date.Date.ToString("yyyy-M-d", CultureInfo.CurrentCulture)).Distinct().ToList()));

            return View(await applicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Entries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.ProdEntry
                .Include(e => e.AppUser)
                .SingleOrDefaultAsync(m => m.Id == id)
                .ConfigureAwait(false);
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        [HttpGet]
        public IActionResult Output(int id)
        {
            var model = _context.ProdEntry.FirstOrDefault(e => e.Id == id);
            return PartialView("_Output", model);
        }

        // GET: Entries/Create
        [HttpGet]
        public IActionResult CreateFito()
        {
            return View();
        }

        [HttpPost]
        //[Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFito(IFormFile qqfile)
        {
            if (qqfile == null) return Json(new { success = false, error = "error file null" });
            string error = null;

            if (qqfile.Length > 0)
            {
                var toskip = new List<string> { "Centre", "Fitoplanctons", "PSMB" };
                try
                {
                    using (var stream = new StreamReader(qqfile.OpenReadStream(), Encoding.GetEncoding("Windows-1252"), true))
                    {
                        var html = stream.ReadToEnd();
                        var temp = new TableToExcel();
                        temp.Process(html, out ExcelPackage xlsx);
                        //var package = new ExcelPackage(xlsx);
                        error = await Import.Fito(xlsx, _context, toskip).ConfigureAwait(false);
                    }
                }
                catch (FileNotFoundException ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
        }
            return Json(new { success = string.IsNullOrWhiteSpace(error), error });
        }

        // GET: Entries/Create
        [HttpGet]
        public IActionResult Create()
        {
            var tipos = new Tipo[]
            {
                Tipo.Semilla,Tipo.Cosecha,Tipo.Abastecimiento,Tipo.Producción
            };

            ViewData[nameof(Tipo)] = new SelectList(
                            from Tipo s in tipos
                            select new
                            {
                                Id = (int)s,
                                Name = s.GetAttrName("es")
                            },
                            "Id", "Name");

            return View();
        }

        // POST: Entries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[RequestFormSizeLimit(valueCountLimit: 200000, Order = 1)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Excel,Reportes")] ProdEntry entry)
        {
            if (ModelState.IsValid)
            {
                if (entry?.Excel == null) return View(nameof(Create));

                entry.AppUserId = _userManager.GetUserId(User);
                entry.IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                entry.FileName = entry.Excel.FileName;
                entry.Date = DateTime.Now;
                entry.Success = false;
                _context.Add(entry);
                await _context.SaveChangesAsync()
                    .ConfigureAwait(false);

                var result = string.Empty;

                //var debug = false;

                var toskip = new List<string> { "Row", "Sheet", "Centre", "Rows", "Id", "Origin" };

                if (entry.Reportes != Tipo.Producción)
                {
                    toskip.Add("TipoProduccion");
                    toskip.Add("Dato");
                    toskip.Add("TipoItemProduccion");
                }

                if (entry.Reportes != Tipo.Semilla)
                {
                    toskip.Add("OrigenId");
                    toskip.Add("Origen");
                }

                Stream stream = entry.Excel.OpenReadStream();
                ExcelPackage package = new ExcelPackage(stream);

                Task import = Task.Factory.StartNew(async () =>
                {
                    using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>()
                            .CreateScope())
                    {
                        var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                        var hub = serviceScope.ServiceProvider.GetService<IHubContext<EntryHub>>();
                        await Import.Read<Planilla>(package, entry, context, hub, toskip);
                    }
                });

                return RedirectToAction(nameof(Index), new { id = entry.Id });
            }
            var Filters = new Dictionary<string, List<string>>
            {
                ["Tipo"] = new List<string> { "Semilla", "Cosecha", "Abastecimiento", "Producción" }
            };

            ViewData[nameof(Tipo)] = EViewData.Enum2Select<Tipo>(Filters, "Name");
            return View(entry);
        }

        // GET: Entries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.ProdEntry.SingleOrDefaultAsync(m => m.Id == id)
                .ConfigureAwait(false);
            if (entry == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUser, "Id", "Id", entry.AppUserId);
            return View(entry);
        }

        // POST: Entries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,IP,ProcessStart,ProcessTime,Stage,Reportes")] Entry entry)
        {
            if (id != entry?.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry);
                    await _context.SaveChangesAsync()
                        .ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntryExists(entry.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUser, "Id", "Id", entry.AppUserId);
            return View(entry);
        }

        // GET: Entries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.ProdEntry
                .Include(e => e.AppUser)
                .SingleOrDefaultAsync(m => m.Id == id)
                .ConfigureAwait(false);
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.ProdEntry.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.ProdEntry.Remove(entry);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        private bool EntryExists(int id)
        {
            return _context.ProdEntry.Any(e => e.Id == id);
        }
    }
}
