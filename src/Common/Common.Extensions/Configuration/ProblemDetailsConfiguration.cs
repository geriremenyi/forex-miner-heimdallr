//----------------------------------------------------------------------------------------
// <copyright file="JwtAuthenticationConfiguration.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Extensions
{
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions for problem details configuration
    /// </summary>
    public static class ProblemDetailsConfiguration
    {
        /// <summary>
        /// Configure ProblemDetails for exception handling
        /// </summary>
        /// <param name="services">The services</param>
        public static void AddProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(setup =>
            {
                setup.Map<Data.Exceptions.ProblemDetailsException>(exception => new ProblemDetails
                {
                    Type = $"https://httpstatuses.com/{(int)exception.Status}",
                    Title = exception.Status.ToString(),
                    Detail = exception.Message,
                    Status = (int)exception.Status
                });
            });
        }

        /// <summary>
        /// Use the configured ProblemDetails exception handling
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="env">Environment</param>
        public static void UseProblemDetails(this IApplicationBuilder app, bool isDevelopmentEnvironment)
        {
            if (isDevelopmentEnvironment)
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseProblemDetails();
        }
    }
}
