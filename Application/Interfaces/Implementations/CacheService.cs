using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Application.Interfaces.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                return default;

            var cachedResponse = await _cache.GetAsync(cacheKey.ToLower(), cancellationToken);
            if (cachedResponse != null)
            {
                return JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(cachedResponse));
            }

            return default;
        }

        public async Task SetAsync<T>(string cacheKey, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cacheKey) || data == null)
                return;

            var serializedData = Encoding.Default.GetBytes(JsonConvert.SerializeObject(data));

            await _cache.SetAsync(cacheKey.ToLower(), serializedData, options, cancellationToken);
        }

        public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
                return;

            await _cache.RemoveAsync(cacheKey.ToLower(), cancellationToken);
        }
    }
}
