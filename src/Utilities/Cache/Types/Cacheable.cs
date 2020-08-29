namespace ForexMiner.Heimdallr.Utilities.Cache.Types
{
    using ForexMiner.Heimdallr.Utilities.Cache.Services;
    using System;
    using System.Threading.Tasks;

    public abstract class Cacheable<T>
    {
        private readonly ICacheService _cacheService;

        public abstract string CacheKey { get; }

        public abstract Func<T> CacheValueProviderFunction { get;  }

        public abstract CacheCreateTarget CacheTarget { get; }

        public Cacheable(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<T> GetValue() {
            return await _cacheService.GetOrCreateCacheValue<T>(CacheKey, CacheValueProviderFunction, CacheTarget);
        }

        public async Task InvalidateValue() 
        {
            await _cacheService.InvalidateCacheValue(CacheKey);
        }

        public async Task<T> GetValueWithForceRefresh() 
        {
            await InvalidateValue();
            return await GetValue();
        }
    }
}
