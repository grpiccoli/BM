using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.IO;

namespace BiblioMit.Extensions
{
    public static class ViewPageExtensions
    {
        private const string BLOCK_BUILDER = "BlockBuilder";

        public static HtmlString Block(this RazorPageBase webPage, Func<dynamic, HelperResult> template, string name)
        {
            var sb = new StringBuilder();
            using TextWriter tw = new StringWriter(sb);
            var encoder = (HtmlEncoder)webPage?.ViewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder));

            if (webPage.ViewContext.HttpContext.Request.Headers["x-requested-with"] != "XMLHttpRequest")
            {
                var scriptBuilder = webPage.ViewContext.HttpContext.Items[name + BLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

                template?.Invoke(null).WriteTo(tw, encoder);
                scriptBuilder.Append(sb.ToString());
                webPage.ViewContext.HttpContext.Items[name + BLOCK_BUILDER] = scriptBuilder;

                return new HtmlString(string.Empty);
            }

            template?.Invoke(null).WriteTo(tw, encoder);

            return new HtmlString(sb.ToString());
        }

        public static HtmlString WriteBlocks(this RazorPageBase webPage, string name)
        {
            var scriptBuilder = webPage?.ViewContext.HttpContext.Items[name + BLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            return new HtmlString(scriptBuilder.ToString());
        }
    }
}
