namespace ForexMiner.Heimdallr.Users.Api.Configuration
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Users.Api.Database;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class UserManagerServiceConfiguration
    {
        public static void AddUserManagerServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<UsersApiDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ForexMinerDb")));

            // Other utilities
            services.AddAutoMapper(typeof(Startup));

            // Services defined in UserManager
            services.AddScoped<IUserService, UserService>();
        }
    }
}
