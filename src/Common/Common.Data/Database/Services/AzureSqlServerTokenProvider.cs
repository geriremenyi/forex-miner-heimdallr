//----------------------------------------------------------------------------------------
// <copyright file="AzureSqlServerTokenProvider.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Services
{
    using Azure.Core;
    using Azure.Identity;
    using ForexMiner.Heimdallr.Common.Caching.Services;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AzureSqlServerTokenProvider : IAzureSqlServerTokenProvider
    {
        private static readonly string[] AZURE_SQL_SCOPES = new[] { "https://database.windows.net//.default" };

        private const string CACHE_KEY = "AzureSqlServer-Token";

        private readonly ICachingService _cachingService;

        public AzureSqlServerTokenProvider(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            // Make sure token is not expired
            var cachedToken = await await _cachingService.GetOrCreateValueAsync(CACHE_KEY, async () => await RequestNewAccessTokenAsync(cancellationToken), cancellationToken);
            if (cachedToken.ExpiresOn <= DateTimeOffset.Now)
            {
                cachedToken = await RequestNewAccessTokenAsync(cancellationToken);
            }

            return cachedToken.Token;
        }

        public string GetAccessToken()
        {
            // Make sure token is not expired
            var cachedToken =  _cachingService.GetOrCreateValue(CACHE_KEY, () => RequestNewAccessToken());
            if (cachedToken.ExpiresOn <= DateTimeOffset.Now)
            {
                cachedToken = RequestNewAccessToken();
            }

            return cachedToken.Token;
        }

        private async Task<AccessToken> RequestNewAccessTokenAsync(CancellationToken cancellationToken = default) 
        {
            var tokenRequestContext = new TokenRequestContext(AZURE_SQL_SCOPES);
            var token = await new DefaultAzureCredential().GetTokenAsync(tokenRequestContext, cancellationToken);

            return token;
        }

        private AccessToken RequestNewAccessToken()
        {
            var tokenRequestContext = new TokenRequestContext(AZURE_SQL_SCOPES);
            var token = new DefaultAzureCredential().GetToken(tokenRequestContext);

            return token;
        }
    }
}
