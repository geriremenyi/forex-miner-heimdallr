namespace ForexMiner.Heimdallr.UserManager.Common.Types
{
    using ForexMiner.Heimdallr.Utilities.Cache.Services;
    using ForexMiner.Heimdallr.Utilities.Cache.Types;
    using ForexMiner.Heimdallr.Utilities.Constants;
    using System;

    public class JwtEncryptionSecret : Cacheable<string>
    {
        private static readonly string SECRET_NAMESPACE = "jwt";

        private static readonly string SECRET_NAME = "encryption";

        public override string CacheKey => $"{CacheConstants.Secret}-{SECRET_NAMESPACE}-{SECRET_NAME}";

        public override Func<string> CacheValueProviderFunction => throw new NotImplementedException();

        public override CacheCreateTarget CacheTarget => throw new NotImplementedException();

        public JwtEncryptionSecret(ICacheService cacheService) : base(cacheService)
        {
            
        }
    }
}
