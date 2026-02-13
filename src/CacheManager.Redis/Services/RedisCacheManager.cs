using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CacheManager.Redis.Extensions;
using CacheManager.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Services
{
    public sealed class RedisCacheManager<TEntity>(IRedisDistributedCache cache) : IRedisCacheManager<TEntity>
        where TEntity : class
    {
        private static readonly JsonSerializerOptions DefaultSerializerOptions = new(JsonSerializerDefaults.General);

        public bool TryGet(string key, out TEntity? response)
        {
            response = null;
            if (!key.HasValue()) return false;
            
            var cachedResponse = cache.Get(key);
            if (cachedResponse == null) return false;

            response = Deserialize(cachedResponse);
            return response is not null;
        }

        public async Task<TEntity?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            if (!key.HasValue()) return null;
            
            var cachedResponse = await cache.GetAsync(key, cancellationToken);
            return cachedResponse is null ? null : Deserialize(cachedResponse);
        }

        public void Set(string key, TEntity entity)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            var payload = Serialize(entity);

            if (cache.CacheOptions is null)
            {
                cache.Set(key, payload);
            }
            else
            {
                cache.Set(key, payload, cache.CacheOptions);
            }
        }

        public bool TrySet(string key, TEntity entity)
        {
            if (!key.HasValue())
                return false;

            var payload = Serialize(entity);

            if (cache.CacheOptions is null)
            {
                cache.Set(key, payload);
            }
            else
            {
                cache.Set(key, payload, cache.CacheOptions);
            }

            return true;
        }

        public void Set(string key, TEntity entity, DistributedCacheEntryOptions options)
        {
            if (!key.HasValue())
                throw new ArgumentException("key should have value", nameof(key));
            
            cache.Set(key, Serialize(entity), options);
        }

        public bool TrySet(string key, TEntity entity, DistributedCacheEntryOptions options)
        {
            if (!key.HasValue())
                return false;
            
            cache.Set(key, Serialize(entity), options);
            return true;
        }

        public Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);

            var payload = Serialize(entity);

            return cache.CacheOptions is null
                ? cache.SetAsync(key, payload, cancellationToken)
                : cache.SetAsync(key, payload, cache.CacheOptions, cancellationToken);
        }

        public Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            return cache.SetAsync(key, Serialize(entity), options, cancellationToken);
        }

        public void Refresh(string key)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            cache.Refresh(key);
        }

        public bool TryRefresh(string key)
        {
            if (!key.HasValue())
                return false;
            
            cache.Refresh(key);

            return true;
        }

        public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            return cache.RefreshAsync(key, cancellationToken);
        }

        public void Remove(string key)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            cache.Remove(key);
        }

        public bool TryRemove(string key)
        {
            if (!key.HasValue())
                return false;
            
            cache.Remove(key);

            return true;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            return cache.RemoveAsync(key, cancellationToken);
        }
            

        private TEntity? Deserialize(byte[] bytes)
        {
            try
            {
                return JsonSerializer.Deserialize<TEntity>(bytes, cache.SerializerOptions ?? DefaultSerializerOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private byte[] Serialize(TEntity entity)
            => JsonSerializer.SerializeToUtf8Bytes(entity, cache.SerializerOptions ?? DefaultSerializerOptions);
    }
}