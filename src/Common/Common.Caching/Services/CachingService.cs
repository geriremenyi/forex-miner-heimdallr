//----------------------------------------------------------------------------------------
// <copyright file="CachingService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Caching.Services
{
    using ForexMiner.Heimdallr.Common.Caching.Providers.Distributed;
    using ForexMiner.Heimdallr.Common.Caching.Providers.InMemory;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Caching service implementation
    /// </summary>
    public class CachingService : ICachingService
    {
        /// <summary>
        /// In-memory cache provider
        /// </summary>
        private readonly IInMemoryCacheProvider _inMemoryCacheProvider;

        /// <summary>
        /// Distributed cache provider
        /// </summary>
        private readonly IDistributedCacheProvider _distributedCacheProvider;

        /// <summary>
        /// Caching service constructor
        /// 
        /// Setups the cache providers
        /// </summary>
        /// <param name="inMemoryCacheProvider">The in-memory cache provider</param>
        /// <param name="distributedCacheProvider">The distributed cache provider</param>
        public CachingService(IInMemoryCacheProvider inMemoryCacheProvider, IDistributedCacheProvider distributedCacheProvider)
        {
            _inMemoryCacheProvider = inMemoryCacheProvider;
            _distributedCacheProvider = distributedCacheProvider;
        }

        /// <summary>
        /// Get or create cache value
        /// </summary>
        /// <typeparam name="T">Type of the value cached or to be cached</typeparam>
        /// <param name="cacheKey">Key of the cache</param>
        /// <param name="valueProviderFunc">Value provider function in case the value is not in the cache</param>
        /// <returns>The requested cache value</returns>
        public T GetOrCreateValue<T>(string cacheKey, Func<T> valueProviderFunc)
        {
            // Try to get cache value from local cache
            var localCacheValue = _inMemoryCacheProvider.Get<T>(cacheKey);
            if (localCacheValue != null)
            {
                // Touch distributed cache (async way without waiting for the result) to mark usage but otherwise return the local value
                _ = _distributedCacheProvider.GetAsync<T>(cacheKey);
                return localCacheValue;
            }

            // Try to get cache value from the distributed cache if not found in the local one
            var distributedCacheValue = _distributedCacheProvider.Get<T>(cacheKey);
            if (distributedCacheValue != null)
            {
                // "Download" value to local cache
                _inMemoryCacheProvider.Set(cacheKey, distributedCacheValue);
                return distributedCacheValue;
            }

            // Cache not found in any of the cache providers so obtain it using the given provider function
            var cacheValueToSet = valueProviderFunc.Invoke();

            // Fill in cache
            _inMemoryCacheProvider.Set(cacheKey, cacheValueToSet);
            _distributedCacheProvider.Set(cacheKey, cacheValueToSet);

            // Return the obtained value 
            return cacheValueToSet;
        }

        /// <summary>
        /// Async get or create cache value
        /// </summary>
        /// <typeparam name="T">Type of the value cached or to be cached</typeparam>
        /// <param name="cacheKey">Key of the cache</param>
        /// <param name="valueProviderFunc">Value provider function in case the value is not in the cache</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The requested cache value</returns>
        public async Task<T> GetOrCreateValueAsync<T>(string cacheKey, Func<T> valueProviderFunc, CancellationToken cancellationToken = default)
        {
            // Try to get cache value from local cache
            var localCacheValue = await _inMemoryCacheProvider.GetAsync<T>(cacheKey, cancellationToken);
            if (localCacheValue != null)
            {
                // Touch distributed cache to mark usage but otherwise return the local value
                _ = _distributedCacheProvider.GetAsync<T>(cacheKey, cancellationToken);
                return localCacheValue;
            }

            // Try to get cache value from the distributed cache if not found in the local one
            var distributedCacheValue = await _distributedCacheProvider.GetAsync<T>(cacheKey, cancellationToken);
            if (distributedCacheValue != null)
            {
                // "Download" value to local cache
                _ = _inMemoryCacheProvider.SetAsync(cacheKey, distributedCacheValue, cancellationToken);
                return distributedCacheValue;
            }

            // Cache not found in any of the cache providers so obtain it using the given provider function
            var cacheValueToSet = valueProviderFunc.Invoke();

            // Fill in cache
            _ = _inMemoryCacheProvider.SetAsync(cacheKey, cacheValueToSet, cancellationToken);
            _ = _distributedCacheProvider.SetAsync(cacheKey, cacheValueToSet, cancellationToken);

            // Return the obtained value 
            return cacheValueToSet;
        }

        /// <summary>
        /// Async invalidate a cache value
        /// </summary>
        /// <param name="cacheKey">The cache key to invalidate</param>
        public void InvalidateValue(string cacheKey)
        {
            // Delete from local cache if present
            _inMemoryCacheProvider.Remove(cacheKey);

            // Delete from distributed cache if present
            _distributedCacheProvider.Remove(cacheKey);
        }

        /// <summary>
        /// Async invalidate a cache value
        /// </summary>
        /// <param name="cacheKey">The cache key to invalidate</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task InvalidateValueAsync(string cacheKey, CancellationToken cancellationToken = default)
        {
            // Delete from local cache if present
            await _inMemoryCacheProvider.RemoveAsync(cacheKey, cancellationToken);

            // Delete from distributed cache if present
            await _distributedCacheProvider.RemoveAsync(cacheKey, cancellationToken);
        }
    }
}
