namespace ForexMiner.Heimdallr.Cache.Providers
{
    using StackExchange.Redis;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class RedisCacheProvider : ICacheProvider
    {
        private readonly string _redisConnectionString;
        private ConnectionMultiplexer _innerCache;

        public RedisCacheProvider(string redisHost, string redisPort)
        {
            _redisConnectionString = $"{redisHost}:{redisPort}";
        }

        public async Task<T> Get<T>(string key)
        {
            var cacheDb = await EnsureDbInitialized();
            var value =  (string) await cacheDb.StringGetAsync(key);
            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task Set<T>(string key, T value)
        {
            var cacheDb = await EnsureDbInitialized();
            var valueAsAString = JsonSerializer.Serialize(value);
            await cacheDb.StringSetAsync(key, valueAsAString);
        }

        public async Task Remove<T>(string key)
        {
            var cacheDb = await EnsureDbInitialized();
            await cacheDb.KeyDeleteAsync(key);
        }

        public async Task Flush()
        {
            await _innerCache.GetServer(_redisConnectionString).FlushDatabaseAsync();
        }

        private async Task<IDatabase> EnsureDbInitialized()
        {
            if (_innerCache == null)
            {
                try
                {
                    _innerCache = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);
                }
                catch (RedisConnectionException redisEx)
                {
                    throw redisEx;
                }
            }

            return _innerCache.GetDatabase();
        }
    }
}
