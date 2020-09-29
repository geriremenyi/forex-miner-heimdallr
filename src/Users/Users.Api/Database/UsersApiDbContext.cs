namespace ForexMiner.Heimdallr.Users.Api.Database
{
    using Microsoft.EntityFrameworkCore;

    public class UsersApiDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersApiDbContext(DbContextOptions<UsersApiDbContext> options) : base(options) { }
    }
}
