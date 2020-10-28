//----------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Api
{
    using ForexMiner.Heimdallr.Connections.Api.Configuration;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
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

        /// <summary>
        /// Startup constructor
        /// </summary>
        /// <param name="environment">The environment</param>
        /// <param name="configuration">The configuration</param>
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
            services.AddConnectionsApiServices(_environment, _configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ForexMinerHeimdallrDbContext dbContext)
        {
            // Custom 
            app.UseConnectionsApiServices(_environment, dbContext);

            // System
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
