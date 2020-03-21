using BiblioMit.Authorization;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Controllers
{
    [Authorize(Policy = "Coordenadas")]
    public class CoordinatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Coordinates
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["CentreSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var coordinates = from c in _context.Coordinate.Include(c => c.Centre)
                              select c;
            switch (sortOrder)
            {
                case "name_desc":
                    coordinates = coordinates.OrderByDescending(c => c.Centre);
                    break;
                default:
                    coordinates = coordinates.OrderBy(c => c.Centre);
                    break;
            }
            return View(await coordinates.AsNoTracking().ToListAsync().ConfigureAwait(false));
        }

        // GET: Coordinates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coordinate = await _context.Coordinate
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (coordinate == null)
            {
                return NotFound();
            }

            return View(coordinate);
        }

        // GET: Coordinates/Create
        [Authorize(Roles = "Administrador",Policy ="Coordenadas")]
        public IActionResult Create()
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id");
            return View();
        }

        // POST: Coordinates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador", Policy = "Coordenadas")]
        public async Task<IActionResult> Create([Bind("Id,CentreId,RegionId,ComunaId,ProvinciaId,Latitude,Longitude,Vertex")] Coordinate coordinate)
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (coordinate == null) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Add(coordinate);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index");
            }
            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id", coordinate.CentreId);
            return View(coordinate);
        }

        // GET: Coordinates/Edit/5
        [Authorize(Roles = "Administrador,Editor", Policy = "Coordenadas")]
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

            var coordinate = await _context.Coordinate.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (coordinate == null)
            {
                return NotFound();
            }
            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id", coordinate.CentreId);
            return View(coordinate);
        }

        // POST: Coordinates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Editor", Policy = "Coordenadas")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CentreId,RegionId,ComunaId,ProvinciaId,Latitude,Longitude,Vertex")] Coordinate coordinate)
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (coordinate == null || id != coordinate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coordinate);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoordinateExists(coordinate.Id))
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
            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id", coordinate.CentreId);
            return View(coordinate);
        }

        // GET: Coordinates/Delete/5
        [Authorize(Roles = "Administrador", Policy = "Coordenadas")]
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

            var coordinate = await _context.Coordinate
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (coordinate == null)
            {
                return NotFound();
            }

            return View(coordinate);
        }

        // POST: Coordinates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador", Policy = "Coordenadas")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var coordinate = await _context.Coordinate.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Coordinate.Remove(coordinate);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool CoordinateExists(int id)
        {
            return _context.Coordinate.Any(e => e.Id == id);
        }
    }
}
