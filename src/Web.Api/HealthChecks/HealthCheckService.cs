using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Web.Api.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HealthCheckService
    {
        public static IServiceCollection AddHealthCheck(this IServiceCollection service)
        {
                service.AddHealthChecks()
                .AddLivenessHealthCheck("Liveness", HealthStatus.Unhealthy, new List<string>(){"Liveness"})
                .AddReadinessHealthCheck("Readiness", HealthStatus.Unhealthy, new List<string>{ "Readiness" });
                return service.AddHostedService<StartupHostedService>()
                .AddSingleton<ReadinessHealthCheck>()
                .AddSingleton<LivenessHealthCheck>();
        }
    }
}