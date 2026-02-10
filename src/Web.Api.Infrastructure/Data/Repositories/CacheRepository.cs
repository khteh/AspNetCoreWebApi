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
    private readonly ILogger<CacheRepository> _logger;
    /// <summary>
    /// https://github.com/dotnet/AspNetCore.Docs/issues/36757
    /// https://github.com/dotnet/AspNetCore.Docs/issues/36758
    /// </summary>
    private readonly HybridCache _cache;
    public CacheRepository(ILogger<CacheRepository> logger, HybridCache cache) => (_logger, _cache) = (logger, cache);
    /// <summary>
    /// Get or create a new cache entry
    /// </summary>
    /// <typeparam name="T">The type of the cache value</typeparam>
    /// <param name="key"></param>
    /// <param name="factory">Factory method used to create the object for the new cache entry in case of cache miss</param>
    /// <param name="tags"></param>
    /// <param name="expiry"></param>
    /// <param name="token"></param>
    /// <returns>The object of type T</returns>
    public async Task<T?> GetOrCreate<T>(string key, Func<CancellationToken, ValueTask<T>> factory, List<string> tags, TimeSpan expiry, CancellationToken token = default)
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
                return await _cache.GetOrCreateAsync(
                    key, // Unique key to the cache entry
                    async cancel => await factory(cancel),
                    options,
                    tags,
                    cancellationToken: token
                );
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(GetOrCreate)} Failed to add cache key! {e}");
            }
        }
        else
            _logger.LogError($"{nameof(GetOrCreate)} Cannot add cache entry with empty key!");
        return default;
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