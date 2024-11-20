using System.Text.Json;
using CacheManager.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace CacheManager.Redis.Services
{
    public sealed class RedisDistributedCache(
        IOptions<RedisCacheOptions> optionsAccessor,
        JsonSerializerOptions? serializerOptions = null,
        DistributedCacheEntryOptions? cacheOptions = null)
        : RedisCache(optionsAccessor), IRedisDistributedCache
    {
        public JsonSerializerOptions? SerializerOptions { get; } = serializerOptions;
        public DistributedCacheEntryOptions? CacheOptions { get; } = cacheOptions;
    }
}