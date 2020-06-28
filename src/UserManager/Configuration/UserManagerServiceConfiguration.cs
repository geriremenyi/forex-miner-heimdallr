namespace ForexMiner.Heimdallr.UserManager.Configuration
{
    using AutoMapper;
    using ForexMiner.Heimdallr.UserManager.Database;
    using ForexMiner.Heimdallr.UserManager.Services;
    using ForexMiner.Heimdallr.Utilities.Cache.Providers;
    using ForexMiner.Heimdallr.Utilities.Cache.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class UserManagerServiceConfiguration
    {
        public static void AddUserManagerServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Caching
            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetConnectionString("ForexMinerRedisCache"); });
            services.AddScoped<IInMemoryCacheProvider, InMemoryCacheProvider>();
            services.AddScoped<IDistributedCacheProvider, DistributedCacheProvider>();
            services.AddScoped<ICacheService, CacheService>();

            // Database
            services.AddDbContext<UserManagerDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ForexMinerDb")));

            // Other utilities
            services.AddAutoMapper(typeof(Startup));

            // Services defined in UserManager
            services.AddScoped<IUserService, UserService>();
        }
    }
}
