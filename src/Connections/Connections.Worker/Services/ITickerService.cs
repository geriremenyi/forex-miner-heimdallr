//----------------------------------------------------------------------------------------
// <copyright file="ITickerService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Worker.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Ticker service interface
    /// </summary>
    public interface ITickerService
    {
        /// <summary>
        /// Do a tick and handle trading
        /// </summary>
        /// <returns>A task</returns>
        public Task Tick();
    }
}
