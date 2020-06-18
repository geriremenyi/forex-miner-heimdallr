namespace ForexMiner.Heimdallr.UserManager
{
    using AutoMapper;
    using ForexMiner.Heimdallr.UserManager.Database;
    using ForexMiner.Heimdallr.UserManager.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
