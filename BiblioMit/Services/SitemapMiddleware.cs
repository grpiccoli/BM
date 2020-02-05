using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class SitemapMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Uri _rootUrl;
        public SitemapMiddleware(RequestDelegate next, Uri rootUrl)
        {
            _next = next;
            _rootUrl = rootUrl;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context != null && context.Request.Path.Value.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase))
            {
                var stream = context.Response.Body;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/xml";
                string sitemapContent = "<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">";
                var controllers = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type)
                    || type.Name.EndsWith("controller", StringComparison.CurrentCultureIgnoreCase)).ToList();

                foreach (var controller in controllers)
                {
                    var cnt = 0;
                    var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        var test1 = method.ReturnType.Name == "ActionResult"
                            || method.ReturnType.Name == "IActionResult" || method.ReturnType.Name == "Task`1";
                        var test2 = method.CustomAttributes.Any(c => c.AttributeType == typeof(AllowAnonymousAttribute));

                        if(test1 && test2)
                        {
                            cnt++;
                            sitemapContent += "<url>";
                            sitemapContent += string.Format(CultureInfo.InvariantCulture,
                                "<loc>{0}/{1}/{2}</loc>", _rootUrl,
                            controller.Name.ToUpperInvariant()
                            .Replace("controller", "", StringComparison.CurrentCultureIgnoreCase), 
                            method.Name.ToUpperInvariant());
                            sitemapContent += string.Format(CultureInfo.InvariantCulture,
                                "<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                            sitemapContent += "</url>";
                        }
                    }
                }
                sitemapContent += "</urlset>";
                using (var memoryStream = new MemoryStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(sitemapContent);
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(stream, bytes.Length).ConfigureAwait(false);
                }
            }
            else
                await _next(context).ConfigureAwait(false);
        }
    }

    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseSitemapMiddleware(this IApplicationBuilder app,
            string rootUrl = "https://www.bibliomit.cl")
        {
            return app.UseMiddleware<SitemapMiddleware>(new[] { new Uri(rootUrl) });
        }
    }
}
