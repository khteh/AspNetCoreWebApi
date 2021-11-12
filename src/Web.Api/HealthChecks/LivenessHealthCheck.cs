using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using System;
namespace Web.Api.HealthChecks;
internal class LivenessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Some Liveness check
        Console.WriteLine("Liveness Health check executed.");
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}