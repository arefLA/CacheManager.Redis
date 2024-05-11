using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
            => FunctionConditionRunner(
                Cache.CacheOptions is null, 
                () => Cache.Set(key, GetBytes(entity)),
                () => Cache.Set(key, GetBytes(entity), Cache.CacheOptions!));

        public virtual void Set(string key, TEntity entity, DistributedCacheEntryOptions options)
            => Cache.Set(key, GetBytes(entity), options);

        public virtual Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default)
            => FunctionConditionRunner(
                Cache.CacheOptions is null,
                () => Cache.SetAsync(key, GetBytes(entity), cancellationToken),
                () => Cache.SetAsync(key, GetBytes(entity), Cache.CacheOptions!, cancellationToken));

        public virtual Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
            => Cache.SetAsync(key, GetBytes(entity), options, cancellationToken);

        public virtual void Refresh(string key) => Cache.Refresh(key);

        public virtual Task RefreshAsync(string key, CancellationToken cancellationToken = default) =>
            Cache.RefreshAsync(key, cancellationToken);

        public virtual void Remove(string key) => Cache.Remove(key);

        public virtual Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
            Cache.RemoveAsync(key, cancellationToken);

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