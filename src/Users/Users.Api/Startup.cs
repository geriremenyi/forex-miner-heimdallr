//----------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Users.Api
{
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Users.Api.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Text.Json.Serialization;

    public class Startup
    {
        /// <summary>
        /// Hosting environment
        /// </summary>
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Configuration object
        /// </summary>
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Routing
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddApiVersioning();

            // All utility- and local services
            services.AddUsersApiServices(_configuration);
        }

        public void Configure(IApplicationBuilder app, ForexMinerHeimdallrDbContext dbContext, IEntitySeedService seedService)
        {
            // Custom 
            app.UseUsersApiServices(_environment, dbContext, seedService);

            // System
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
