using System;

namespace Wheatech.Modulize.Samples.Caching
{
    public interface ICacheService
    {
        T Get<T>(string key);

        void Put<T>(string key, T value);

        void Put<T>(string key, T value, TimeSpan validFor);

        void Remove(string key);

        void Clear();
    }
}
