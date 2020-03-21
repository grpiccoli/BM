using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using System;
using Microsoft.Extensions.Localization;
using BiblioMit.Services;
using BiblioMit.Models.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using BiblioMit.Extensions;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class SamplingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<HomeController> _localizer;

        public SamplingsController(
            ApplicationDbContext context,
            IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: Samplings
        public async Task<IActionResult> Index()
        {
            var ApplicationDbContext = _context.Sampling
                .Include(s => s.Centre)
                    .ThenInclude(c => c.Company);
            return View(await ApplicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Samplings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            ViewData["lang"] = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToUpperInvariant();

            ViewData["SampleId"] = id;

            return View(await _context.Individual
                .Include(s => s.Sampling)
                    .ThenInclude(s => s.Centre)
                        .ThenInclude(c => c.Company)
                .Include(s => s.Softs)
                .Where(m => m.SamplingId == id)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false));

        }

        // GET: Samplings/Create
        public IActionResult Create()
        {
            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id");
            return View();
        }

        // POST: Samplings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CentreId,Date,Salinity,Temp,O2")] Sampling sampling)
        {
            if (sampling == null) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Add(sampling);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index");
            }
            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id", sampling.CentreId);
            return View(sampling);
        }

        // GET: Samplings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sampling = await _context
                .Sampling
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sampling == null)
            {
                return NotFound();
            }
            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id", sampling.CentreId);
            return View(sampling);
        }

        [HttpGet]
        public async Task<IActionResult> AddIndividual(int sampleId)
        {
            var sample = await _context.Sampling
                .Include(i => i.Individuals)
                .SingleOrDefaultAsync(s => s.Id == sampleId).ConfigureAwait(false);

            Individual model = new Individual
            {
                SamplingId = sample.Id,
                Sampling = sample
            };

            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToUpperInvariant();

            var sexes = from Sex e in Enum.GetValues(typeof(Sex))
                        select new { Id = e, Name = e.GetDisplayName(lang) };
            ViewData["Sex"] = new SelectList(sexes, "Id", "Name");

            var maturities = from Maturity e in Enum.GetValues(typeof(Maturity))
                             select new { Id = e, Name = e.GetDisplayName(lang) };
            ViewData["Maturity"] = new SelectList(maturities, "Id", "Name");

            return PartialView("_AddIndividual", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIndividual([Bind("Id,SamplingId,Sex,Maturity,Length,Comment,Number,Depth")] Individual individual)
        {
            if (individual != null && ModelState.IsValid)
            {
                individual.Id = Convert.ToInt32(string.Format(CultureInfo.InvariantCulture, "{0}{1}", individual.SamplingId, individual.Number), CultureInfo.InvariantCulture);
                if (_context.Individual.Any(i => i.Id == individual.Id))
                {
                    string msg = _localizer["The Subject"] + " " + individual.Number + " " + _localizer["of the Sample"] + " " + individual.SamplingId + " " + _localizer["already exists."];
                    var model = new ErrorViewModel
                    {
                        Message = msg
                    };
                    return View("Error", model);
                }
                _context.Add(individual);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Details", "Samplings", new { Id = individual.SamplingId });
            }
            return RedirectToAction("_AddIndividual");
        }

        // GET: Samplings/Edit/5
        [HttpGet]
        public IActionResult EditSoft(int id, int sample, SoftType softType, bool tissue, bool count, bool degree)
        {
            var softs = _context.Soft
                .Include(i => i.Individual)
                .Where(m => m.IndividualId == id && m.SoftType == softType);

            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToUpperInvariant();
            ViewData["lang"] = lang;

            var degrees = from Degree e in Enum.GetValues(typeof(Degree))
                          select new { Id = e, Name = e.GetDisplayName(lang) };
            ViewData["Degree"] = new SelectList(degrees, "Id", "Name");

            IndividualSoftTissueViewModel model = new IndividualSoftTissueViewModel()
            {
                Id = id,
                SamplingId = sample,
                SoftType = softType,
                Check = softs.Any(),
            };

            model.Configs.AddRangeOverride(new Dictionary<string, bool>
                {
                    { "tissue", tissue },
                    { "count", count },
                    { "degree", degree }
                });


            if (tissue)
            {
                model.Tissues.AddRange(Enum.GetValues(typeof(Tissue))
                    .Cast<Tissue>()
                    .Select(t => new TissueView
                    {
                        Check = softs.Any(s => s.Tissue == t),
                        Count = softs.Any(s => s.Tissue == t) ? softs.FirstOrDefault(s => s.Tissue == t).Count : null,
                        Degree = softs.Any(s => s.Tissue == t) ? softs.FirstOrDefault(s => s.Tissue == t).Degree : null,
                        Text = t.GetDisplayName(lang),
                        Value = ((int)t).ToString(CultureInfo.InvariantCulture),
                    }).ToList());
            }

            if (count && !tissue)
            {
                model.Count = softs.Any() ? softs.First().Count : null;
            }

            return PartialView("_EditSoft", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSoft([Bind("Id,SamplingId,SoftType,Tissues,Count,Configs,Check")] IndividualSoftTissueViewModel individual)
        {
            if (individual != null && ModelState.IsValid)
            {
                try
                {
                    if (individual.Configs["tissue"])
                    {
                        var softs = _context.Soft
                            .Where(m => m.IndividualId == individual.Id && m.SoftType == individual.SoftType);

                        if (individual.Configs["count"])
                        {
                            var Options = Enum.GetValues(typeof(Tissue))
                            .Cast<Tissue>()
                            .Select(t => new
                            {
                                Count = softs.Any(s => s.Tissue == t) ? softs.FirstOrDefault(s => s.Tissue == t).Count : null
                            }).ToList();
                            for (int i = 0; i < Options.Count; i++)
                            {
                                if (Options[i].Count != individual.Tissues[i].Count)
                                {
                                    if (individual.Tissues[i].Count == 0)
                                    {
                                        var soft = await softs
                                                                .SingleOrDefaultAsync(s =>
                                                                s.IndividualId == individual.Id
                                                                && s.SoftType == individual.SoftType
                                                                && s.Tissue == (Tissue)i).ConfigureAwait(false);
                                        _context.Soft.Remove(soft);
                                    }
                                    else
                                    {
                                        if (softs.Any(s => s.Tissue == (Tissue)i))
                                        {
                                            var soft = softs.SingleOrDefault(s => s.Tissue == (Tissue)i);
                                            soft.Count = individual.Tissues[i].Count;
                                            _context.Soft.Update(soft);
                                        }
                                        else
                                        {
                                            var soft = new Soft
                                            {
                                                IndividualId = individual.Id,
                                                SoftType = individual.SoftType,
                                                Tissue = (Tissue)i,
                                                Count = individual.Tissues[i].Count
                                            };
                                            _context.Soft.Add(soft);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (individual.Configs["degree"])
                            {
                                var Options = Enum.GetValues(typeof(Tissue))
                                .Cast<Tissue>()
                                .Select(t => new
                                {
                                    Degree = softs.Any(s => s.Tissue == t) ? softs.FirstOrDefault(s => s.Tissue == t).Degree : null
                                }).ToList();
                                for (int c = 0; c < Options.Count; c++)
                                {
                                    if (individual.Tissues[c].Degree == Degree.d0)
                                    {
                                        if (Options[c].Degree == null)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            var soft = await softs
                                                            .SingleOrDefaultAsync(s =>
                                                            s.IndividualId == individual.Id
                                                            && s.SoftType == individual.SoftType
                                                            && s.Tissue == (Tissue)c).ConfigureAwait(false);
                                            _context.Soft.Remove(soft);
                                        }
                                    }
                                    else if (Options[c].Degree == null && individual.Tissues[c].Degree != Degree.d0)
                                    {
                                        var soft = new Soft
                                        {
                                            IndividualId = individual.Id,
                                            SoftType = individual.SoftType,
                                            Tissue = (Tissue)c,
                                            Degree = individual.Tissues[c].Degree
                                        };
                                        _context.Add(soft);
                                    }
                                    else if (individual.Tissues[c].Degree != Options[c].Degree)
                                    {
                                        var soft = softs.SingleOrDefault(s => s.Tissue == (Tissue)c);
                                        soft.Degree = individual.Tissues[c].Degree;
                                        _context.Soft.Update(soft);
                                    }
                                }
                            }
                            else
                            {
                                var Options = Enum.GetValues(typeof(Tissue))
                                .Cast<Tissue>()
                                .Select(t => new
                                {
                                    Check = softs.Any(s => s.Tissue == t)
                                }).ToList();
                                for (int i = 0; i < Options.Count; i++)
                                {
                                    if (Options[i].Check != individual.Tissues[i].Check)
                                    {
                                        if (individual.Tissues[i].Check)
                                        {
                                            var soft = new Soft
                                            {
                                                IndividualId = individual.Id,
                                                SoftType = individual.SoftType,
                                                Tissue = (Tissue)i
                                            };
                                            _context.Soft.Add(soft);
                                        }
                                        else
                                        {
                                            var soft = await softs
                                                                    .SingleOrDefaultAsync(s =>
                                                                    s.IndividualId == individual.Id
                                                                    && s.SoftType == individual.SoftType
                                                                    && s.Tissue == (Tissue)i).ConfigureAwait(false);
                                            _context.Soft.Remove(soft);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var soft = _context.Soft
                            .FirstOrDefault(
                            m =>
                            m.IndividualId == individual.Id &&
                            m.SoftType == individual.SoftType);

                        if (individual.Configs["count"])
                        {
                            if (soft == null)
                            {
                                if (individual.Count > 0)
                                {
                                    var nuevo = new Soft
                                    {
                                        IndividualId = individual.Id,
                                        SoftType = individual.SoftType,
                                        Count = individual.Count
                                    };
                                    _context.Soft.Add(nuevo);
                                }
                            }
                            else if (soft.Count != individual.Count)
                            {
                                if (individual.Count == 0)
                                {
                                    _context.Soft.Remove(soft);
                                }
                                else
                                {
                                    soft.Count = individual.Count;
                                    _context.Soft.Update(soft);
                                }
                            }
                        }
                        else
                        {
                            if (soft == null && individual.Check)
                            {
                                var nuevo = new Soft
                                {
                                    IndividualId = individual.Id,
                                    SoftType = individual.SoftType
                                };
                                _context.Soft.Add(nuevo);
                            }
                            else if (soft != null && !individual.Check)
                            {
                                _context.Soft.Remove(soft);
                            }
                        }
                    }
                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    return RedirectToAction("Details", "Samplings", new { Id = individual.SamplingId });
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
            }
            return View("Error", individual);
        }

        // GET: Samplings/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditIndividual(int? id)
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

            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToUpperInvariant();

            var sexes = from Sex e in Enum.GetValues(typeof(Sex))
                        select new { Id = e, Name = e.GetDisplayName(lang) };
            ViewData["Sex"] = new SelectList(sexes, "Id", "Name");

            var maturities = from Maturity e in Enum.GetValues(typeof(Maturity))
                             select new { Id = e, Name = e.GetDisplayName(lang) };
            ViewData["Maturity"] = new SelectList(maturities, "Id", "Name");

            return PartialView("_EditIndividual", individual);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIndividual(int id, [Bind("Id,SamplingId,Sex,Maturity,Length,Comment")] Individual individual)
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
                return RedirectToAction("Details", "Samplings", new { Id = individual.SamplingId });
            }
            ViewData["SamplingId"] = new SelectList(_context.Sampling, "Id", "Id", individual.SamplingId);
            return RedirectToAction("_EditIndividual");
        }

        // POST: Samplings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CentreId,Date,Salinity,Temp,O2")] Sampling sampling)
        {
            if (sampling == null || id != sampling.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sampling);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SamplingExists(sampling.Id))
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
            ViewData["CentreId"] = new SelectList(_context.Centre, "Id", "Id", sampling.CentreId);
            return View(sampling);
        }

        // GET: Samplings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sampling = await _context.Sampling
                .Include(s => s.Centre)
                .SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sampling == null)
            {
                return NotFound();
            }

            return View(sampling);
        }

        // POST: Samplings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sampling = await _context.Sampling.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _context.Sampling.Remove(sampling);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Index");
        }

        private bool SamplingExists(int id)
        {
            return _context.Sampling.Any(e => e.Id == id);
        }

        private bool IndividualExists(int id)
        {
            return _context.Individual.Any(e => e.Id == id);
        }
    }
}
