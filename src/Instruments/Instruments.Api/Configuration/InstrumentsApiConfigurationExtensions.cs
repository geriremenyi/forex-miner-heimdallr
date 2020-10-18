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
    using Microsoft.Extensions.Hosting;

    public static class InstrumentsApiConfigurationExtensions {
        public static void AddInstrumentsApiServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            // Cors policy
            services.AddCorsPolicy();

            // Problem details
            services.AddProblemDetails();

            // JWT token
            services.AddJwtAuthentication(configuration["Jwt:IssuerSigningKey"]);

            // Database
            services.AddDatabase(environment.IsDevelopment(), configuration["SqlServer:ConnectionString"], configuration["RedisCache:ConnectionString"]);

            // Local services
            services.AddScoped<IInstrumentService, InstrumentService>();

            // Storage service
            services.AddInstrumentsStorageServices(configuration["StorageAccount:Url"]);
        }

        public static void UseInstrumentsApiServices(this IApplicationBuilder app, IWebHostEnvironment environment, ForexMinerHeimdallrDbContext dbContext)
        {
            // CORS
            app.UseCorsPolicy();

            // ProblemDetails
            app.UseProblemDetails(environment.IsDevelopment());

            // Database migration
            dbContext.MigrateDatabase();
        }
    }
}
