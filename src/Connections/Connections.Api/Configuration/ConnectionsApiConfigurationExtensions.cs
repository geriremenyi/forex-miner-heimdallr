//----------------------------------------------------------------------------------------
// <copyright file="ConnectionsApiConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Api.Configuration
{
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Common.Extensions;
    using ForexMiner.Heimdallr.Connections.Api.Services;
    using ForexMiner.Heimdallr.Connections.Secret.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Mapping;

    /// <summary>
    /// Extension class for service configuration
    /// </summary>
    public static class ConnectionsApiConfigurationExtensions
    {

        /// <summary>
        /// Add required services to the service collection
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="configuration">The configuration object</param>
        public static void AddConnectionsApiServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            // Cors policy
            services.AddCorsPolicy();

            // Problem details
            services.AddProblemDetails();

            // JWT token
            services.AddJwtAuthentication(configuration["Jwt-IssuerSigningKey"]);

            // Database
            services.AddDatabase(configuration["SqlServer-ConnectionString"]);

            // Auto mapping
            services.AddAutoMapper(typeof(ContractContractMappings), typeof(DatabaseContractMappings), typeof(OandaContractMappings));

            // Connections secret service
            services.AddConnectionsSecretServices(environment.IsDevelopment(), configuration["KeyVault-Uri"], configuration["RedisCache-ConnectionString"]);

            // Local services
            services.AddScoped<IConnectionService, ConnectionService>();
        }

        public static void UseConnectionsApiServices(this IApplicationBuilder app, IWebHostEnvironment environment, ForexMinerHeimdallrDbContext dbContext)
        {
            // CORS
            app.UseCorsPolicy();

            // ProblemDetails
            app.UseProblemDetails(environment.IsDevelopment());
        }

    }
}
