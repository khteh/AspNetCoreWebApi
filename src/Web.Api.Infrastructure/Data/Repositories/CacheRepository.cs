using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Web.Api.Core.Interfaces.Gateways.Repositories;
namespace Web.Api.Infrastructure.Data.Repositories;
public class CacheRepository : ICacheRepository
{
    //private readonly IMapper _mapper;
    private readonly ILogger<CacheRepository> _logger;
    private readonly IDistributedCache _cache;
    public CacheRepository(ILogger<CacheRepository> logger, IDistributedCache cache) => (_logger, _cache) = (logger, cache);

    public async Task<bool> AddOrUpdate<T>(string key, T value, TimeSpan expiry)
    {
        if (!string.IsNullOrEmpty(key))
        {
            try
            {
                string strValue = JsonSerializer.Serialize(value);
                byte[] valueBytes = Encoding.UTF8.GetBytes(strValue);
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(expiry);
                await _cache.SetAsync(key, valueBytes, options);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(AddOrUpdate)} Failed to add cache key! {e}");
            }
        }
        else
            _logger.LogError($"{nameof(AddOrUpdate)} Cannot add cache entry with empty key!");
        return false;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            try
            {
                byte[] cacheValue = await _cache.GetAsync(key);
                if (cacheValue == null) return default(T);
                return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cacheValue));
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(GetAsync)} Failed to retrieve cache key! {e}");
            }
        }
        else
            _logger.LogError($"{nameof(GetAsync)} Cannot retrieve cache entry with empty key!");
        return default(T);
    }
}