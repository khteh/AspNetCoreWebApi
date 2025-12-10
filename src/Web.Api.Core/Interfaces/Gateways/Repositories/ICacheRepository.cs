using System;
using System.Threading.Tasks;

namespace Web.Api.Core.Interfaces.Gateways.Repositories;

public interface ICacheRepository
{
    Task<bool> AddOrUpdate<T>(string key, T value, TimeSpan expiry);
    Task<T?> GetAsync<T>(string key);
}
