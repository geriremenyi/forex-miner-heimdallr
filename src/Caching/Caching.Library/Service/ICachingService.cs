namespace ForexMiner.Heimdallr.Caching.Library.Service
{
    using System;
    using System.Threading.Tasks;

    public interface ICachingService
    {
        public Task<T> GetOrCreateValue<T>(string cacheKey, Func<T> valueProviderFunc);

        public Task InvalidateValue(string cacheKey);
    }
}
