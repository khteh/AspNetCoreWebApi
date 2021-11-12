using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace Web.Api.HealthChecks;
internal class StartupHostedService : IHostedService, IDisposable
{
    private readonly int _delaySeconds = 10;
    private readonly ILogger _logger;
    private readonly ReadinessHealthCheck _healthCheck;
    public StartupHostedService(ILogger<StartupHostedService> logger,
            ReadinessHealthCheck healthCheck)
    {
            _logger = logger;
            _healthCheck = healthCheck;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start task.");
        #if false
        IApplicationLifetime.ApplicationStarted sets _healthCheck.StartupTaskCompleted to true
        // Simulate the effect of a long-running startup task.
        Task.Run(async () =>
        {
            await Task.Delay(_delaySeconds * 1000);
            _healthCheck.StartupTaskCompleted = true;
            _logger.LogInformation($"Startup Background Service has started.");
        });
        #endif
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Startup Background Service is stopping.");
        return Task.CompletedTask;
    }
    public void Dispose() => GC.SuppressFinalize(this);
}