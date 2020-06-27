﻿namespace ForexMiner.Heimdallr.Cache.Services
{
    using ForexMiner.Heimdallr.Cache.Providers;
    using ForexMiner.Heimdallr.Cache.Utilities;
    using System;
    using System.Threading.Tasks;

    public class CacheService : ICacheService
    {
        private readonly IInMemoryCacheProvider _inMemoryCacheProvider;
        private readonly IDistributedCacheProvider _distributedCacheProvider;

        public CacheService(IInMemoryCacheProvider inMemoryCacheProvider, IDistributedCacheProvider distributedCacheProvider)
        {
            _inMemoryCacheProvider = inMemoryCacheProvider;
            _distributedCacheProvider = distributedCacheProvider;
        }

        public async Task<T> GetOrCreateCacheValue<T>(CacheType cacheType, string cacheNamespace, string cacheName, Func<T> fallbackValueProvider, CacheCreateTarget cacheCreateTarget = CacheCreateTarget.InMemory)
        {
            // Construct string cache key
            var cacheKey = GetCacheKey(cacheType, cacheNamespace, cacheName);

            // Try to get cache value from local cache
            var localCacheValue = await _inMemoryCacheProvider.Get<T>(cacheKey);
            if (localCacheValue != null)
            {
                return localCacheValue;
            }

            // Try to get cache value from the distributed cache if not found in the local one
            var distributedCacheValue = await _distributedCacheProvider.Get<T>(cacheKey);
            if (distributedCacheValue != null)
            {
                // "Download" value to local cache
                _ = _inMemoryCacheProvider.Set(cacheKey, distributedCacheValue);
                return distributedCacheValue;
            }

            // Cache not found in any of the cache providers so obtain it using the given provider function
            var cacheValueToSet = fallbackValueProvider.Invoke();

            // Fill in cache
            switch (cacheCreateTarget)
            {
                case CacheCreateTarget.Both:
                    _ = _inMemoryCacheProvider.Set(cacheKey, cacheValueToSet);
                    _ = _distributedCacheProvider.Set(cacheKey, cacheValueToSet);
                    break;
                default:
                case CacheCreateTarget.InMemory:
                    _ = _inMemoryCacheProvider.Set(cacheKey, cacheValueToSet);
                    break;
            }
            
            // Return the obtained value 
            return cacheValueToSet;

        }

        private string GetCacheKey(CacheType cacheType, string cacheNamespace, string cacheName) => $"{cacheType}-{cacheNamespace}-{cacheName}";
    }
}
