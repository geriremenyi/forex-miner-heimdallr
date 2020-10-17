//----------------------------------------------------------------------------------------
// <copyright file="IAzureSqlServerTokenProvider.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for azure hosted SQL Server token provider service
    /// </summary>
    public interface IAzureSqlServerTokenProvider
    {
        /// <summary>
        /// Get the access token to access SQL Server asynchronously
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Token either from cache or from managed identity token provider</returns>
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the access token to access SQL Server asynchronously
        /// </summary>
        /// <returns>Token either from cache or from managed identity token provider</returns>
        string GetAccessToken();
    }
}
