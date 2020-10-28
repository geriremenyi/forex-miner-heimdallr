//----------------------------------------------------------------------------------------
// <copyright file="DatabaseConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------


namespace ForexMiner.Heimdallr.Common.Extensions
{
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

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
        public static void AddDatabase(this IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<ForexMinerHeimdallrDbContext>((provider, options) => options.UseSqlServer(dbConnectionString));
        }

        public static void MigrateDatabase(this ForexMinerHeimdallrDbContext dbContext, bool isDevEnvironment)
        {
            try
            {
                if (dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (SqlException ex)
            {
                // Swallow migration race condition exception on dev docker containers
                if (!isDevEnvironment)
                {
                    throw ex;
                }
                else 
                {
                    // Retry in 5 sec for dev
                    // as healthchecks doesn't stop it failing
                    Thread.Sleep(5000);
                    if (dbContext.Database.GetPendingMigrations().Count() > 0)
                    {
                        dbContext.Database.Migrate();
                    }
                }
            }
        }
    }
}
