using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis
{
    public sealed class RedisCacheMangerOptions
    {
        public string? InstanceName { get; set; } = null;
        public JsonSerializerOptions? SerializerOptions { get; set; } = null;
        public DistributedCacheEntryOptions? DefaultCacheOptions { get; set; } = null;
        public Type? CustomImplementation { get; set; } = null;
    }
}