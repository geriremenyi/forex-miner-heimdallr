namespace Caching.Library.Configuration
{
    using Caching.Library.Providers.Distributed;
    using Caching.Library.Providers.InMemory;
    using Caching.Library.Service;
    using Microsoft.Extensions.DependencyInjection;

    public static class CachingLibraryServiceConfiguration
    {
        public static void AddCachingService(this IServiceCollection services, string redisCacheConnectionString)
        {
            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options => { options.Configuration = redisCacheConnectionString; });
            services.AddScoped<IInMemoryCacheProvider, InMemoryCacheProvider>();
            services.AddScoped<IDistributedCacheProvider, DistributedCacheProvider>();
            services.AddScoped<ICachingService, CachingService>();
        }

    }
}
