using System;
using System.IO;
using System.Net;
using System.Linq;
using BiblioMit.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BiblioMit.Models;

namespace BiblioMit
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = 200;
                options.Limits.MaxConcurrentUpgradedConnections = 200;
                //options.Limits.MaxRequestBodySize = 20_000_000;
                //options.Limits.MinRequestBodyDataRate =
                //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                //options.Limits.MinResponseDataRate =
                //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
            })
            .UseUrls("http://localhost:5008/")
            .UseStartup<Startup>();
    }
}
