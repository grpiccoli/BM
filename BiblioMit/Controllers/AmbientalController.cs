using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BiblioMit.Data;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Dynamic;
using Range = BiblioMit.Models.Range;
using System.Threading.Tasks;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class AmbientalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AmbientalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult ExportMenu()
        {
            var model = User.Identity.IsAuthenticated ? 
            new List<object>
            {
                new
                {
                    label = "...",
                    menu = new List<object>
                    {
                        new
                        {
                            label = "Imagen",
                            menu = new List<object>
                            {
                                new { type = "png", label = "PNG" },
                                new { type = "jpg", label = "JPG" },
                                new { type = "gif", label = "GIF" },
                                new { type = "svg", label = "SVG" },
                                new { type = "pdf", label = "PDF" }
                            }
                        },
                        new
                        {
                            label = "Datos",
                            menu = new List<object>
                            {
                                new { type = "json", label = "JSON" },
                                new { type = "csv", label = "CSV" },
                                new { type = "xlsx", label = "XLSX" }
                            }
                        },
                        new { label = "Imprimir", type = "print" }
                    }
                }
            }:
            new List<object>
            {
                new
                {
                    label = "...",
                    menu = new List<object>
                    {
                        new
                        {
                            label = "Imagen",
                            menu = new List<object>
                            {
                                new { type = "jpg", label = "JPG" },
                            }
                        },
                        new { label = "Imprimir", type = "print" }
                    }
                }
            };

            return Json(model);
        }

        [AllowAnonymous]
        // GET: Arrivals
        public async Task<IActionResult> PSMBList()
        {
            var areas = new List<object>
            {
                new
                {
                    label = "Cuencas",
                    id = 1,
                    choices = new List<object>
                    {
                        new
                        {
                            value = "1",
                            label = "Norte",
                            selected = true
                        },
                        new
                        {
                            value = "3",
                            label = "Centro"
                        },
                        new
                        {
                            value = "2",
                            label = "Sur"
                        }
                    }
                }
            };

            var areasList = await _context.Comuna
            .Where(c => c.Polygons.Any() && c.Centres.Any(e => e.EnsayoFitos.Any()))
            .GroupBy(c => c.Cuenca).ToListAsync().ConfigureAwait(false);

            areas.AddRange(areasList
                .Select(g => new
                {
                    label = "Cuenca " + g.Key.Name,
                    id = g.Key.Id + 1,
                    choices = g.Select(i => new
                    {
                        value = i.Id.ToString(CultureInfo.InvariantCulture),
                        label = $"{i.Name}"
                    })
                }));

            if (User.Identity.IsAuthenticated)
            {
                var psmbsList = await _context.PSMB
                .Where(c => c.Coordinates.Any() && c.Centres.Any(e => e.EnsayoFitos.Any()))
                .GroupBy(c => c.Comuna.Cuenca).ToListAsync().ConfigureAwait(false);

                areas.AddRange(psmbsList
                .Select(g => new
                {
                    label = "Cuenca " + g.Key.Name,
                    id = g.Key.Id + 1,
                    choices = g.Select(i => new
                    {
                        value = (i.Id * 100).ToString(CultureInfo.InvariantCulture),
                        label = $"{i.Id} {i.Name}, {i.Comuna.Name}"
                    })
                }));
            }
            return Json(areas);
        }

        [AllowAnonymous]
        public JsonResult VariableList()
        {
            var fito = new List<object>
            {
                new
                {
                    value = "phy",
                    label = "Fitoplancton Total"
                }
            };
            fito.AddRange(_context.Groups.Select(p => new
            {
                value = p.Id.ToString(CultureInfo.InvariantCulture),
                label = $"{p.Name} Total"
            }));

            var result = new List<object>
            {
                new
                {
                    label = "Variables Oceanográficas",
                    id = 1,
                    choices = new List<object>
                    {
                        new
                        {
                            value = "t",
                            label = "Temperatura",
                            selected = true
                        },
                        new
                        {
                            value = "ph",
                            label = "pH"
                        },
                        new
                        {
                            value = "sal",
                            label = "Salinidad"
                        },
                        new
                        {
                            value = "o2",
                            label = "Oxígeno"
                        }
                    }
                },
                new
                {
                    label = "Fitoplancton",
                    id = 2,
                    choices = fito
                }
            };
            return Json(result);
        }
        public JsonResult TLData(int a, int psmb, int sp, int? t, int? l, int? rs, int? s, string start, string end) {
            var i = Convert.ToDateTime(start, CultureInfo.InvariantCulture);
            var f = Convert.ToDateTime(end, CultureInfo.InvariantCulture);
            IEnumerable<ExpandoObject> data = new List<ExpandoObject>() { };
            //a 1 semilla, 2 larva, 3 reproductor
            var psmbs = new Dictionary<int,int>{
                {20, 101990},
                {21, 101017},
                {22, 103633}
            };
            var sps = new Dictionary<int, int>{
                {31, 1},
                {32, 2},
                {33, 3}
            };
            //TallaRange 0-7
            //LarvaType 0 D, 1 U, 2 O
            //0 101990 Quetalco, 1 101017 Vilipulli, 2 103633 Carahue
            //1 chorito, 2 cholga, 3 choro
            switch (a)
            {
                case 11:
                    if (t.HasValue)
                    {
                        var date1 = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                        .Select(offset => new Talla { SpecieSeed = new SpecieSeed { Seed = new Seed { Date = i.AddDays(offset) } } });
                        var ensayos1 = _context.Talla
                        .Include(tl => tl.SpecieSeed)
                        .ThenInclude(ss => ss.Seed)
                        .Where(tl =>
                        (psmb == 23 || tl.SpecieSeed.Seed.CentreId == psmbs[psmb])
                        && (sp == 34 || tl.SpecieSeed.SpecieId == sps[sp])
                        && tl.Range == (Range)(t.Value % 10))
                        .ToList();
                        ensayos1.AddRange(date1);
                        data = ensayos1
                        .GroupBy(tl => tl.SpecieSeed.Seed.Date.Date)
                        .OrderBy(g => g.Key)
                        .Select(g =>
                        {
                            dynamic expando = new ExpandoObject();
                            expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var cs = g.Where(gg => gg.Id != 0).Select(m => m.Proportion);
                            ((IDictionary<string, object>)expando)
                            .Add($"{a}_{psmb}_{sp}_{t.Value}", cs.Any() ? (double?)Math.Round(cs.Average()) : null);
                            return (ExpandoObject)expando;
                        });
                    }
                    break;
                case 12:
                    if (l.HasValue)
                    {
                        var date2 = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                        .Select(offset => new Larva { Larvae = new Larvae { Date = i.AddDays(offset) } });
                        var ensayos2 = _context.Larva
                        .Include(tl => tl.Larvae)
                        .Where(tl =>
                        (psmb == 23 || tl.Larvae.CentreId == psmbs[psmb])
                        && (sp == 34 || tl.SpecieId == sps[sp])
                        && tl.LarvaType == (LarvaType)(l.Value % 10))
                        .ToList();
                        ensayos2.AddRange(date2);
                        data = ensayos2
                        .GroupBy(tl => tl.Larvae.Date.Date)
                        .OrderBy(g => g.Key)
                        .Select(g =>
                        {
                            dynamic expando = new ExpandoObject();
                            expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var cs = g.Where(gg => gg.Id != 0).Select(m => m.Count);
                            ((IDictionary<string, object>)expando)
                            .Add($"{a}_{psmb}_{sp}_{l.Value}", cs.Any() ? (double?)Math.Round(cs.Average()) : null);
                            return (ExpandoObject)expando;
                        });
                    }
                    break;
                case 13:
                    if (s.HasValue)
                    {
                        var date3 = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                        .Select(offset => new Spawning { Date = i.AddDays(offset) });
                        var ensayos3 = _context.Spawning
                        .Where(tl =>
                        (psmb == 23 || tl.CentreId == psmbs[psmb]))
                        .ToList();
                        ensayos3.AddRange(date3);
                        data = ensayos3
                        .GroupBy(tl => tl.Date.Date)
                        .OrderBy(g => g.Key)
                        .Select(g =>
                        {
                            dynamic expando = new ExpandoObject();
                            expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var cs = g.Where(gg => gg.Id != 0).Select(m => s.Value == 70 ? m.FemaleIG : m.MaleIG);
                            ((IDictionary<string, object>)expando)
                            .Add($"{a}_{psmb}_{sp}_{s.Value}", cs.Any() ? (double?)Math.Round(cs.Average()) : null);
                            return (ExpandoObject)expando;
                        });
                    }
                    break;
                case 14:
                    var date4 = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                    .Select(offset => new SpecieSeed { Seed = new Seed { Date = i.AddDays(offset) } });
                    var ensayos4 = _context.SpecieSeed
                    .Include(ss => ss.Seed)
                    .Where(tl =>
                    (psmb == 23 || tl.Seed.CentreId == psmbs[psmb])
                    && (sp == 34 || tl.SpecieId == sps[sp]))
                    .ToList();
                    ensayos4.AddRange(date4);
                    data = ensayos4
                    .GroupBy(tl => tl.Seed.Date.Date)
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        dynamic expando = new ExpandoObject();
                        expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        var cs = g.Where(gg => gg.Id != 0).Select(m => m.Capture);
                        ((IDictionary<string, object>)expando)
                        .Add($"{a}_{psmb}_{sp}", cs.Any() ? (double?)Math.Round(cs.Average()) : null);
                        return (ExpandoObject)expando;
                    });
                    break;
                case 15:
                    if (rs.HasValue)
                    {
                        var date5 = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                        .Select(offset => new RepStage { Spawning = new Spawning { Date = i.AddDays(offset) } });
                        var ensayos5 = _context.RepStage
                        .Where(tl =>
                        (psmb == 23 || tl.Spawning.CentreId == psmbs[psmb])
                        && tl.Stage == (Stage)(rs.Value % 10))
                        .ToList();
                        ensayos5.AddRange(date5);
                        data = ensayos5
                        .GroupBy(tl => tl.Spawning.Date.Date)
                        .OrderBy(g => g.Key)
                        .Select(g =>
                        {
                            dynamic expando = new ExpandoObject();
                            expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var cs = g.Where(gg => gg.Id != 0).Select(m => m.Proportion);
                            ((IDictionary<string, object>)expando)
                            .Add($"{a}_{psmb}_{sp}_{rs.Value}", cs.Any() ? (double?)Math.Round(cs.Average()) : null);
                            return (ExpandoObject)expando;
                        });
                    }
                    break;
                case 16:
                    if (s.HasValue)
                    {
                        var date6 = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                        .Select(offset => new Spawning { Date = i.AddDays(offset) });
                        var ensayos6 = _context.Spawning
                        .Where(tl =>
                        (psmb == 23 || tl.CentreId == psmbs[psmb]))
                        .ToList();
                        ensayos6.AddRange(date6);
                        data = ensayos6
                        .GroupBy(tl => tl.Date.Date)
                        .OrderBy(g => g.Key)
                        .Select(g =>
                        {
                            dynamic expando = new ExpandoObject();
                            expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var cs = g.Where(gg => gg.Id != 0).Select(m => s.Value == 70 ? m.FemaleProportion : m.MaleProportion);
                            ((IDictionary<string, object>)expando)
                            .Add($"{a}_{psmb}_{sp}_{s.Value}", cs.Any() ? (double?)Math.Round(cs.Average()) : null);
                            return (ExpandoObject)expando;
                        });
                    }
                    break;
                case 17:
                    var date7 = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                    .Select(offset => new SpecieSeed { Seed = new Seed { Date = i.AddDays(offset) } });
                    var ensayos7 = _context.SpecieSeed
                    .Include(ss => ss.Seed)
                    .Where(tl =>
                    (psmb == 23 || tl.Seed.CentreId == psmbs[psmb])
                    && (sp == 34 || tl.SpecieId == sps[sp]))
                    .ToList();
                    ensayos7.AddRange(date7);
                    data = ensayos7
                    .GroupBy(tl => tl.Seed.Date.Date)
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        dynamic expando = new ExpandoObject();
                        expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        var cs = g.Where(gg => gg.Id != 0).Select(m => m.Proportion);
                        ((IDictionary<string, object>)expando)
                        .Add($"{a}_{psmb}_{sp}", cs.Any() ? (double?)Math.Round(cs.Average()) : null);
                        return (ExpandoObject)expando;
                    });
                    break;
            }
            return Json(data);
        }
        public JsonResult TLList()
        {
            var tl = new List<object>
            {
                new
                {
                    label = "Análisis",
                    id = 1,
                    choices = new List<object>{
                        new {
                            value = "14",
                            label = "Captura por Especie"
                        },
                        new {
                            value = "17",
                            label = "% Especie"
                        },
                        new {
                            value = "11",
                            label = "% Talla por Especie"
                        },
                        new {
                            value = "12",
                            label = "Larvas"
                        },
                        new {
                            value = "13",
                            label = "IG Reproductores"
                        },
                        new {
                            value = "15",
                            label = "% Estado Reproductivo"
                        },
                        new {
                            value = "16",
                            label = "% Sexo"
                        }
                    }
                },
                new
                {
                    label = "PSMBs",
                    id = 2,
                    choices = new List<object>{
                        new {
                            value = "20",
                            label = "10219 Quetalco"
                        },
                        new {
                            value = "21",
                            label = "10220 Vilipulli"
                        },
                        new {
                            value = "22",
                            label = "10431 Carahue"
                        },
                        new {
                            value = "23",
                            label = "Todos los PSMBs"
                        }
                    }
                },
                new
                {
                    label = "Especies",
                    id = 3,
                    choices = new List<object>{
                        new {
                            value = "31",
                            label = "Chorito (<i>Mytilus chilensis</i>)"
                        },
                        new {
                            value = "32",
                            label = "Cholga (<i>Aulacomya atra</i>)"
                        },
                        new {
                            value = "33",
                            label = "Choro (<i>Choromytilus chorus</i>)"
                        },
                        new {
                            value = "33",
                            label = "Todas las especies"
                        }
                    }
                },
                new
                {
                    label = "Tallas (%)",
                    id = 4,
                    choices = new List<object>{
                        new {
                            value = "40",
                            label = "0 - 1 (mm)"
                        },
                        new {
                            value = "41",
                            label = "1 - 2 (mm)"
                        },
                        new {
                            value = "42",
                            label = "2 - 5 (mm)"
                        },
                        new {
                            value = "43",
                            label = "5 - 10 (mm)"
                        },
                        new {
                            value = "44",
                            label = "10 - 15 (mm)"
                        },
                        new {
                            value = "45",
                            label = "15 - 20 (mm)"
                        },
                        new {
                            value = "46",
                            label = "20 - 25 (mm)"
                        },
                        new {
                            value = "47",
                            label = "25 - 30 (mm)"
                        },
                        new {
                            value = "48",
                            label = "Todas las tallas"
                        },

                    }
                },
                new
                {
                    label = "Tipo de Larva (conteo)",
                    id = 5,
                    choices = new List<object>{
                        new {
                            value = "50",
                            label = "Larva D (D)"
                        },
                        new {
                            value = "51",
                            label = "Larva umbonada (U)"
                        },
                        new {
                            value = "52",
                            label = "Larva con ojo (O)"
                        },
                        new {
                            value = "53",
                            label = "Larvas Totales"
                        }
                    }
                },
                new
                {
                    label = "Estado reproductivo (%)",
                    id = 6,
                    choices = new List<object>{
                        new {
                            value = "60",
                            label = "En madurez"
                        },
                        new {
                            value = "61",
                            label = "Maduro"
                        },
                        new {
                            value = "62",
                            label = "Desovado"
                        },
                        new {
                            value = "63",
                            label = "En desove"
                        }
                    }
                },
                new
                {
                    label = "Sexo",
                    id = 7,
                    choices = new List<object>{
                        new {
                            value = "70",
                            label = "Hembra"
                        },
                        new {
                            value = "71",
                            label = "Macho"
                        }
                    }
                }
            };
            return Json(tl);
        }
        [AllowAnonymous]
        // GET: Arrivals
        public IActionResult MapData()
        {
            var psmb = _context.PSMB
                .Where(c => c.Coordinates.Any() && c.Centres.Any(e => e.EnsayoFitos.Any()))
                .Select(c => new
                {
                    position = c.Coordinates.OrderBy(o => o.Vertex).Select(o => new
                    {
                        lat = o.Latitude,
                        lng = o.Longitude
                    }),
                    id = c.Id * 100,
                    name = "PSMB " + c.Name,
                    comuna = c.Comuna.Name,
                    provincia = c.Comuna.Provincia.Name,
                    region = c.Comuna.Provincia.Region.Name
                });

            var comuna = _context.Comuna
                .Where(c => c.Polygons.Any() && c.Centres.Any(e => e.EnsayoFitos.Any()))
                .Select(c => new
                {
                    position = c.Polygons.Select(p => p.Coordinates.OrderBy(o => o.Vertex).Select(o => new
                    {
                        lat = o.Latitude,
                        lng = o.Longitude
                    })),
                    c.Id,
                    name = "Comuna " + c.Name,
                    provincia = c.Provincia.Name,
                    region = c.Provincia.Region.Name
                });

            var cuencas = _context.Cuenca
                .Where(c => c.Coordinates.Any())
                .Select(c => new
                {
                    position = c.Coordinates.OrderBy(o => o.Vertex).Select(o => new
                    {
                        lat = o.Latitude,
                        lng = o.Longitude
                    }),
                    c.Id,
                    name = "Cuenca " + c.Name,
                    region = "Los Lagos"
                });

            var map = new List<object>();
            map.AddRange(cuencas);
            map.AddRange(comuna);
            if (User.Identity.IsAuthenticated) 
            map.AddRange(psmb);

            return Json(map);
        }
        [AllowAnonymous]
        public IActionResult FitoData(int area, string var, string start, string end)
        {
            var i = Convert.ToDateTime(start, CultureInfo.InvariantCulture);
            var f = Convert.ToDateTime(end, CultureInfo.InvariantCulture);
            var date = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                .Select(offset => new EnsayoFito { FechaMuestreo = i.AddDays(offset) });
            IEnumerable<ExpandoObject> data = new List<ExpandoObject>() { };
            var ensayos = new List<EnsayoFito>();
            if (area < 100) // Cuenca
            {
                ensayos = _context.EnsayoFito
                    .Where(e => e.PSMB.Comuna.CuencaId == area && e.FechaMuestreo >= i && e.FechaMuestreo <= f)
                    .Include(e => e.Fitoplanctons)
                    .ToList();
            }
            else if (area < 20_000) //Comuna
            {
                ensayos = _context.EnsayoFito
                        .Where(e => e.PSMB.ComunaId == area && e.FechaMuestreo >= i && e.FechaMuestreo <= f)
                        .Include(e => e.Fitoplanctons)
                        .ToList();
            }
            else //PSMB * 100
            {
                var psmb = area / 100;
                ensayos = _context.EnsayoFito
                        .Where(e => e.PSMBId == psmb && e.FechaMuestreo >= i && e.FechaMuestreo <= f)
                        .Include(e => e.Fitoplanctons)
                        .ToList();
            }
            ensayos.AddRange(date);
            switch (var)
            {
                case "phy":
                    data = ensayos
                    .GroupBy(e => e.FechaMuestreo.Date)
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        dynamic expando = new ExpandoObject();
                        expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        var cs = g.Where(m => m.Fitoplanctons != null).SelectMany(m => m.Fitoplanctons.Select(p => p.C));
                        ((IDictionary<string, object>)expando)
                        .Add($"{var}_{area}", cs.Any() ? (double?)Math.Round(cs.Average(), 2) : null);
                        return (ExpandoObject)expando;
                    });
                    break;
                default:
                    var id = Convert.ToInt16(var, CultureInfo.InvariantCulture);
                    data = ensayos
                    .GroupBy(e => e.FechaMuestreo.Date)
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        dynamic expando = new ExpandoObject();
                        expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        var cs = g.Where(m => m.Fitoplanctons != null && m.Fitoplanctons.Any(p => p.GroupsId == id))
                        .SelectMany(m => m.Fitoplanctons.Where(p => p.GroupsId == id).Select(p => p.C));
                        ((IDictionary<string, object>)expando)
                        .Add($"{var}_{area}", cs.Any() ?
                        (double?)Math.Round(cs.Average(), 2) : null);
                        return (ExpandoObject)expando;
                    });
                    break;
            }
            return Json(data);
        }
        [AllowAnonymous]
        public IActionResult GraphData(int area, string var, string start, string end)
        {
            var i = Convert.ToDateTime(start, CultureInfo.InvariantCulture);
            var f = Convert.ToDateTime(end, CultureInfo.InvariantCulture);
            var date = Enumerable.Range(0, 1 + f.Subtract(i).Days)
                .Select(offset => new EnsayoFito { FechaMuestreo = i.AddDays(offset) });
            var index = new Dictionary<string, string>
            {
                { "t", "Temperatura" },
                { "ph", "Ph" },
                { "sal", "Salinidad" },
                { "o2", "Oxigeno" }
            };
            var ensayos = new List<EnsayoFito>();
            if (area < 100) // Cuenca
            {
                ensayos = _context.EnsayoFito
                        .Where(e => e.PSMB.Comuna.CuencaId == area && e.FechaMuestreo >= i && e.FechaMuestreo <= f)
                        .ToList();
            }
            else if (area < 20_000) //Comuna
            {
                ensayos = _context.EnsayoFito
                        .Where(e => e.PSMB.ComunaId == area && e.FechaMuestreo >= i && e.FechaMuestreo <= f)
                        .ToList();
            }
            else //PSMB * 100
            {
                var psmb = area / 100;
                ensayos = _context.EnsayoFito
                        .Where(e => e.PSMBId == psmb && e.FechaMuestreo >= i && e.FechaMuestreo <= f)
                        .ToList();
            }

            ensayos.AddRange(date);
            var data = ensayos
            .GroupBy(e => e.FechaMuestreo.Date)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                dynamic expando = new ExpandoObject();
                expando.date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                ((IDictionary<string, object>)expando)
                .Add($"{var}_{area}", g.Any(m => m[index[var]] != null) ? (double?)Math.Round(g.Average(m => (double?)m[index[var]]).Value, 2) : null);
                return (ExpandoObject)expando;
            }).ToList();
            return Json(data);
        }
        [AllowAnonymous]
        public IActionResult Map(int[] c, int[] i)
        {
            var selc = c.ToList();
            var seli = i.ToList();

            TextInfo textInfo = new CultureInfo("es-CL", false).TextInfo;

            ViewData["c"] = string.Join(",", c);
            ViewData["i"] = string.Join(",", i);

            var centres = _context.PSMB
                .Where(
                a => a.Centres.Any(b => b.Type == (CentreTypes)1)
                && a.ComunaId != 0)
                .Include(a => a.Coordinates)
                .Where(
                a => a.Coordinates.Any()
                )
                .Include(a => a.Comuna)
                    .ThenInclude(a => a.Provincia)
                    .ThenInclude(a => a.Region)
                    ;

            return View(centres);
        }

        public JsonResult GetSpecies(int groupId)
        {
            return Json(_context.Phytoplankton.Where(p => p.GroupsId == groupId)
                .Select(p => new {
                    name = p.Species,
                    id = p.Species,
                    icon = "",
                    unit = "(Cel/mL)",
                    group = p.Groups.Name
                }).Distinct());
        }
        [AllowAnonymous]
        public IActionResult Graph()
        {
            ViewData["start"] = _context.EnsayoFito.Min(e => e.FechaMuestreo).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            ViewData["end"] = _context.EnsayoFito.Max(e => e.FechaMuestreo).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            return View();
        }
    }
}