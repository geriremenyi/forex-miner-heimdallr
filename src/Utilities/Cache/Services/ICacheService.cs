namespace ForexMiner.Heimdallr.Utilities.Cache.Services
{
    using ForexMiner.Heimdallr.Utilities.Cache.Types;
    using System;
    using System.Threading.Tasks;

    public interface ICacheService
    {
        public Task<T> GetOrCreateCacheValue<T>(
            string cacheKey,
            Func<T> cacheValueProviderFunction,
            CacheCreateTarget cacheCreateTarget = CacheCreateTarget.InMemory
        );

        public Task InvalidateCacheValue(string cacheKey);
    }
}
