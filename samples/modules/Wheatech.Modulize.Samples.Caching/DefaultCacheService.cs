using System;
using System.Globalization;
using System.Runtime.Caching;

namespace Wheatech.Modulize.Samples.Caching
{
    /// <summary>
    /// Provides a per tenant <see cref="ICacheService"/> implementation.
    /// </summary>
    public class DefaultCacheService : ICacheService
    {
        private event EventHandler Signaled;

        // MemoryCache is optimal with one instance, see: http://stackoverflow.com/questions/8463962/using-multiple-instances-of-memorycache/13425322#13425322
        private readonly MemoryCache _cache = MemoryCache.Default;

        public void Put<T>(string key, T value)
        {
            // Keys are already prefixed by DefaultCacheService so no need to do it here again.
            _cache.Set(key, value, GetCacheItemPolicy(ObjectCache.InfiniteAbsoluteExpiration));
        }

        public void Put<T>(string key, T value, TimeSpan validFor)
        {
            _cache.Set(key, value, GetCacheItemPolicy(new DateTimeOffset(DateTime.UtcNow.Add(validFor))));
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            if (Signaled != null)
            {
                Signaled(null, EventArgs.Empty);
            }
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        private CacheItemPolicy GetCacheItemPolicy(DateTimeOffset absoluteExpiration)
        {
            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = ObjectCache.NoSlidingExpiration
            };

            cacheItemPolicy.ChangeMonitors.Add(new TenantCacheClearMonitor(this));

            return cacheItemPolicy;
        }

        public class TenantCacheClearMonitor : ChangeMonitor
        {
            private readonly DefaultCacheService _cacheService;

            public override string UniqueId { get; } = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

            public TenantCacheClearMonitor(DefaultCacheService cacheService)
            {
                _cacheService = cacheService;
                _cacheService.Signaled += OnSignalRaised;
                InitializationComplete();
            }

            protected override void Dispose(bool disposing)
            {
                Dispose();
                _cacheService.Signaled -= OnSignalRaised;
            }

            private void OnSignalRaised(object sender, EventArgs e)
            {
                // Cache objects are obligated to remove entry upon change notification.
                OnChanged(null);
            }
        }
    }
}
