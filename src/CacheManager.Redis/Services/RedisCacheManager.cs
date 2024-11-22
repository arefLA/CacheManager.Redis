using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CacheManager.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Services
{
    public class RedisCacheManager<TEntity>(IRedisDistributedCache cache) : IRedisCacheManager<TEntity>
        where TEntity : class
    {
        public virtual bool TryGet(string key, out TEntity? response)
        {
            response = default;
            if (string.IsNullOrWhiteSpace(key)) return false;

            var cachedResponse = cache.Get(key);
            if (cachedResponse is null) return false;

            try
            {
                response = JsonSerializer.Deserialize<TEntity>(cachedResponse, cache.SerializerOptions);
                return true;
            }
            catch (Exception) //Why?
            {
                return false;
            }
        }

        public virtual async Task<TEntity?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            var cachedResponse = await cache.GetAsync(key, cancellationToken);
            if (cachedResponse is null) return null;

            using var memoryStream = new MemoryStream(cachedResponse);

            return await JsonSerializer.DeserializeAsync<TEntity>(memoryStream, cache.SerializerOptions,
                cancellationToken);
        }

        public virtual void Set(string key, TEntity entity)
        {
            Guard.Against.NullOrWhiteSpace(key);

            if (cache.CacheOptions is null)
            {
                cache.Set(key, GetBytes(entity));
            }
            else
            {
                cache.Set(key, GetBytes(entity), cache.CacheOptions!);
            }
        }

        public bool TrySet(string key, TEntity entity)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            Set(key, entity);
            return true;
        }

        public virtual void Set(string key, TEntity entity, DistributedCacheEntryOptions options)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            cache.Set(key, GetBytes(entity), options);
        }

        public bool TrySet(string key, TEntity entity, DistributedCacheEntryOptions options)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            Set(key, entity, options);
            return true;
        }

        public virtual Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);

            if (cache.CacheOptions is null)
            {
                return cache.SetAsync(key, GetBytes(entity), cancellationToken);
            }

            return SetAsync(key, entity, cache.CacheOptions!, cancellationToken);
        }

        public virtual Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);

            return cache.SetAsync(key, GetBytes(entity), options, cancellationToken);
        }

        public virtual void Refresh(string key)
        {
            Guard.Against.NullOrWhiteSpace(key);

            cache.Refresh(key);
        }

        public bool TryRefresh(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            cache.Refresh(key);

            return true;
        }

        public virtual Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);

            return cache.RefreshAsync(key, cancellationToken);
        }

        public virtual void Remove(string key)
        {
            Guard.Against.NullOrWhiteSpace(key);

            cache.Remove(key);
        }

        public bool TryRemove(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            cache.Remove(key);

            return true;
        }

        public virtual Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);

            return cache.RemoveAsync(key, cancellationToken);
        }

        private byte[] GetBytes(TEntity entity)
            => JsonSerializer.SerializeToUtf8Bytes(entity, cache.SerializerOptions);
    }
}