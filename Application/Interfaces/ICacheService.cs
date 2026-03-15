using Microsoft.Extensions.Caching.Distributed;

namespace Application.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken);
        Task SetAsync<T>(string cacheKey, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken);
        Task RemoveAsync(string cacheKey, CancellationToken cancellationToken);
    }
}
