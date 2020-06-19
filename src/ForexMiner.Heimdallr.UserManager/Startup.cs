namespace ForexMiner.Heimdallr.UserManager
{
    using AutoMapper;
    using ForexMiner.Heimdallr.UserManager.Database;
    using ForexMiner.Heimdallr.UserManager.Services;
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.ComponentModel.DataAnnotations;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<UserManagerDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ForexMinerDb")));
            
            // Routing
            services.AddControllers();
            services.AddApiVersioning();

            // ProblemDetails
            services.AddProblemDetails(setup =>
            {
                setup.Map<DTO.Exceptions.ProblemDetailsException>(exception => new ProblemDetails
                {
                    Type = $"https://httpstatuses.com/{((int) exception.Status)}",
                    Title = exception.Status.ToString(),
                    Detail = exception.Message,
                    Status = (int) exception.Status
                });
            });

            // Automapper
            services.AddAutoMapper(typeof(Startup));

            // UserManager services
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManagerDbContext userManagerDbContext)
        {
            // Development
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Database
            userManagerDbContext.Database.Migrate();

            // ProblemDetails
            app.UseProblemDetails();

            // Routing
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
