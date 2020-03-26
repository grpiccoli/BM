using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BiblioMit.Models;
using Microsoft.AspNetCore.Authorization;
using BiblioMit.Models.PostViewModels;
using BiblioMit.Models.ForumViewModels;
using System;
using BiblioMit.Models.HomeViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Globalization;
using BiblioMit.Services;

namespace BiblioMit.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        //private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IPost _postService;
        private readonly INodeService _nodeService;

        public HomeController(
            //IStringLocalizer<HomeController> localizer,
            IPost postService
            , INodeService nodeService
            )
        {
            //_localizer = localizer;
            _postService = postService;
            _nodeService = nodeService;
        }

        public IActionResult Manual()
        {
            return Redirect($"{Request.Scheme}://{Request.Host.Value}/MANUAL_DE_USO_BIBLIOMIT/MANUAL_DE_USO_BIBLIOMIT.html");
        }

        public IActionResult Analytics()
        {
            return View();
        }

        public IActionResult GetAnalyticsData(string freq)
        {
            using var service = GetService();
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
                        DateRanges = new[] { new DateRange { StartDate = st.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), EndDate = "today" } },
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
                        month = d.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                        date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        views = int.Parse(r.Metrics.First().Values.First(), CultureInfo.InvariantCulture)
                    };
                });

            return freq switch
            {
                "day" => Json(logins),
                "month" =>
                    Json(logins.GroupBy(l => l.month).Select(g => new { date = g.Key, views = g.Sum(s => s.views) })),
                _ => Json(logins.Sum(l => l.views))
            };
        }

        public static AnalyticsReportingService GetService()
        {
            var credential = GetCredential();

            return new AnalyticsReportingService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "BiblioMit",
            });
        }

        public static GoogleCredential GetCredential()
        {
            using var stream = new FileStream("BiblioMit-cb7f4de3a209.json", FileMode.Open, FileAccess.Read);
            return GoogleCredential.FromStream(stream)
.CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly);
        }

        [HttpPost]
		public IActionResult Translate(string text, string to)
        {
            var translated = _nodeService.Run("./wwwroot/js/translate.js", new string[] { text, to });
            return Json(translated);
        }
		
        [HttpGet]
        public IActionResult Search()
        {
            return PartialView("_CustomSearch");
        }

        public IActionResult Index()
        {
            ViewData["Url"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            return View();
        }
        public IActionResult Forum()
        {
            var model = BuildHomeIndexModel();
            return View(model);
        }

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
                DatePosted = p.Created.ToString(CultureInfo.InvariantCulture),
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

        private static ForumListingModel BuildForumListing(Post p)
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
            return new HomeIndexModel
            {
                LatestPosts = _postService.GetLatestsPosts(5).Select(p => new PostListingModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorId = p.UserId,
                    AuthorName = p.User.Name,
                    AuthorRating = p.User.Rating,
                    DatePosted = p.Created.ToString(CultureInfo.InvariantCulture),
                    RepliesCount = p.Replies.Count(),
                    Forum = GetForumListingForPost(p)
                }),
                SearchQuery = string.Empty
            };
        }

        private static ForumListingModel GetForumListingForPost(Post post)
        {
            var forum = post.Forum;
            return new ForumListingModel
            {
                Name = forum.Title,
                Id = forum.Id,
                ImageUrl = forum.ImageUrl
            };
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Acerca de BiblioMit.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contáctenos.";

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, Uri redirectUri)
        {
            if (!string.IsNullOrWhiteSpace(culture))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }

            if(redirectUri == null)
            {
                redirectUri = new Uri("~/", UriKind.Relative);
            }

            return LocalRedirect(redirectUri.ToString());
        }
    }
}
