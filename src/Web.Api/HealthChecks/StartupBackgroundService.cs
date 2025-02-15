using ILogger = Microsoft.Extensions.Logging.ILogger;
namespace Web.Api.HealthChecks;
internal class StartupBackgroundService : BackgroundService
{
    //private readonly int _delaySeconds = 10;
    private readonly ILogger _logger;
    private readonly ReadinessHealthCheck _healthCheck;
    public StartupBackgroundService(ILogger<StartupBackgroundService> logger,
            ReadinessHealthCheck healthCheck)
    {
        _logger = logger;
        _healthCheck = healthCheck;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(StartupBackgroundService)}.{nameof(ExecuteAsync)}");
        // Simulate the effect of a long-running task.
        //await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        _healthCheck.StartupCompleted = true;
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(StartupBackgroundService)}.{nameof(StartAsync)}");
#if false
        IApplicationLifetime.ApplicationStarted sets _healthCheck.StartupCompleted to true
        // Simulate the effect of a long-running startup task.
        Task.Run(async () =>
        {
            await Task.Delay(_delaySeconds * 1000);
            _healthCheck.StartupCompleted = true;
            _logger.LogInformation($"Startup Background Service has started.");
        });
#endif
        //return Task.CompletedTask;
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(StartupBackgroundService)}.{nameof(StopAsync)}");
        //return Task.CompletedTask;
    }
    public override void Dispose() => GC.SuppressFinalize(this);
}