using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CacheManager.Redis.Extensions;
using CacheManager.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Services
{
    public class RedisCacheManager<TEntity> : IRedisCacheManager<TEntity> where TEntity : class
    {
        protected readonly IRedisDistributedCache Cache;

        public RedisCacheManager(IRedisDistributedCache cache)
            => Cache = cache;

        public virtual bool TryGet(string key, out TEntity? response)
        {
            response = default;
            if (!key.HasValue()) return false;
            
            var cachedResponse = Cache.Get(key);
            if (cachedResponse == null) return false;
    
            var responseString = Encoding.UTF8.GetString(cachedResponse);
            
            return responseString.TryDeserialize(out response, Cache.SerializerOptions);
        }

        public virtual async Task<TEntity?> TryGetAsync(string key, CancellationToken cancellationToken = default)
        {
            if (!key.HasValue()) return null;
            
            var cachedResponse = await Cache.GetAsync(key, cancellationToken);
            if (cachedResponse is null) return null;
            
            var responseString = Encoding.UTF8.GetString(cachedResponse);

            var isSerialized = responseString.TryDeserialize<TEntity>(out var response,Cache.SerializerOptions);

            return isSerialized
                ? response
                : null;
        }

        public virtual void Set(string key, TEntity entity)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            FunctionConditionRunner(
                Cache.CacheOptions is null, 
                () => Cache.Set(key, GetBytes(entity)),
                () => Cache.Set(key, GetBytes(entity), Cache.CacheOptions!));
        }

        public bool TrySet(string key, TEntity entity)
        {
            if (!key.HasValue())
                return false;
            
            FunctionConditionRunner(
                Cache.CacheOptions is null, 
                () => Cache.Set(key, GetBytes(entity)),
                () => Cache.Set(key, GetBytes(entity), Cache.CacheOptions!));
            return true;
        }

        public virtual void Set(string key, TEntity entity, DistributedCacheEntryOptions options)
        {
            if (!key.HasValue())
                throw new ArgumentException("key should have value", nameof(key));
            
            Cache.Set(key, GetBytes(entity), options);
        }

        public bool TrySet(string key, TEntity entity, DistributedCacheEntryOptions options)
        {
            if (!key.HasValue())
                return false;
            
            Cache.Set(key, GetBytes(entity), options);
            return true;
        }

        public virtual Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            return FunctionConditionRunner(
                Cache.CacheOptions is null,
                () => Cache.SetAsync(key, GetBytes(entity), cancellationToken),
                () => Cache.SetAsync(key, GetBytes(entity), Cache.CacheOptions!, cancellationToken));
        }

        public virtual Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            return Cache.SetAsync(key, GetBytes(entity), options, cancellationToken);
        }

        public virtual void Refresh(string key)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            Cache.Refresh(key);
        }

        public bool TryRefresh(string key)
        {
            if (!key.HasValue())
                return false;
            
            Cache.Refresh(key);

            return true;
        }

        public virtual Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            return Cache.RefreshAsync(key, cancellationToken);
        }

        public virtual void Remove(string key)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            Cache.Remove(key);
        }

        public bool TryRemove(string key)
        {
            if (!key.HasValue())
                return false;
            
            Cache.Remove(key);

            return true;
        }

        public virtual Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrWhiteSpace(key);
            
            return Cache.RemoveAsync(key, cancellationToken);
        }
            

        private byte[] GetBytes(TEntity entity)
            => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(entity, Cache.SerializerOptions));

        private static void FunctionConditionRunner(bool condition, Action trueAction, Action falseAction)
        {
            if (condition)
                trueAction();
            else
                falseAction();
        }

        private static Task FunctionConditionRunner(bool condition, Func<Task> trueAction, Func<Task> falseAction)
            => condition ? trueAction() : falseAction();
    }
}