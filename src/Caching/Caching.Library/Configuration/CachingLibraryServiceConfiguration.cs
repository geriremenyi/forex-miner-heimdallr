namespace ForexMiner.Heimdallr.Caching.Library.Configuration
{
    using ForexMiner.Heimdallr.Caching.Library.Providers.Distributed;
    using ForexMiner.Heimdallr.Caching.Library.Providers.InMemory;
    using ForexMiner.Heimdallr.Caching.Library.Service;
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
