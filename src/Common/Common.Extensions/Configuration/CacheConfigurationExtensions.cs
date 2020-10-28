//----------------------------------------------------------------------------------------
// <copyright file="CacheConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Extensions
{
    using ForexMiner.Heimdallr.Common.Caching.Providers.Distributed;
    using ForexMiner.Heimdallr.Common.Caching.Providers.InMemory;
    using ForexMiner.Heimdallr.Common.Caching.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions for cache configuration
    /// </summary>
    public static class CacheConfigurationExtensions
    {
        /// <summary>
        /// Add custom implementation services for the in-memory and distributed cache combination
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="redisConnectionString">Connection string to access redis instance</param>
        public static void AddCachingService(this IServiceCollection services, string redisConnectionString)
        {
            // Adding in-memory and distributed cache service implementations
            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionString; });

            // Adding custom wrapper around those implementations
            services.AddSingleton<IInMemoryCacheProvider, InMemoryCacheProvider>();
            services.AddSingleton<IDistributedCacheProvider, DistributedCacheProvider>();
            services.AddSingleton<ICachingService, CachingService>();
        }
    }
}
