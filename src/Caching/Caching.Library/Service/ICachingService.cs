using System;
using System.Threading.Tasks;

namespace Caching.Library.Service
{
    public interface ICachingService
    {
        public Task<T> GetOrCreateValue<T>(string cacheKey, Func<T> valueProviderFunc);

        public Task InvalidateValue(string cacheKey);
    }
}
