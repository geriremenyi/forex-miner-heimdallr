//----------------------------------------------------------------------------------------
// <copyright file="ForexMinerHeimdallrDbContext" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Context
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Database context of the forex-miner-heimdallr.
    /// 
    /// This contains the entire database shceme for all microservices
    /// implemented under forex-miner-heimdallr package.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ForexMinerHeimdallrDbContext : DbContext
    {
        /// <summary>
        /// List of registered users in the application
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// List of instruments defined tradeable by the application
        /// </summary>
        public DbSet<Instrument> Instruments { get; set; }

        /// <summary>
        /// All users' all trades opened through the application
        /// </summary>
        public DbSet<Trade> Trades { get; set; }

        /// <summary>
        /// Constructor of the ForexMinerHeimdallrDbContext
        /// </summary>
        /// <param name="options">Connection parameters</param>
        public ForexMinerHeimdallrDbContext(DbContextOptions<ForexMinerHeimdallrDbContext> options) : base(options) { }
    }
}
