using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(BiblioMit.Areas.Identity.IdentityHostingStartup))]
namespace BiblioMit.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder?.ConfigureServices((context, services) => {
            });
        }
    }
}