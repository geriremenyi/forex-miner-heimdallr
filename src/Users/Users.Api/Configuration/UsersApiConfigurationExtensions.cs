namespace ForexMiner.Heimdallr.Users.Api.Configuration
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using ForexMiner.Heimdallr.Common.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Builder;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public static class UsersApiConfigurationExtensions
    {
        public static void AddUsersApiServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            // Cors policy
            services.AddCorsPolicy();

            // Problem details
            services.AddProblemDetails();

            // JWT token
            services.AddJwtAuthentication(configuration["Jwt:IssuerSigningKey"]);

            // Database
            services.AddDatabase(environment.IsDevelopment(), configuration["SqlServer:ConnectionString"], configuration["RedisCache:ConnectionString"]);

            // Other utilities
            services.AddAutoMapper(typeof(UsersApiConfigurationExtensions));

            // Local services
            services.AddScoped<IUserService, UserService>();
        }

        public static void UseUsersApiServices(this IApplicationBuilder app, IWebHostEnvironment environment, ForexMinerHeimdallrDbContext dbContext)
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
