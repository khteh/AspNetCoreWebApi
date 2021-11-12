using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace Web.Api.HealthChecks;
internal class ReadinessHealthCheck : IHealthCheck
{
    public bool StartupTaskCompleted { get; set; } = false;
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Some Readiness check
        Console.WriteLine("Readiness health check executed.");
        return StartupTaskCompleted ? Task.FromResult(HealthCheckResult.Healthy("The startup task is finished.")) :
                    Task.FromResult(HealthCheckResult.Unhealthy("The startup task is still running."));
    }
}