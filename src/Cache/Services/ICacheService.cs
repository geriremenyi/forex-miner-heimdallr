﻿namespace ForexMiner.Heimdallr.Cache.Services
{
    using ForexMiner.Heimdallr.Cache.Utilities;
    using System;
    using System.Threading.Tasks;

    public interface ICacheService
    {
        public Task<T> GetOrCreateCacheValue<T>(
            CacheType cacheType,
            string cacheNamespace,
            string cacheName,
            Func<T> fallbackValueProvider,
            CacheCreateTarget cacheCreateTarget = CacheCreateTarget.Local
        );
    }
}
