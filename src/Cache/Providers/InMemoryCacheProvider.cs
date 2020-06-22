namespace ForexMiner.Heimdallr.Cache.Providers
{
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Threading.Tasks;

    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly MemoryCacheEntryOptions _innerCacheOptions;
        private readonly IMemoryCache _innerCache;

        public InMemoryCacheProvider(IMemoryCache innerCache)
        {
            _innerCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            _innerCache = innerCache;
        }

        public Task<T> Get<T>(string key)
        {
            return Task.FromResult(_innerCache.Get<T>(key));
        }

        public Task Set<T>(string key, T value)
        {
            _innerCache.Set<T>(key, value, _innerCacheOptions);
            return Task.CompletedTask;
        }

        public Task Remove<T>(string key)
        {
            _innerCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
