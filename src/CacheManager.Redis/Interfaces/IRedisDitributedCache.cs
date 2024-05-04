using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Interfaces
{
    internal interface IRedisDitributedCache : IDistributedCache
    {
        JsonSerializerOptions? SerializerOptions { get; }
        DistributedCacheEntryOptions? CacheOptions { get; }
    }
}