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
    public class SoftsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoftsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Samplings
        public async Task<IActionResult> Index()
        {
            var ApplicationDbContext = _context.Soft.Include(s => s.Individual);
            return View(await ApplicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Samplings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var soft = await _context.Soft
                .Include(s => s.Individual)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (soft == null)
            {
                return NotFound();
            }

            return View(soft);
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
        public async Task<IActionResult> Create([Bind("Id,IndividualId,SoftType,Tissue,Comment,Count,Degree")] Soft soft)
        {
            if (soft == null) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Add(soft);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index");
            }
            ViewData["IndividualId"] = new SelectList(_context.Individual, "Id", "Id", soft.IndividualId);
            return View(soft);
        }

        // GET: Samplings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var soft = await _context.Soft.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (soft == null)
            {
                return NotFound();
            }
            ViewData["IndividualId"] = new SelectList(_context.Individual, "Id", "Id", soft.IndividualId);
            return View(soft);
        }

        // POST: Samplings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IndividualId,SoftType,Tissue,Comment,Count,Degree")] Soft soft)
        {
            if (soft == null || id != soft.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(soft);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftExists(soft.Id))
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
            ViewData["IndividualId"] = new SelectList(_context.Individual, "Id", "Id", soft.IndividualId);
            return View(soft);
        }

        // GET: Samplings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var soft = await _context.Soft
                .Include(s => s.Individual)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (soft == null)
            {
                return NotFound();
            }

            return View(soft);
        }

        // POST: Samplings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var soft = await _context.Soft.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Soft.Remove(soft);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool SoftExists(int id)
        {
            return _context.Soft.Any(e => e.Id == id);
        }
    }
}