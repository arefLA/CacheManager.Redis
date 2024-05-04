using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CacheManager.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace CacheManager.Redis.Services
{
    internal class RedisDistributedCache : RedisCache, IRedisDitributedCache
    {
        public RedisDistributedCache(IOptions<RedisCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }
        
        public RedisDistributedCache(IOptions<RedisCacheOptions> optionsAccessor, JsonSerializerOptions serializerOptions) : base(optionsAccessor)
        {
            SerializerOptions = serializerOptions;
        }
        
        public RedisDistributedCache(IOptions<RedisCacheOptions> optionsAccessor, DistributedCacheEntryOptions cacheOptions) : base(optionsAccessor)
        {
            CacheOptions = cacheOptions;
        }

        public RedisDistributedCache(IOptions<RedisCacheOptions> optionsAccessor, DistributedCacheEntryOptions cacheOptions, JsonSerializerOptions serializerOptions) : base(optionsAccessor)
        {
            SerializerOptions = serializerOptions;
            CacheOptions = cacheOptions;
        }
        
        public JsonSerializerOptions? SerializerOptions { get; }
        public DistributedCacheEntryOptions? CacheOptions { get; }
    }
}