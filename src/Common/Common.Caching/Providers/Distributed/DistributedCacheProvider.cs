// ----------------------------------------------------------------------------------------
// <copyright file="IDistributedCacheProvider.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Caching.Providers.Distributed
{
    using Microsoft.Extensions.Caching.Distributed;
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class DistributedCacheProvider : IDistributedCacheProvider
    {
        /// <summary>
        /// Sliding expiration in minutes for the in memory cache values
        /// </summary>
        private static readonly int CACHE_EXPIRY_IN_MINS = 30;

        /// <summary>
        /// Distributed cache options
        /// </summary>
        private readonly DistributedCacheEntryOptions _innerCacheOptions;

        /// <summary>
        /// Distributed cache service implementation
        /// </summary>
        private readonly IDistributedCache _innerCache;

        /// <summary>
        /// Constructor of the distributed cache provider
        /// 
        /// Initializes the cache options and setups the actual distributed (redis) cache service implementation
        /// </summary>
        /// <param name="innerCache">The distributed cache service implementation</param>
        public DistributedCacheProvider(IDistributedCache innerCache)
        {
            _innerCacheOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(CACHE_EXPIRY_IN_MINS));
            _innerCache = innerCache;
        }

        /// <summary>
        /// Get cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Value of the cache</returns>
        public T Get<T>(string key)
        {
            var cacheValue = _innerCache.Get(key);
            return cacheValue == null ? default : JsonSerializer.Deserialize<T>(cacheValue);
        }

        /// <summary>
        /// Async get cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Value of the cache</returns>
        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            var cacheValue = await _innerCache.GetAsync(key, cancellationToken);
            return cacheValue == null ? default : JsonSerializer.Deserialize<T>(cacheValue);
        }

        /// <summary>
        /// Set cache
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        public void Set<T>(string key, T value)
        {
            _innerCache.Set(key, JsonSerializer.SerializeToUtf8Bytes(value), _innerCacheOptions);
        }

        /// <summary>
        /// Async set cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            await _innerCache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value), _innerCacheOptions, cancellationToken);
        }

        /// <summary>
        /// Remove cache value
        /// </summary>
        /// <param name="key">Cache key</param>
        public void Remove(string key)
        {
            _innerCache.Remove(key);
        }

        /// <summary>
        /// Async remove cache value
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _innerCache.RemoveAsync(key, cancellationToken);
        }
    }
}
