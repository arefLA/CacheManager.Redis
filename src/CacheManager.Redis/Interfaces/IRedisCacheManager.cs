using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Interfaces
{
    public interface IRedisCacheManager<TEntity> where TEntity : class
    {
        bool TryGet(string key, out TEntity? response);

        /// <summary>
        /// Gets an entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located entity or null.</returns>
        Task<TEntity?> TryGetAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        void Set(string key, TEntity entity);
        
        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="options">The cache options for the entity.</param>
        void Set(string key, TEntity entity, DistributedCacheEntryOptions options);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="options">The cache options for the entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        void Refresh(string key);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task RefreshAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        void Remove(string key);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}