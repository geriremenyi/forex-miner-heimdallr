namespace ForexMiner.Heimdallr.UserManager.Services
{
    using ForexMiner.Heimdallr.Cache.Services;
    using ForexMiner.Heimdallr.Cache.Utilities;
    using ForexMiner.Heimdallr.Data.Constants;
    using System.Threading.Tasks;

    public class UserSecretService : IUserSecretService
    {
        private readonly ICacheService _cacheService;

        public UserSecretService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<string> GetJwtEncryptionSecret()
        {
            return await _cacheService.GetOrCreateCacheValue<string>(CacheType.Secret, JwtConstants.Namespace, JwtConstants.EncryptionSecret, () => "aYPg2QjKQBY4Uqx8");
        }
    }
}
