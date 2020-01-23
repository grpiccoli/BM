using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public class SeedBackground : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IStringLocalizer _localizer;
        //private Timer _timer;
        public SeedBackground(
            IServiceProvider services,
            IStringLocalizer<SeedBackground> localizer,
            ILogger<SeedBackground> logger)
        {
            _localizer = localizer;
            Services = services;
            _logger = logger;
        }
        public IServiceProvider Services { get; }
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                _localizer["Consume Scoped Service Hosted Service is working."]);

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<ISeed>();

                await scopedProcessingService.Seed().ConfigureAwait(false);
            }

        }
        // noop
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
