using System.Text.Json;
using CacheManager.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace CacheManager.Redis.Services
{
    internal sealed class RedisDistributedCache : RedisCache, IRedisDistributedCache
    {
        public RedisDistributedCache(
            IOptions<RedisCacheOptions> optionsAccessor, 
            JsonSerializerOptions? serializerOptions = null, 
            DistributedCacheEntryOptions? cacheOptions = null) : base(optionsAccessor)
        {
            SerializerOptions = serializerOptions;
            CacheOptions = cacheOptions;
        }
        
        public JsonSerializerOptions? SerializerOptions { get; }
        public DistributedCacheEntryOptions? CacheOptions { get; }
    }
}