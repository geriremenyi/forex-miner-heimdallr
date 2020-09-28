namespace ForexMiner.Heimdallr.Configurations.Api
{
    using ForexMiner.Heimdallr.Common.ServiceConfiguration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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

            // Exception handling
            services.AddProblemDetailsExceptionHandling();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Exception handling
            app.UseProblemDetails(env);

            // Routing and authentication
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
