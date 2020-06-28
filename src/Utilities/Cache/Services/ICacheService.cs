namespace ForexMiner.Heimdallr.Utilities.Cache.Services
{
    using ForexMiner.Heimdallr.Utilities.Cache.Types;
    using System;
    using System.Threading.Tasks;

    public interface ICacheService
    {
        public Task<T> GetOrCreateCacheValue<T>(
            CacheType cacheType,
            string cacheNamespace,
            string cacheName,
            Func<T> fallbackValueProvider,
            CacheCreateTarget cacheCreateTarget = CacheCreateTarget.InMemory
        );
    }
}
