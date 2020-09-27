namespace ForexMiner.Heimdallr.Caching.Library.Providers.InMemory
{
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Threading.Tasks;

    public class InMemoryCacheProvider : IInMemoryCacheProvider
    {
        private static readonly int CACHE_EXPIRY_IN_MINS = 5;

        private readonly MemoryCacheEntryOptions _innerCacheOptions;
        private readonly IMemoryCache _innerCache;

        public InMemoryCacheProvider(IMemoryCache innerCache)
        {
            _innerCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(CACHE_EXPIRY_IN_MINS));
            _innerCache = innerCache;
        }

        public Task<T> Get<T>(string key)
        {
            return Task.FromResult(_innerCache.Get<T>(key));
        }

        public Task Set<T>(string key, T value)
        {
            _innerCache.Set(key, value, _innerCacheOptions);
            return Task.CompletedTask;
        }

        public Task Remove(string key)
        {
            _innerCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
