/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

namespace TLAuto.Cache
{
    public interface ICacheProvider
    {
        string Get(string key);

        T Get<T>(string key);

        void Set<T>(string key, T data, int cacheTime);

        void Set(string key, object data, int cacheTime);

        void Set(string key, object data);

        bool ContainsKey(string key);

        void Remove(string key);
    }
}