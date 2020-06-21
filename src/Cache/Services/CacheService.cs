namespace ForexMiner.Heimdallr.Cache.Services
{
    using ForexMiner.Heimdallr.Cache.Providers;
    using ForexMiner.Heimdallr.Cache.Utilities;
    using System;
    using System.Threading.Tasks;

    public class CacheService : ICacheService
    {
        private readonly ICacheProvider _localCacheProvider;
        private readonly ICacheProvider _distributedCacheProvider;

        public CacheService(ICacheProvider localCacheProvider, ICacheProvider distributedCacheProvider)
        {
            _localCacheProvider = localCacheProvider;
            _distributedCacheProvider = distributedCacheProvider;
        }

        public async Task<T> GetOrCreateCache<T>(CacheType cacheType, string cacheNamespace, string cacheName, Func<T> cacheValueProvider)
        {
            // Construct string cache key
            var cacheKey = CacheKey(cacheType, cacheNamespace, cacheName);

            // Try to get cache value from local cache
            var localCacheValue = await _localCacheProvider.Get<T>(cacheKey);
            if (localCacheValue != null)
            {
                return localCacheValue;
            }

            // Try to get cache value from the distributed cache if not found locally
            // And distributed cache is given
            var distributedCacheValue = await _distributedCacheProvider.Get<T>(cacheKey);
            if (distributedCacheValue != null)
            {
                return distributedCacheValue;
            }

            // Cache not found in any of the cache providers so create it in all of the give providers
            var cacheValueToSet = cacheValueProvider.Invoke();
            await _localCacheProvider.Set(cacheKey, cacheValueToSet);
            await _distributedCacheProvider.Set(cacheKey, cacheValueToSet);

            return cacheValueToSet;

        }

        private string CacheKey(CacheType cacheType, string cacheNamespace, string cacheName) => $"{cacheType}-{cacheNamespace}-{cacheName}";
    }
}
