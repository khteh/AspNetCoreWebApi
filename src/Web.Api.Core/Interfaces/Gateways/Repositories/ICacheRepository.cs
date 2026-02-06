using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Api.Core.Interfaces.Gateways.Repositories;

public interface ICacheRepository
{
    Task<T> GetOrCreate<T>(string key, Func<CancellationToken, ValueTask<T>> factory, List<string> tags, TimeSpan expiry, CancellationToken token = default);
    Task<bool> Remove(List<string> keys, CancellationToken token = default);
    Task<bool> RemoveByTags(List<string> keys, CancellationToken token = default);
}
