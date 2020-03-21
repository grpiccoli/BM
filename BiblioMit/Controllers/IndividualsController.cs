using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using BiblioMit.Data;
using BiblioMit.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class IndividualsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IndividualsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Samplings
        public async Task<IActionResult> Index()
        {
            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            ViewData["lang"] = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToUpperInvariant();
            var ApplicationDbContext = _context.Individual.Include(s => s.Sampling).ThenInclude(s => s.Centre);
            return View(await ApplicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Samplings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var individual = await _context.Individual
                .Include(s => s.Sampling)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (individual == null)
            {
                return NotFound();
            }

            return View(individual);
        }

        // GET: Samplings/Create
        public IActionResult Create()
        {
            ViewData["SamplingId"] = new SelectList(_context.Sampling, "Id", "Id");
            return View();
        }

        // POST: Samplings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SamplingId,Sex,Maturity")] Individual individual)
        {
            if (individual == null) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Add(individual);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index");
            }
            ViewData["SamplingId"] = new SelectList(_context.Sampling, "Id", "Id", individual.SamplingId);
            return View(individual);
        }

        // GET: Samplings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var individual = await _context.Individual.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (individual == null)
            {
                return NotFound();
            }
            ViewData["SamplingId"] = new SelectList(_context.Sampling, "Id", "Id", individual.SamplingId);
            return View(individual);
        }

        // POST: Samplings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SamplingId,Sex,Maturity")] Individual individual)
        {
            if (individual == null || id != individual.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(individual);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IndividualExists(individual.Id))
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
            ViewData["SamplingId"] = new SelectList(_context.Sampling, "Id", "Id", individual.SamplingId);
            return View(individual);
        }

        // GET: Samplings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var individual = await _context.Individual
                .Include(s => s.Sampling)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (individual == null)
            {
                return NotFound();
            }

            return View(individual);
        }

        // POST: Samplings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var individual = await _context.Individual.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Individual.Remove(individual);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool IndividualExists(int id)
        {
            return _context.Individual.Any(e => e.Id == id);
        }


    }
}