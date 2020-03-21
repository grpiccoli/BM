using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using BiblioMit.Models;
using BiblioMit.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class ColumnasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColumnasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? pg, int? rpp, string srt,
            bool? asc, string[] val)
        {
            return RedirectToAction(nameof(Columnas), new { pg, rpp, srt, asc, val });
        }
        // GET: Columnas
        public async Task<IActionResult> Columnas(int? pg, int? rpp, string srt,
            bool? asc, string[] val)
        {
            if (pg == null) pg = 1;
            if (rpp == null) rpp = 20;
            if (string.IsNullOrEmpty(srt)) srt = "ExcelId";
            if (asc == null) asc = true;

            bool _asc = asc.Value;

            var pre = _context.Columna.Pre();
            var sort = _context.Columna.FilterSort(srt);
            ViewData = _context.Columna.ViewData(pre, pg, rpp, srt, asc, val);
            var Filters = ViewData["Filters"] as IDictionary<string, List<string>>;

            var applicationDbContext = _asc ?
                pre
                .OrderBy(x => sort.GetValue(x))
                //.Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.Excel) :
                pre
                .OrderByDescending(x => sort.GetValue(x))
                //.Skip((pg.Value - 1) * rpp.Value).Take(rpp.Value)
                .Include(c => c.Excel);

            ViewData["ExcelId"] = new MultiSelectList(
                from ExcelFile e in _context.ExcelFile
                select new
                { e.Id, e.Name }, "Id", "Name", Filters.ContainsKey("ExcelId") ?
                Filters["ExcelId"] : null);

            return View(await applicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Columnas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var columna = await _context.Columna
                .Include(c => c.Excel)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (columna == null)
            {
                return NotFound();
            }

            return View(columna);
        }

        // GET: Columnas/Create
        public IActionResult Create()
        {
            ViewData["ExcelId"] = new SelectList(_context.ExcelFile, "Id", "Id");
            return View();
        }

        // POST: Columnas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ExcelId,Description")] Columna columna)
        {
            if(columna != null)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(columna);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                ViewData["ExcelId"] = new SelectList(_context.ExcelFile, "Id", "Id", columna.ExcelId);
                return View(columna);
            }
            return NotFound();
        }

        // GET: Columnas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var columna = await _context.Columna.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (columna == null)
            {
                return NotFound();
            }
            ViewData["ExcelId"] = new SelectList(_context.ExcelFile, "Id", "Id", columna.ExcelId);
            return View(columna);
        }

        [HttpPost]
        public async Task<JsonResult> Editar(string planilla, string atributo, string columna, string conversion)
        {
            var model = await _context.Columna.SingleOrDefaultAsync(c => c.Excel.Name == planilla && c.Name == atributo).ConfigureAwait(false);
            model.Description = string.IsNullOrWhiteSpace(columna) ? null : columna;
            model.Operation = string.IsNullOrWhiteSpace(conversion) ? null : conversion;

            _context.Columna.Update(model);
            var result = await _context.SaveChangesAsync().ConfigureAwait(false);

            return Json(result);
        }

        // POST: Columnas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ExcelId,Description")] Columna columna)
        {
            if (columna == null || id != columna.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(columna);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColumnaExists(columna.Id))
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
            ViewData["ExcelId"] = new SelectList(_context.ExcelFile, "Id", "Id", columna.ExcelId);
            return View(columna);
        }

        // GET: Columnas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var columna = await _context.Columna
                .Include(c => c.Excel)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (columna == null)
            {
                return NotFound();
            }

            return View(columna);
        }

        // POST: Columnas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var columna = await _context.Columna.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Columna.Remove(columna);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        private bool ColumnaExists(int id)
        {
            return _context.Columna.Any(e => e.Id == id);
        }
    }
}
