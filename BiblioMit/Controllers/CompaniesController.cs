using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using BiblioMit.Services;
using System.Globalization;

namespace BiblioMit.Controllers
{
    [Authorize(Policy = "Instituciones")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        [Authorize(Roles = "Administrador,Editor,Invitado")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Company
                .Include(c => c.Centres)
                    .ThenInclude(c => c.Coordinates)
                .Include(c => c.Centres)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false));
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .Include(c => c.Centres)
                .ThenInclude(c => c.Contacts)
                .Include(c => c.Centres)
                .ThenInclude(c => c.Comuna)
                .ThenInclude(c => c.Provincia)
                .ThenInclude(c => c.Region)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,BsnssName,Acronym")] CompanyViewModel company)
        {
            if (company == null) return NotFound();
            if (ModelState.IsValid)
            {
                Company corp = new Company
                {
                    Id = Convert.ToInt32(string.Format(new InterceptProvider(), "{0:I}", company.RUT), CultureInfo.InvariantCulture),
                    BsnssName = company.BsnssName,
                    Acronym = company.Acronym
                };
                _context.Add(corp);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Administrador,Editor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (company == null)
            {
                return NotFound();
            }
            var comp = new CompanyViewModel
            {
                RUT = company.Id.ToString("{0:U}", new InterceptProvider()),
                BsnssName = company.BsnssName
            };
            return View(comp);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Editor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BsnssName,Acronym")] Company company)
        {
            if (company == null || id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Company.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Company.Remove(company);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
