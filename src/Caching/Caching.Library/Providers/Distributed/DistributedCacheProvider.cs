namespace ForexMiner.Heimdallr.Caching.Library.Providers.Distributed
{
    using Microsoft.Extensions.Caching.Distributed;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class DistributedCacheProvider : IDistributedCacheProvider
    {
        private static readonly int CACHE_EXPIRY_IN_MINS = 30;

        private readonly DistributedCacheEntryOptions _innerCacheOptions;
        private readonly IDistributedCache _innerCache;

        public DistributedCacheProvider(IDistributedCache innerCache)
        {
            _innerCacheOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(CACHE_EXPIRY_IN_MINS));
            _innerCache = innerCache;
        }

        public async Task<T> Get<T>(string key)
        {
            return JsonSerializer.Deserialize<T>(await _innerCache.GetAsync(key));
        }

        public async Task Set<T>(string key, T value)
        {
            await _innerCache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value), _innerCacheOptions);
        }

        public async Task Remove(string key)
        {
            await _innerCache.RemoveAsync(key);
        }
    }
}
