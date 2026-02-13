using System.Threading;
using System.Threading.Tasks;
using CacheManager.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Tests.Fakers
{
    public class FakeCustomCacheManager<T> : IRedisCacheManager<T> where T : class
    {
        public bool TryGet(string key, out T? response)
            => throw new System.NotImplementedException();

        public Task<T?> GetAsync(string key, CancellationToken cancellationToken = default)
            => throw new System.NotImplementedException();

        public void Set(string key, T entity)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySet(string key, T entity)
            => throw new System.NotImplementedException();

        public void Set(string key, T entity, DistributedCacheEntryOptions options)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySet(string key, T entity, DistributedCacheEntryOptions options)
            => throw new System.NotImplementedException();

        public Task SetAsync(string key, T entity, CancellationToken cancellationToken = default)
            => throw new System.NotImplementedException();

        public Task SetAsync(string key, T entity, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
            => throw new System.NotImplementedException();

        public void Refresh(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryRefresh(string key)
            => throw new System.NotImplementedException();

        public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
            => throw new System.NotImplementedException();

        public void Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryRemove(string key)
            => throw new System.NotImplementedException();

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
            => throw new System.NotImplementedException();
    }
}