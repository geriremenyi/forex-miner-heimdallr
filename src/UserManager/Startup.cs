namespace ForexMiner.Heimdallr.UserManager
{
    using ForexMiner.Heimdallr.UserManager.Configuration;
    using ForexMiner.Heimdallr.UserManager.Database;
    using ForexMiner.Heimdallr.Utilities.Configuration;
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

            // UserManager services and all dependencies
            services.AddUserManagerServices(_configuration);

            // Exception handling
            services.AddProblemDetailsExceptionHandling();

            // JWT authentication
            services.AddJwtAuthentication(_configuration["JWT:IssuerSigningKey"]);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManagerDbContext userManagerDbContext)
        {
            // Custom middlewares for UserManager
            userManagerDbContext.Database.Migrate();

            // Exception handling
            app.UseProblemDetails(env);

            // Routing and authentication
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
