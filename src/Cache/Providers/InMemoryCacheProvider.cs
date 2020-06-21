namespace ForexMiner.Heimdallr.Cache.Providers
{
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Threading.Tasks;

    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _innerCache;

        public InMemoryCacheProvider(IMemoryCache innerCache)
        {
            _innerCache = innerCache;
        }

        public Task<T> Get<T>(string key)
        {
            return Task.FromResult(_innerCache.Get<T>(key));
        }

        public Task Set<T>(string key, T value)
        {
            _innerCache.Set<T>(key, value);
            return Task.CompletedTask;
        }

        public Task Remove<T>(string key)
        {
            _innerCache.Remove(key);
            return Task.CompletedTask;
        }

        public Task Flush()
        {
            _innerCache.Dispose();
            return Task.CompletedTask;
        }
    }
}
