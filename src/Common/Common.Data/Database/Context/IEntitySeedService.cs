//----------------------------------------------------------------------------------------
// <copyright file="IDbSeed.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Context
{
    /// <summary>
    /// Interface for entity seed service
    /// </summary>
    public interface IEntitySeedService
    {
        /// <summary>
        /// Seed data
        /// </summary>
        public void Seed();
    }
}
