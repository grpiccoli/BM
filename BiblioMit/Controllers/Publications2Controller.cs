using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using BiblioMit.Data;
using System.Linq;
using System.Diagnostics;
using System.Net.Http;
using HtmlAgilityPack;
using BiblioMit.Models;
using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using BiblioMit.Models.VM;
using System.Drawing;
using Newtonsoft.Json;
using BiblioMit.Services;
//using PaulMiami.AspNetCore.Mvc.Recaptcha;

namespace BiblioMit.Controllers
{
    [AllowAnonymous]
    public class Publications2Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INodeService _node;
        private readonly TextInfo _TI;

        public Publications2Controller(
            ApplicationDbContext context,
            INodeService node)
        {
            _context = context;
            _node = node;
            _TI = new CultureInfo("es-CL", false).TextInfo;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            int? pg, //page
            int? trpp, //results per page
            string srt, //value to sort by
            bool? asc, //ascending or descending sort
            //string[] val, //array of filter:value
            string[] src, //List of engines to search
            string q //search value
            //[FromServices] INodeServices nodeServices
            )
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            #region Variables
            if (!pg.HasValue) pg = 1;
            if (!trpp.HasValue) trpp = 20;
            if (!asc.HasValue) asc = true;
            if (srt == null) srt = "source";
            var order = asc.Value ? "asc" : "desc";

            ViewData[nameof(srt)] = srt;
            ViewData[nameof(pg)] = pg;
            ViewData[nameof(q)] = q;
            ViewData[nameof(asc)] = asc;
            ViewData[nameof(trpp)] = trpp;
            ViewData["any"] = false;
            IEnumerable<PublicationVM> publications = new List<PublicationVM>();
            #endregion
            #region universities dictionary
            var ues = new Dictionary<string, string>()
                {
                    {"uchile", "Universidad de Chile"},
                    {"ula", "Universidad Los Lagos"},
                    //{"utal","Universidad de Talca"},
                    {"umag","Universidad de Magallanes"},
                    //{"ust", "Universidad Santo Tom\u00E1s"},
                    {"ucsc","Universidad Cat\u00F3lica de la Sant\u00EDsima Concepci\u00F3n"},
                    {"uct","Universidad Cat\u00F3lica de Temuco"},
                    {"uach","Universidad Austral de Chile"},
                    {"udec","Universidad de Concepci\u00F3n"},
                    {"pucv","Pontificia Universidad Cat\u00F3lica de Valpara\u00EDso"},
                    {"puc","Pontificia Universidad Cat\u00F3lica"},
                };
            #endregion
            #region diccionario Proyectos conicyt
            var conicyt = new Dictionary<string, string>()
                {
                    {"FONDECYT","Fondo Nacional de Desarrollo Cient\u00EDfico y Tecnol\u00F3gico"},
                    {"FONDEF","Fondo de Fomento al Desarrollo Cient\u00EDfico y Tecnol\u00F3gico"},
                    {"FONDAP","Fondo de Financiamiento de Centros de Investigaci\u00F3n en \u00C1reas Prioritarias"},
                    {"PIA","Programa de Investigaci\u00F3n Asociativa"},
                    {"REGIONAL","Programa Regional de Investigaci\u00F3n Cient\u00EDfica y Tecnol\u00F3gica"},
                    {"BECAS","Programa Regional de Investigaci\u00F3n Cient\u00EDfica y Tecnol\u00F3gica"},
                    {"CONICYT","Programa Regional de Investigaci\u00F3n Cient\u00EDfica y Tecnol\u00F3gica"},
                    {"PROYECTOS","Programa Regional de Investigaci\u00F3n Cient\u00EDfica y Tecnol\u00F3gica"},
                };
            #endregion
            #region diccionario de Proyectos
            var proj = conicyt.Concat(new Dictionary<string, string>() {
                    //{"FAP","Fondo de Administración Pesquero"},//"subpesca"
                    {"FIPA","Fondo de Investigaci\u00F3n Pesquera y de Acuicultura"},//"subpesca"
                    {"CORFO","Corporaci\u00F3n de Fomento a la Producci\u00F3n"}//"corfo"
                }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            #endregion
            #region Artículos Indexados
            var gs = new Dictionary<string, string>()
                {{"gscholar","Google Acad\u00E9mico"}};
            #endregion
            #region Patentes
            var gp = new Dictionary<string, string>()
                {{"gpatents","Google Patentes" }};
            #endregion
            ViewData[nameof(ues)] = ues;
            ViewData[nameof(proj)] = proj;
            ViewData[nameof(gs)] = gs;
            ViewData[nameof(gp)] = gp;

            if (src != null && src.Any())
            {
                ViewData["srcs"] = src;
                if (src[0].Contains(',', StringComparison.InvariantCultureIgnoreCase)) src = src[0].Split(',');
                ViewData[nameof(src)] = src;

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var tot = src.Length;
                    var rpp = (int)Math.Ceiling((double)trpp.Value / tot);
                    var srt_utal = srt;
                    string sort_by, srt_uach;
                    int ggl;

                    switch (srt)
                    {
                        case "title":
                            sort_by = "dc.title_sort";
                            ggl = 0;
                            srt_uach = "ftitre";
                            break;
                        case "date":
                            sort_by = "dc.date.issued_dt";
                            ggl = 1;
                            srt_uach = "udate";
                            break;
                        default:
                            sort_by = "score";
                            srt_utal = "rnk";
                            ggl = 0;
                            srt_uach = "sdxscore";
                            break;
                    }
                    var pubs = await GetPubsAsync(src, q, rpp, pg, sort_by, order, srt_uach, ggl).ConfigureAwait(false);
                    var Publications = pubs.SelectMany(x => x.Item1);

                    var NoResults = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Tesis).ToDictionary(x => x.Item2, x => x.Item3);
                    var NoArticles = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Articulo).ToDictionary(x => x.Item2, x => x.Item3);
                    var NoPatents = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Patente).ToDictionary(x => x.Item2, x => x.Item3);
                    var NoProjs = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Proyecto).ToDictionary(x => x.Item2, x => x.Item3);
                    var nor = NoResults.Count;
                    var nop = NoProjs.Count;
                    var noa = NoArticles.Count;
                    var nopat = NoPatents.Count;
                    var NoTot = NoResults.Concat(NoProjs).Concat(NoArticles).Concat(NoPatents)
                    .GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);

                    int tesiscnt = 0, projcnt = 0, artscnt = 0, patscnt = 0, low1, NoPages;

                    var tesisGradient = GetGradients(Color.DarkGreen, Color.LightGreen, nor);
                    var proyectosGradient = GetGradients(Color.DarkRed, Color.Pink, nop);
                    var articulosGradient = GetGradients(Color.DarkBlue, Color.LightBlue, noa);
                    var patentesGradient = GetGradients(Color.Brown, Color.Yellow, nopat);

                    List<object> tesisData = new List<object> { },
                        projData = new List<object> { },
                        globalData = new List<object> { },
                        artsData = new List<object> { },
                        patsData = new List<object> { };
                    var l = new List<int>() { nor, nop, noa, nopat };

                    var repos = new List<Dictionary<string, string>> { ues, proj, gs, gp }.SelectMany(d => d).ToDictionary(d => d.Key, d => d.Value);

                    if (nor > 0)
                    {
                        foreach (var n in NoResults.Select((value, i) => new { i, value }))
                        {
                            object tmp = new
                            {
                                repositorio = $@"{ues[n.value.Key]
                                .Replace("Universidad", "U.", StringComparison.InvariantCultureIgnoreCase)
                                .Replace("Católica", "C.", StringComparison.InvariantCultureIgnoreCase)} ({n.value.Key})",
                                resultados = n.value.Value,
                                color = ColorToHex(tesisGradient.ElementAt(n.i))
                            };
                            tesisData.Add(tmp);
                            tesiscnt += n.value.Value;
                        }
                        globalData.AddRange(tesisData);
                    }
                    if (nop > 0)
                    {
                        foreach (var n in NoProjs.Select((value, i) => new { i, value }))
                        {
                            object tmp = new
                            {
                                repositorio = $"{n.value.Key}",
                                resultados = n.value.Value,
                                color = ColorToHex(proyectosGradient.ElementAt(n.i))
                            };
                            projData.Add(tmp);
                            projcnt += n.value.Value;
                        }
                        globalData.AddRange(projData);
                    }
                    if (noa > 0)
                    {
                        foreach (var n in NoArticles.Select((value, i) => new { i, value }))
                        {
                            object tmp = new
                            {
                                repositorio = $"{n.value.Key}",
                                resultados = n.value.Value,
                                color = ColorToHex(articulosGradient.ElementAt(n.i))
                            };
                            artsData.Add(tmp);
                            artscnt += n.value.Value;
                        }
                        globalData.AddRange(artsData);
                    }
                    if (nopat > 0)
                    {
                        foreach (var n in NoPatents.Select((value, i) => new { i, value }))
                        {
                            object tmp = new
                            {
                                repositorio = $"{n.value.Key}",
                                resultados = n.value.Value,
                                color = ColorToHex(patentesGradient.ElementAt(n.i))
                            };
                            patsData.Add(tmp);
                            patscnt += n.value.Value;
                        }
                        globalData.AddRange(patsData);
                    }

                    var chartData = new List<List<object>> { globalData, artsData, tesisData, projData, patsData };

                    ViewData["NoPages"] = NoPages = NoTot.Any() ? (int)Math.Ceiling((double)NoTot.Aggregate((b, r) => b.Value > r.Value ? b : r).Value / rpp) : 1;

                    ViewData["any"] = tot > 0;
                    ViewData["multiple"] = tot > l.Max();
                    ViewData["tesis"] = tesiscnt > 0;
                    ViewData[nameof(tot)] = tot;
                    ViewData["projects"] = projcnt > 0;
                    ViewData["articles"] = artscnt > 0;
                    ViewData["patents"] = patscnt > 0;
                    ViewData["couple"] = tot > 1;
                    var sum = projcnt + tesiscnt + artscnt + patscnt;
                    ViewData["all"] = string.Format(CultureInfo.InvariantCulture, "{0:n0}", sum);
                    ViewData[nameof(artscnt)] = string.Format(CultureInfo.InvariantCulture, "{0:n0}", artscnt);
                    ViewData[nameof(tesiscnt)] = string.Format(CultureInfo.InvariantCulture, "{0:n0}", tesiscnt);
                    ViewData[nameof(projcnt)] = string.Format(CultureInfo.InvariantCulture, "{0:n0}", projcnt);
                    ViewData[nameof(patscnt)] = string.Format(CultureInfo.InvariantCulture, "{0:n0}", patscnt);
                    ViewData["%arts"] = sum == 0 ? sum : artscnt * 100 / sum;
                    ViewData["%tesis"] = sum == 0 ? sum : tesiscnt * 100 / sum;
                    ViewData["%proj"] = sum == 0 ? sum : projcnt * 100 / sum;
                    ViewData["%pats"] = sum == 0 ? sum : patscnt * 100 / sum;
                    ViewData["chartData"] = JsonConvert.SerializeObject(chartData);
                    ViewData["arrow"] = asc.Value ? "&#x25BC;" : "&#x25B2;";
                    ViewData["prevDisabled"] = pg == 1 ? "disabled" : "";
                    ViewData["nextDisabled"] = pg == NoPages ? "disabled" : "";
                    ViewData["low"] = low1 = pg.Value > 6 ? pg.Value - 5 : 1;
                    ViewData["high"] = NoPages > low1 + 6 ? low1 + 6 : NoPages;

                    publications = srt switch
                    {
                        "date" =>
                            asc.Value ?
                                Publications.OrderBy(p => p.Date.Year) :
                                Publications.OrderByDescending(p => p.Date.Year),
                        "title" =>
                            asc.Value ?
                                Publications.OrderBy(p => p.Title) :
                                Publications.OrderByDescending(p => p.Title),
                        _ =>
                            asc.Value ?
                                Publications.OrderBy(p => p.Source) :
                                Publications.OrderByDescending(p => p.Source)
                    };
                }
            }
            else 
            {
                ViewData["srcs"] = string.Empty;
                ViewData[nameof(src)] = Array.Empty<string>();
            }
            stopWatch.Stop();
            ViewData["runtime"] = stopWatch.ElapsedMilliseconds;
            ViewData["interval"] = Convert.ToInt32(stopWatch.ElapsedMilliseconds / 500);
            return View(publications);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Translate(string text, string lang)
        {
            var result = _node.Run("./wwwroot/js/translate.js", new string[] { text, lang });
            return Json(result);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Agenda(
            int? pg, //page
            int? trpp, //results per page
            string[] src, //List of engines to search
            string stt,
            string[] fund
            //[FromServices] INodeServices nodeServices
            )
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            #region Variables
            if (!pg.HasValue) pg = 1;
            if (!trpp.HasValue) trpp = 20;
            if (string.IsNullOrWhiteSpace(stt)) stt = "abierto";
            ViewData[nameof(src)] = src;
            ViewData["srcs"] = string.Join(",", src);
            ViewData[nameof(pg)] = pg;
            ViewData[nameof(trpp)] = trpp;
            ViewData["any"] = false;
            var Agendas = new List<AgendaVM>();
            #endregion

            //FIA //FIC //FOPA //FAP //FIP //FIPA
            var conicyt1 = new Dictionary<string, Uri>()
            {
                { "fondap", new Uri($"http://www.conicyt.cl/fondap/category/concursos/?estado={stt}") },
                { "becasconicyt", new Uri($"http://www.conicyt.cl/becasconicyt/category/fichas-concursos/?estado={stt}") },
                { "fondecyt", new Uri($"http://www.conicyt.cl/fondecyt/category/concursos/fondecyt-regular/?estado={stt}") },
                { "fondequip", new Uri($"http://www.conicyt.cl/fondequip/category/concursos/?estado={stt}") }
            };

            var conicyt2 = new Dictionary<string, Uri>()
            {
                { "fondef", new Uri("http://www.conicyt.cl/fondef/") },
                { "fonis", new Uri("http://www.conicyt.cl/fonis/") },
                { "pia", new Uri("http://www.conicyt.cl/pia/") },
                { "regional", new Uri("http://www.conicyt.cl/regional/") },
                { "informacioncientifica", new Uri("http://www.conicyt.cl/informacioncientifica/") },
                { "pai", new Uri("http://www.conicyt.cl/pai/") },
                { "pci", new Uri("http://www.conicyt.cl/pci/") },
                { "explora", new Uri("http://www.conicyt.cl/explora/") }
            };

            // páginas CONICYT 1
            var conicyt1_funds = fund.Intersect(conicyt1.Keys);
            if (conicyt1_funds.Any())
            {
                foreach (string fondo in conicyt1_funds)
                {
                    using IHtmlDocument bc_doc = await GetDoc(conicyt1[fondo]).ConfigureAwait(false);
                    var co = GetCo("conicyt");
                    Agendas.AddRange(from n in bc_doc.QuerySelectorAll("div.lista_concurso")
                                     let cells = n.Children
                                     let title = cells?.ElementAt(0)?.QuerySelector("a")
                                     select new AgendaVM
                                     {
                                         Company = co,
                                         Fund = fondo.ToUpperInvariant() + " ("
                                         + bc_doc.QuerySelector("a[rel='home'] span")?.TextContent + ")",
                                         Title = title?.InnerHtml,
                                         MainUrl = GetUri(title),
                                         Start = GetDateAgenda(cells[1]),
                                         End = GetDateAgenda(cells[2])
                                     });
                }
            }

            //páginas CONICYT 2
            var conicyt2_funds = fund.Intersect(conicyt2.Keys);
            //$postParams = @{valtab='evaluacion';blogid='20'}
            //Invoke-WebRequest -UseBasicParsing http://www.conicyt.cl/fondef/wp-content/themes/fondef/ajax/getpostconcursos.php -Method POST -Body $postParams

            if (conicyt2_funds.Any())
            {
                foreach (string fondo in conicyt2_funds)
                {
                    var values = new Dictionary<string, string>
                            {
                                { "valtab", stt },
                                { "blogid", "20" }
                            };
                    using var content = new FormUrlEncodedContent(values);
                    using HttpClient bc = new HttpClient();
                    using HttpResponseMessage response = await bc.PostAsync(new Uri($"{conicyt2[fondo]}wp-content/themes/fondef/ajax/getpostconcursos.php"), content).ConfigureAwait(false);
                    HtmlDocument bc_doc = new HtmlDocument();
                    bc_doc.Load(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    HtmlNodeCollection bc_entrys = bc_doc.DocumentNode.SelectNodes(".//div/a");
                    //        }
                    //        catch { continue; }
                    //    }
                    //}

                    //if (conicyt2_funds.Count() != 0)
                    //{
                    //    foreach (string fondo in conicyt2_funds)
                    //    {
                    //        try
                    //        {
                    //            HttpClient bc = new HttpClient();
                    //            HttpResponseMessage bc_result = await bc.GetAsync(funds["conicyt2"][fondo]);
                    //            HtmlDocument bc_doc = new HtmlDocument();
                    //            bc_doc.Load(await bc_result.Content.ReadAsStreamAsync());
                    //            HtmlNodeCollection bc_entrys = bc_doc.DocumentNode.SelectSingleNode("//div[@class='container_tabs']").SelectNodes(".//div/a");
                    if (bc_entrys is null) { continue; }
                    //HtmlNode name = bc_doc.DocumentNode.SelectSingleNode("//a[@rel='home']");
                    //string Fund = name.SelectSingleNode(".//span/following-sibling::text()").InnerHtml.Trim();
                    //string Acrn = name.SelectSingleNode(".//span").InnerHtml.Trim();
                    string Fund = "";
                    string Acrn = fondo.ToUpperInvariant();
                    Regex ress1 = new Regex(@"[\d-]+");
                    string[] formats = { "yyyy", "yyyy-MM", "d-MM-yyyy" };
                    foreach (HtmlNode entry in bc_entrys)
                    {
                        var Entry = new AgendaVM()
                        {
                            Company = _context.Company.SingleOrDefault(c => c.Acronym == "conicyt"),
                            Fund = Acrn + " (" + Fund + ")",
                            Title = entry.SelectSingleNode(".//h4")?.InnerHtml,
                            MainUrl = GetUri(entry),
                        };
                        var parsed = DateTime.TryParseExact(ress1.Match(entry.SelectSingleNode(".//p")?.InnerHtml).ToString(),
                                                formats,
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out DateTime Date);
                        if (parsed) Entry.End = Date;
                        Agendas.Add(Entry);
                    }
                }
            }

            //CORFO DIVIdIR POR REGION Y ACTOR?
            Regex ress = new Regex(@"corfo\d+");
            var corfo_funds = fund.Where(item => ress.IsMatch(item));
            if (corfo_funds.Any())
            {
                foreach (string fondo in corfo_funds)
                {
                    var corfo = "https://www.corfo.cl/sites/cpp/programas-y-convocatorias?p=1456407859853-1456408533016-1456408024098-1456408533181&at=&et=&e=&o=&buscar_resultado=&bus=&r=";
                    var num = fondo.Replace("corfo", "", StringComparison.InvariantCulture);
                    using HttpClient bc = new HttpClient();
                    using HttpResponseMessage bc_result = await bc.GetAsync(new Uri(corfo + num)).ConfigureAwait(false);
                    HtmlDocument bc_doc = new HtmlDocument();
                    bc_doc.Load(await bc_result.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    HtmlNodeCollection bc_entrys = bc_doc.DocumentNode.SelectNodes("//div[contains(@class, 'col-sm-12') and contains(@class, 'areas')]/a");
                    foreach (HtmlNode entry in bc_entrys)
                    {
                        if (entry.InnerHtml.Contains("Cerradas", StringComparison.InvariantCulture))
                        {
                            if (stt == "abierto" || stt == "proximo")
                            {
                                continue;
                            }

                            if ((entry.InnerHtml.Contains("En Evaluación", StringComparison.InvariantCulture) && stt != "evaluacion")
                                || (!entry.InnerHtml.Contains("En Evaluación", StringComparison.InvariantCulture) && stt == "evaluacion"))
                            {
                                continue;
                            }
                        }

                        var Entry = new AgendaVM()
                        {
                            Company = _context.Company.SingleOrDefault(c => c.Acronym == "corfo"),
                            Fund = "CORFO",
                            Title = entry.SelectSingleNode(".//h4")?.InnerHtml,
                            MainUrl = new Uri(new Uri(corfo + num), entry.Attributes["href"]?.Value),
                            Description = StringManipulations.HtmlToPlainText(entry.SelectSingleNode(".//div[@class='col-md-9 col-sm-8']")?.InnerHtml),
                        };

                        Regex ress2 = new Regex(@"[\d\/]+");
                        string[] formats = { "dd/MM/yyyy" };
                        var parsed = DateTime.TryParseExact(ress2.Match(entry.SelectNodes(".//li")?[2].InnerHtml).ToString(),
                                                formats,
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out DateTime Date);
                        if (parsed) Entry.End = Date;
                        //Entry.End = entry.InnerHtml.Contains("Disponible todo el año") ? null : null;
                        Agendas.Add(Entry);
                    }
                }
            }

            ViewData["any"] = Agendas.Count > 0;
            ViewData["fund"] = fund;
            ViewData["conicyt1"] = conicyt1;
            ViewData["conicyt2"] = conicyt2;
            ViewData["stt"] = string.IsNullOrEmpty(stt) ? "" : stt.ToString(CultureInfo.InvariantCulture);
            ViewData["regiones"] = from c in _context.Region select c;
            //render
            stopWatch.Stop();
            ViewData["runtime"] = stopWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
            ViewData["interval"] = Convert.ToInt32(stopWatch.ElapsedMilliseconds / 500);
            return View(Agendas);
        }

        public Task<(IEnumerable<PublicationVM>, string, int)[]> GetPubsAsync(
            string[] src, string q, int rpp, int? pg, string sortBy, string order, string srtUach, int ggl)
        {
            if (sortBy == null || order == null) return null;
            var uchile = GetUchileAsync
                (
                src,
                //23s
                //sort_by   dc.date.issued_dt   dc.title_sort   score
                //order     asc                 desc
                new Uri($"http://repositorio.uchile.cl/discover?filtertype_1=type&filter_relational_operator_1=equals&filter_1=Tesis&submit_apply_filter=&query={q}&rpp={rpp}&page={pg}&sort_by={sortBy}&order={order}"),
                "p.pagination-info", 2,
                "div#aspect_discovery_SimpleSearch_div_search-results > div",
                "a",
                "span.ds-dc_contributor_author-authority",
                "div.artifact-info > span.publisher-date > span.date"
                );

            var ula = GetUlaAsync
                (
                src,
                new Uri($"http://medioteca.ulagos.cl/biblioscripts/titulo_claves.idc?texto={q}"),
                "font[face='Arial']:has(> a)",
                "a",
                "font > small:only-child",
                pg, rpp
                );

            var umag = GetUmagAsync
                (
                src,
                new Uri($"http://www.bibliotecadigital.umag.cl/discover?query={q}&rpp={rpp}&page={pg}"),
                "h2.lineMid > span:has(> span)", 1,
                "div.artifact-description",
                "div.artifact-title > a",
                "div.artifact-info > span.author > span",
                "div.artifact-info > span.publisher-date > span.date"
                );

            var ucsc = GetUcscAsync
                (
                src,
                //24s
                //sort_by   dc.date.issued_dt   dc.title_sort   score
                //order     asc                 desc
                new Uri($"http://repositoriodigital.ucsc.cl/discover?scope=25022009/6&submit=&query={q}&rpp={rpp}&page={pg}&sort_by={sortBy}&order={order}"),
                "p.pagination-info", 2,
                "div.ds-static-div.primary > div > div.artifact-description",
                "a",
                "div.artifact-info > span.author.h4 > small > span",
                "div.artifact-info > span.publisher-date.h4 > small > span.date"
                );

            var uct = GetUctAsync
                (
                src,
                //31s
                //sort_by   dc.date.issued_dt   dc.title_sort   score
                //order desc asc
                new Uri("http://repositoriodigital.uct.cl/discover?rpp=" +
                    $"{rpp}&page={pg}&query={q}&sort_by={sortBy}&order={order}"),
                "p.pagination-info", 2,
                "div#aspect_discovery_SimpleSearch_div_search-results > div",
                "a",
                "a[href*='img']",
                "a[href*='img']",
                "a[href*='img']"
                );

            var uach = GetUachAsync
                (
                src,
                //14s
                //sf        ftitre      fauteur     contributeur        udate       sdxscore
                new Uri("http://cybertesis.uach.cl/sdx/uach/resultats-filtree.xsp?biblio_op=or&figures_op=or&tableaux_op=or&citations_op=or&notes_op=or&base=documents&position=2&texte_op=or&titres=" +
                    $"{q}&tableaux={q}&figures={q}&biblio={q}&notes={q}&citations={q}&texte={q}&hpp={rpp}&p={pg}&sf={srtUach}"),
                "div[align='left']:has(> b.label)", 0,
                "td.ressource[valign='top'][align='left']",
                "td:not([valign='top']) > div > a", "span.url > a",
                "span.auteur",
                "span.date"
                );

            var udec = GetUdecAsync
            (
                src,
                //18s
                //sort_by   dc.date.issued_dt   dc.title_sort   score
                //order     asc                 desc
                new Uri("http://repositorio.udec.cl/discover?group_by=none&etal=0&rpp=" +
                                $"{rpp}&page={pg}&query={q}&sort_by={sortBy}&order={order}"),
                "h2.ds-div-head:has(> span)", 1,
                "ul.ds-artifact-list > ul > li > div.artifact-description",
                "div.artifact-title > a",
                "div.artifact-info > span.author > span",
                "div.artifact-info > span.publisher-date > span.date"
            );

            var pucv = GetPucvAsync
                (
                    src,
                    new Uri($"http://opac.pucv.cl/cgi-bin/wxis.exe/iah/scripts/?IsisScript=iah.xis&lang=es&base=BDTESIS&nextAction=search&exprSearch={q}&isisFrom={(pg - 1) * rpp + 1}"),
                    "div.rowResult > div.columnB:has(> a) > b", 0,
                    "div.contain:has(> div.selectCol)",
                    "tr > td > font > b > font > font",
                    "a[href*='pdf']", "a[href*='img']",
                    "tr > td > font > b:only-child",
                    "a[href*='indexSearch=AU']",
                    rpp
                );

            var puc = GetPucAsync
            (
                src,
                //15s
                //sort_by   dc.date.issued_dt   dc.title_sort   score
                //order     asc                 desc
                new Uri($"https://repositorio.uc.cl/discover?scope=11534/1&group_by=none&etal=0&rpp={rpp}&page={pg}&query={q}&sort_by={sortBy}&order={order}&submit=Go"),
                "//h2[@class='ds-div-head' and span]", 1,
                "//ul[@class='ds-artifact-list']/ul/li/div[@class='artifact-description']",
                ".//div[@class='artifact-title']/a",
                ".//div[@class='artifact-info']/span[@class='publisher-date']/span[@class='date']",
                ".//div[@class='artifact-info']/span[@class='author']/span"
            );

            var fondecyt = GetConicyt(src, "FONDECYT", "108045", rpp, sortBy, order, pg, q);
            var fondef = GetConicyt(src, "FONDEF", "108046", rpp, sortBy, order, pg, q);
            var fondap = GetConicyt(src, "FONDAP", "108044", rpp, sortBy, order, pg, q);
            var pia = GetConicyt(src, "PIA", "108042", rpp, sortBy, order, pg, q);
            var regional = GetConicyt(src, "REGIONAL", "108050", rpp, sortBy, order, pg, q);
            var becas = GetConicyt(src, "BECAS", "108040", rpp, sortBy, order, pg, q);
            var conicyt = GetConicyt(src, "CONICYT", "108088", rpp, sortBy, order, pg, q);
            var proyectos = GetConicyt(src, "PROYECTOS", "93475", rpp, sortBy, order, pg, q);

            var fipa = GetFipaAsync(src,
                new Uri($"http://subpesca-engine.newtenberg.com/mod/find/cgi/find.cgi?action=query&engine=SwisheFind&rpp={rpp}&cid=514&stid=&iid=613&grclass=&pnid=&pnid_df=&pnid_tf=&pnid_search=678,682,683,684,681,685,510,522,699,679&limit=200&searchon=&channellink=w3:channel&articlelink=w3:article&pvlink=w3:propertyvalue&notarticlecid=&use_cid_owner_on_links=&show_ancestors=1&show_pnid=1&cids=514&keywords={q}&start={(pg - 1) * rpp}&group=0&expanded=1&searchmode=undefined&prepnidtext=&javascript=1"),
                "p.PP", 2, "li > a");

            var corfo = GetCorfoAsync(src,
                //order     DESC            ASC
                //sort_by   dc.title_sort
                //group_by=none
                new Uri("http://repositoriodigital.corfo.cl/discover?query=" +
                    $"{q}&rpp={rpp}&page={pg}&group_by=none&etal=0&sort_by={sortBy.Replace(".issued", "", StringComparison.InvariantCultureIgnoreCase)}&order={order.ToUpperInvariant()}"),
                "p.pagination-info", 2,
                "div.artifact-description", "a",
                "span.author > small",
                "span.date", "div.abstract"
                );

            var gscholar = GetGscholarAsync(src,
                new Uri($"https://scholar.google.com/scholar?q={q}&start={rpp * ( pg - 1) + 1}&scisbd={ggl}"),
                "div.gs_ab_mdw:has(> b)",
                "div.gs_ri",
                "a",
                "h3.gs_rt",
                "div.gs_a", rpp, "gscholar"
                );

            var gpatents = GetGscholarAsync(src,
                new Uri($"https://scholar.google.cl/scholar?as_q={q}" +
                    "&as_epq=&as_oq=&as_eq=&as_occt=any&as_sauthors=&as_publication=Google+Patents&as_ylo=&as_yhi=&btnG=&hl=en&as_sdt=0%2C5&as_vis=1" +
                    $"&start={rpp * (pg - 1) + 1}&scisbd={ggl}"),
                "div.gs_ab_mdw:has(> b)",
                "div.gs_ri",
                "a",
                "h3.gs_rt",
                "div.gs_a", rpp, "gpatents"
                );

            return Task.WhenAll(
                uchile, ula, umag, ucsc, uct, uach, udec, pucv, puc,
                fondecyt, fondef, fondap, pia, regional, becas, conicyt, proyectos, fipa, corfo
                , gscholar, gpatents
                );
        }

        public static async Task<(IEnumerable<PublicationVM>, string, int)> GetGscholarAsync(string[] src,
Uri url, string NoResultsSelect, string nodeSelect, string quriSelect, string titleSelect,
string dateSelect, int rpp, string acronym)
        {
            if (src.Contains(acronym))
            {
                //try
                //{
                    Regex resss = new Regex(@"([0-9]+,)*[0-9]+");
                    Regex yr = new Regex(@"[0-9]{4}");
                    Regex aut = new Regex(@"\A(?:(?![0-9]{4}).)*");
                    var co = new Company
                    {
                        Acronym = acronym,
                        Id = 55555555,
                        BsnssName = "Google Inc",
                        Address = "1600 Amphitheatre Parkway, Mountain View, CA"
                    };
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (from n in doc.QuerySelectorAll(nodeSelect).Take(rpp)
                        let t = n?.QuerySelector(titleSelect)?.TextContent
                        select new PublicationVM()
                        {
                            Source = acronym,
                            Uri = GetUri(n.QuerySelector(quriSelect)),
                            Title = t?.Substring(t.LastIndexOf(']') + 1),
                            Typep = Typep.Articulo,
                            Company = co,
                            Date = GetDateGS(n, dateSelect),
                            Authors = GetAuthorsGS(n, dateSelect)
                        }, acronym, GetNoResultsGS(doc, NoResultsSelect));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetCorfoAsync(string[] src,
Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string quriSelect,
string authorSelect, string dateSelect, string abstractSelect)
        {
            var acronym = "CORFO";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(60706000);
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (from n in doc.QuerySelectorAll(nodeSelect)
                        let t = n?.QuerySelector(quriSelect)
                        select new PublicationVM()
                        {
                            Source = acronym,
                            Uri = GetUri(url, t),
                            Title = t?.TextContent,
                            Typep = Typep.Proyecto,
                            Company = co,
                            Date = GetDate(n, dateSelect),
                            Authors = GetAuthorsCorfo(n, authorSelect),
                            Abstract = GetAbstract(n, abstractSelect)
                        }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetFipaAsync(string[] src,
Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect)
        {
            var acronym = "FIPA";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(60719000);
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (from n in doc.QuerySelectorAll(nodeSelect)
                        select new PublicationVM()
                        {
                            Source = acronym,
                            Title = n.TextContent,
                            Typep = Typep.Proyecto,
                            Uri = GetUri(new Uri("http://www.subpesca.cl/fipa/613/w3-article-88970.html"), n),
                            Company = co,
                        }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public static string ColorToHex(Color color)
        {
            return "#" + color.R.ToString("X2", CultureInfo.InvariantCulture) +
                         color.G.ToString("X2", CultureInfo.InvariantCulture) +
                         color.B.ToString("X2", CultureInfo.InvariantCulture);
        }

        public static IEnumerable<Color> GetGradients(Color start, Color end, int steps)
        {
            if(steps > 2)
            {
                Color stepper = Color.FromArgb((byte)((end.A - start.A) / (steps - 1)),
                               (byte)((end.R - start.R) / (steps - 1)),
                               (byte)((end.G - start.G) / (steps - 1)),
                               (byte)((end.B - start.B) / (steps - 1)));

                for (int i = 0; i < steps; i++)
                {
                    yield return Color.FromArgb(start.A + (stepper.A * i),
                                                start.R + (stepper.R * i),
                                                start.G + (stepper.G * i),
                                                start.B + (stepper.B * i));
                }
            }
            else
            {
                yield return start;
                yield return end;
                yield break;
            }
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetConicyt(string[] src,
string acronym, string parameter, int rpp, string sortBy, string order, int? pg, string q)
        {
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(60915000);
                    var url = new Uri($"http://repositorio.conicyt.cl/handle/10533/{parameter}/discover?query={q}&page={pg - 1}&rpp={rpp}&sort_by={sortBy}&order={order}");
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (doc.QuerySelectorAll("div.row.ds-artifact-item")
.Select(n => new PublicationVM()
{
Source = acronym,
Title = n.QuerySelector("h4.title-list")?.TextContent,
Typep = Typep.Proyecto,
Uri = GetUri(url, n.QuerySelector("div.artifact-description > a")),
Authors = GetAuthors(n, "span.ds-dc_contributor_author-authority"),
Date = GetDate(n, "span.date"),
Company = co,
Journal = GetJournalConicyt(n)
}), acronym, GetNoResults(doc, "p.pagination-info", 2));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetPucAsync(string[] src,
Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string quriSelect,
string dateSelect, string authorSelect)
        {
            var acronym = "puc";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(acronym);
                    var doc = await GetDocXPath(url).ConfigureAwait(false);
                    return (from n in doc.DocumentNode.SelectNodes(nodeSelect)
                            let t = n?.SelectSingleNode(quriSelect)
                            select new PublicationVM()
                            {
                                Source = acronym,
                                Title = t?.InnerText,
                                Typep = Typep.Tesis,
                                Uri = GetUri(url, t),
                                Authors = GetAuthors(n, authorSelect),
                                Date = GetDate(n, dateSelect),
                                Company = co,
                            }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetPucvAsync(string[] src,
Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string dateSelect,
string quriSelect, string quriSelectAlt, string titleSelect, string authorSelect, int rpp)
        {
            var acronym = "pucv";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(acronym);
                using var doc = await GetDocStream(url).ConfigureAwait(false);
                return (from n in doc.QuerySelectorAll(nodeSelect).Take(rpp)
                        let date = n.QuerySelector(dateSelect)?.Text()
                        select new PublicationVM()
                        {
                            Typep = Typep.Tesis,
                            Source = acronym,
                            Title = n.QuerySelector(titleSelect)?.TextContent,
                            Uri = GetUri(url, n.QuerySelector(quriSelect), n.QuerySelector(quriSelectAlt)),
                            Authors = GetAuthors(n, authorSelect),
                            Date = GetDate(date, date.Length - 4),
                            Company = co,
                        }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUdecAsync(string[] src,
Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string quriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "udec";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(acronym);
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (
from n in doc.QuerySelectorAll(nodeSelect)
let m = n.QuerySelector(quriSelect)
select new PublicationVM()
{
Typep = Typep.Tesis,
Source = acronym,
Title = m?.TextContent,
Uri = GetUri(url, m),
Authors = GetAuthors(n, authorSelect),
Company = co,
Date = GetDate(n, dateSelect)
}, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUachAsync(string[] src,
    Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string titleSelect, 
    string quriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "uach";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(acronym);
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (doc.QuerySelectorAll(nodeSelect).Select(n => new PublicationVM()
                {
                    Source = acronym,
                    Title = n.QuerySelector(titleSelect)?.TextContent,
                    Uri = GetUri(url, n.QuerySelector(quriSelect)),
                    Authors = GetAuthors(n, authorSelect),
                    Typep = Typep.Tesis,
                    Company = co,
                    Date = GetDate(n, dateSelect)
                }), acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUctAsync(string[] src,
Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string quriSelect, string journalSelect, string authorSelect, string dateSelect)
        {
            var acronym = "uct";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    Regex regex = new Regex("[a-zA-Z]");
                using var doc = await GetDoc(url).ConfigureAwait(false);

                return (
from n in doc.QuerySelectorAll(nodeSelect)
let m = n.QuerySelector(quriSelect)
let j = n.QuerySelector(journalSelect)?.TextContent
let d = n.QuerySelector(dateSelect)?.TextContent
select new PublicationVM()
{
                            //otros
                            Typep = Typep.Tesis,
Source = acronym,
Title = m?.TextContent,
Uri = GetUri(url, m),
Journal = j,
Authors = GetAuthors(n, authorSelect, regex),
Company = co,
Date = GetDate(d, j.LastIndexOf(",", StringComparison.InvariantCultureIgnoreCase) + 2)
}, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch (DomException de)
                {
                    Console.WriteLine(de);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUcscAsync(string[] src,
Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string quriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "ucsc";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(acronym);
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (
from n in doc.QuerySelectorAll(nodeSelect)
let m = n.QuerySelector(quriSelect)
select new PublicationVM()
{
Source = acronym,
Title = m?.TextContent,
Uri = GetUri(url, m),
Authors = GetAuthors(n, authorSelect),
Typep = Typep.Tesis,
Company = co,
Date = GetDate(n, dateSelect)
}, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUmagAsync(string[] src,
    Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string quriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "umag";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(acronym);
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (from n in doc.QuerySelectorAll(nodeSelect)
                        let m = n.QuerySelector(quriSelect)
                        select new PublicationVM()
                        {
                            Source = acronym,
                            Title = m?.TextContent,
                            Uri = GetUri(url, m),
                            Authors = GetAuthors(n, authorSelect),
                            Typep = Typep.Tesis,
                            Company = co,
                            Date = GetDate(n, dateSelect)
                        }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUchileAsync(string[] src, 
            Uri url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string quriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "uchile";
            if (src.Contains(acronym))
            {
                //try
                //{
                    var co = GetCo(acronym);
                using var doc = await GetDoc(url).ConfigureAwait(false);
                return (doc.QuerySelectorAll(nodeSelect).Select(n => new PublicationVM()
                {
                    Source = acronym,
                    Title = n?.TextContent,
                    Uri = GetUri(url, n.QuerySelector(quriSelect)),
                    Authors = GetAuthors(n, authorSelect),
                    //Typep = GetTypep(n.QuerySelector("span.tipo_obra").Text().ToLower()),
                    Typep = Typep.Tesis,
                    Company = co,
                    Date = GetDate(n, dateSelect)
                }), acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                //}
                //catch
                //{

                //}
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUlaAsync(string[] src,
    Uri url, string nodeSelect, string quriSelect, string authorSelect, int? pg, int rpp)
        {
            var acronym = "ula";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using var doc = await GetDocStream(url).ConfigureAwait(false);
                    var num = doc.QuerySelectorAll(nodeSelect);
                    return (num.Skip(rpp * (pg.Value - 1)).Take(rpp).Select(n => new PublicationVM()
                    {
                        Typep = Typep.Tesis,
                        Source = acronym,
                        Title = n.QuerySelector(quriSelect).TextContent,
                        Uri = GetUri(url, n.QuerySelector(quriSelect)),
                        Authors = GetAuthors(n, authorSelect),
                        Company = co,
                    }), acronym, num.Count());
                }
                catch
                {
                    throw new Exception("");
                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public static (string, string) GetJournalDoi(IElement node, string acronym)
        {
            string doi = "https://dx.doi.org/";
            switch (acronym)
            {
                case "uchile":
                    var titls = node?.QuerySelector("h4.discoUch span").Attributes["title"].Value;
                    List<int> indexes = titls.AllIndexesOf("rft_id");
                    if (indexes.Count == 3)
                    {
                        return (QueryHelpers.ParseQuery(titls[indexes[0]..indexes[1]])["rft_id"],
                        doi + QueryHelpers.ParseQuery(titls[indexes[1]..indexes[2]])["rft_id"].ToString().Replace("doi: ", "", StringComparison.InvariantCultureIgnoreCase));
                    }
                    return (null, null);
                default:
                    return (null, null);
            }
        }

        public static string GetJournalConicyt(IElement node)
        {
            if(node != null)
            {
                var items = node.QuerySelectorAll("#code");
                if (items.Any())
                {
                    var journal = "N° de Proyecto: " + items[0].TextContent;
                    if (items.Length > 3)
                    {
                        return journal + " Institución Responsable: " + items[3].TextContent;
                    }
                    return journal;
                }
            }
            return null;
        }

        public static Typep GetTypep(string type)
        {
            return type switch
            {
                "tesis" => Typep.Tesis,
                "artículo" => Typep.Articulo,
                _ => Typep.Desconocido
            };
        }

        public static Uri GetUri(Uri rep, IElement link)
        {
            if(link != null)
            {
                return new Uri(rep, link.Attributes["href"].Value);
            }
            else
            {
                return null;
            }
        }

        public static Uri GetUri(Uri rep, HtmlNode link)
        {
            return link != null ? new Uri(rep, link.Attributes["href"].Value) : null;
        }

        public static Uri GetUri(Uri rep, IElement link, IElement backlink)
        {
            return backlink != null ? new Uri(rep, link == null ? backlink.Attributes["href"].Value : link.Attributes["href"].Value) : null;
        }

        public static Uri GetUri(IElement link)
        {
            return link != null ? new Uri(link.Attributes["href"].Value) : null;
        }

        public static Uri GetUri(HtmlNode link)
        {
            return link != null ? new Uri(link.Attributes["href"].Value) : null;
        }

        public static async Task<IHtmlDocument> GetDoc(Uri rep)
        {
            try
            {
                var parser = new HtmlParser();
                using HttpClient hc = new HttpClient();
                return await parser.ParseDocumentAsync(await hc.GetStringAsync(rep).ConfigureAwait(false)).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static async Task<IHtmlDocument> GetDocStream(Uri rep)
        {
            var parser = new HtmlParser();
            using HttpClient hc = new HttpClient();
            return await parser.ParseDocumentAsync(await hc.GetStreamAsync(rep).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public static async Task<HtmlDocument> GetDocXPath(Uri rep)
        {
            var doc = new HtmlDocument();
            using (HttpClient hc = new HttpClient())
                doc.Load(await hc.GetStreamAsync(rep).ConfigureAwait(false));
            return doc;
        }

        public Company GetCo(string u)
        {
            return _context.Company.SingleOrDefault(c => c.Acronym == u);
        }

        public Company GetCo(int rut)
        {
            return _context.Company.SingleOrDefault(c => c.Id == rut);
        }

        public static int GetNoResultsGS(IHtmlDocument doc, string selector)
        {
            if(doc != null)
            {
                Regex res = new Regex(@"([0-9]+,)*[0-9]+");
                var parsed = int.TryParse(res.Match(doc.QuerySelector(selector).TextContent).Value.Replace(",", "", StringComparison.InvariantCultureIgnoreCase), out int result);
                if (parsed) return result;
            }
            return 0;
        }

        public static int GetNoResults(IHtmlDocument doc, string selector, int pos)
        {
            if (doc != null)
            {
                Regex res = new Regex(@"[\d\.,]+");
                var parsed = int.TryParse(res.Matches(doc.QuerySelector(selector).TextContent)[pos].Value, out int result);
                if (parsed) return result;
            }
            return 0;
        }

        public static int GetNoResults(HtmlDocument doc, string selector, int pos)
        {
            Regex res = new Regex(@"[\d\.,]+");
            var parsed = int.TryParse(res.Matches(doc?.DocumentNode.SelectSingleNode(selector).InnerText)[pos].Value, out int result);
            return parsed ? result : 0;
        }

        public static DateTime GetDate(HtmlNode node, string selector)
        {
            Regex res = new Regex(@"[\d\-]+");
            string[] formats = { "yyyy", "yyyy-MM" };
            var parsed = DateTime.TryParseExact(res.Match(node?.SelectSingleNode(selector).InnerText).Value,
                                    formats,
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime Date);
            return parsed ? Date : new DateTime();
        }

        public static DateTime GetDateGS(IElement node, string selector)
        {
                Regex res = new Regex(@"[\d]+");
                string[] formats = { "yyyy" };
                var parsed = DateTime.TryParseExact(res.Match(node?.QuerySelector(selector).TextContent).Value,
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime Date);
                return parsed ? Date : new DateTime();
        }

        public static DateTime GetDate(IElement node, string selector)
        {
                Regex res = new Regex(@"[\d\-]+");
                string[] formats = { "yyyy", "yyyy-MM", "yyyy-MM-dd" };
                var parsed = DateTime.TryParseExact(res.Match(node?.QuerySelector(selector).TextContent).Value,
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime Date);
                return parsed ? Date : new DateTime();
        }

        public static DateTime GetDateAgenda(IElement node)
        {
            string[] formats = { "dd 'de' MMMM 'de'  yyyy" };
            Regex ress1 = new Regex(@"\d[\dA-Za-z\s]+\d");
            var parsed = DateTime.TryParseExact(ress1.Match(node?.TextContent).Value,
                formats,
                CultureInfo.GetCultureInfo("es-CL"),
                DateTimeStyles.None,
                out DateTime Date);
            return parsed ? Date : new DateTime();
        }

        public static DateTime GetDate(string journal, int start)
        {
            if(journal != null)
            {
                string[] formats = { "yyyy" };
                var parsed = DateTime.TryParseExact(journal.Substring(start, 4),
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime Date);
                if(parsed) return Date;
            }
            return new DateTime();
        }

        public static string GetAbstract(IElement node, string selector)
        {
            return node?.QuerySelector(selector).TextContent;
        }

        public static IEnumerable<AuthorVM> GetAuthorsGS(IElement node, string selector)
        {
            if(node != null)
            {
                Regex aut = new Regex(@"\A(?:(?![0-9]{4}).)*");
                return aut.Match(node.QuerySelector(selector).TextContent).Value.Trim().Trim('-').Split(',')
                    .Select(a => a.Split(' '))
                    .Select(nn =>
                    new AuthorVM
                    {
                        Last = nn[0],
                        Name = nn.Length > 1 ? nn[1] : ""
                    });
            }
            return new List<AuthorVM>();
        }

        public static IEnumerable<AuthorVM> GetAuthorsCorfo(IElement node, string selector)
        {
            return node?.QuerySelector(selector).TextContent.Split(';')
                .Select(nn =>
                new AuthorVM
                {
                    Name = nn
                });
        }

        public static IEnumerable<AuthorVM> GetAuthors(IElement node, string selector)
        {
            return node?.QuerySelectorAll(selector)
                .Select(a => a.TextContent.TrimEnd('.').Split(','))
                .Select(nn =>
                new AuthorVM
                {
                    Last = nn[0],
                    Name = nn.Length > 1 ? nn[1] : ""
                });
        }

        public static IEnumerable<AuthorVM> GetAuthors(HtmlNode node, string selector)
        {
            return node?.SelectNodes(selector)
                .Select(a => a.InnerText.TrimEnd('.').Split(','))
                .Select(nn =>
                new AuthorVM
                {
                    Last = nn[0],
                    Name = nn.Length > 1 ? nn[1] : ""
                });
        }

        public static IEnumerable<AuthorVM> GetAuthors(IElement node, string selector, Regex filter)
        {
            return node?.QuerySelectorAll(selector)
                .Where(i => filter.IsMatch(i.TextContent))
                .Select(a => a.TextContent.Split(','))
                .Select(nn =>
                new AuthorVM
                {
                    Last = nn[0],
                    Name = nn.Length > 1 ? nn[1] : ""
                });
        }
    }
    public class Repository
    {
        public string Acronym { get; set; }
        public Uri Url { get; set; }
        public string AnchorSelect { get; set; }
        public string NoResultsSelect { get; set; }
        public int NoResultsPosition { get; set; }
        public string ResultSelect { get; set; }
        public string AuthorSelect { get; set; }
        public string DateSelect { get; set; }
    }
}
