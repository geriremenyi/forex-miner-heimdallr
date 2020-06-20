using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;

namespace ForexMiner.Heimdallr.UserManager.Services
{
    public class UserSecretService : IUserSecretService
    {
        private IMemoryCache _cache;

        public UserSecretService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GetJwtEncryptionSecret()
        {
            return _cache.GetOrCreate<string>("JwtEncryptionSecret", cacheEntry => {
                return "aYPg2QjKQBY4Uqx8";
            });
        }
    }
}
