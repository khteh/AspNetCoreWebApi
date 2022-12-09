using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace Web.Api.HealthChecks;
internal class ReadinessHealthCheck : IHealthCheck
{
    private volatile bool _isReady;
    public bool StartupCompleted
    {
        get => _isReady;
        set => _isReady = value;
    }
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Some Readiness check
        Console.WriteLine("Readiness health check executed.");
        return StartupCompleted ? Task.FromResult(HealthCheckResult.Healthy("The startup task is finished.")) :
                    Task.FromResult(HealthCheckResult.Unhealthy("The startup task is still running."));
    }
}