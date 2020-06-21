namespace ForexMiner.Heimdallr.UserManager.Database
{
    using Microsoft.EntityFrameworkCore;

    public class UserManagerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserManagerDbContext(DbContextOptions<UserManagerDbContext> options) : base(options) { }
    }
}
