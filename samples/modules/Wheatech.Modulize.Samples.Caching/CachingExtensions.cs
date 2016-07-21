using System;

namespace Wheatech.Modulize.Samples.Caching
{
    public static class CachingExtensions
    {
        public static T Get<T>(this ICacheService cacheService, string key, Func<T> factory, TimeSpan validFor)
        {
            var result = cacheService.Get<T>(key);

            if (result == null && factory != null)
            {
                var computed = factory();

                if (validFor == TimeSpan.MinValue)
                    cacheService.Put(key, computed);
                else
                    cacheService.Put(key, computed, validFor);
                return computed;
            }

            return result;
        }

        public static T Get<T>(this ICacheService cacheService, string key, Func<T> factory)
        {
            return Get(cacheService, key, factory, TimeSpan.MinValue);
        }
    }
}
