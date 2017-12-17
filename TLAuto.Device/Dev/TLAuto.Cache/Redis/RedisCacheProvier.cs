/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace TLAuto.Cache.Redis
{
    public class RedisCacheProvier : ICacheProvider
    {
        private readonly ConnectionMultiplexer _redis;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectStr">eg:localhost,password=123123</param>
        public RedisCacheProvier(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);
        }

        public string Get(string key)
        {
            var db = _redis.GetDatabase();
            return db.StringGet(key);
        }

        public T Get<T>(string key)
        {
            var db = _redis.GetDatabase();
            return JsonConvert.DeserializeObject<T>(db.StringGet(key));
        }

        public void Set<T>(string key, T data, int cacheTime)
        {
            var db = _redis.GetDatabase();
            string str = JsonConvert.SerializeObject(data);
            db.StringSet(key, str, TimeSpan.FromMinutes(cacheTime));
        }

        public void Set(string key, object data, int cacheTime)
        {
            var db = _redis.GetDatabase();
            string str = JsonConvert.SerializeObject(data);
            db.StringSet(key, str, TimeSpan.FromMinutes(cacheTime));
        }

        public void Set(string key, object data)
        {
            var db = _redis.GetDatabase();
            string str = JsonConvert.SerializeObject(data);
            db.StringSet(key, str);
        }

        public bool ContainsKey(string key)
        {
            var db = _redis.GetDatabase();
            return db.KeyExists(key);
        }

        public void Remove(string key)
        {
            var db = _redis.GetDatabase();
            db.KeyDelete(key);
        }
    }
}