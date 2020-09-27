namespace ForexMiner.Heimdallr.Caching.Library.Service
{
    using ForexMiner.Heimdallr.Caching.Library.Providers.Distributed;
    using ForexMiner.Heimdallr.Caching.Library.Providers.InMemory;
    using System;
    using System.Threading.Tasks;

    public class CachingService : ICachingService
    {
        private readonly IInMemoryCacheProvider _inMemoryCacheProvider;
        private readonly IDistributedCacheProvider _distributedCacheProvider;

        public CachingService(IInMemoryCacheProvider inMemoryCacheProvider, IDistributedCacheProvider distributedCacheProvider)
        {
            _inMemoryCacheProvider = inMemoryCacheProvider;
            _distributedCacheProvider = distributedCacheProvider;
        }

        public async Task<T> GetOrCreateValue<T>(string cacheKey, Func<T> cacheValueProviderFunction)
        {
            // Try to get cache value from local cache
            var localCacheValue = await _inMemoryCacheProvider.Get<T>(cacheKey);
            if (localCacheValue != null)
            {
                // Touch distributed cache to mark usage but otherwise return the local value
                _ = _distributedCacheProvider.Get<T>(cacheKey);
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
            _ = _inMemoryCacheProvider.Set(cacheKey, cacheValueToSet);
            _ = _distributedCacheProvider.Set(cacheKey, cacheValueToSet);

            // Return the obtained value 
            return cacheValueToSet;
        }

        public async Task InvalidateValue(string cacheKey)
        {
            // Delete from local cache
            await _inMemoryCacheProvider.Remove(cacheKey);

            // Delete from distributed cache
            await _distributedCacheProvider.Remove(cacheKey);
        }
    }
}
