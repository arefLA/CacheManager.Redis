using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheManager.Redis.Interfaces
{
    public interface IRedisCacheManager<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets an entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="response"></param>
        /// <returns>A boolean which shows if the get was successful and out the located entity or null</returns>
        /// <remarks>
        /// return false if the key is null or whitespace
        /// </remarks>
        bool TryGet(string key, out TEntity? response);

        /// <summary>
        /// Gets an entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located entity or null.</returns>
        /// <remarks>
        /// return false if the key is null or whitespace
        /// </remarks>
        Task<TEntity?> TryGetAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Set(string key, TEntity entity);
        
        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="options">The cache options for the entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Set(string key, TEntity entity, DistributedCacheEntryOptions options);

        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sets the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="entity">The entity to set in the cache.</param>
        /// <param name="options">The cache options for the entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task SetAsync(string key, TEntity entity, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Refresh(string key);

        /// <summary>
        /// Refreshes an entity in the cache based on its key, resetting its sliding expiration timeout (if any).
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task RefreshAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        void Remove(string key);

        /// <summary>
        /// Removes the entity with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested entity.</param>
        /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="key" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="key" /> is an empty or white space string.
        /// </remarks>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}