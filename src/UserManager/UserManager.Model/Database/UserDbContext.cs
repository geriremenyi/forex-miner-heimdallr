namespace ForexMiner.Heimdallr.UserManager.Model.Database
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class UserDbContext : DbContext
    {
        private IConfiguration configuration;

        public DbSet<User> Users { get; set; }

        public UserDbContext(IConfiguration configuration) => this.configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(configuration.GetConnectionString("ForexMinerDatabase"));
    }
}
