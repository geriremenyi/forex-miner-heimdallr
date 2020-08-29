namespace ForexMiner.Heimdallr.Utilities.Cache.Services
{
    using ForexMiner.Heimdallr.Utilities.Cache.Providers;
    using ForexMiner.Heimdallr.Utilities.Cache.Types;
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

        public async Task<T> GetOrCreateCacheValue<T>(string cacheKey, Func<T> cacheValueProviderFunction, CacheCreateTarget cacheCreateTarget = CacheCreateTarget.InMemory)
        {
            // Try to get cache value from local cache
            var localCacheValue = await _inMemoryCacheProvider.Get<T>(cacheKey);
            if (localCacheValue != null)
            {
                if (cacheCreateTarget == CacheCreateTarget.Both) 
                {
                    // Touch distributed cache value to keep value alive
                    _ = _distributedCacheProvider.Get<T>(cacheKey);
                }
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
            var cacheValueToSet = cacheValueProviderFunction.Invoke();

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

        public async Task InvalidateCacheValue(string cacheKey)
        {
            // Delete from local cache
            await _inMemoryCacheProvider.Remove(cacheKey);

            // Delete from distributed cache
            await _distributedCacheProvider.Remove(cacheKey);
        }
    }
}
