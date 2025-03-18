using Microsoft.Extensions.Caching.Memory;


namespace BusinessAccessLayer.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetCache<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
        }

        public T GetCache<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T value);
            return value;
        }
    }
}
