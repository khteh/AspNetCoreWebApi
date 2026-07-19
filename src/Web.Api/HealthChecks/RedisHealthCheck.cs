using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Web.Api.Core.Configuration;
namespace Web.Api.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redisCache;
    private readonly RedisCache _config;
    private readonly ILogger<RedisHealthCheck> _logger;
    public RedisHealthCheck(IOptions<RedisCache> config, IConnectionMultiplexer redisCache, ILogger<RedisHealthCheck> logger) => (_config, _redisCache, _logger) = (config.Value, redisCache, logger);
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Some Liveness check
        _logger.LogInformation($"{nameof(RedisHealthCheck)} Redis Health check executed. {_config.Connection}");
        try
        {
            foreach (var endPoint in _redisCache.GetEndPoints(configuredOnly: true))
            {
                IServer server = _redisCache.GetServer(endPoint);
                if (server.ServerType != ServerType.Cluster)
                {
                    await _redisCache.GetDatabase().PingAsync();
                    await server.PingAsync();
                }
                else
                {
                    var clusterInfo = await server.ExecuteAsync("CLUSTER", "INFO");
                    if (clusterInfo is object && !clusterInfo.IsNull)
                    {
                        if (!clusterInfo.ToString()!.Contains("cluster_state:ok"))
                        {
                            _logger.LogError($"{nameof(RedisHealthCheck)} CLUSTER is not healthy for endpoint {endPoint}");
                            return new HealthCheckResult(context.Registration.FailureStatus, description: $"CLUSTER is not is healthy for endpoint {endPoint}");
                        }
                    }
                    else
                    {
                        _logger.LogError($"{nameof(RedisHealthCheck)} CLUSTER is unhealthy for endpoint {endPoint}");
                        return new HealthCheckResult(context.Registration.FailureStatus, description: $"CLUSTER unhealthy for endpoint {endPoint}");
                    }
                }
            }
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }

}