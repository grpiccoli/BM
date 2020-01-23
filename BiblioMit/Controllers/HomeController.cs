using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using BiblioMit.Models.PostViewModels;
using BiblioMit.Models.ForumViewModels;
using System;
using BiblioMit.Models.HomeViewModels;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.NodeServices;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using BiblioMit.Models.Entities;
using System.Globalization;

namespace BiblioMit.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        //private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IPost _postService;
        private readonly INodeServices _nodeService;

        public HomeController(
            //IStringLocalizer<HomeController> localizer,
            IPost postService
            , INodeServices nodeService
            )
        {
            //_localizer = localizer;
            _postService = postService;
            _nodeService = nodeService;
        }

        [AllowAnonymous]
        public IActionResult Manual()
        {
            var builder = new UriBuilder
            {
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                Path = "MANUAL_DE_USO_BIBLIOMIT/MANUAL_DE_USO_BIBLIOMIT.html"
            };
            return Redirect(builder.Uri.ToString());
        }

        public IActionResult Analytics()
        {
            return View();
        }

        public IActionResult GetAnalyticsData(string freq)
        {
            using (var service = GetService())
            {
                var st = new DateTime(2018, 8, 28);

                var request = new GetReportsRequest
                {
                    ReportRequests = new[]
                    {
                    new ReportRequest
                    {
                        ViewId = "180792983",
                        Metrics = new[] { new Metric { Expression = "ga:entrances" } },
                        Dimensions = new[]
                        {
                            new Dimension { Name = "ga:landingPagePath" },
                            new Dimension { Name = "ga:date" }
                        },
                        DateRanges = new[] { new DateRange { StartDate = st.ToString("yyyy-MM-dd"), EndDate = "today" } },
                        OrderBys = new [] { new OrderBy { FieldName = "ga:date", SortOrder = "ASCENDING" } }
                    }
                }
                };

                var batchRequest = service.Reports.BatchGet(request);
                var response = batchRequest.Execute();

                var logins =
                    response.Reports.First().Data.Rows
                    .Select(r =>
                    {
                        DateTime.TryParseExact(r.Dimensions[1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d);
                        return new
                        {
                            month = d.ToString("yyyy-MM"),
                            date = d.ToString("yyyy-MM-dd"),
                            views = int.Parse(r.Metrics.First().Values.First())
                        };
                    });

                switch (freq)
                {
                    case "day":
                        return Json(logins);
                    case "month":
                        return Json(logins.GroupBy(l => l.month).Select(g => new { date = g.Key, views = g.Sum(s => s.views) }));
                    default:
                    case "all":
                        return Json(logins.Sum(l => l.views));
                }
            }
        }

        public static AnalyticsReportingService GetService()
        {
            var credential = GetCredential();

            return new AnalyticsReportingService(new AnalyticsReportingService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "BiblioMit",
            });
        }

        public static GoogleCredential GetCredential()
        {
            using (var stream = new FileStream("BiblioMit-cb7f4de3a209.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream)
                    .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly);
            }
        }

        [HttpPost]
		public async Task<IActionResult> Translate(string text, string to)
        {
            var translated = await _nodeService.InvokeAsync<string>("./wwwroot/js/translate.js", text, to);
            return Json(translated);
        }
		
        [HttpGet]
        public IActionResult Search()
        {
            return PartialView("_CustomSearch");
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["Url"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            return View();
        }
        [AllowAnonymous]
        public IActionResult Forum()
        {
            var model = BuildHomeIndexModel();
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Results(string searchQuery)
        {
            var posts = _postService.GetFilteredPosts(searchQuery);
            var noResults = (!string.IsNullOrEmpty(searchQuery) && !posts.Any());
            var postListings = posts.Select(p => new PostListingModel
            {
                Id = p.Id,
                AuthorId = p.User.Id,
                AuthorName = p.User.UserName,
                AuthorRating = p.User.Rating,
                Title = p.Title,
                DatePosted = p.Created.ToString(),
                RepliesCount = p.Replies.Count(),
                Forum = BuildForumListing(p)
            });

            var model = new SearchResultModel
            {
                Posts = postListings,
                SearchQuery = searchQuery,
                EmptySearchResults = noResults
            };
            return View(model);
        }

        private ForumListingModel BuildForumListing(Post p)
        {
            var forum = p.Forum;
            return new ForumListingModel
            {
                Id = forum.Id,
                ImageUrl = forum.ImageUrl,
                Name = forum.Title,
                Description = forum.Description
            };
        }

        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            return RedirectToAction("Results", new { searchQuery });
        }

        private HomeIndexModel BuildHomeIndexModel()
        {
            var latestsPosts = _postService.GetLatestsPosts(5);

            var posts = latestsPosts.Select(p => new PostListingModel
            {
                Id = p.Id,
                Title = p.Title,
                AuthorId = p.UserId,
                AuthorName = p.User.Name,
                AuthorRating = p.User.Rating,
                DatePosted = p.Created.ToString(),
                RepliesCount = p.Replies.Count(),
                Forum = GetForumListingForPost(p)
            });
            return new HomeIndexModel
            {
                LatestPosts = posts,
                SearchQuery = ""
            };
        }

        private ForumListingModel GetForumListingForPost(Post post)
        {
            var forum = post.Forum;
            return new ForumListingModel
            {
                Name = forum.Title,
                Id = forum.Id,
                ImageUrl = forum.ImageUrl
            };
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            ViewData["Message"] = "Acerca de BiblioMit.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Contáctenos.";

            return View();
        }


        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
