//----------------------------------------------------------------------------------------
// <copyright file="ICacheProvider.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Caching.Providers
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Cache provider interface
    /// </summary>
    public interface ICacheProvider
    {

        /// <summary>
        /// Async get cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Value of the cache</returns>
        public Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Value of the cache</returns>
        public T Get<T>(string key);

        /// <summary>
        /// Async set cache value
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set cache
        /// </summary>
        /// <typeparam name="T">Type of the cache value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Cache value</param>
        public void Set<T>(string key, T value);

        /// <summary>
        /// Async remove cache value
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public Task RemoveAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Remove cache value
        /// </summary>
        /// <param name="key">Cache key</param>
        public void Remove(string key);
    }
}
