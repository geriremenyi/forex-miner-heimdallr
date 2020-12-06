//----------------------------------------------------------------------------------------
// <copyright file="ICachingService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Caching.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Service interface for caching
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// get or create cache value
        /// </summary>
        /// <typeparam name="T">Type of the value cached or to be cached</typeparam>
        /// <param name="cacheKey">Key of the cache</param>
        /// <param name="valueProviderFunc">Value provider function in case the value is not in the cache</param>
        /// <returns>The requested cache value</returns>
        public T GetOrCreateValue<T>(string cacheKey, Func<T> valueProviderFunc);

        /// <summary>
        /// Async get or create cache value
        /// </summary>
        /// <typeparam name="T">Type of the value cached or to be cached</typeparam>
        /// <param name="cacheKey">Key of the cache</param>
        /// <param name="valueProviderFunc">Value provider function in case the value is not in the cache</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The requested cache value</returns>
        public Task<T> GetOrCreateValueAsync<T>(string cacheKey, Func<T> valueProviderFunc, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async invalidate a cache value
        /// </summary>
        /// <param name="cacheKey">The cache key to invalidate</param>
        public void InvalidateValue(string cacheKey);

        /// <summary>
        /// Async invalidate a cache value
        /// </summary>
        /// <param name="cacheKey">The cache key to invalidate</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public Task InvalidateValueAsync(string cacheKey, CancellationToken cancellationToken = default);
    }
}
