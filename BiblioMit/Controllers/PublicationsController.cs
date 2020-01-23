using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using BiblioMit.Data;
using System.Linq;
using Microsoft.AspNetCore.NodeServices;
using System.Diagnostics;
using System.Net.Http;
using HtmlAgilityPack;
using BiblioMit.Models;
using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using BiblioMit.Models.VM;
using System.Drawing;
using Newtonsoft.Json;
//using PaulMiami.AspNetCore.Mvc.Recaptcha;

namespace BiblioMit.Controllers
{
    [AllowAnonymous]
    public class PublicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublicationsController(ApplicationDbContext context)
        {
            _context = context;
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

            if (src.Any() && src[0].Contains(','))
            {
                src = src[0].Split(',');
            }
            ViewData[nameof(src)] = src;
            ViewData["srcs"] = string.Join(",", src);
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

            if (!string.IsNullOrWhiteSpace(q))
            {
                var rpp = (int)Math.Ceiling((double)trpp.Value / src.Count());
                var srt_utal = srt;
                string sort_by, srt_uach;
                int srt_uct, ggl;

                switch (srt)
                {
                    case "title":
                        sort_by = "dc.title_sort";
                        srt_uct = 1;
                        ggl = 0;
                        srt_uach = "ftitre";
                        break;
                    case "date":
                        sort_by = "dc.date.issued_dt";
                        srt_uct = 2;
                        ggl = 1;
                        srt_uach = "udate";
                        break;
                    default:
                        sort_by = "score";
                        srt_utal = "rnk";
                        ggl = srt_uct = 0;
                        srt_uach = "sdxscore";
                        break;
                }
                var pubs = await GetPubsAsync(src, q, rpp, pg, sort_by, order, srt_uct, srt_uach, ggl);
                var Publications = pubs.SelectMany(x => x.Item1);

                var NoResults = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Tesis).ToDictionary(x => x.Item2, x => x.Item3);
                var NoArticles = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Articulo).ToDictionary(x => x.Item2, x => x.Item3);
                var NoPatents = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Patente).ToDictionary(x => x.Item2, x => x.Item3);
                var NoProjs = pubs.Where(x => x.Item1.Any() && x.Item1.First().Typep == Typep.Proyecto).ToDictionary(x => x.Item2, x => x.Item3);
                var nor = NoResults.Count();
                var nop = NoProjs.Count();
                var noa = NoArticles.Count();
                var nopat = NoPatents.Count();
                var tot = src.Count();
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
                            repositorio = $"{ues[n.value.Key].Replace("Universidad", "U.").Replace("Católica", "C.")} ({n.value.Key})",
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
                ViewData["all"] = string.Format("{0:n0}", sum);
                ViewData[nameof(artscnt)] = string.Format("{0:n0}", artscnt);
                ViewData[nameof(tesiscnt)] = string.Format("{0:n0}", tesiscnt);
                ViewData[nameof(projcnt)] = string.Format("{0:n0}", projcnt);
                ViewData[nameof(patscnt)] = string.Format("{0:n0}", patscnt);
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

                switch (srt)
                {
                    case "date":
                        publications = asc.Value ?
                            Publications.OrderBy(p => p.Date.Year) :
                            Publications.OrderByDescending(p => p.Date.Year);
                        break;
                    case "title":
                        publications = asc.Value ?
                            Publications.OrderBy(p => p.Title) :
                            Publications.OrderByDescending(p => p.Title);
                        break;
                    default:
                        publications = asc.Value ?
                            Publications.OrderBy(p => p.Source) :
                            Publications.OrderByDescending(p => p.Source);
                        break;
                }
            }

            stopWatch.Stop();
            ViewData["runtime"] = stopWatch.ElapsedMilliseconds;
            ViewData["interval"] = Convert.ToInt32(stopWatch.ElapsedMilliseconds / 500);
            return View(publications);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Translate([FromServices] INodeServices nodeServices)
        {
            var result = await nodeServices.InvokeAsync<string>("./wwwroot/js/translate.js", "hello", "es");
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
            var conicyt1 = new Dictionary<string, string>()
            {
                { "fondap", $"http://www.conicyt.cl/fondap/category/concursos/?estado={stt}" },
                { "becasconicyt", $"http://www.conicyt.cl/becasconicyt/category/fichas-concursos/?estado={stt}" },
                { "fondecyt", $"http://www.conicyt.cl/fondecyt/category/concursos/fondecyt-regular/?estado={stt}" },
                { "fondequip", $"http://www.conicyt.cl/fondequip/category/concursos/?estado={stt}" }
            };

            var conicyt2 = new Dictionary<string, string>()
            {
                { "fondef", "http://www.conicyt.cl/fondef/" },
                { "fonis", "http://www.conicyt.cl/fonis/" },
                { "pia", "http://www.conicyt.cl/pia/" },
                { "regional", "http://www.conicyt.cl/regional/" },
                { "informacioncientifica", "http://www.conicyt.cl/informacioncientifica/" },
                { "pai", "http://www.conicyt.cl/pai/" },
                { "pci", "http://www.conicyt.cl/pci/" },
                { "explora", "http://www.conicyt.cl/explora/" }
            };

            // páginas CONICYT 1
            var conicyt1_funds = fund.Intersect(conicyt1.Keys);
            if (conicyt1_funds.Count() != 0)
            {
                foreach (string fondo in conicyt1_funds)
                {
                    try
                    {
                        using(IHtmlDocument bc_doc = await GetDoc(conicyt1[fondo]).ConfigureAwait(false))
                        {
                            var co = GetCo("conicyt");
                            Agendas.AddRange(from n in bc_doc.QuerySelectorAll("div.lista_concurso")
                                             let cells = n.Children
                                             let title = cells.ElementAt(0).QuerySelector("a")
                                             select new AgendaVM
                                             {
                                                 Company = co,
                                                 Fund = fondo.ToUpper() + " (" + bc_doc.QuerySelector("a[rel='home'] span").TextContent + ")",
                                                 Title = title.InnerHtml,
                                                 MainUrl = GetUri(title),
                                                 Start = GetDateAgenda(cells[1]),
                                                 End = GetDateAgenda(cells[2])
                                             });
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            //páginas CONICYT 2
            var conicyt2_funds = fund.Intersect(conicyt2.Keys);
            //$postParams = @{valtab='evaluacion';blogid='20'}
            //Invoke-WebRequest -UseBasicParsing http://www.conicyt.cl/fondef/wp-content/themes/fondef/ajax/getpostconcursos.php -Method POST -Body $postParams

            if (conicyt2_funds.Count() != 0)
            {
                foreach (string fondo in conicyt2_funds)
                {
                    try
                    {
                        var values = new Dictionary<string, string>
                                {
                                    { "valtab", stt },
                                    { "blogid", "20" }
                                };
                        var content = new FormUrlEncodedContent(values);
                        using (HttpClient bc = new HttpClient())
                        using (HttpResponseMessage response = await bc.PostAsync($"{conicyt2[fondo]}wp-content/themes/fondef/ajax/getpostconcursos.php", content))
                        {
                                HtmlDocument bc_doc = new HtmlDocument();
                            bc_doc.Load(await response.Content.ReadAsStreamAsync());
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
                            string Acrn = fondo.ToUpper();
                            foreach (HtmlNode entry in bc_entrys)
                            {
                                try
                                {
                                    Regex ress1 = new Regex(@"[\d-]+");
                                    var Entry = new AgendaVM()
                                    {
                                        Company = _context.Company.SingleOrDefault(c => c.Acronym == "CONICYT".ToLower()),
                                        Fund = Acrn + " (" + Fund + ")",
                                        Title = entry.SelectSingleNode(".//h4").InnerHtml,
                                        MainUrl = GetUri(entry),
                                    };
                                    string[] formats = { "yyyy", "yyyy-MM", "d-MM-yyyy" };
                                    DateTime.TryParseExact(ress1.Match(entry.SelectSingleNode(".//p").InnerHtml).ToString(),
                                                            formats,
                                                            CultureInfo.InvariantCulture,
                                                            DateTimeStyles.None,
                                                            out DateTime Date);
                                    Entry.End = Date;
                                    Agendas.Add(Entry);
                                }
                                catch { continue; }
                            }
                        }
                    }
                    catch { continue; }
                }
            }

            //CORFO DIVIdIR POR REGION Y ACTOR?
            Regex ress = new Regex(@"corfo\d+");
            var corfo_funds = fund.Where(item => ress.IsMatch(item));
            if (corfo_funds.Count() != 0)
            {
                foreach (string fondo in corfo_funds)
                {
                    try
                    {
                        var corfo = "https://www.corfo.cl/sites/cpp/programas-y-convocatorias?p=1456407859853-1456408533016-1456408024098-1456408533181&at=&et=&e=&o=&buscar_resultado=&bus=&r=";
                        var num = fondo.Replace("corfo", "");
                        using (HttpClient bc = new HttpClient())
                        using (HttpResponseMessage bc_result = await bc.GetAsync(corfo + num))
                        {
                            HtmlDocument bc_doc = new HtmlDocument();
                            bc_doc.Load(await bc_result.Content.ReadAsStreamAsync());
                            HtmlNodeCollection bc_entrys = bc_doc.DocumentNode.SelectNodes("//div[contains(@class, 'col-sm-12') and contains(@class, 'areas')]/a");
                            foreach (HtmlNode entry in bc_entrys)
                            {
                                if (entry.InnerHtml.Contains("Cerradas"))
                                {
                                    if (stt == "abierto" || stt == "proximo")
                                    {
                                        continue;
                                    }

                                    if ((entry.InnerHtml.Contains("En Evaluación") && stt != "evaluacion") || (!entry.InnerHtml.Contains("En Evaluación") && stt == "evaluacion"))
                                    {
                                        continue;
                                    }
                                }

                                var Entry = new AgendaVM()
                                {
                                    Company = _context.Company.SingleOrDefault(c => c.Acronym == "CORFO".ToLower()),
                                    Fund = "CORFO",
                                    Title = entry.SelectSingleNode(".//h4").InnerHtml,
                                    MainUrl = new Uri(new Uri(corfo + num), entry.Attributes["href"].Value),
                                    Description = StringManipulations.HtmlToPlainText(entry.SelectSingleNode(".//div[@class='col-md-9 col-sm-8']").InnerHtml),
                                };

                                try
                                {
                                    Regex ress2 = new Regex(@"[\d\/]+");
                                    string[] formats = { "dd/MM/yyyy" };
                                    DateTime.TryParseExact(ress2.Match(entry.SelectNodes(".//li")[2].InnerHtml).ToString(),
                                                            formats,
                                                            CultureInfo.InvariantCulture,
                                                            DateTimeStyles.None,
                                                            out DateTime Date);
                                    Entry.End = Date;
                                }
                                catch
                                {
                                    //Entry.End = entry.InnerHtml.Contains("Disponible todo el año") ? null : null;
                                }
                                Agendas.Add(Entry);
                            }
                        }
                    }
                    catch { }
                }
            }

            ViewData["any"] = Agendas.Count > 0;
            ViewData["fund"] = fund;
            ViewData["conicyt1"] = conicyt1;
            ViewData["conicyt2"] = conicyt2;
            ViewData["stt"] = string.IsNullOrEmpty(stt) ? "" : stt.ToString();
            ViewData["regiones"] = from c in _context.Region select c;
            //render
            stopWatch.Stop();
            ViewData["runtime"] = stopWatch.ElapsedMilliseconds.ToString();
            ViewData["interval"] = Convert.ToInt32(stopWatch.ElapsedMilliseconds / 500);
            return View(Agendas);
        }

        public Task<(IEnumerable<PublicationVM>, string, int)[]> GetPubsAsync(
            string[] src, string q, int rpp, int? pg, string sort_by, string order, int srt_uct, string srt_uach, int ggl)
        {
            var uchile = GetUchileAsync
                (
                src,
                //23s
                //sort_by   dc.date.issued_dt   dc.title_sort   score
                //order     asc                 desc
                $"http://repositorio.uchile.cl/discover?filtertype_1=type&filter_relational_operator_1=equals&filter_1=Tesis&submit_apply_filter=&query={q}&rpp={rpp}&page={pg}&sort_by={sort_by}&order={order}",
                "p.pagination-info", 2,
                "div#aspect_discovery_SimpleSearch_div_search-results > div",
                "a",
                "span.ds-dc_contributor_author-authority",
                "div.artifact-info > span.publisher-date > span.date"
                );

            var ula = GetUlaAsync
                (
                src,
                $"http://medioteca.ulagos.cl/biblioscripts/titulo_claves.idc?texto={q}",
                "font[face='Arial']:has(> a)",
                "a",
                "font > small:only-child",
                pg, rpp
                );

            var umag = GetUmagAsync
                (
                src,
                $"http://www.bibliotecadigital.umag.cl/discover?query={q}&rpp={rpp}&page={pg}",
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
                $"http://repositoriodigital.ucsc.cl/discover?scope=25022009/6&submit=&query={q}&rpp={rpp}&page={pg}&sort_by={sort_by}&order={order}",
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
                //sort_by   0(relevance)       1(title)     2(issue date)    3(submit date)
                //order DESC ASC
                "http://repositoriodigital.uct.cl/advanced-search?conjunction1=AND&field1=ANY&num_search_field=3&etal=0&rpp=" +
                    $"{rpp}&results_per_page={rpp}&page={pg}&query1={q}&sort_by={srt_uct}&order={order.ToUpper()}",
                "#aspect_artifactbrowser_AdvancedSearch_p_result-query", 0,
                "ul.ds-artifact-list > li > div:not(.artifact-preview)",
                "div.artifact-title > a",
                "div.artifact-info > a:not(.enlacerecursivo)",
                "span.ds-dc_contributor_author-authority"
                );

            var uach = GetUachAsync
                (
                src,
                //14s
                //sf        ftitre      fauteur     contributeur        udate       sdxscore
                "http://cybertesis.uach.cl/sdx/uach/resultats-filtree.xsp?biblio_op=or&figures_op=or&tableaux_op=or&citations_op=or&notes_op=or&base=documents&position=2&texte_op=or&titres=" +
                    $"{q}&tableaux={q}&figures={q}&biblio={q}&notes={q}&citations={q}&texte={q}&hpp={rpp}&p={pg}&sf={srt_uach}",
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
                "http://repositorio.udec.cl/discover?group_by=none&etal=0&rpp=" +
                                $"{rpp}&page={pg}&query={q}&sort_by={sort_by}&order={order}",
                "h2.ds-div-head:has(> span)", 1,
                "ul.ds-artifact-list > ul > li > div.artifact-description",
                "div.artifact-title > a",
                "div.artifact-info > span.author > span",
                "div.artifact-info > span.publisher-date > span.date"
            );

            var pucv = GetPucvAsync
                (
                    src,
                    $"http://opac.pucv.cl/cgi-bin/wxis.exe/iah/scripts/?IsisScript=iah.xis&lang=es&base=BDTESIS&nextAction=search&exprSearch={q}&isisFrom={(pg - 1) * rpp + 1}",
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
                $"https://repositorio.uc.cl/discover?scope=11534/1&group_by=none&etal=0&rpp={rpp}&page={pg}&query={q}&sort_by={sort_by}&order={order}&submit=Go",
                "//h2[@class='ds-div-head' and span]", 1,
                "//ul[@class='ds-artifact-list']/ul/li/div[@class='artifact-description']",
                ".//div[@class='artifact-title']/a",
                ".//div[@class='artifact-info']/span[@class='publisher-date']/span[@class='date']",
                ".//div[@class='artifact-info']/span[@class='author']/span"
            );

            var fondecyt = GetConicyt(src, "FONDECYT", "108045", rpp, sort_by, order, pg, q);
            var fondef = GetConicyt(src, "FONDEF", "108046", rpp, sort_by, order, pg, q);
            var fondap = GetConicyt(src, "FONDAP", "108044", rpp, sort_by, order, pg, q);
            var pia = GetConicyt(src, "PIA", "108042", rpp, sort_by, order, pg, q);
            var regional = GetConicyt(src, "REGIONAL", "108050", rpp, sort_by, order, pg, q);
            var becas = GetConicyt(src, "BECAS", "108040", rpp, sort_by, order, pg, q);
            var conicyt = GetConicyt(src, "CONICYT", "108088", rpp, sort_by, order, pg, q);
            var proyectos = GetConicyt(src, "PROYECTOS", "93475", rpp, sort_by, order, pg, q);

            var fipa = GetFipaAsync(src,
                $"http://subpesca-engine.newtenberg.com/mod/find/cgi/find.cgi?action=query&engine=SwisheFind&rpp={rpp}&cid=514&stid=&iid=613&grclass=&pnid=&pnid_df=&pnid_tf=&pnid_search=678,682,683,684,681,685,510,522,699,679&limit=200&searchon=&channellink=w3:channel&articlelink=w3:article&pvlink=w3:propertyvalue&notarticlecid=&use_cid_owner_on_links=&show_ancestors=1&show_pnid=1&cids=514&keywords={q}&start={(pg - 1) * rpp}&group=0&expanded=1&searchmode=undefined&prepnidtext=&javascript=1",
                "p.PP", 2, "li > a");

            var corfo = GetCorfoAsync(src,
                //order     DESC            ASC
                //sort_by   dc.title_sort
                //group_by=none
                "http://repositoriodigital.corfo.cl/discover?query=" +
                    $"{q}&rpp={rpp}&page={pg}&group_by=none&etal=0&sort_by={sort_by.Replace(".issued", "")}&order={order.ToUpper()}",
                "p.pagination-info", 2,
                "div.artifact-description", "a",
                "span.author > small",
                "span.date", "div.abstract"
                );

            var gscholar = GetGscholarAsync(src,
                $"https://scholar.google.com/scholar?q={q}&start={rpp * ( pg - 1) + 1}&scisbd={ggl}",
                "div.gs_ab_mdw:has(> b)",
                "div.gs_ri",
                "a",
                "h3.gs_rt",
                "div.gs_a", rpp, "gscholar"
                );

            var gpatents = GetGscholarAsync(src,
                $"https://scholar.google.cl/scholar?as_q={q}" +
                    "&as_epq=&as_oq=&as_eq=&as_occt=any&as_sauthors=&as_publication=Google+Patents&as_ylo=&as_yhi=&btnG=&hl=en&as_sdt=0%2C5&as_vis=1" +
                    $"&start={rpp * (pg - 1) + 1}&scisbd={ggl}",
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
string url, string NoResultsSelect, string nodeSelect, string uriSelect, string titleSelect,
string dateSelect, int rpp, string acronym)
        {
            if (src.Contains(acronym))
            {
                try
                {
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
                    using (var doc = await GetDoc(url))
                        return (from n in doc.QuerySelectorAll(nodeSelect).Take(rpp)
                                let t = n.QuerySelector(titleSelect).TextContent
                                select new PublicationVM()
                                {
                                    Source = acronym,
                                    Uri = GetUri(n.QuerySelector(uriSelect)),
                                    Title = t.Substring(t.LastIndexOf(']') + 1),
                                    Typep = Typep.Articulo,
                                    Company = co,
                                    Date = GetDateGS(n, dateSelect),
                                    Authors = GetAuthorsGS(n, dateSelect)
                                }, acronym, GetNoResultsGS(doc, NoResultsSelect));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetCorfoAsync(string[] src,
string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string uriSelect,
string authorSelect, string dateSelect, string abstractSelect)
        {
            var acronym = "CORFO";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(60706000);
                    using (var doc = await GetDoc(url))
                        return (from n in doc.QuerySelectorAll(nodeSelect)
                                let t = n.QuerySelector(uriSelect)
                                select new PublicationVM()
                                {
                                    Source = acronym,
                                    Uri = GetUri(url, t),
                                    Title = t.TextContent,
                                    Typep = Typep.Proyecto,
                                    Company = co,
                                    Date = GetDate(n, dateSelect),
                                    Authors = GetAuthorsCorfo(n, authorSelect),
                                    Abstract = GetAbstract(n, abstractSelect)
                                }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetFipaAsync(string[] src,
string url, string NoResultsSelect, int NoResultsPos, string nodeSelect)
        {
            var acronym = "FIPA";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(60719000);
                    using (var doc = await GetDoc(url))
                        return (from n in doc.QuerySelectorAll(nodeSelect)
                                select new PublicationVM()
                                {
                                    Source = acronym,
                                    Title = n.TextContent,
                                    Typep = Typep.Proyecto,
                                    Uri = GetUri("http://www.subpesca.cl/fipa/613/w3-article-88970.html", n),
                                    Company = co,
                                }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public static string ColorToHex(Color color)
        {
            return "#" + color.R.ToString("X2") +
                         color.G.ToString("X2") +
                         color.B.ToString("X2");
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
string acronym, string parameter, int rpp, string sort_by, string order, int? pg, string q)
        {
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(60915000);
                    var url = $"http://repositorio.conicyt.cl/handle/10533/{parameter}/discover?query={q}&page={pg - 1}&rpp={rpp}&sort_by={sort_by}&order={order}";
                    using (var doc = await GetDoc(url))
                        return (doc.QuerySelectorAll("div.row.ds-artifact-item")
                            .Select(n => new PublicationVM()
                            {
                                Source = acronym,
                                Title = n.QuerySelector("h4.title-list").TextContent,
                                Typep = Typep.Proyecto,
                                Uri = GetUri(url, n.QuerySelector("div.artifact-description > a")),
                                Authors = GetAuthors(n, "span.ds-dc_contributor_author-authority"),
                                Date = GetDate(n, "span.date"),
                                Company = co,
                                Journal = GetJournalConicyt(n)
                            }), acronym, GetNoResults(doc, "p.pagination-info", 2));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetPucAsync(string[] src,
string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string uriSelect,
string dateSelect, string authorSelect)
        {
            var acronym = "puc";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    var doc = await GetDocXPath(url);
                    return (from n in doc.DocumentNode.SelectNodes(nodeSelect)
                            let t = n.SelectSingleNode(uriSelect)
                            select new PublicationVM()
                            {
                                Source = acronym,
                                Title = t.InnerText,
                                Typep = Typep.Tesis,
                                Uri = GetUri(url, t),
                                Authors = GetAuthors(n, authorSelect),
                                Date = GetDate(n, dateSelect),
                                Company = co,
                            }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetPucvAsync(string[] src,
string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string dateSelect,
string uriSelect, string uriSelectAlt, string titleSelect, string authorSelect, int rpp)
        {
            var acronym = "pucv";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using (var doc = await GetDocStream(url))
                        return (from n in doc.QuerySelectorAll(nodeSelect).Take(rpp)
                                let date = n.QuerySelector(dateSelect).Text()
                                select new PublicationVM()
                                {
                                    Typep = Typep.Tesis,
                                    Source = acronym,
                                    Title = n.QuerySelector(titleSelect).TextContent,
                                    Uri = GetUri(url, n.QuerySelector(uriSelect), n.QuerySelector(uriSelectAlt)),
                                    Authors = GetAuthors(n, authorSelect),
                                    Date = GetDate(date, date.Length - 4),
                                    Company = co,
                                }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUdecAsync(string[] src,
string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string uriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "udec";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using (var doc = await GetDoc(url))
                        return (
                            from n in doc.QuerySelectorAll(nodeSelect)
                            let m = n.QuerySelector(uriSelect)
                            select new PublicationVM()
                            {
                                Typep = Typep.Tesis,
                                Source = acronym,
                                Title = m.TextContent,
                                Uri = GetUri(url, m),
                                Authors = GetAuthors(n, authorSelect),
                                Company = co,
                                Date = GetDate(n, dateSelect)
                            }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUachAsync(string[] src,
    string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string titleSelect, 
    string uriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "uach";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using (var doc = await GetDoc(url))
                        return (doc.QuerySelectorAll(nodeSelect).Select(n => new PublicationVM()
                        {
                            Source = acronym,
                            Title = n.QuerySelector(titleSelect).TextContent,
                            Uri = GetUri(url, n.QuerySelector(uriSelect)),
                            Authors = GetAuthors(n, authorSelect),
                            Typep = Typep.Tesis,
                            Company = co,
                            Date = GetDate(n, dateSelect)
                        }), acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUctAsync(string[] src,
string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string uriSelect, string journalSelect, string authorSelect)
        {
            var acronym = "uct";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    Regex regex = new Regex("[a-zA-Z]");
                    using (var doc = await GetDoc(url))
                    return (
                        from n in doc.QuerySelectorAll(nodeSelect)
                        let m = n.QuerySelector(uriSelect)
                        let j = n.QuerySelector(journalSelect).TextContent
                        select new PublicationVM()
                        {
                            //otros
                            Typep = Typep.Tesis,
                            Source = acronym,
                            Title = m.TextContent,
                            Uri = GetUri(url, m),
                            Journal = j,
                            Authors = GetAuthors(n, authorSelect, regex),
                            Company = co,
                            Date = GetDate(j, j.LastIndexOf(",") + 2)
                        }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUcscAsync(string[] src,
string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string uriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "ucsc";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using (var doc = await GetDoc(url))
                        return (
                            from n in doc.QuerySelectorAll(nodeSelect)
                            let m = n.QuerySelector(uriSelect)
                            select new PublicationVM()
                            {
                                Source = acronym,
                                Title = m.TextContent,
                                Uri = GetUri(url, m),
                                Authors = GetAuthors(n, authorSelect),
                                Typep = Typep.Tesis,
                                Company = co,
                                Date = GetDate(n, dateSelect)
                            }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUmagAsync(string[] src,
    string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string uriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "umag";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using (var doc = await GetDoc(url))
                        return (from n in doc.QuerySelectorAll(nodeSelect)
                                let m = n.QuerySelector(uriSelect)
                                select new PublicationVM()
                                {
                                    Source = acronym,
                                    Title = m.TextContent,
                                    Uri = GetUri(url, m),
                                    Authors = GetAuthors(n, authorSelect),
                                    Typep = Typep.Tesis,
                                    Company = co,
                                    Date = GetDate(n, dateSelect)
                                }, acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUchileAsync(string[] src, 
            string url, string NoResultsSelect, int NoResultsPos, string nodeSelect, string uriSelect, string authorSelect, string dateSelect)
        {
            var acronym = "uchile";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using (var doc = await GetDoc(url))
                        return (doc.QuerySelectorAll(nodeSelect).Select(n => new PublicationVM()
                        {
                            Source = acronym,
                            Title = n.TextContent,
                            Uri = GetUri(url, n.QuerySelector(uriSelect)),
                            Authors = GetAuthors(n, authorSelect),
                            //Typep = GetTypep(n.QuerySelector("span.tipo_obra").Text().ToLower()),
                            Typep = Typep.Tesis,
                            Company = co,
                            Date = GetDate(n, dateSelect)
                        }), acronym, GetNoResults(doc, NoResultsSelect, NoResultsPos));
                }
                catch
                {

                }
            }
            return (new List<PublicationVM>(), acronym, 0);
        }

        public async Task<(IEnumerable<PublicationVM>, string, int)> GetUlaAsync(string[] src,
    string url, string nodeSelect, string uriSelect, string authorSelect, int? pg, int rpp)
        {
            var acronym = "ula";
            if (src.Contains(acronym))
            {
                try
                {
                    var co = GetCo(acronym);
                    using (var doc = await GetDocStream(url))
                    {
                        var num = doc.QuerySelectorAll(nodeSelect);
                        return (num.Skip(rpp * (pg.Value - 1)).Take(rpp).Select(n => new PublicationVM()
                        {
                            Typep = Typep.Tesis,
                            Source = acronym,
                            Title = n.QuerySelector(uriSelect).TextContent,
                            Uri = GetUri(url, n.QuerySelector(uriSelect)),
                            Authors = GetAuthors(n, authorSelect),
                            Company = co,
                        }), acronym, num.Count());
                    }
                }
                catch
                {

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
                    var titls = node.QuerySelector("h4.discoUch span").Attributes["title"].Value;
                    List<int> indexes = titls.AllIndexesOf("rft_id");
                    if (indexes.Count == 3)
                    {
                        return (QueryHelpers.ParseQuery(titls.Substring(indexes[0], indexes[1] - indexes[0]))["rft_id"],
                        doi + QueryHelpers.ParseQuery(titls.Substring(indexes[1], indexes[2] - indexes[1]))["rft_id"].ToString().ToLower().Replace("doi: ", ""));
                    }
                    return (null, null);
                default:
                    return (null, null);
            }
        }

        public static string GetJournalConicyt(IElement node)
        {
            try
            {
                var items = node.QuerySelectorAll("#code");
                var journal = "N° de Proyecto: " + items[0].TextContent;
                try
                {
                    return journal + " Institución Responsable: " + items[3].TextContent;
                }
                catch
                {
                    return journal;
                }
            }
            catch
            {
                return null;
            }
        }

        public static Typep GetTypep(string type)
        {
            switch (type)
            {
                case "tesis":
                    return Typep.Tesis;
                case "artículo":
                    return Typep.Articulo;
                default:
                    return Typep.Desconocido;
            }
        }

        public static Uri GetUri(string rep, IElement link)
        {
            try
            {
                return new Uri(new Uri(rep), link.Attributes["href"].Value);
            }
            catch
            {
                return null;
            }
        }

        public static Uri GetUri(string rep, HtmlNode link)
        {
            try
            {
                return new Uri(new Uri(rep), link.Attributes["href"].Value);
            }
            catch
            {
                return null;
            }
        }

        public static Uri GetUri(string rep, IElement link, IElement backlink)
        {
            try
            {
                return new Uri(new Uri(rep), link == null ? backlink.Attributes["href"].Value : link.Attributes["href"].Value);
            }
            catch
            {
                return null;
            }
        }

        public static Uri GetUri(IElement link)
        {
            try
            {
                return new Uri(link.Attributes["href"].Value);
            }
            catch
            {
                return null;
            }
        }

        public static Uri GetUri(HtmlNode link)
        {
            try
            {
                return new Uri(link.Attributes["href"].Value);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<IHtmlDocument> GetDoc(string rep)
        {
            var parser = new HtmlParser();
            using (HttpClient hc = new HttpClient())
                return await parser.ParseDocumentAsync(await hc.GetStringAsync(rep));
        }

        public static async Task<IHtmlDocument> GetDocStream(string rep)
        {
            var parser = new HtmlParser();
            using (HttpClient hc = new HttpClient())
                return await parser.ParseDocumentAsync(await hc.GetStreamAsync(rep));
        }

        public static async Task<HtmlDocument> GetDocXPath(string rep)
        {
            var doc = new HtmlDocument();
            using (HttpClient hc = new HttpClient())
                doc.Load(await hc.GetStreamAsync(rep));
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
            try
            {
                Regex res = new Regex(@"([0-9]+,)*[0-9]+");
                int.TryParse(res.Match(doc.QuerySelector(selector).TextContent).Value.Replace(",", ""), out int result);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetNoResults(IHtmlDocument doc, string selector, int pos)
        {
            try
            {
                Regex res = new Regex(@"[\d\.,]+");
                int.TryParse(res.Matches(doc.QuerySelector(selector).TextContent)[pos].Value, out int result);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetNoResults(HtmlDocument doc, string selector, int pos)
        {
            try
            {
                Regex res = new Regex(@"[\d\.,]+");
                int.TryParse(res.Matches(doc.DocumentNode.SelectSingleNode(selector).InnerText)[pos].Value, out int result);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        public static DateTime GetDate(HtmlNode node, string selector)
        {
            try
            {
                Regex res = new Regex(@"[\d\-]+");
                string[] formats = { "yyyy", "yyyy-MM" };
                DateTime.TryParseExact(res.Match(node.SelectSingleNode(selector).InnerText).Value,
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime Date);
                return Date;
            }
            catch
            {
                return new DateTime();
            }
        }

        public static DateTime GetDateGS(IElement node, string selector)
        {
            try
            {
                Regex res = new Regex(@"[\d]+");
                string[] formats = { "yyyy" };
                var tmp = node.QuerySelector(selector).TextContent;
                var tmp2 = res.Match(tmp).Value;
                DateTime.TryParseExact(res.Match(node.QuerySelector(selector).TextContent).Value,
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime Date);
                return Date;
            }
            catch
            {
                return new DateTime();
            }
        }

        public static DateTime GetDate(IElement node, string selector)
        {
            try
            {
                Regex res = new Regex(@"[\d\-]+");
                string[] formats = { "yyyy", "yyyy-MM", "yyyy-MM-dd" };
                var tmp = node.QuerySelector(selector).TextContent;
                var tmp2 = res.Match(tmp).Value;
                DateTime.TryParseExact(res.Match(node.QuerySelector(selector).TextContent).Value,
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime Date);
                return Date;
            }
            catch
            {
                return new DateTime();
            }
        }

        public static DateTime GetDateAgenda(IElement node)
        {
            string[] formats = { "dd 'de' MMMM 'de'  yyyy" };
            Regex ress1 = new Regex(@"\d[\dA-Za-z\s]+\d");
            DateTime.TryParseExact(ress1.Match(node.TextContent).Value,
                formats,
                CultureInfo.GetCultureInfo("es-CL"),
                DateTimeStyles.None,
                out DateTime Date);
            return Date;
        }

        public static DateTime GetDate(string journal, int start)
        {
            string[] formats = { "yyyy" };
            DateTime.TryParseExact(journal.Substring(start,4),
                                    formats,
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime Date);
            return Date;
        }

        public static string GetAbstract(IElement node, string selector)
        {
            try
            {
                return node.QuerySelector(selector).TextContent;
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<AuthorVM> GetAuthorsGS(IElement node, string selector)
        {
            try
            {
                Regex aut = new Regex(@"\A(?:(?![0-9]{4}).)*");
                return aut.Match(node.QuerySelector(selector).TextContent).Value.Trim().Trim('-').Split(',')
                    .Select(a => a.Split(' '))
                    .Select(nn =>
                    new AuthorVM
                    {
                        Last = nn[0],
                        Name = nn.Count() > 1 ? nn[1] : ""
                    });
            }
            catch
            {
                return new List<AuthorVM>();
            }
        }

        public static IEnumerable<AuthorVM> GetAuthorsCorfo(IElement node, string selector)
        {
            try
            {
                return node.QuerySelector(selector).TextContent.Split(';')
                    .Select(nn =>
                    new AuthorVM
                    {
                        Name = nn
                    });
            }
            catch
            {
                return new List<AuthorVM>();
            }
        }

        public static IEnumerable<AuthorVM> GetAuthors(IElement node, string selector)
        {
            try
            {
                return node.QuerySelectorAll(selector)
                    .Select(a => a.TextContent.TrimEnd('.').Split(','))
                    .Select(nn =>
                    new AuthorVM
                    {
                        Last = nn[0],
                        Name = nn.Count() > 1 ? nn[1] : ""
                    });
            }
            catch
            {
                return new List<AuthorVM>();
            }
        }

        public static IEnumerable<AuthorVM> GetAuthors(HtmlNode node, string selector)
        {
            try
            {
                return node.SelectNodes(selector)
                    .Select(a => a.InnerText.TrimEnd('.').Split(','))
                    .Select(nn =>
                    new AuthorVM
                    {
                        Last = nn[0],
                        Name = nn.Count() > 1 ? nn[1] : ""
                    });
            }
            catch
            {
                return new List<AuthorVM>();
            }
        }

        public static IEnumerable<AuthorVM> GetAuthors(IElement node, string selector, Regex filter)
        {
            return node.QuerySelectorAll(selector)
                .Where(i => filter.IsMatch(i.TextContent))
                .Select(a => a.TextContent.Split(','))
                .Select(nn =>
                new AuthorVM
                {
                    Last = nn[0],
                    Name = nn.Count() > 1 ? nn[1] : ""
                });
        }
    }
}
