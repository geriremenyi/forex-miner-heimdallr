namespace ForexMiner.Heimdallr.DataModel
{
    using Microsoft.EntityFrameworkCore;

    public class ForexMinerHeimdallrDbContext : DbContext
    {
        public DbSet<User.Database.User> Users { get; set; }
    }
}
