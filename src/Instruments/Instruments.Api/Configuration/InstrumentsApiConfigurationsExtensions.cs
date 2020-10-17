namespace ForexMiner.Heimdallr.Instruments.Api.Configuration
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using ForexMiner.Heimdallr.Instruments.Api.Services;
    using ForexMiner.Heimdallr.Instruments.Configuration;
    using Microsoft.Extensions.Configuration;
    using ForexMiner.Heimdallr.Common.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;

    public static class InstrumentsApiConfigurationsExtensions {
        public static void AddInstrumentsApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cors policy
            services.AddCorsPolicy();

            // Problem details
            services.AddProblemDetails();

            // JWT token
            services.AddJwtAuthentication(configuration["Jwt:IssuerSigningKey"]);

            // Database
            services.AddDatabase(configuration["Sql:ConnectionString"], configuration["Redis:ConnectionString"]);

            // Local services
            services.AddScoped<IInstrumentService, InstrumentService>();

            // Storage service
            services.AddInstrumentsStorageServices(configuration["StorageAccount:Url"]);
        }

        public static void UseInstrumentsApiServices(this IApplicationBuilder app, IWebHostEnvironment env, ForexMinerHeimdallrDbContext dbContext)
        {
            // CORS
            app.UseCorsPolicy();

            // ProblemDetails
            app.UseProblemDetails(env);

            // Database migration
            dbContext.Database.Migrate();
        }
    }
}
