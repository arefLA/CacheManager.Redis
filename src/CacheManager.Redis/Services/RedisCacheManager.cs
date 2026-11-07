using System;
using System.Linq;
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

        public async Task<TResult> GetOrSetAsync<TResult>(string key, Func<CancellationToken, Task<TResult>> factory,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
            where TResult : TEntity?
        {
            Guard.Against.NullOrWhiteSpace(key);
            Guard.Against.Null(factory);
            EnsureCanRoundTrip<TResult>();

            var cachedResponse = await cache.GetAsync(key, cancellationToken);
            if (cachedResponse is not null)
            {
                var cachedEntity = Deserialize(cachedResponse);
                if (cachedEntity is TResult cachedResult) return cachedResult;
                if (cachedEntity is not null)
                    throw new InvalidOperationException(
                        $"The cached value for key '{key}' is {cachedEntity.GetType().Name}, not {typeof(TResult).Name}. " +
                        "Use a different key for each type.");
            }

            var entity = await factory(cancellationToken);
            if (entity is null) return entity;

            await StoreAsync(key, Serialize(entity), options, cancellationToken);

            return entity;
        }

        public void Set(string key, TEntity entity)
        {
            Guard.Against.NullOrWhiteSpace(key);

            Store(key, Serialize(entity));
        }

        public bool TrySet(string key, TEntity entity)
        {
            if (!key.HasValue())
                return false;

            Store(key, Serialize(entity));

            return true;
        }

        public void Set(string key, TEntity entity, DistributedCacheEntryOptions options)
        {
            Guard.Against.NullOrWhiteSpace(key);

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

            return StoreAsync(key, Serialize(entity), null, cancellationToken);
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

        public async Task<bool> TryRefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            if (!key.HasValue())
                return false;

            await cache.RefreshAsync(key, cancellationToken);

            return true;
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

        public async Task<bool> TryRemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            if (!key.HasValue())
                return false;

            await cache.RemoveAsync(key, cancellationToken);

            return true;
        }
            

        // Stores with the registered default options, or with no options when none are configured.
        private void Store(string key, byte[] payload)
        {
            if (cache.CacheOptions is null)
            {
                cache.Set(key, payload);
            }
            else
            {
                cache.Set(key, payload, cache.CacheOptions);
            }
        }

        // Stores with the given options, else the registered default options, else no options.
        private Task StoreAsync(string key, byte[] payload, DistributedCacheEntryOptions? options,
            CancellationToken cancellationToken)
        {
            var effectiveOptions = options ?? cache.CacheOptions;

            return effectiveOptions is null
                ? cache.SetAsync(key, payload, cancellationToken)
                : cache.SetAsync(key, payload, effectiveOptions, cancellationToken);
        }

        // A subclass result is only safe to cache if the serializer writes and reads it back as that subclass
        // (e.g. [JsonDerivedType] on TEntity). Otherwise only TEntity's fields are stored and the next read
        // returns a TEntity, so fail on the first call instead of the second.
        private void EnsureCanRoundTrip<TResult>()
        {
            if (typeof(TResult) == typeof(TEntity)) return;

            // options.GetTypeInfo is cached per options instance, so this costs no reflection after the first call.
            // MakeReadOnly fills in the default resolver for unused options, as the serializer does on first use.
            var options = cache.SerializerOptions ?? DefaultSerializerOptions;
            options.MakeReadOnly(populateMissingResolver: true);
            var derivedTypes = options.GetTypeInfo(typeof(TEntity)).PolymorphismOptions?.DerivedTypes;

            if (derivedTypes is null || derivedTypes.All(d => d.DerivedType != typeof(TResult)))
                throw new InvalidOperationException(
                    $"IRedisCacheManager<{typeof(TEntity).Name}> can't cache {typeof(TResult).Name}: only the {typeof(TEntity).Name} fields would be stored. " +
                    $"Use IRedisCacheManager<{typeof(TResult).Name}>, or add [JsonDerivedType(typeof({typeof(TResult).Name}))] to {typeof(TEntity).Name}.");
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