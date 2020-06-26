namespace ForexMiner.Heimdallr.UserManager
{
    using AutoMapper;
    using ForexMiner.Heimdallr.UserManager.Database;
    using ForexMiner.Heimdallr.UserManager.Services;
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;

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
                setup.Map<Data.Exceptions.ProblemDetailsException>(exception => new ProblemDetails
                {
                    Type = $"https://httpstatuses.com/{((int) exception.Status)}",
                    Title = exception.Status.ToString(),
                    Detail = exception.Message,
                    Status = (int) exception.Status
                });
            });

            // Automapper
            services.AddAutoMapper(typeof(Startup));

            // In-memory cache
            services.AddMemoryCache();

            // Auth
            // configure jwt authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("aYPg2QjKQBY4Uqx8")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
