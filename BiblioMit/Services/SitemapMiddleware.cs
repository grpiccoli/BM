using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class SitemapMiddleware
    {
        private RequestDelegate _next;
        private string _rootUrl;
        public SitemapMiddleware(RequestDelegate next, string rootUrl)
        {
            _next = next;
            _rootUrl = rootUrl;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase))
            {
                var stream = context.Response.Body;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/xml";
                string sitemapContent = "<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">";
                var controllers = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type)
                    || type.Name.EndsWith("controller")).ToList();

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
                            sitemapContent += string.Format("<loc>{0}/{1}/{2}</loc>", _rootUrl,
                            controller.Name.ToLower().Replace("controller", ""), method.Name.ToLower());
                            sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
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
                    await memoryStream.CopyToAsync(stream, bytes.Length);
                }
            }
            else
                await _next(context);
        }
    }

    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseSitemapMiddleware(this IApplicationBuilder app,
            string rootUrl = "https://www.bibliomit.cl")
        {
            return app.UseMiddleware<SitemapMiddleware>(new[] { rootUrl });
        }
    }
}
