namespace ForexMiner.Heimdallr.Users.Api
{
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Users.Api.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Routing
            services.AddControllers();
            services.AddApiVersioning();

            // All utility- and local services
            services.AddUsersApiServices(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ForexMinerHeimdallrDbContext dbContext)
        {
            // Custom 
            app.UseUsersApiServices(env, dbContext);
            
            // System
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
