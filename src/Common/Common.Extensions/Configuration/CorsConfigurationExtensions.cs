//----------------------------------------------------------------------------------------
// <copyright file="CorsConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions for CORS configuration
    /// </summary>
    public static class CorsConfigurationExtensions
    {
        /// <summary>
        /// Name of the cors policy to create and use
        /// </summary>
        public const string CORS_PLOICY_NAME = "ForexMinerCorsPolicy";

        /// <summary>
        /// Add CORS policy with a name
        /// </summary>
        /// <param name="services">The services</param>
        public static void AddCorsPolicy(this IServiceCollection services) 
        {
            services.AddCors(c =>
            {
                c.AddPolicy(CORS_PLOICY_NAME, options =>
                {
                    options.AllowAnyOrigin();
                    options.AllowAnyMethod();
                    options.AllowAnyHeader();
                });
            });
        }

        /// <summary>
        /// Use the created CORS policy
        /// </summary>
        /// <param name="app">Application builder</param>
        public static void UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(CORS_PLOICY_NAME);
        }
    }
}
