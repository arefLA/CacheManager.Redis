using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Interfaces
{
    public interface IRedisDistributedCache : IDistributedCache
    {
        JsonSerializerOptions? SerializerOptions { get; }
        DistributedCacheEntryOptions? CacheOptions { get; }
    }
}