namespace ForexMiner.Heimdallr.Cache.Providers
{
    using System.Threading.Tasks;

    public interface ICacheProvider
    {
        public Task<T> Get<T>(string key);

        public Task Set<T>(string key, T value);

        public Task Remove<T>(string key);

        public Task Flush();
    }
}
