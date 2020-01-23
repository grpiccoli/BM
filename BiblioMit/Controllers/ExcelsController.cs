using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class ExcelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExcelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Excels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Excel.Include(e => e.Columnas).ToListAsync());
        }

        // GET: Excels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excel = await _context.Excel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (excel == null)
            {
                return NotFound();
            }

            return View(excel);
        }

        // GET: Excels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Excels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Excel excel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(excel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(excel);
        }

        // GET: Excels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excel = await _context.Excel.SingleOrDefaultAsync(m => m.Id == id);
            if (excel == null)
            {
                return NotFound();
            }
            return View(excel);
        }

        // POST: Excels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Excel excel)
        {
            if (id != excel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(excel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExcelExists(excel.Id))
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
            return View(excel);
        }

        // GET: Excels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excel = await _context.Excel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (excel == null)
            {
                return NotFound();
            }

            return View(excel);
        }

        // POST: Excels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var excel = await _context.Excel.SingleOrDefaultAsync(m => m.Id == id);
            _context.Excel.Remove(excel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExcelExists(int id)
        {
            return _context.Excel.Any(e => e.Id == id);
        }
    }
}
