using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedResponse = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedResponse))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (expirationTime.HasValue)
            {
                options.SetAbsoluteExpiration(expirationTime.Value);
            }

            var serializedResponse = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
            await _cache.SetStringAsync(key, serializedResponse, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefixKey)
        {
            // Note: StackExchange.Redis doesn't support a direct delete by prefix natively on IDistributedCache.
            // If prefix deletion is needed, one usually tracks keys or uses a different library like StackExchange.Redis directly.
            // For now, this is a placeholder. If wildcards are heavily used, they need to be implemented via server.Keys()
            // However, the requested keys are exact like "products:all" and "product:{id}" so we can invalidate them explicitly
            // without needing wildcard prefix matching in most cases.
            await Task.CompletedTask;
        }
    }
}
