using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.Interfaces.Gateways.Repositories;
namespace Web.Api.Infrastructure.Data.Repositories;

public class CacheRepository : ICacheRepository
{
    //private readonly IMapper _mapper;
    private readonly ILogger<CacheRepository> _logger;
    private readonly HybridCache _cache;
    public CacheRepository(ILogger<CacheRepository> logger, HybridCache cache) => (_logger, _cache) = (logger, cache);

    public async Task<bool> AddOrCreate<T>(string key, T value, List<string> tags, TimeSpan expiry, CancellationToken token = default)
    {
        if (!string.IsNullOrEmpty(key))
        {
            try
            {
                var options = new HybridCacheEntryOptions()
                {
                    Expiration = expiry,
                    LocalCacheExpiration = expiry
                };
                await _cache.GetOrCreateAsync(
                    key, // Unique key to the cache entry
                    async cancel => value,
                    options,
                    tags,
                    cancellationToken: token
                );
                return true;
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(AddOrCreate)} Failed to add cache key! {e}");
            }
        }
        else
            _logger.LogError($"{nameof(AddOrCreate)} Cannot add cache entry with empty key!");
        return false;
    }

    public async Task<bool> Remove(List<string> keys, CancellationToken token = default)
    {
        if (keys != null && keys.Any())
        {
            try
            {
                await _cache.RemoveAsync(keys);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(Remove)} Failed to add cache key! {e}");
            }
        }
        else
            _logger.LogError($"{nameof(Remove)} Cannot add cache entry with empty key!");
        return false;
    }
    public async Task<bool> RemoveByTags(List<string> tags, CancellationToken token = default)
    {
        if (tags != null && tags.Any())
        {
            try
            {
                await _cache.RemoveByTagAsync(tags);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(RemoveByTags)} Failed to add cache key! {e}");
            }
        }
        else
            _logger.LogError($"{nameof(RemoveByTags)} Cannot add cache entry with empty key!");
        return false;

    }
}