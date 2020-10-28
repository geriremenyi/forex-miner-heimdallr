//----------------------------------------------------------------------------------------
// <copyright file="InMemoryCacheProvider.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Caching.Providers.InMemory
{
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// In-memory cache provider
    /// </summary>
    public class InMemoryCacheProvider : IInMemoryCacheProvider
    {
        /// <summary>
        /// Sliding expiration in minutes for the in memory cache values
        /// </summary>
        private static readonly int CACHE_EXPIRY_IN_MINS = 5;

        /// <summary>
        /// In-memory cache options
        /// </summary>
        private readonly MemoryCacheEntryOptions _innerCacheOptions;
        
        /// <summary>
        /// In-memory cache service implementation
        /// </summary>
        private readonly IMemoryCache _innerCache;

        /// <summary>
        /// Constructor of the in-memory cache
        /// 
        /// Initializes the cache options and setups the actual in-memory cache service implementation
        /// </summary>
        /// <param name="innerCache">The in-memory cache service implementation</param>
        public InMemoryCacheProvider(IMemoryCache innerCache)
        {
            _innerCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(CACHE_EXPIRY_IN_MINS));
            _innerCache = innerCache;
        }

        /// <summary>
        /// Async get cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Value of the cache</returns>
        public Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_innerCache.Get<T>(key));
        }

        /// <summary>
        /// Get cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Value of the cache</returns>
        public T Get<T>(string key)
        {
            return _innerCache.Get<T>(key);
        }

        /// <summary>
        /// Async set cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            _innerCache.Set(key, value, _innerCacheOptions);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set cache
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        public void Set<T>(string key, T value)
        {
            _innerCache.Set(key, value, _innerCacheOptions);
        }

        /// <summary>
        /// Async remove cache value
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _innerCache.Remove(key);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Remove cache value
        /// </summary>
        /// <param name="key">Cache key</param>
        public void Remove(string key)
        {
            _innerCache.Remove(key);
        }
    }
}
