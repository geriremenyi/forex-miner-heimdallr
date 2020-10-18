//----------------------------------------------------------------------------------------
// <copyright file="IAzureSqlServerTokenProvider.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Services
{
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interception object which handles azure token authentication
    /// </summary>
    public class TokenAuthenticationDbConnectionInterceptor : DbConnectionInterceptor
    {
        /// <summary>
        /// Host url of Azure sql server DBs
        /// </summary>
        private const string AZURE_DB_HOST = "database.windows.net";

        /// <summary>
        /// Azure token provider service
        /// </summary>
        private readonly IAzureSqlServerTokenProvider _tokenProvider;
        
        /// <summary>
        /// TokenAuthenticationDbConnectionInterceptor constructor
        /// 
        /// Sets up the token provider service
        /// </summary>
        /// <param name="tokenProvider"></param>
        public TokenAuthenticationDbConnectionInterceptor(IAzureSqlServerTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        /// <summary>
        /// Override connection opening
        /// </summary>
        /// <param name="connection">DB connection</param>
        /// <param name="eventData">DB connection data</param>
        /// <param name="result">Interception results</param>
        /// <returns>Interception results</returns>
        public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            var sqlConnection = (SqlConnection)connection;
            if (IsTokenBasedAuthNeeded(sqlConnection))
            {
                sqlConnection.AccessToken = _tokenProvider.GetAccessToken();
            }

            return base.ConnectionOpening(connection, eventData, result);
        }

        /// <summary>
        /// Override async connection opening
        /// </summary>
        /// <param name="connection">DB connection</param>
        /// <param name="eventData">DB connection data</param>
        /// <param name="result">Interception results</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Interception results</returns>
        public override async Task<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            var sqlConnection = (SqlConnection)connection;
            if (IsTokenBasedAuthNeeded(sqlConnection))
            {
                sqlConnection.AccessToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
            }

            return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        }

        /// <summary>
        /// Decides wheter a connection requires an auth token
        /// </summary>
        /// <param name="connection">DB connection in question</param>
        /// <returns>Is the token needed?</returns>
        private static bool IsTokenBasedAuthNeeded(SqlConnection connection)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);

            return (
                connectionStringBuilder.DataSource.Contains(AZURE_DB_HOST, System.StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrEmpty(connectionStringBuilder.UserID)
            );
        }

    }
}
