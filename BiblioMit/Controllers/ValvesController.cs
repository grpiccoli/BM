using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using BiblioMit.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class ValvesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ValvesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Samplings
        public async Task<IActionResult> Index()
        {
            var ApplicationDbContext = _context.Valve.Include(s => s.Individual);
            return View(await ApplicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Samplings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valve = await _context.Valve
                .Include(s => s.Individual)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (valve == null)
            {
                return NotFound();
            }

            return View(valve);
        }

        // GET: Samplings/Create
        public IActionResult Create()
        {
            ViewData["IndividualId"] = new SelectList(_context.Individual, "Id", "Id");
            return View();
        }

        // POST: Samplings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IndividualId,ValveType,Species,Comment")] Valve valve)
        {
            if(valve != null)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(valve);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    return RedirectToAction("Index");
                }
                ViewData["IndividualId"] = new SelectList(_context.Individual, "Id", "Id", valve.IndividualId);
                return View(valve);
            }
            return NotFound();
        }

        // GET: Samplings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valve = await _context.Valve.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (valve == null)
            {
                return NotFound();
            }
            ViewData["IndividualId"] = new SelectList(_context.Individual, "Id", "Id", valve.IndividualId);
            return View(valve);
        }

        // POST: Samplings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IndividualId,ValveType,Species,Comment")] Valve valve)
        {
            if (valve == null || id != valve.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(valve);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ValveExists(valve.Id))
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
            ViewData["IndividualId"] = new SelectList(_context.Individual, "Id", "Id", valve.IndividualId);
            return View(valve);
        }

        // GET: Samplings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valve = await _context.Valve
                .Include(s => s.Individual)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (valve == null)
            {
                return NotFound();
            }

            return View(valve);
        }

        // POST: Samplings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var valve = await _context.Valve.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Valve.Remove(valve);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool ValveExists(int id)
        {
            return _context.Valve.Any(e => e.Id == id);
        }


    }
}