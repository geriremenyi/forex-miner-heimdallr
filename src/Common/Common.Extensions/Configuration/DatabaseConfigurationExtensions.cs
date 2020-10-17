//----------------------------------------------------------------------------------------
// <copyright file="DatabaseConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------


namespace ForexMiner.Heimdallr.Common.Extensions
{
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Common.Data.Database.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions for database configuration
    /// </summary>
    public static class DatabaseConfigurationExtensions
    {
        /// <summary>
        /// Add database service configurations
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="dbConnectionString">Database connection string</param>
        public static void AddDatabase(this IServiceCollection services, string dbConnectionString, string redisConnectionString)
        {
            // Caching services
            services.AddCachingService(redisConnectionString);

            // Token provider service
            services.AddSingleton<IAzureSqlServerTokenProvider, AzureSqlServerTokenProvider>();

            // Db connection interceptor
            services.AddSingleton<TokenAuthenticationDbConnectionInterceptor>();

            // Db context
            services.AddDbContext<ForexMinerHeimdallrDbContext>((provider, options) =>
            {
                options.UseSqlServer(dbConnectionString);
                options.AddInterceptors(provider.GetRequiredService<TokenAuthenticationDbConnectionInterceptor>());
            });
        }
    }
}
