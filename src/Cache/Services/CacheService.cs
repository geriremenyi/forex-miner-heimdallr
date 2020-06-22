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

        public async Task<T> GetOrCreateCacheValue<T>(CacheType cacheType, string cacheNamespace, string cacheName, Func<T> fallbackValueProvider, CacheCreateTarget cacheCreateTarget = CacheCreateTarget.Local)
        {
            // Construct string cache key
            var cacheKey = GetCacheKey(cacheType, cacheNamespace, cacheName);

            // Try to get cache value from local cache
            var localCacheValue = await _localCacheProvider.Get<T>(cacheKey);
            if (localCacheValue != null)
            {
                // Update it in distributed cache if cache create target suggests
                if (cacheCreateTarget == CacheCreateTarget.Both)
                { 
                    _ = _distributedCacheProvider.Set(cacheKey, localCacheValue);
                }

                return localCacheValue;
            }

            // Try to get cache value from the distributed cache if not found in the local one
            var distributedCacheValue = await _distributedCacheProvider.Get<T>(cacheKey);
            if (distributedCacheValue != null)
            {
                // "Download" value to local cache
                _ = _localCacheProvider.Set(cacheKey, distributedCacheValue);
                return distributedCacheValue;
            }

            // Cache not found in any of the cache providers so obtain it using the given provider function
            var cacheValueToSet = fallbackValueProvider.Invoke();

            // Fill in cache
            switch (cacheCreateTarget)
            {
                case CacheCreateTarget.Both:
                    _ = _localCacheProvider.Set(cacheKey, cacheValueToSet);
                    _ = _distributedCacheProvider.Set(cacheKey, cacheValueToSet);
                    break;
                default:
                case CacheCreateTarget.Local:
                    _ = _localCacheProvider.Set(cacheKey, cacheValueToSet);
                    break;
            }
            
            // Return the obtained value 
            return cacheValueToSet;

        }

        private string GetCacheKey(CacheType cacheType, string cacheNamespace, string cacheName) => $"{cacheType}-{cacheNamespace}-{cacheName}";
    }
}
