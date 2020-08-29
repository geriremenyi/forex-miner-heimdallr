namespace Caching.Library
{
    using Caching.Library.Service;
    using System;
    using System.Threading.Tasks;

    public abstract class Cacheable<T>
    {
        private readonly ICachingService _cachingService;

        public abstract string CacheKey { get; }

        public abstract Func<T> CacheValueProviderFunction { get; }

        public Cacheable(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public async Task<T> GetValue()
        {
            return await _cachingService.GetOrCreateValue<T>(CacheKey, CacheValueProviderFunction);
        }

        public async Task<T> GetValueWithForceRefresh()
        {
            await _cachingService.InvalidateValue(CacheKey);
            return await GetValue();
        }
    }
}
