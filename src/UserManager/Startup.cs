namespace ForexMiner.Heimdallr.UserManager
{
    using AutoMapper;
    using ForexMiner.Heimdallr.UserManager.Configuration;
    using ForexMiner.Heimdallr.UserManager.Database;
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Routing
            services.AddControllers();
            services.AddApiVersioning();

            // Exception handling
            services.AddProblemDetailsExceptionHandling();

            // JWT authentication
            services.AddJwtAuthentication();

            // UserManager services and all dependencies
            services.AddUserManagerServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManagerDbContext userManagerDbContext)
        {
            // Custom middlewares for UserManager
            userManagerDbContext.Database.Migrate();

            // Exception handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseProblemDetails();

            // Routing and authentication
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
