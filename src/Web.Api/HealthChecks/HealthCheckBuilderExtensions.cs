using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace Web.Api.HealthChecks;
public static class HealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddLivenessHealthCheck(
            this IHealthChecksBuilder builder,
            string name,
            HealthStatus? failureStatus,
            IEnumerable<string> tags) =>
    builder.AddCheck<LivenessHealthCheck>(
                name,
                failureStatus,
                tags);
    public static IHealthChecksBuilder AddReadinessHealthCheck(
            this IHealthChecksBuilder builder,
            string name,
            HealthStatus? failureStatus,
            IEnumerable<string> tags) =>
    builder.AddCheck<ReadinessHealthCheck>(
                name,
                failureStatus,
                tags);
}