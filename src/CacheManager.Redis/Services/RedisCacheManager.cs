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
    internal class RedisCacheManager<TEntity> : IRedisCacheManager<TEntity> where TEntity : class
    {
        private readonly IRedisDitributedCache _cache;

        public RedisCacheManager(IRedisDitributedCache cache)
            => _cache = cache;

        public bool TryGet(string key, out TEntity? response)
        {
            response = default;
            var cachedResponse = _cache.Get(key);
            if (cachedResponse == null) return false;
    
            var responseString = Encoding.UTF8.GetString(cachedResponse);
            
            return responseString.TryDeserialize(out response, _cache.SerializerOptions);
        }

        public async Task<TEntity?> TryGetAsync(string key, CancellationToken cancellationToken = default)
        {
            var cachedResponse = await _cache.GetAsync(key, cancellationToken);
            if (cachedResponse is null) return null;
            
            var responseString = Encoding.UTF8.GetString(cachedResponse);

            var isSerialized = responseString.TryDeserialize<TEntity>(out var response,_cache.SerializerOptions);

            return isSerialized
                ? response
                : null;
        }

        public void Set(string key, TEntity entity)
            => FunctionConditionRunner(
                _cache.CacheOptions is null, 
                () => _cache.Set(key, GetBytes(entity)),
                () => _cache.Set(key, GetBytes(entity), _cache.CacheOptions!));

        public void Set(string key, TEntity entity, DistributedCacheEntryOptions options)
            => _cache.Set(key, GetBytes(entity), options);

        public Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default)
            => FunctionConditionRunner(
                _cache.CacheOptions is null,
                () => _cache.SetAsync(key, GetBytes(entity), cancellationToken),
                () => _cache.SetAsync(key, GetBytes(entity), _cache.CacheOptions!, cancellationToken));

        public Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
            => _cache.SetAsync(key, GetBytes(entity), options, cancellationToken);

        public void Refresh(string key) => _cache.Refresh(key);

        public Task RefreshAsync(string key, CancellationToken cancellationToken = default) =>
            _cache.RefreshAsync(key, cancellationToken);

        public void Remove(string key) => _cache.Remove(key);

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
            _cache.RemoveAsync(key, cancellationToken);

        private byte[] GetBytes(TEntity entity)
            => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(entity, _cache.SerializerOptions));

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