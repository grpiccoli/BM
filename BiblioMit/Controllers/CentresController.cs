using BiblioMit.Authorization;
using BiblioMit.Data;
using BiblioMit.Models;
using BiblioMit.Models.VM.MapsVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class CentresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CentresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Centres
        [Authorize(Roles = "Invitado,Editor,Administrator",Policy ="Centros")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Centre
                .Include(c => c.Company)
                .Include(c => c.Comuna)
                    .ThenInclude(c => c.Provincia)
                    .ThenInclude(c => c.Region);
            return View(await applicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Centres
        public async Task<IActionResult> Centres(int[] i, int[] c, bool? r)
        {
            if (!r.HasValue) r = false;
            var comunas = _context.Comuna
                .Include(o => o.Provincia)
                    .ThenInclude(o => o.Region)
                .Include(o => o.Centres)
                .Where(o => o.Centres.Any() && o.Id != 0);
            ViewData["comunas"] = comunas;
            var companies = r.Value ? 
                _context.Company
                .Where(o => o.Acronym != null) :
                _context.Company
                .Where(o => o.Acronym == null);
            ViewData["company"] = companies;
            ViewData["c"] = c;
            ViewData["i"] = i;
            ViewData["r"] = r.Value;
            ViewData["Title"] = r.Value ? "Centros I+D" : "Productores";
            ViewData["Main"] = r.Value ? "Centros de Investigaci�n, Tecnolog�a y desarrollo" : "Compa��as Mitilicultoras";
            var model = await _context.Centre
                .Include(o => o.Company)
                .Include(o => o.Coordinates)
                .Include(o => o.Samplings)
                .Include(o => o.Comuna)
                    .ThenInclude(o => o.Provincia)
                    .ThenInclude(o => o.Region)
                .Where(o =>
                o.Name == null &&
                c.Any() ? c.ToString().Contains(Convert.ToString(o.ComunaId, CultureInfo.InvariantCulture),
                StringComparison.InvariantCultureIgnoreCase) : true &&
                i.Any() ? i.ToString().Contains(Convert.ToString(o.CompanyId, CultureInfo.InvariantCulture),
                StringComparison.InvariantCultureIgnoreCase) : true)
                .ToListAsync().ConfigureAwait(false);

            return View(model);
        }

        public IActionResult Producers(int[] c, int[] i)
        {
            var selc = c.ToList();
            var seli = i.ToList();

            ViewData["comunas"] = from Comuna u in _context.Comuna
                .Where(a => a.Id != 0 &&
                a.Centres.Any(b => b.Type == CentreTypes.Cultivo))
                .Include(a => a.Centres)
                .Include(a => a.Provincia)
                    .ThenInclude(a => a.Region)
                    select new BSSVM {
                        Selected = selc.Contains(u.Id),
                        Subtext =
                              $"{StringManipulations.ToRomanNumeral(u.Provincia.Region.Id)} {u.Provincia.Region.Name}, {u.Provincia.Name}",
                        Value = u.Id,
                        Text = u.Name,
                        Tokens = string.Join(" ",u.Centres.Select(k => k.Address))
                    };

            TextInfo textInfo = new CultureInfo("es-CL", false).TextInfo;

            ViewData["company"] = from Company u in _context.Company
                .Where(a => a.Acronym != null)
                                  select new BSSVM
                                  {
                                      Tokens = u.BsnssName + string.Join(" ", u.Centres.Select(k => k.Address)),
                                      Selected = seli.Contains(u.Id),
                                      Subtext =
                                      $"({u.Acronym}) {u.Id}-{StringManipulations.GetDigit(u.Id)}",
                                      Value = u.Id,
                                      Text = textInfo.ToTitleCase(textInfo.ToLower(u.BsnssName.Substring(0, Math.Min(u.BsnssName.Length, 50)))),
                                      Hellip = u.BsnssName.Length > 50
                                  };

            ViewData["c"] = string.Join(",", c);
            ViewData["i"] = string.Join(",", i);

            var centres = _context.Centre
                .Where(
                a => a.Type == (CentreTypes)1 && a.CompanyId != 55555555)
                .Include(a => a.Coordinates)
                .Where(
                a => a.Coordinates.Any()
                && c.Any() && a.ComunaId.HasValue ? selc.Contains(a.ComunaId.Value) : true
                && i.Any() ? seli.Contains(a.CompanyId) : true
                )
                .Include(a => a.Company)
                .Include(a => a.Samplings)
                .Include(a => a.Comuna)
                    .ThenInclude(a => a.Provincia)
                    .ThenInclude(a => a.Region);

            return View(centres);
        }

        // GET: Centres
        [AllowAnonymous]
        public IActionResult Research(int[] c, int[] i)
        {
            var selc = c.ToList();
            var seli = i.ToList();

            ViewData["comunas"] = from Comuna u in _context.Comuna
                .Where(a => a.Id != 0 && 
                a.Centres.Any(b => b.Type == CentreTypes.Investigacion))
                .Include(a => a.Centres)
                .Include(a => a.Provincia)
                    .ThenInclude(a => a.Region)
                          select new BSSVM {
                              Selected = selc.Contains(u.Id),
                              Subtext = 
                              $"{StringManipulations.ToRomanNumeral(u.Provincia.Region.Id)} {u.Provincia.Region.Name}, {u.Provincia.Name}",
                              Value = u.Id,
                              Text = u.Name
                          };

            TextInfo textInfo = new CultureInfo("es-CL", false).TextInfo;

            ViewData["company"] = from Company u in _context.Company
                .Where(a => a.Acronym != null)
                                  select new BSSVM {
                                      Icon = $"bib-{u.Acronym}-mono",
                                      Tokens = u.BsnssName,
                                      Selected = seli.Contains(u.Id),
                                      Subtext =
                                      $"({u.Acronym}) {u.Id}-{StringManipulations.GetDigit(u.Id)}",
                                      Value = u.Id,
                                      Text = textInfo.ToTitleCase(textInfo.ToLower(u.BsnssName.Substring(0, Math.Min(u.BsnssName.Length, 50)))),
                                      Hellip = u.BsnssName.Length > 50
                                    };

            ViewData["c"] = string.Join(",",c);
            ViewData["i"] = string.Join(",",i);

            var centres = _context.Centre
                .Where(
                a => a.Type == (CentreTypes)5 && a.CompanyId != 55555555)
                .Include(a => a.Coordinates)
                .Where(
                a => a.Coordinates.Any()
                && c.Any() && a.ComunaId.HasValue ? selc.Contains(a.ComunaId.Value) : true
                && i.Any() ? seli.Contains(a.CompanyId) : true
                )
                .Include(a => a.Company)
                .Include(a => a.Comuna)
                    .ThenInclude(a => a.Provincia)
                    .ThenInclude(a => a.Region);

            var polygons = new List<object>();

            foreach (var g in centres)
            {
                polygons.Add(g.Coordinates.OrderBy(o => o.Vertex).Select(m => 
                new {
                    lat = m.Latitude,
                    lng = m.Longitude
                }));
            }



            return View(centres);
        }

        // GET: Centres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centre = await _context.Centre
                .Include(c => c.Company)
                .Include(c => c.Coordinates)
                .Include(c => c.Contacts)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (centre == null)
            {
                return NotFound();
            }

            return View(centre);
        }

        // GET: Centres/Create
        [Authorize(Roles = "Administrador", Policy = "Centros")]
        public IActionResult Create()
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            ViewData["ComunaId"] = new SelectList(_context.Comuna, "Id", "Name");
            var values = from CentreTypes e in Enum.GetValues(typeof(CentreTypes))
                         select new { Id = e, Name = e.ToString() };
            ViewData["Type"] = new SelectList(values, "Id", "Name");

            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id");
            return View();
        }

        // POST: Centres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador", Policy = "Centros")]
        public async Task<IActionResult> Create([Bind("Id,ComunaId,Type,Url,Acronym,CompanyId,Name,Address")] Centre centre)
        {
            if (centre == null) return NotFound();

            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if(!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                _context.Add(centre);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index");
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", centre.CompanyId);
            return View(centre);
        }

        // GET: Centres/Edit/5
        [Authorize(Roles = "Editor,Administrador", Policy = "Centros")]
        public async Task<IActionResult> Edit(int? id)
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var centre = await _context.Centre.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (centre == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", centre.CompanyId);
            ViewData["ComunaId"] = new SelectList(_context.Comuna, "Id", "Name", centre.ComunaId);
            var values = from CentreTypes e in Enum.GetValues(typeof(CentreTypes))
                         select new { Id = e, Name = e.ToString() };
            ViewData["Type"] = new SelectList(values, "Id", "Name", centre.Type);
            return View(centre);
        }

        // POST: Centres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor,Administrador", Policy = "Centros")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ComunaId,Type,Url,Acronym,CompanyId,Name,Address")] Centre centre)
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (centre == null || id != centre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(centre);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CentreExists(centre.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", centre.CompanyId);
            return View(centre);
        }

        // GET: Centres/Delete/5
        [Authorize(Roles = "Administrador", Policy = "Centros")]
        public async Task<IActionResult> Delete(int? id)
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var centre = await _context.Centre
                .Include(c => c.Company)
                .Include(c => c.Comuna)
                    .ThenInclude(c => c.Provincia)
                    .ThenInclude(c => c.Region)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (centre == null)
            {
                return NotFound();
            }

            return View(centre);
        }

        // POST: Centres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador", Policy = "Centros")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var centre = await _context.Centre.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Centre.Remove(centre);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool CentreExists(int id)
        {
            return _context.Centre.Any(e => e.Id == id);
        }
    }
}
