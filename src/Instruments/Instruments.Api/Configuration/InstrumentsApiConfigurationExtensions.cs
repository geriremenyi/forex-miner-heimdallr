namespace ForexMiner.Heimdallr.Instruments.Api.Configuration
{
    using Microsoft.Extensions.DependencyInjection;
    using ForexMiner.Heimdallr.Instruments.Api.Services;
    using ForexMiner.Heimdallr.Instruments.Configuration;
    using Microsoft.Extensions.Configuration;
    using ForexMiner.Heimdallr.Common.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Microsoft.Extensions.Hosting;
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Mapping;

    public static class InstrumentsApiConfigurationExtensions {
        public static void AddInstrumentsApiServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
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

            // Local services
            services.AddScoped<IInstrumentService, InstrumentService>();

            // Storage service
            services.AddInstrumentsStorageServices(configuration["StorageAccount-ConnectionString"]);
        }

        public static void UseInstrumentsApiServices(this IApplicationBuilder app, IWebHostEnvironment environment, ForexMinerHeimdallrDbContext dbContext)
        {
            // CORS
            app.UseCorsPolicy();

            // ProblemDetails
            app.UseProblemDetails(environment.IsDevelopment());
        }
    }
}
