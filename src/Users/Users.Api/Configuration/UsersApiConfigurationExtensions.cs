//----------------------------------------------------------------------------------------
// <copyright file="UsersApiConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Users.Api.Configuration
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using ForexMiner.Heimdallr.Common.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Builder;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Extension class for service configuration
    /// </summary>
    public static class UsersApiConfigurationExtensions
    {
        /// <summary>
        /// Add required services to the service collection
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="configuration">The configuration object</param>
        public static void AddUsersApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cors policy
            services.AddCorsPolicy();

            // Problem details
            services.AddProblemDetails();

            // JWT token
            services.AddJwtAuthentication(configuration["Jwt-IssuerSigningKey"]);

            // Database
            services.AddDatabase(configuration["SqlServer-ConnectionString"]);

            // Auto mapper
            services.AddAutoMapper(typeof(UsersApiConfigurationExtensions));

            // Local services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEntitySeedService, UserSeedService>();
        }

        /// <summary>
        /// Use the added services
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="environment">The environment</param>
        /// <param name="dbContext">The database context</param>
        /// <param name="seedService">The entity seed service</param>
        public static void UseUsersApiServices(this IApplicationBuilder app, IWebHostEnvironment environment, ForexMinerHeimdallrDbContext dbContext, IEntitySeedService seedService)
        {
            // CORS
            app.UseCorsPolicy();

            // ProblemDetails
            app.UseProblemDetails(environment.IsDevelopment());

            // Database migration
            dbContext.MigrateDatabase(environment.IsDevelopment());

            // Seeding
            seedService.Seed();
        }
    }
}
